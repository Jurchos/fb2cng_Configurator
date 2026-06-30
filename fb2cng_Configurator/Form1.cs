using System;
using System.Drawing;
using System.Windows.Forms;

namespace fb2cng_Configurator
{
    public partial class Form1 : Form
    {
        // Оголошення основних динамічних контейнерів
        private Panel footerPanel;
        private Panel scrollMenuPanel;
        private TableLayoutPanel mainLayout;
        // Елементи інтерфейсу конфігуратора YAML
        private ComboBox langComboBox;
        private Button btnDumpConfig;
        private TextBox txtConfigName;
        private CheckBox chkCss;
        private TextBox txtCssPath;
        private Button btnBrowseCss;
        private CheckBox chkFb2Name;

        private Label lblOutNameTitle;
        private GroupBox grpOutName;
        private ComboBox[] cmbOutFields = new ComboBox[8];
        private CheckBox[] chkAsFolder = new CheckBox[8];

        private CheckBox chkTranslit;
        private CheckBox chkReaderSize;
        private Label lblWidth, lblHeight, lblDpi;
        private TextBox txtWidth, txtHeight, txtDpi;

        private CheckBox chkCover;
        private ComboBox cmbCoverMode;
        private CheckBox chkNotes;
        private ComboBox cmbNotesMode;
        private CheckBox chkOpenFromCover;
        private CheckBox chkFixZip;

        private Button btnHelp, btnTheme, btnOk, btnCancel;
        private Label lblLang, lblConfigName;

        // Гнучкі аліаси для сумісності з файлом Form1_Logic.cs
        private CheckBox chkFootnotes { get { return chkNotes; } }
        private CheckBox chkTocType { get { return chkCover; } }
        private CheckBox chkOpenCover { get { return chkOpenFromCover; } }

        public Form1()
        {
            // 1. Системна ініціалізація форми
            InitializeComponent();

            // 2. Завантаження налаштувань конфігуратора
            try
            {
                Config.LoadSettings();
            }
            catch (Exception configEx)
            {
                System.Diagnostics.Debug.WriteLine("Помилка завантаження файлу конфігурації: " + configEx.Message);
            }

            // 3. Ініціалізація та побудова візуальної частини
            try
            {
                SetupInterface();
                ApplyTheme();

                // Безпечно встановлюємо початкову мову в комбобокс
                if (langComboBox != null)
                {
                    if (Config.CurrentLanguage == "Ukrainian") langComboBox.SelectedIndex = 1;
                    else if (Config.CurrentLanguage == "Russian") langComboBox.SelectedIndex = 2;
                    else langComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Критичний збій ініціалізації вікна:\n\n{ex.Message}\n\nМісце помилки:\n{ex.StackTrace}",
                                "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Безпечне калькулювання розмірів перед виходом з програми
            this.FormClosing += (s, e) =>
            {
                Rectangle screenArea = Screen.PrimaryScreen.WorkingArea;

                // Перевіряємо, чи вікно у звичайному стані (не максимізоване)
                if (this.WindowState == FormWindowState.Normal)
                {
                    Config.WindowWidthPct = (float)Width / screenArea.Width;
                    Config.WindowHeightPct = (float)Height / screenArea.Height;
                }
                else
                {
                    // Якщо вікно було розгорнуте на повну, беремо його нормальні межі до максимізації
                    Config.WindowWidthPct = (float)RestoreBounds.Width / screenArea.Width;
                    Config.WindowHeightPct = (float)RestoreBounds.Height / screenArea.Height;
                }

                // Захист від критичних аномалій (щоб значення не стали нульовими)
                if (Config.WindowWidthPct < 0.1f) Config.WindowWidthPct = 0.35f;
                if (Config.WindowHeightPct < 0.1f) Config.WindowHeightPct = 0.70f;

                // Зберігаємо оновлені параметри конфігуратора
                Config.SaveSettings();
            };
        }

        // Системний перехоплювач моменту виведення вікна на екран
        protected override void OnLoad(EventArgs e)
        {
            // Спочатку дозволяємо операційній системі повністю побудувати форму та її сітки
            base.OnLoad(e);

            try
            {
                // Отримуємо чисту робочу зону монітора (вона ПРАВИЛЬНО виключає висоту панелі завдань!)
                Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;

                // Розраховуємо габарити на основі збережених відсотків
                int targetWidth = (int)(workingArea.Width * Config.WindowWidthPct);
                int targetHeight = (int)(workingArea.Height * Config.WindowHeightPct);

                // Захист: якщо збережена висота намагається вийти за межі робочої зони, 
                // ми примусово обрізаємо її на 20 пікселів вище панелі завдань, щоб кнопки ніколи не ховалися!
                if (targetHeight > workingArea.Height)
                {
                    targetHeight = workingArea.Height - 20;
                }

                // Встановлюємо фінальні скориговані розміри вікна
                Size = new Size(targetWidth, targetHeight);

                // Оновлюємо позицію форми, щоб вона була рівно по центру екрана
                Location = new Point(
                    workingArea.Left + ((workingArea.Width - Width) / 2),
                    workingArea.Top + ((workingArea.Height - Height) / 2)
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Помилка OnLoad ресайзу: " + ex.Message);
            }
        }

        private void SetupInterface()
        {
            // 1. Налаштування вікна для динамічного масштабування та ресайзу
            Size = new Size(590, 600);
            MinimumSize = new Size(535, 595);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
            StartPosition = FormStartPosition.CenterScreen;

            // Шрифти автоматично масштабуються операційною системою (High DPI)
            this.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point);

            // 2. Головна вертикальна сітка (Вміст -> Конструктор назв -> Футер)
            // Виправлено Shadowing: присвоюємо значення глобальному полю класу!
            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(10)
            };
            _ = mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _ = mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 350F));
            _ = mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            Controls.Add(mainLayout);

            // 3. Центральний контейнер з автоскролом для параметрів конфігу
            scrollMenuPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(10, 0, 10, 0) };
            mainLayout.Controls.Add(scrollMenuPanel, 0, 0);

            // Двоколонкова гнучка сітка для елементів керування
            TableLayoutPanel settingsGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 12,
                AutoSize = true,
                Padding = new Padding(0, 0, 5, 0)
            };
            settingsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            settingsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            scrollMenuPanel.Controls.Add(settingsGrid);

            // --- 1. Мова ---
            lblLang = new Label { Text = "Language:", Anchor = AnchorStyles.Left, AutoSize = true };
            langComboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            langComboBox.Items.AddRange(new string[] { "English", "Українська", "Русский" });
            langComboBox.SelectedIndexChanged += LangComboBox_SelectedIndexChanged;
            settingsGrid.Controls.Add(lblLang, 0, 0);
            settingsGrid.Controls.Add(langComboBox, 1, 0);

            // --- 2. Кнопка завантаження конфігу (Висота адаптується під розмір тексту)
            btnDumpConfig = new Button
            {
                Dock = DockStyle.Fill,
                AutoSize = true,                  // Дозволяємо кнопці рости по вертикалі разом із текстом
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(0, 5, 0, 5), // Гарантовані красиві зазори над і під текстом
                Margin = new Padding(0, 10, 0, 10)
            };
            btnDumpConfig.Click += BtnDumpConfig_Click;
            settingsGrid.Controls.Add(btnDumpConfig, 0, 1);
            settingsGrid.SetColumnSpan(btnDumpConfig, 2);

            // --- 3. Назва конфіг-файлу ---
            lblConfigName = new Label { Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding(0, 5, 0, 5) };
            txtConfigName = new TextBox { Dock = DockStyle.Fill, Text = "config.yaml" };
            settingsGrid.Controls.Add(lblConfigName, 0, 2);
            settingsGrid.Controls.Add(txtConfigName, 1, 2);

            // --- 4. Налаштування CSS (Вкладена сітка без фіксованої висоти для High DPI)
            chkCss = new CheckBox { Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding(0, 5, 0, 5) };
            TableLayoutPanel cssInnerGrid = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Margin = new Padding(0), AutoSize = true };
            cssInnerGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75F));
            cssInnerGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            cssInnerGrid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            txtCssPath = new TextBox { Dock = DockStyle.Fill, Enabled = false };
            btnBrowseCss = new Button { Dock = DockStyle.Fill, Enabled = false, Text = "...", Height = txtCssPath.PreferredSize.Height };
            chkCss.CheckedChanged += (s, e) => { txtCssPath.Enabled = btnBrowseCss.Enabled = chkCss.Checked; };
            btnBrowseCss.Click += BtnBrowseCss_Click;

            cssInnerGrid.Controls.Add(txtCssPath, 0, 0);
            cssInnerGrid.Controls.Add(btnBrowseCss, 1, 0);
            settingsGrid.Controls.Add(chkCss, 0, 3);
            settingsGrid.Controls.Add(cssInnerGrid, 1, 3);

            // --- 5. Використовувати оригінальне ім'я FB2 ---
            chkFb2Name = new CheckBox { Dock = DockStyle.Fill, AutoSize = true, Margin = new Padding(0, 8, 0, 8) };
            settingsGrid.Controls.Add(chkFb2Name, 0, 4);
            settingsGrid.SetColumnSpan(chkFb2Name, 2);

            // --- 7. Транслітерація ---
            chkTranslit = new CheckBox { Dock = DockStyle.Fill, AutoSize = true, Margin = new Padding(0, 5, 0, 5) };
            settingsGrid.Controls.Add(chkTranslit, 0, 6);
            settingsGrid.SetColumnSpan(chkTranslit, 2);

            // --- 8. Розмір екрана читалки ---
            chkReaderSize = new CheckBox { Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding(0, 5, 0, 5) };
            settingsGrid.Controls.Add(chkReaderSize, 0, 7);

            // Оптимізована сітка: фіксовані пікселі для підписів, гнучкі відсотки для TextBox
            TableLayoutPanel sizeGrid = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, RowCount = 1, Margin = new Padding(0), AutoSize = true };
            
            // Задаємо жорстку ширину для міток (30px достатньо для "W:", "H:", "DPI:") та рівний простір для полів введення
            sizeGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 55F)); // W:
            sizeGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F)); // Поле W
            sizeGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 55F)); // H:
            sizeGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F)); // Поле H
            sizeGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,75F)); // DPI: (трохи ширше для 3-4 літер)
            sizeGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F)); // Поле DPI
            
            sizeGrid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            lblWidth = new Label { Text = "W:", Anchor = AnchorStyles.Right, AutoSize = true, Enabled = false };
            txtWidth = new TextBox { Dock = DockStyle.Fill, Text = "1264", Enabled = false };
            lblHeight = new Label { Text = "H:", Anchor = AnchorStyles.Right, AutoSize = true, Enabled = false };
            txtHeight = new TextBox { Dock = DockStyle.Fill, Text = "1680", Enabled = false };
            lblDpi = new Label { Text = "DPI:", Anchor = AnchorStyles.Right, AutoSize = true, Enabled = false };
            txtDpi = new TextBox { Dock = DockStyle.Fill, Text = "300", Enabled = false };

            chkReaderSize.CheckedChanged += (s, e) =>
            {
                bool en = chkReaderSize.Checked;
                lblWidth.Enabled = txtWidth.Enabled = lblHeight.Enabled = txtHeight.Enabled = lblDpi.Enabled = txtDpi.Enabled = en;
            };

            sizeGrid.Controls.Add(lblWidth, 0, 0); sizeGrid.Controls.Add(txtWidth, 1, 0);
            sizeGrid.Controls.Add(lblHeight, 2, 0); sizeGrid.Controls.Add(txtHeight, 3, 0);
            sizeGrid.Controls.Add(lblDpi, 4, 0); sizeGrid.Controls.Add(txtDpi, 5, 0);
            settingsGrid.Controls.Add(sizeGrid, 1, 7);

            // --- 9. Спосіб відображення виносок (Повністю виправлено ключі!) ---
            chkNotes = new CheckBox { Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding(0, 5, 0, 5) };
            cmbNotesMode = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Enabled = false };
            cmbNotesMode.Items.AddRange(new string[] { "default", "float", "floatRenumbered" });
            cmbNotesMode.SelectedIndex = 0;
            chkNotes.CheckedChanged += (s, e) => cmbNotesMode.Enabled = chkNotes.Checked;
            settingsGrid.Controls.Add(chkNotes, 0, 8);
            settingsGrid.Controls.Add(cmbNotesMode, 1, 8);

            // --- 10. Тип навігаційної ієрархії (Повністю виправлено ключі!) ---
            chkCover = new CheckBox { Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding(0, 5, 0, 5) };
            cmbCoverMode = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Enabled = false };
            cmbCoverMode.Items.AddRange(new string[] { "normal", "old_kindle", "flat" });
            cmbCoverMode.SelectedIndex = 0;
            chkCover.CheckedChanged += (s, e) => cmbCoverMode.Enabled = chkCover.Checked;
            settingsGrid.Controls.Add(chkCover, 0, 9);
            settingsGrid.Controls.Add(cmbCoverMode, 1, 9);

            // --- 11. Додаткові чекбокси ---
            chkOpenFromCover = new CheckBox { Dock = DockStyle.Fill, AutoSize = true, Margin = new Padding(0, 5, 0, 5) };
            chkFixZip = new CheckBox { Dock = DockStyle.Fill, AutoSize = true, Margin = new Padding(0, 5, 0, 5) };

            settingsGrid.Controls.Add(chkOpenFromCover, 0, 10); settingsGrid.SetColumnSpan(chkOpenFromCover, 2);
            settingsGrid.Controls.Add(chkFixZip, 0, 11); settingsGrid.SetColumnSpan(chkFixZip, 2);

            // 4. КОНСТРУКТОР СТРУКТУРИ НАЗВИ (Вертикальне розташування для ідеального High DPI)
            lblOutNameTitle = new Label { Dock = DockStyle.Fill, AutoSize = true, Font = new Font(this.Font, FontStyle.Bold), Margin = new Padding(0, 10, 0, 5) };
            grpOutName = new GroupBox { Dock = DockStyle.Fill, Text = "", Margin = new Padding(2, 5, 2, 5) };
            mainLayout.Controls.Add(grpOutName, 0, 1);

            TableLayoutPanel templateGrid = new TableLayoutPanel
            {
                Name = "templateGrid", // Для безпомилкового накладання кольорів теми
                Dock = DockStyle.Fill,
                ColumnCount = 2,       // Лише 2 стовпчики: Комбобокс та Чекбокс
                RowCount = 8,          // 8 рядків один під одним
                Padding = new Padding(5)
            };
            grpOutName.Controls.Add(templateGrid);

            // Перший стовпчик (Комбобокси) займає 75% ширини, другий (Чекбокси папок) — 25%
            templateGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75F));
            templateGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            // Створюємо 8 рівних рядків по вертикалі
            for (int i = 0; i < 8; i++)
            {
                templateGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 12.5F));
            }

            for (int i = 0; i < 8; i++)
            {
                int index = i;
                cmbOutFields[index] = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
                chkAsFolder[index] = new CheckBox { Text = "Fold", Dock = DockStyle.Fill, Enabled = false, Font = new Font(Font.FontFamily, 10.5F), Margin = new Padding(10, 0, 0, 0) };

                cmbOutFields[index].Items.AddRange(new string[] { "", "", "", "", "", "", "", "", "" });
                cmbOutFields[index].SelectedIndex = 0;
                if (index > 0) cmbOutFields[index].Enabled = false;

                cmbOutFields[index].SelectedIndexChanged += (s, e) => CmbOutFields_SelectedIndexChanged(index);

                // Додаємо елементи: Комбобокс у стовпчик 0, Чекбокс у стовпчик 1 для кожного рядка `index`
                templateGrid.Controls.Add(cmbOutFields[index], 0, index);
                templateGrid.Controls.Add(chkAsFolder[index], 1, index);
            }
            chkFb2Name.CheckedChanged += ChkFb2Name_CheckedChanged;

            // 5. НИЖНЯ ПАНЕЛЬ (ФУТЕР) З КНОПКАМИ ДІЙ (Автоматична ширина з GDI+ захистом)
            int baseFontHeight = 19;
            float dpiScale = (float)Font.Height / baseFontHeight;

            float smoothedScale = (float)Math.Sqrt(dpiScale);
            if (dpiScale <= 1.0f) smoothedScale = dpiScale;

            // Розраховуємо тільки висоту футера, кнопки підлаштуються самі
            int dynamicFooterHeight = (int)(baseFontHeight * 2.5f * smoothedScale);

            // М'які горизонтальні відступи всередині кнопок
            int hPadding = (int)(14 * smoothedScale);

            mainLayout.RowStyles[2].SizeType = SizeType.Absolute;
            mainLayout.RowStyles[2].Height = dynamicFooterHeight;

            footerPanel = new Panel { Dock = DockStyle.Fill, AutoSize = false };
            mainLayout.Controls.Add(footerPanel, 0, 2);

            // --- ЛІВА ЧАСТИНА ФУТЕРА ---
            Panel leftButtonsPanel = new Panel { Dock = DockStyle.Left, AutoSize = true, Padding = new Padding(5) };
            footerPanel.Controls.Add(leftButtonsPanel);

            btnHelp = new Button
            {
                Text = "Help",
                Dock = DockStyle.Left,
                AutoSize = true, // ПОВЕРНУЛИ АВТОМАТИЧНУ ШИРИНУ
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                UseCompatibleTextRendering = true, // ЗАХИСТ ВІД ОБРІЗАННЯ ТЕКСТУ ЗНИЗУ!
                Padding = new Padding(hPadding, 0, hPadding, 0),
                Margin = new Padding(5)
            };

            btnTheme = new Button
            {
                Text = "Theme",
                Dock = DockStyle.Left,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                UseCompatibleTextRendering = true, // Вмикає правильний вертикальний рендеринг
                Padding = new Padding(hPadding, 0, hPadding, 0),
                Margin = new Padding(5)
            };
            leftButtonsPanel.Controls.Add(btnTheme);
            leftButtonsPanel.Controls.Add(btnHelp);


            // --- ПРАВА ЧАСТИНА ФУТЕРА ---
            Panel rightButtonsPanel = new Panel { Dock = DockStyle.Right, AutoSize = true, Padding = new Padding(5) };
            footerPanel.Controls.Add(rightButtonsPanel);

            btnCancel = new Button
            {
                Text = "Cancel",
                Dock = DockStyle.Right,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                UseCompatibleTextRendering = true,
                Padding = new Padding(hPadding + 4, 0, hPadding + 4, 0),
                Margin = new Padding(5)
            };

            btnOk = new Button
            {
                Text = "OK",
                Dock = DockStyle.Right,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                UseCompatibleTextRendering = true,
                Padding = new Padding(hPadding + 8, 0, hPadding + 8, 0),
                Margin = new Padding(5)
            };
            rightButtonsPanel.Controls.Add(btnOk);
            rightButtonsPanel.Controls.Add(btnCancel);

            // Прив'язка стандартних подій дій
            btnTheme.Click += (s, e) => { Config.IsDarkTheme = !Config.IsDarkTheme; ApplyTheme(); };
            btnCancel.Click += (s, e) => this.Close();
            btnHelp.Click += (s, e) => ShowHelp();
            btnOk.Click += (s, e) => SaveYamlConfiguration();
        }

        // Обов'язковий метод для візуального компілятора Windows Forms
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(620, 740);
            this.Name = "Form1";
            this.Text = "fb2cng Configurator";
            this.ResumeLayout(false);
        }
    }
}
