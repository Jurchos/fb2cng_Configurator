using fb2cng_Configurator;
using fb2cng_Configurator.fb2cng_Configurator;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace fb2cng_Configurator
{
    public partial class Form1 : Form
    {
        // Оголошення основних динамічних контейнерів
        private TableLayoutPanel mainLayout;
        private Panel footerPanel;
        private Panel scrollMenuPanel;

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

        // Гнучкі аліаси для 100% сумісності з файлом Form1_Logic.cs
        private CheckBox chkFootnotes => chkNotes;
        private CheckBox chkTocType => chkCover;
        private CheckBox chkOpenCover => chkOpenFromCover;

        public Form1()
        {
            // 1. Спершу обов'язково викликаємо системну ініціалізацію форми
            InitializeComponent();

            // 2. ІЗОЛЮЄМО ЗБИТКОВИЙ КЛАС CONFIG! 
            // Якщо Config.LoadSettings() містить помилку або падає — програма більше НЕ ЗАКРИЄТЬСЯ,
            // а просто продовжить запуск інтерфейсу із дефолтними налаштуваннями.
            try
            {
                Config.LoadSettings();
            }
            catch (Exception configEx)
            {
                // Помилка конфігу просто запишеться в дебаг-лог, не перериваючи роботу додатка
                System.Diagnostics.Debug.WriteLine("Помилка завантаження файлу конфігурації: " + configEx.Message);
            }

            // 3. Головний перехоплювач для візуальної частини
            try
            {
                SetupInterface();
                ApplyTheme();

                // Безпечно встановлюємо початкову мову в комбобокс
                if (langComboBox != null)
                {
                    if (Config.CurrentLanguage == "Ukrainan") langComboBox.SelectedIndex = 1;
                    else if (Config.CurrentLanguage == "Russian") langComboBox.SelectedIndex = 2;
                    else langComboBox.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                // Цей блок спрацює, ТІЛЬКИ якщо помилка виникне безпосередньо в дизайні
                MessageBox.Show($"Критичний збій ініціалізації вікна:\n\n{ex.Message}\n\nМісце помилки:\n{ex.StackTrace}",
                                "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupInterface()
        {
            // 1. Налаштування вікна для динамічного масштабування та ресайзу
            this.Size = new Size(600, 720);
            this.MinimumSize = new Size(540, 650);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Шрифти автоматично масштабуються операційною системою (High DPI)
            this.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);

            // 2. Головна вертикальна сітка (Вміст -> Конструктор назв -> Футер)
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(10)
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 135F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            this.Controls.Add(mainLayout);

            // 3. Центральний контейнер з автоскролом для параметрів конфігу
            scrollMenuPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(0, 0, 15, 0) };
            mainLayout.Controls.Add(scrollMenuPanel, 0, 0);

            // Двоколонкова гнучка сітка для елементів керування
            TableLayoutPanel settingsGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 13,
                AutoSize = true,
                Padding = new Padding(0, 0, 5, 0)
            };
            settingsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            settingsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            scrollMenuPanel.Controls.Add(settingsGrid);

            // --- 1. Мова ---
            lblLang = new Label { Text = "Language:", Anchor = AnchorStyles.Left, AutoSize = true };
            langComboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            langComboBox.Items.AddRange(new string[] { "English", "Українська", "Русский" });
            langComboBox.SelectedIndexChanged += LangComboBox_SelectedIndexChanged;
            settingsGrid.Controls.Add(lblLang, 0, 0);
            settingsGrid.Controls.Add(langComboBox, 1, 0);

            // --- 2. Кнопка скидання/завантаження конфігу ---
            btnDumpConfig = new Button { Dock = DockStyle.Fill, Height = 35, Margin = new Padding(0, 10, 0, 10) };
            btnDumpConfig.Click += BtnDumpConfig_Click;
            settingsGrid.Controls.Add(btnDumpConfig, 0, 1);
            settingsGrid.SetColumnSpan(btnDumpConfig, 2);

            // --- 3. Назва конфіг-файлу ---
            lblConfigName = new Label { Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding(0, 5, 0, 5) };
            txtConfigName = new TextBox { Dock = DockStyle.Fill, Text = "config.yaml" };
            settingsGrid.Controls.Add(lblConfigName, 0, 2);
            settingsGrid.Controls.Add(txtConfigName, 1, 2);

            // --- 4. Налаштування CSS (Вкладена сітка для TextBox + Button)
            chkCss = new CheckBox { Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding(0, 5, 0, 5) };
            TableLayoutPanel cssInnerGrid = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, Height = 30, Margin = new Padding(0) };
            cssInnerGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            cssInnerGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));

            txtCssPath = new TextBox { Dock = DockStyle.Fill, Enabled = false };
            btnBrowseCss = new Button { Dock = DockStyle.Fill, Enabled = false, Text = "..." };
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

            // --- 6. Назва для конструктора (залишаємо підпис) ---
            lblOutNameTitle = new Label { Dock = DockStyle.Fill, AutoSize = true, Font = new Font(this.Font, FontStyle.Bold), Margin = new Padding(0, 5, 0, 5) };
            settingsGrid.Controls.Add(lblOutNameTitle, 0, 5);
            settingsGrid.SetColumnSpan(lblOutNameTitle, 2);

            // --- 7. Транслітерація ---
            chkTranslit = new CheckBox { Dock = DockStyle.Fill, AutoSize = true, Margin = new Padding(0, 5, 0, 5) };
            settingsGrid.Controls.Add(chkTranslit, 0, 6);
            settingsGrid.SetColumnSpan(chkTranslit, 2);

            // --- 8. Розмір екрана читалки ---
            chkReaderSize = new CheckBox { Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding(0, 5, 0, 5) };
            settingsGrid.Controls.Add(chkReaderSize, 0, 7);

            // Вкладена горизонтальна сітка для Width, Height, DPI в один рядок
            TableLayoutPanel sizeGrid = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 6, RowCount = 1, Height = 32, Margin = new Padding(0) };
            for (int k = 0; k < 6; k++) sizeGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66F));

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

            // --- 9. Спосіб відображення виносок ---
            chkCover = new CheckBox { Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding(0, 5, 0, 5) };
            cmbCoverMode = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Enabled = false };
            cmbCoverMode.Items.AddRange(new string[] { "default", "float", "floatRenumbered" });
            cmbCoverMode.SelectedIndex = 0;
            chkCover.CheckedChanged += (s, e) => cmbCoverMode.Enabled = chkCover.Checked;
            settingsGrid.Controls.Add(chkCover, 0, 8);
            settingsGrid.Controls.Add(cmbCoverMode, 1, 8);

            // --- 10. Тип навігаційної ієрархії ---
            chkNotes = new CheckBox { Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding(0, 5, 0, 5) };
            cmbNotesMode = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Enabled = false };
            cmbNotesMode.Items.AddRange(new string[] { "normal", "old_kindle", "flat" });
            cmbNotesMode.SelectedIndex = 0;
            chkNotes.CheckedChanged += (s, e) => cmbNotesMode.Enabled = chkNotes.Checked;
            settingsGrid.Controls.Add(chkNotes, 0, 9);
            settingsGrid.Controls.Add(cmbNotesMode, 1, 9);

            // --- 11. Додаткові чекбокси ---
            chkOpenFromCover = new CheckBox { Dock = DockStyle.Fill, AutoSize = true, Margin = new Padding(0, 5, 0, 5) };
            chkFixZip = new CheckBox { Dock = DockStyle.Fill, AutoSize = true, Margin = new Padding(0, 5, 0, 5) };

            settingsGrid.Controls.Add(chkOpenFromCover, 0, 10); settingsGrid.SetColumnSpan(chkOpenFromCover, 2);
            settingsGrid.Controls.Add(chkFixZip, 0, 11); settingsGrid.SetColumnSpan(chkFixZip, 2);

            // 4. КОНСТРУКТОР СТРУКТУРИ НАЗВИ (Створюємо горизонтальну сітку на 8 стовпчиків)
            grpOutName = new GroupBox { Dock = DockStyle.Fill, Text = "" };
            mainLayout.Controls.Add(grpOutName, 0, 1);

            TableLayoutPanel templateGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 8,
                RowCount = 2,
                Padding = new Padding(5)
            };
            grpOutName.Controls.Add(templateGrid);

            // Задаємо однакову ширину 12.5% для кожного з 8 стовпчиків
            for (int i = 0; i < 8; i++)
            {
                templateGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12.5F));
            }

            templateGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 55F)); // Рядок під ComboBox
            templateGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 45F)); // Рядок під CheckBox

            for (int i = 0; i < 8; i++)
            {
                int index = i;
                cmbOutFields[index] = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
                chkAsFolder[index] = new CheckBox { Text = "Fold", Dock = DockStyle.Fill, Enabled = false, Font = new Font(this.Font.FontFamily, 9F) };

                cmbOutFields[index].Items.AddRange(new string[] { "", "", "", "", "", "", "", "", "" });
                cmbOutFields[index].SelectedIndex = 0;
                if (index > 0) cmbOutFields[index].Enabled = false;

                cmbOutFields[index].SelectedIndexChanged += (s, e) => CmbOutFields_SelectedIndexChanged(index);

                templateGrid.Controls.Add(cmbOutFields[index], index, 0);
                templateGrid.Controls.Add(chkAsFolder[index], index, 1);
            }
            chkFb2Name.CheckedChanged += ChkFb2Name_CheckedChanged;


            // 5. НИЖНЯ ПАНЕЛЬ (ФУТЕР) З КНОПКАМИ ДІЙ
            footerPanel = new Panel { Dock = DockStyle.Fill, Height = 45 };
            mainLayout.Controls.Add(footerPanel, 0, 2);

            FlowLayoutPanel buttonsFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 5, 0, 0)
            };
            footerPanel.Controls.Add(buttonsFlow);

            btnCancel = new Button { Text = "Cancel", Size = new Size(100, 32) };
            btnOk = new Button { Text = "OK", Size = new Size(100, 32) };
            btnTheme = new Button { Text = "Theme", Size = new Size(90, 32) };
            btnHelp = new Button { Text = "Help", Size = new Size(90, 32) };

            btnTheme.Click += (s, e) => { Config.IsDarkTheme = !Config.IsDarkTheme; ApplyTheme(); };
            btnCancel.Click += (s, e) => this.Close();
            btnHelp.Click += (s, e) => ShowHelp();
            btnOk.Click += (s, e) => SaveYamlConfiguration();

            // Додаємо елементи (через RightToLeft вони вирівняються справа наліво)
            buttonsFlow.Controls.AddRange(new Control[] { btnCancel, btnOk, btnTheme, btnHelp });
        }

        // Обов'язковий метод, необхідний для роботи WinForms компілятора
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 720);
            this.Name = "Form1";
            this.Text = "fb2cng Configurator";
            this.ResumeLayout(false);
        }
    }
}