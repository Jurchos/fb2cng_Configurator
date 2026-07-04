using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace fb2cng_Configurator
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Налаштування для High DPI масштабування на .NET 4.8
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDPIAware();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        // ВНУТРІШНЯ БІЗНЕС-ЛОГІКА ПРОГРАМИ (РОБОТА З YAML ТА ПРОЦЕСАМИ)
        public static class YamlService
        {
            private static string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fbc.exe");
            private static string sourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.yaml");

            public static bool IsEngineAvailable()
            {
                return File.Exists(exePath);
            }

            public static bool ExecuteSyncDumpConfig()
            {
                if (!IsEngineAvailable()) return false;

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

                    using (Process proc = Process.Start(psi))
                    {
                        if (proc != null)
                        {
                            // Очікуємо завершення максимум 5 секунд (5000 мілісекунд)
                            if (!proc.WaitForExit(5000))
                            {
                                // Якщо fbc.exe завис або не встиг, примусово вбиваємо його дочірній процес
                                proc.Kill();
                                return false;
                            }
                        }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public static bool SaveConfiguration(
                string configName, bool useCss, string cssPath, bool translit,
                bool customSize, string width, string height, string dpi,
                bool useCoverMode, string coverMode, bool useNotesMode, string notesMode,
                bool openFromCover, bool fixZip, bool useFb2Name,
                int[] fieldIndexes, bool[] folderFlags)
            {
                string targetFileName = configName?.Trim();
                if (string.IsNullOrEmpty(targetFileName)) targetFileName = "config.yaml";
                if (!targetFileName.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase)) targetFileName += ".yaml";

                string targetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, targetFileName);

                if (!File.Exists(sourcePath))
                {
                    bool generated = ExecuteSyncDumpConfig();
                    if (!generated || !File.Exists(sourcePath)) return false;
                }

                try
                {
                    string[] lines = File.ReadAllLines(sourcePath, Encoding.UTF8);

                    if (useCss)
                    {
                        lines = ReplaceYamlValueLine(lines, "stylesheet_path", $"\"{cssPath}\"");
                        if (lines == null) return false;
                    }

                    string newTranslitValue = translit ? "true" : "false";
                    lines = ReplaceYamlValueLine(lines, "file_name_transliterate", newTranslitValue);
                    if (lines == null) return false;
                    if (customSize)
                    {
                        lines = ReplaceYamlValueLine(lines, "width", width); if (lines == null) return false;
                        lines = ReplaceYamlValueLine(lines, "height", height); if (lines == null) return false;
                        lines = ReplaceYamlValueLine(lines, "dpi", dpi); if (lines == null) return false;
                    }

                    if (useCoverMode) { lines = ReplaceYamlValueLine(lines, "toc_type", $"\"{coverMode}\""); if (lines == null) return false; }
                    if (useNotesMode) { lines = ReplaceYamlValueLine(lines, "mode", $"\"{notesMode}\""); if (lines == null) return false; }
                    if (openFromCover) { lines = ReplaceYamlValueLine(lines, "open_from_cover", "true"); if (lines == null) return false; }
                    if (fixZip) { lines = ReplaceYamlValueLine(lines, "fix_zip", "true"); if (lines == null) return false; }

                    string templateBlock = useFb2Name ? "        {{- .OriginalFileName -}}" : BuildGoTemplateFromUI(fieldIndexes, folderFlags);

                    if (!string.IsNullOrEmpty(templateBlock))
                    {
                        lines = ReplaceOutputTemplateBlockSafely(lines, templateBlock);
                        if (lines == null) return false;
                    }

                    File.WriteAllLines(targetPath, lines, Encoding.UTF8);
                    Config.SaveSettings();

                    // Викликаємо вікна через тимчасову форму (Варіант 1)
                    using (var tempForm = new Form1())
                    {
                        var loc = Config.Localization[Config.CurrentLanguage];
                        string successCaption = loc.ContainsKey("SaveSuccessTitle") ? loc["SaveSuccessTitle"] : "Success";
                        tempForm.ShowCustomMessageBox(string.Format(loc["SaveSuccess"], targetFileName), successCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    return true;
                }
                catch (UnauthorizedAccessException) // ПЕРЕХОПЛЮЄМО САМЕ ПОМИЛКУ ДОСТУПУ
                {
                    using (var tempForm = new Form1())
                    {
                        var loc = Config.Localization[Config.CurrentLanguage];
                        string errorCaption = loc.ContainsKey("SaveErrorTitle") ? loc["SaveErrorTitle"] : "Save Error";

                        // Беремо локалізований текст відмови в доступі з Config.cs
                        string accessDeniedMsg = loc.ContainsKey("ErrAccessDenied")
                            ? string.Format(loc["ErrAccessDenied"], targetFileName)
                            : $"Access to the file '{targetFileName}' is denied! Run as Admin.";

                        tempForm.ShowCustomMessageBox(accessDeniedMsg, errorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return false;
                }
                catch (Exception ex) // Для всіх інших критичних помилок
                {
                    using (var tempForm = new Form1())
                    {
                        var loc = Config.Localization[Config.CurrentLanguage];
                        string errorCaption = loc.ContainsKey("SaveErrorTitle") ? loc["SaveErrorTitle"] : "Save Error";
                        tempForm.ShowCustomMessageBox(ex.Message, errorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return false;
                }
            }

            private static string[] ReplaceYamlValueLine(string[] lines, string key, string newValue)
            {
                bool found = false;
                for (int i = 0; i < lines.Length; i++)
                {
                    string trimmed = lines[i].TrimStart();

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
            private static string[] ReplaceOutputTemplateBlockSafely(string[] lines, string newTemplateCode)
            {
                System.Collections.Generic.List<string> result = new System.Collections.Generic.List<string>();
                bool blockFound = false;
                bool skipOldBlockMode = false;

                for (int i = 0; i < lines.Length; i++)
                {
                    string currentLine = lines[i];
                    string trimmed = currentLine.TrimStart();

                    if (!blockFound && (trimmed.StartsWith("output_name_template:") || (trimmed.StartsWith("#") && trimmed.Substring(1).TrimStart().StartsWith("output_name_template:"))))
                    {
                        blockFound = true;
                        int index = currentLine.IndexOf("output_name_template:");
                        if (index == -1) index = currentLine.IndexOf("#");
                        string padding = index > 0 ? currentLine.Substring(0, index) : "";

                        result.Add($"{padding}output_name_template: |");
                        result.Add(newTemplateCode);
                        skipOldBlockMode = true;
                        continue;
                    }

                    if (skipOldBlockMode)
                    {
                        if (string.IsNullOrEmpty(currentLine) || currentLine.Trim().Length == 0) continue;

                        int leadingSpaces = currentLine.Length - currentLine.TrimStart().Length;
                        if (leadingSpaces < 8) skipOldBlockMode = false;
                        else continue;
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

            private static string BuildGoTemplateFromUI(int[] fieldIndexes, bool[] folderFlags)
            {
                StringBuilder sb = new StringBuilder();
                bool isFirst = true;

                for (int i = 0; i < 8; i++)
                {
                    int selIndex = fieldIndexes[i];
                    if (selIndex <= 0) break;

                    string chunk = "";
                    switch (selIndex)
                    {
                        case 1:
                            chunk = "{{- $author := \"\" -}}{{- if gt (len .Authors) 0 -}}{{- with first .Authors -}}{{- if .LastName -}}{{- $author = .LastName -}}{{- if .FirstName }}{{ $author = printf \"%s %s\" $author .FirstName }}{{ end -}}{{- if .MiddleName }}{{ $author = printf \"%s %s\" $author .MiddleName }}{{ end -}}{{- else if .Nickname -}}{{- $author = .Nickname -}}{{- end -}}{{- end -}}{{- if gt (len .Authors) 1 -}}{{- if eq .Language \"ru\" -}}{{- $author = printf \"%s %s\" $author \"и др\" -}}{{- else -}}{{- $author = printf \"%s %s\" $author \", et al\" -}}{{- end -}}{{- end -}}{{- end -}}{{- if $author }}{{ printf \"%s\" $author }}{{ end -}}";
                            break;
                        case 2:
                            chunk = "{{- if gt (len .Series) 0 -}}{{- with first .Series -}}{{ .Name }}{{- end -}}{{- end -}}";
                            break;
                        case 3:
                            chunk = "{{- if gt (len .Series) 0 -}}{{- with first .Series -}}{{- if .Number }}{{ printf \"%02d \" .Number }}{{- end -}}{{- end -}}{{- end -}}{{- .Title -}}";
                            break;
                        case 4:
                            chunk = "{{- .Language -}}";
                            break;
                        case 5:
                            chunk = "{{- if gt (len .Genres) 0 -}}{{ index .Genres 0 }}{{- end -}}";
                            break;
                        case 6:
                            chunk = "{{- .Date -}}";
                            break;
                        case 7:
                            chunk = "{{- .SourceFile -}}";
                            break;
                        case 8:
                            chunk = "{{- .BookID -}}";
                            break;
                    }

                    if (!string.IsNullOrEmpty(chunk))
                    {
                        if (!isFirst && !folderFlags[i - 1]) sb.Append(" - ");
                        sb.Append(chunk);
                        if (folderFlags[i]) sb.Append("/");
                        isFirst = false;
                    }
                }

                return sb.Length > 0 ? "        " + sb.ToString() : "";
            }

            private static void ShowYamlError(string key)
            {
                using (var tempForm = new Form1())
                {
                    var loc = Config.Localization[Config.CurrentLanguage];

                    // 1. Отримуємо локалізований заголовок ("YAML Error", "Помилка YAML" тощо)
                    string caption = loc.ContainsKey("YamlTitle") ? loc["YamlTitle"] : "YAML Error";

                    // 2. Отримуємо локалізований текст помилки з автоматичною підстановкою назви параметра
                    string errMsg = loc.ContainsKey("YamlErr")
                        ? string.Format(loc["YamlErr"], key)
                        : $"Error: Parameter '{key}' was not found in the original config.yaml file!";

                    // 3. Викликаємо наше кастомне вікно з компактною іконкою по центру
                    tempForm.ShowCustomMessageBox(errMsg, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
