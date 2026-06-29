using fb2cng_Configurator;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace fb2cng_Configurator
{
    public partial class Form1 : Form
    {
        // Оголошення елементів форми
        private Panel footerPanel;
        private Panel scrollMenuPanel;

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

        // Нові та виправлені елементи інтерфейсу (Пункти 9, 10, 11)
        private CheckBox chkCover;
        private ComboBox cmbCoverMode;
        private CheckBox chkNotes;
        private ComboBox cmbNotesMode;
        private CheckBox chkOpenFromCover;
        private CheckBox chkFixZip;
        private CheckBox chkEmbedFonts;

        private Button btnHelp, btnTheme, btnOk, btnCancel;
        private Label lblLang, lblConfigName;

        public Form1()
        {
            InitializeComponent();
            Config.LoadSettings(); // Автоматичне завантаження збереженої мови та теми
            SetupInterface();
            ApplyTheme();
            UpdateLocalization();

            // Встановлюємо збережені значення в інтерфейс
            if (Config.CurrentLanguage == "Ukrainan") langComboBox.SelectedIndex = 1;
            else if (Config.CurrentLanguage == "Russian") langComboBox.SelectedIndex = 2;
            else langComboBox.SelectedIndex = 0;
        }

        private void SetupInterface()
        {
            // Збільшене та комфортне вікно з нормальною рамкою Windows
            this.Size = new Size(540, 650);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Встановлення глобального збільшеного шрифту (11pt)
            this.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);

            // Нижня панель (Футер)
            footerPanel = new Panel { Dock = DockStyle.Bottom, Height = 60, Padding = new Padding(5) };
            btnHelp = new Button { Size = new Size(90, 32), Location = new Point(15, 14) };
            btnTheme = new Button { Size = new Size(90, 32), Location = new Point(115, 14) };
            btnOk = new Button { Size = new Size(100, 32), Location = new Point(300, 14) };
            btnCancel = new Button { Size = new Size(100, 32), Location = new Point(410, 14) };

            btnTheme.Click += (s, e) => { Config.IsDarkTheme = !Config.IsDarkTheme; ApplyTheme(); };
            btnCancel.Click += (s, e) => this.Close();
            btnHelp.Click += (s, e) => ShowHelp();
            btnOk.Click += (s, e) => SaveYamlConfiguration();

            footerPanel.Controls.AddRange(new Control[] { btnHelp, btnTheme, btnOk, btnCancel });
            this.Controls.Add(footerPanel);

            // Центральний скрол-контейнер
            scrollMenuPanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(15) };
            this.Controls.Add(scrollMenuPanel);

            // 1. Мова
            lblLang = new Label { Location = new Point(20, 20), AutoSize = true };
            langComboBox = new ComboBox { Location = new Point(240, 17), Width = 160, DropDownStyle = ComboBoxStyle.DropDownList };
            langComboBox.Items.AddRange(new string[] { "English", "Українська", "Русский" });
            langComboBox.SelectedIndexChanged += LangComboBox_SelectedIndexChanged;
            scrollMenuPanel.Controls.AddRange(new Control[] { lblLang, langComboBox });

            // 2. Завантажити
            btnDumpConfig = new Button { Location = new Point(20, 55), Width = 465, Height = 35 };
            btnDumpConfig.Click += BtnDumpConfig_Click;
            scrollMenuPanel.Controls.Add(btnDumpConfig);

            // 3. Назва файлу
            lblConfigName = new Label { Location = new Point(20, 110), AutoSize = true };
            txtConfigName = new TextBox { Location = new Point(240, 107), Width = 245, Text = "config.yaml" };
            scrollMenuPanel.Controls.AddRange(new Control[] { lblConfigName, txtConfigName });

            // 4. CSS
            chkCss = new CheckBox { Location = new Point(20, 145), AutoSize = true };
            txtCssPath = new TextBox { Location = new Point(40, 175), Width = 320, Enabled = false };
            btnBrowseCss = new Button { Location = new Point(370, 173), Width = 115, Height = 28, Enabled = false };
            chkCss.CheckedChanged += (s, e) => { txtCssPath.Enabled = btnBrowseCss.Enabled = chkCss.Checked; };
            btnBrowseCss.Click += BtnBrowseCss_Click;
            scrollMenuPanel.Controls.AddRange(new Control[] { chkCss, txtCssPath, btnBrowseCss });

            // 5. Ім'я файлу fb2
            chkFb2Name = new CheckBox { Location = new Point(20, 215), Width = 465, AutoSize = true };
            scrollMenuPanel.Controls.Add(chkFb2Name);

            // 6. Конструктор структури назви
            lblOutNameTitle = new Label { Location = new Point(20, 250), AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            scrollMenuPanel.Controls.Add(lblOutNameTitle);

            grpOutName = new GroupBox { Location = new Point(20, 275), Width = 465, Height = 270, Text = "" };
            scrollMenuPanel.Controls.Add(grpOutName);

            int startY = 22;
            for (int i = 0; i < 8; i++)
            {
                int index = i;
                cmbOutFields[index] = new ComboBox { Location = new Point(15, startY + (index * 30)), Width = 290, DropDownStyle = ComboBoxStyle.DropDownList };
                chkAsFolder[index] = new CheckBox { Location = new Point(320, startY + (index * 30) + 2), Width = 130, Enabled = false };
                cmbOutFields[index].Items.AddRange(new string[] { "", "", "", "", "", "", "", "", "" });
                cmbOutFields[index].SelectedIndex = 0;
                if (index > 0) cmbOutFields[index].Enabled = false;
                cmbOutFields[index].SelectedIndexChanged += (s, e) => CmbOutFields_SelectedIndexChanged(index);
                grpOutName.Controls.Add(cmbOutFields[index]);
                grpOutName.Controls.Add(chkAsFolder[index]);
            }
            chkFb2Name.CheckedChanged += ChkFb2Name_CheckedChanged;

            // 7. Транслітерація
            chkTranslit = new CheckBox { Location = new Point(20, 560), Width = 465, AutoSize = true };
            scrollMenuPanel.Controls.Add(chkTranslit);

            // 8. Розмір екрана
            chkReaderSize = new CheckBox { Location = new Point(20, 595), AutoSize = true };
            lblWidth = new Label { Location = new Point(40, 625), AutoSize = true, Enabled = false };
            txtWidth = new TextBox { Location = new Point(110, 622), Width = 55, Text = "1264", Enabled = false };
            lblHeight = new Label { Location = new Point(180, 625), AutoSize = true, Enabled = false };
            txtHeight = new TextBox { Location = new Point(250, 622), Width = 55, Text = "1680", Enabled = false };
            lblDpi = new Label { Location = new Point(320, 625), AutoSize = true, Enabled = false };
            txtDpi = new TextBox { Location = new Point(365, 622), Width = 55, Text = "300", Enabled = false };

            chkReaderSize.CheckedChanged += (s, e) => {
                bool en = chkReaderSize.Checked;
                lblWidth.Enabled = txtWidth.Enabled = lblHeight.Enabled = txtHeight.Enabled = lblDpi.Enabled = txtDpi.Enabled = en;
            };
            scrollMenuPanel.Controls.AddRange(new Control[] { chkReaderSize, lblWidth, txtWidth, lblHeight, txtHeight, lblDpi, txtDpi });

            // 9. Спосіб відображення виносок (footnotes: mode:)
            chkCover = new CheckBox { Location = new Point(20, 665), AutoSize = true };
            cmbCoverMode = new ComboBox { Location = new Point(320, 662), Width = 165, DropDownStyle = ComboBoxStyle.DropDownList, Enabled = false };
            cmbCoverMode.Items.AddRange(new string[] { "default", "float", "floatRenumbered" });
            cmbCoverMode.SelectedIndex = 0;
            chkCover.CheckedChanged += (s, e) => cmbCoverMode.Enabled = chkCover.Checked;
            scrollMenuPanel.Controls.AddRange(new Control[] { chkCover, cmbCoverMode });

            // 10. Тип навігаційної ієрархії (toc_type:)
            chkNotes = new CheckBox { Location = new Point(20, 700), AutoSize = true };
            cmbNotesMode = new ComboBox { Location = new Point(320, 697), Width = 165, DropDownStyle = ComboBoxStyle.DropDownList, Enabled = false };
            cmbNotesMode.Items.AddRange(new string[] { "normal", "old_kindle", "flat" });
            cmbNotesMode.SelectedIndex = 0;
            chkNotes.CheckedChanged += (s, e) => cmbNotesMode.Enabled = chkNotes.Checked;
            scrollMenuPanel.Controls.AddRange(new Control[] { chkNotes, cmbNotesMode });

            // Додаткові нові чекбокси (Відкриття книги з титулки та Вилучення дескриптора)
            chkOpenFromCover = new CheckBox { Location = new Point(20, 735), Width = 465, AutoSize = true };
            chkFixZip = new CheckBox { Location = new Point(20, 770), Width = 465, AutoSize = true };
            chkEmbedFonts = new CheckBox { Location = new Point(20, 805), Width = 465, AutoSize = true };

            scrollMenuPanel.Controls.AddRange(new Control[] { chkOpenFromCover, chkFixZip, chkEmbedFonts });

            // Технічний відступ, щоб скрол плавно доходив до кінця і нічого не обрізалося
            Label lblSpacer = new Label { Location = new Point(20, 845), Height = 30, Width = 10 };
            scrollMenuPanel.Controls.Add(lblSpacer);
        }

        private void MyCustomInit()
        {
            SuspendLayout();
            Name = "Form1";
            Text = "Configurator";
            ResumeLayout(false);
        }
    }
}