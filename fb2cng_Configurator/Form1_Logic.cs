using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace fb2cng_Configurator
{
    public partial class Form1 : Form
    {
        // 1. Керування мовою та локалізацією
        private void LangComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (langComboBox.SelectedIndex)
            {
                case 1: Config.CurrentLanguage = "Ukrainian"; break;
                case 2: Config.CurrentLanguage = "Russian"; break;
                default: Config.CurrentLanguage = "English"; break;
            }
            UpdateLocalization();
            ApplyTheme(); // Перезапуск для оновлення кольорів папок на новій мові
        }

        private void UpdateLocalization()
        {
            var loc = Config.Localization[Config.CurrentLanguage];

            string GetText(string key, string defaultText)
            {
                return loc.ContainsKey(key) ? loc[key] : defaultText;
            }

            this.Text = GetText("Title", "fb2cng Configurator");
            lblLang.Text = GetText("Language", "Language:");
            btnDumpConfig.Text = GetText("DumpConfig", "Dump Default Config");
            lblConfigName.Text = GetText("ConfigName", "Config Name:");
            chkCss.Text = GetText("CssEnable", "Use Custom CSS");
            btnBrowseCss.Text = GetText("Browse", "Browse...");
            chkFb2Name.Text = GetText("Fb2Name", "Use Original FB2 Name");
            btnHelp.Text = GetText("Help", "Help");
            btnTheme.Text = GetText("Theme", "Theme");
            btnOk.Text = GetText("Ok", "OK");
            btnCancel.Text = GetText("Cancel", "Cancel");

            lblOutNameTitle.Text = GetText("OutNameTitle", "Output Name Template Constructor");
            chkTranslit.Text = GetText("Translit", "Transliterate Output Name");
            chkReaderSize.Text = GetText("ReaderSize", "Set Custom Display Size");
            lblWidth.Text = GetText("Width", "W:");
            lblHeight.Text = GetText("Height", "H:");
            lblDpi.Text = GetText("Dpi", "DPI:");

            if (chkNotes != null) chkNotes.Text = GetText("FootnotesMode", "Footnotes Mode");
            if (chkCover != null) chkCover.Text = GetText("TocType", "Cover Mode");
            if (chkOpenFromCover != null) chkOpenFromCover.Text = GetText("OpenCover", "Open from Cover");
            if (chkFixZip != null) chkFixZip.Text = GetText("FixZip", "Fix Broken ZIP Archives");

            if (grpOutName != null) grpOutName.Text = GetText("OutNameTitle", "Output Structure");

            if (chkAsFolder != null)
            {
                for (int i = 0; i < 8; i++)
                    if (chkAsFolder[i] != null) chkAsFolder[i].Text = GetText("AsFolder", "Fold");
            }

            string[] itemKeys = { "Item_Empty", "Item_Author", "Item_Series", "Item_Title", "Item_Lang", "Item_Genre", "Item_Date", "Item_Source", "Item_Uuid" };
            string[] defaultItems = { "", "Author", "Series", "Title", "Language", "Genre", "Date", "Source File", "Book UUID" };

            if (cmbOutFields != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (cmbOutFields[i] == null) continue;

                    int currSel = cmbOutFields[i].SelectedIndex;
                    cmbOutFields[i].Items.Clear();

                    for (int k = 0; k < itemKeys.Length; k++)
                    {
                        cmbOutFields[i].Items.Add(GetText(itemKeys[k], defaultItems[k]));
                    }
                    cmbOutFields[i].SelectedIndex = currSel >= 0 ? currSel : 0;
                }
            }
        }

        // 2. Керування візуальною темою
        private void ApplyTheme()
        {
            if (Config.IsDarkTheme)
            {
                Color darkBg = Color.FromArgb(37, 37, 38);
                Color elementBg = Color.FromArgb(45, 45, 48);
                Color textWhite = Color.FromArgb(245, 245, 245);
                Color limeAccent = Color.Lime;

                this.BackColor = darkBg;
                scrollMenuPanel.BackColor = darkBg;
                footerPanel.BackColor = elementBg;
                grpOutName.BackColor = darkBg;

                SetControlsTheme(this, textWhite, elementBg, limeAccent);
            }
            else
            {
                this.BackColor = SystemColors.Control;
                scrollMenuPanel.BackColor = SystemColors.Window;
                footerPanel.BackColor = SystemColors.ControlLight;
                grpOutName.BackColor = SystemColors.Window;

                SetControlsTheme(this, SystemColors.ControlText, SystemColors.Window, SystemColors.HotTrack);
            }
        }

        private void SetControlsTheme(Control parent, Color foreColor, Color backColor, Color folderColor)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is Label || c is GroupBox)
                {
                    c.ForeColor = foreColor;
                }
                else if (c is CheckBox chk)
                {
                    // Абсолютно надійне визначення через Tag та Name батьківського контейнера
                    if ((chk.Tag != null && chk.Tag.ToString() == "FolderCheckBox") ||
                        (chk.Parent != null && chk.Parent.Name == "templateGrid"))
                        chk.ForeColor = folderColor;
                    else
                        chk.ForeColor = foreColor;
                }
                else if (c is TextBox || c is ComboBox || c is Button)
                {
                    c.ForeColor = foreColor;
                    c.BackColor = backColor;
                }

                if (c.HasChildren)
                {
                    SetControlsTheme(c, foreColor, backColor, folderColor);
                }
            }
        }
        // 3. Взаємодія елементів
        private void BtnBrowseCss_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "CSS Files (*.css)|*.css";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string appPath = AppDomain.CurrentDomain.BaseDirectory;
                    string selectedFile = ofd.FileName;
                    string relativePath = selectedFile.StartsWith(appPath) ? selectedFile.Substring(appPath.Length) : Path.GetFileName(selectedFile);
                    txtCssPath.Text = relativePath.Replace("\\", "/");
                }
            }
        }

        private void ChkFb2Name_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFb2Name.Checked)
            {
                grpOutName.Enabled = false;
                foreach (var cmb in cmbOutFields) { cmb.SelectedIndex = 0; cmb.Enabled = false; }
                foreach (var chk in chkAsFolder) { chk.Checked = false; chk.Enabled = false; }
            }
            else
            {
                grpOutName.Enabled = true;
                cmbOutFields[0].Enabled = true;
                CmbOutFields_SelectedIndexChanged(0);
            }
        }

        private void CmbOutFields_SelectedIndexChanged(int index)
        {
            bool hasSelection = cmbOutFields[index].SelectedIndex > 0;
            chkAsFolder[index].Enabled = hasSelection;
            if (!hasSelection) chkAsFolder[index].Checked = false;

            if (index < 7)
            {
                if (hasSelection)
                {
                    cmbOutFields[index + 1].Enabled = true;
                }
                else
                {
                    for (int i = index + 1; i < 8; i++)
                    {
                        cmbOutFields[i].SelectedIndex = 0;
                        cmbOutFields[i].Enabled = false;
                        chkAsFolder[i].Checked = false;
                        chkAsFolder[i].Enabled = false;
                    }
                }
            }
        }

        // 4. Логіка взаємодії з fbc.exe та файлами YAML (Повністю асинхронний запуск процесу)
        private async void BtnDumpConfig_Click(object sender, EventArgs e)
        {
            string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fbc.exe");

            if (!File.Exists(exePath))
            {
                string key = Config.CurrentLanguage == "Ukrainian" ? "ErrFbc" : "ErrFbc";
                var text = Config.Localization.ContainsKey(Config.CurrentLanguage) && Config.Localization[Config.CurrentLanguage].ContainsKey("ErrFbc")
                    ? Config.Localization[Config.CurrentLanguage]["ErrFbc"]
                    : "Error: fbc.exe not found!";
                MessageBox.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Блокуємо кнопку на час генерації для запобігання подвійним клікам
            btnDumpConfig.Enabled = false;
            string previousText = btnDumpConfig.Text;
            btnDumpConfig.Text = Config.CurrentLanguage == "Ukrainian" ? "Генерація..." : "Generating...";

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = "dumpconfig --default config.yaml",
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                // Запуск процесу в окремому потоці (Інтерфейс програми залишається повністю активним!)
                await Task.Run(() =>
                {
                    using (Process proc = Process.Start(psi))
                    {
                        proc?.WaitForExit();
                    }
                });

                string successMsg = Config.CurrentLanguage == "Ukrainian" ? "Файл config.yaml успішно згенеровано!" : "config.yaml successfully generated!";
                MessageBox.Show(successMsg, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Process Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnDumpConfig.Text = previousText;
                btnDumpConfig.Enabled = true;
            }
        }
        // --- 5. ЗБЕРЕЖЕННЯ ТА ПЕРЕЗАПИС ПАРАМЕТРІВ YAML ---
        private void SaveYamlConfiguration()
        {
            string sourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.yaml");
            string targetFileName = txtConfigName.Text.Trim();

            if (string.IsNullOrEmpty(targetFileName)) targetFileName = "config.yaml";
            if (!targetFileName.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase)) targetFileName += ".yaml";

            string targetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, targetFileName);

            // Якщо дефолтного файлу конфігу немає в папці, пробуємо згенерувати його на льоту
            if (!File.Exists(sourcePath))
            {
                BtnDumpConfig_Click(null, null);
                if (!File.Exists(sourcePath)) return;
            }

            string[] lines = File.ReadAllLines(sourcePath, Encoding.UTF8);

            // 1. Обробка шляху користувацького CSS
            if (chkCss.Checked)
            {
                lines = ReplaceYamlValueLine(lines, "stylesheet_path", $"\"{txtCssPath.Text}\"");
                if (lines == null) return;
            }

            // 2. Обробка прапорця транслітерації назви
            string newTranslitValue = chkTranslit.Checked ? "true" : "false";
            lines = ReplaceYamlValueLine(lines, "file_name_transliterate", newTranslitValue);
            if (lines == null) return;

            // 3. Обробка фізичних розмірів екрана електронної книги
            if (chkReaderSize.Checked)
            {
                lines = ReplaceYamlValueLine(lines, "width", txtWidth.Text); if (lines == null) return;
                lines = ReplaceYamlValueLine(lines, "height", txtHeight.Text); if (lines == null) return;
                lines = ReplaceYamlValueLine(lines, "dpi", txtDpi.Text); if (lines == null) return;
            }

            // 4. Обробка виносок та обкладинок (Виправлено переплутані аліаси ключі-значення!)
            if (chkCover.Checked) { lines = ReplaceYamlValueLine(lines, "toc_type", $"\"{cmbCoverMode.SelectedItem}\""); if (lines == null) return; }
            if (chkNotes.Checked) { lines = ReplaceYamlValueLine(lines, "mode", $"\"{cmbNotesMode.SelectedItem}\""); if (lines == null) return; }
            if (chkOpenFromCover.Checked) { lines = ReplaceYamlValueLine(lines, "open_from_cover", "true"); if (lines == null) return; }
            if (chkFixZip.Checked) { lines = ReplaceYamlValueLine(lines, "fix_zip", "true"); if (lines == null) return; }

            // 5. Формування та запис фінального Go-шаблону для назви книги
            string templateBlock = chkFb2Name.Checked ? "        {{- .OriginalFileName -}}" : BuildGoTemplateFromUI();

            if (!string.IsNullOrEmpty(templateBlock))
            {
                lines = ReplaceOutputTemplateBlockSafely(lines, templateBlock);
                if (lines == null) return;
            }

            // Запис готового масиву рядків у новий конфігураційний файл
            try
            {
                File.WriteAllLines(targetPath, lines, Encoding.UTF8);
                Config.SaveSettings();

                var loc = Config.Localization[Config.CurrentLanguage];
                MessageBox.Show(string.Format(loc["SaveSuccess"], targetFileName), "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string[] ReplaceYamlValueLine(string[] lines, string key, string newValue)
        {
            bool found = false;
            for (int i = 0; i < lines.Length; i++)
            {
                string trimmed = lines[i].TrimStart();

                // Шукаємо закоментовані дефолтні рядки (наприклад: #width: 1072)
                if (trimmed.StartsWith("#"))
                {
                    string withoutComment = trimmed.Substring(1).TrimStart();
                    if (withoutComment.StartsWith(key + ":"))
                    {
                        string padding = lines[i].Substring(0, lines[i].IndexOf('#'));
                        lines[i] = $"{padding}{key}: {newValue}";
                        found = true;
                        break;
                    }
                }
                // Шукаємо розкоментовані активні рядки (наприклад: width: 1264)
                else if (trimmed.StartsWith(key + ":"))
                {
                    string padding = lines[i].Substring(0, lines[i].IndexOf(key));
                    lines[i] = $"{padding}{key}: {newValue}";
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                ShowYamlError(key);
                return null;
            }

            return lines;
        }
        // --- 6. БЕЗПЕЧНА ЗАМІНА БАГАТОРАД КОВОГО БЛОКУ YAML ---
        private string[] ReplaceOutputTemplateBlockSafely(string[] lines, string newTemplateCode)
        {
            System.Collections.Generic.List<string> result = new System.Collections.Generic.List<string>();
            bool blockFound = false;
            bool skipOldBlockMode = false;

            for (int i = 0; i < lines.Length; i++)
            {
                string currentLine = lines[i];
                string trimmed = currentLine.TrimStart();

                // Визначаємо початок секції шаблону назви (активної або закоментованої)
                if (!blockFound && (trimmed.StartsWith("output_name_template:") || (trimmed.StartsWith("#") && trimmed.Substring(1).TrimStart().StartsWith("output_name_template:"))))
                {
                    blockFound = true;
                    int index = currentLine.IndexOf("output_name_template:");
                    if (index == -1) index = currentLine.IndexOf("#");
                    string padding = index > 0 ? currentLine.Substring(0, index) : "";

                    // Записуємо заголовок блоку із символом літералу "|" для Go-шаблонів
                    result.Add($"{padding}output_name_template: |");
                    result.Add(newTemplateCode);
                    skipOldBlockMode = true;
                    continue;
                }

                // Логіка пропуску старого вкладеного вмісту Go-шаблону
                if (skipOldBlockMode)
                {
                    if (string.IsNullOrEmpty(currentLine) || currentLine.Trim().Length == 0) continue;

                    int leadingSpaces = currentLine.Length - currentLine.TrimStart().Length;

                    // Якщо відступи стали меншими за 8 пробілів — це кінець блоку шаблону
                    if (leadingSpaces < 8)
                    {
                        skipOldBlockMode = false;
                    }
                    else
                    {
                        continue; // Пропускаємо старий рядок Go-шаблону
                    }
                }

                result.Add(currentLine);
            }

            if (!blockFound)
            {
                ShowYamlError("output_name_template");
                return null;
            }

            return result.ToArray();
        }

        // --- 7. ГЕНЕРАТОР СКЛАДНИХ GO-ШАБЛОНІВ ДЛЯ FB2CNG ---
        private string BuildGoTemplateFromUI()
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;

            for (int i = 0; i < 8; i++)
            {
                int selIndex = cmbOutFields[i].SelectedIndex;
                if (selIndex <= 0) break; // Зупиняємося, якщо зустріли порожній елемент "[Не вибрано]"

                string chunk = "";

                switch (selIndex)
                {
                    case 1: // Автор книги (розумний вивід прізвища та ініціалів + ", et al" / "и др")
                        chunk = "{{- $author := \"\" -}}{{- if gt (len .Authors) 0 -}}{{- with first .Authors -}}{{- if .LastName -}}{{- $author = .LastName -}}{{- if .FirstName }}{{ $author = printf \"%s %s\" $author .FirstName }}{{ end -}}{{- if .MiddleName }}{{ $author = printf \"%s %s\" $author .MiddleName }}{{ end -}}{{- else if .Nickname -}}{{- $author = .Nickname -}}{{- end -}}{{- end -}}{{- if gt (len .Authors) 1 -}}{{- if eq .Language \"ru\" -}}{{- $author = printf \"%s %s\" $author \"и др\" -}}{{- else -}}{{- $author = printf \"%s %s\" $author \", et al\" -}}{{- end -}}{{- end -}}{{- end -}}{{- if $author }}{{ printf \"%s\" $author }}{{ end -}}";
                        break;
                    case 2: // Чиста назва серії (якщо книга входить у серію)
                        chunk = "{{- if gt (len .Series) 0 -}}{{- with first .Series -}}{{ .Name }}{{- end -}}{{- end -}}";
                        break;
                    case 3: // Назва книги + Двозначний номер у серії попереду (наприклад: "02 Назва")
                        chunk = "{{- if gt (len .Series) 0 -}}{{- with first .Series -}}{{- if .Number }}{{ printf \"%02d \" .Number }}{{- end -}}{{- end -}}{{- end -}}{{- .Title -}}";
                        break;
                    case 4: // Дволітерний або трилітерний код мови книги
                        chunk = "{{- .Language -}}";
                        break;
                    case 5: // Головний жанр книги
                        chunk = "{{- if gt (len .Genres) 0 -}}{{ index .Genres 0 }}{{- end -}}";
                        break;
                    case 6: // Текстова дата створення чи публікації
                        chunk = "{{- .Date -}}";
                        break;
                    case 7: // Оригінальне базове ім'я вхідного файлу
                        chunk = "{{- .SourceFile -}}";
                        break;
                    case 8: // Унікальний ідентифікатор книги UUID
                        chunk = "{{- .BookID -}}";
                        break;
                }

                if (!string.IsNullOrEmpty(chunk))
                {
                    // Додаємо дефіс як роздільник елементів, якщо попередній елемент не був папкою
                    if (!isFirst && !chkAsFolder[i - 1].Checked)
                    {
                        sb.Append(" - ");
                    }

                    sb.Append(chunk);

                    // Якщо стоїть прапорець "як папка", створюємо вкладену директорію в структурі файлу
                    if (chkAsFolder[i].Checked)
                    {
                        sb.Append("/");
                    }

                    isFirst = false;
                }
            }

            if (sb.Length > 0)
            {
                return "        " + sb.ToString();
            }

            return "";
        }

        // --- 8. ДІАЛОГОВІ МОДЛЬНІ ВІКНА (ПОВНІСТЮ ЛОКАЛІЗОВАНІ) ---
        private void ShowYamlError(string key)
        {
            string errMsg = Config.CurrentLanguage == "Ukrainian"
                ? $"Помилка: Параметр '{key}' не знайдено в оригінальному файлі config.yaml!"
                : (Config.CurrentLanguage == "Russian"
                    ? $"Ошибка: Параметр '{key}' не найден в оригинальном файле config.yaml!"
                    : $"Error: Parameter '{key}' was not found in the original config.yaml file!");

            MessageBox.Show(errMsg, "YAML Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowHelp()
        {
            string helpMessage = Config.CurrentLanguage == "Ukrainian"
                ? "fb2cng Конфігуратор шаблонів\nРозроблено для набору інструментів fb2cng GUI.\n\nПрограма автоматично збирає правильні та валідні Go-шаблони для консольного конвертера fb2cng.exe та модифікує файли конфігурації YAML без порушення їх структури."
                : (Config.CurrentLanguage == "Russian"
                    ? "fb2cng Конфигуратор шаблонов\nРазработано для набора инструментов fb2cng GUI.\n\nПрограмма автоматически собирает правильные и валидные Go-шаблоны для консольного конвертера fb2cng.exe и модифицирует файлы конфигурации YAML без нарушения их структуры."
                    : "fb2cng Template Configurator\nCreated for fb2cng GUI toolset.\n\nThis application automatically builds correct and valid Go templates for the fb2cng.exe engine and modifies YAML configuration files without corrupting their layout.");

            MessageBox.Show(helpMessage, "Help / Довідка", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
