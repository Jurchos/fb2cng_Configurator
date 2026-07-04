using System;
using System.Collections.Generic;
using System.IO;

namespace fb2cng_Configurator
{
    public static class Config
    {
        public static string CurrentLanguage { get; set; } = "English";
        public static bool IsDarkTheme { get; set; } = false;

        private static readonly string settingsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_settings.txt");

        public static void SaveSettings()
        {
            try
            {
                File.WriteAllLines(settingsFile, new string[] {
                    CurrentLanguage,
                    IsDarkTheme.ToString()
                });
            }
            catch { }
        }

        public static void LoadSettings()
        {
            try
            {
                if (File.Exists(settingsFile))
                {
                    string[] lines = File.ReadAllLines(settingsFile);
                    if (lines != null && lines.Length >= 2)
                    {
                        if (!string.IsNullOrEmpty(lines[0]))
                        {
                            CurrentLanguage = lines[0];
                        }

                        if (bool.TryParse(lines[1], out bool dark))
                        {
                            IsDarkTheme = dark;
                        }
                    }
                }
            }
            catch
            {
                CurrentLanguage = "English";
                IsDarkTheme = false;
            }
        }

        public static Dictionary<string, Dictionary<string, string>> Localization = new Dictionary<string, Dictionary<string, string>>();

        static Config()
        {
            // 1. АНГЛІЙСЬКА ЛОКАЛІЗАЦІЯ
            Localization["English"] = new Dictionary<string, string>
            {
                ["Title"] = "fb2cng Template Configurator",
                ["Language"] = "Language:",
                ["DumpConfig"] = "Load Default config.yaml",
                ["ConfigName"] = "Name of custom template:",
                ["CssEnable"] = "Use Custom CSS Stylesheet",
                ["Fb2Name"] = "Use the fb2 filename for the source file",
                ["Help"] = "Help",
                ["Theme"] = "Theme",
                ["Ok"] = "OK",
                ["Cancel"] = "Cancel",
                ["ErrTitle"] = "Error",
                ["ErrFbc"] = "Error: fbc.exe not found in the application folder!",
                ["OutNameTitle"] = "Output File Name Structure",
                ["AsFolder"] = "as a folder",
                ["Translit"] = "Transliterate output filename",
                ["ReaderSize"] = "Reader screen size (W / H / DPI)",
                ["Width"] = "W:",
                ["Height"] = "H:",
                ["Dpi"] = "DPI:",
                ["Item_Empty"] = "[Not selected]",
                ["Item_Author"] = "Author (.Authors)",
                ["Item_Series"] = "Series (.Series)",
                ["Item_Title"] = "Book Title (.Title)",
                ["Item_Lang"] = "Language (.Language)",
                ["Item_Genre"] = "Genre (.Genres)",
                ["Item_Date"] = "Date (.Date)",
                ["Item_Source"] = "Source File (.SourceFile)",
                ["Item_Uuid"] = "Book UUID (.BookID)",
                ["Short_id"] = "Shortened book ID (_xx)",
                ["FootnotesMode"] = "Footnotes display method:",
                ["TocType"] = "Navigation hierarchy type:",
                ["OpenCover"] = "Open book from the cover page",
                ["FixZip"] = "Remove data descriptor (Fix ZIP)",
                ["SaveSuccessTitle"] = "Success",
                ["SaveErrorTitle"] = "Save Error",
                ["ErrAccessDenied"] = "Access Denied: The file '{0}' is locked!\nPlease check if it is marked as 'Read-Only' or opened in another application.",
                ["SaveSuccess"] = "Configuration successfully saved to {0}!",
                ["YamlTitle"] = "YAML Error",
                ["YamlErr"] = "Error: Key '{0}' not found in template config.yaml!",
                ["HelpText"] = "fb2cng Configurator Help:\n\n1. Select your options." +
                                                          "\n2. Use the Name Constructor to build the file structure." +
                                                          "\n3. Click OK to save." +
                                                          "\n\nCreated by Jurchos & Gemini" +
                                                          "\nVersion: 0.3.",
                ["GenTitle"] = "Success",
                ["GenSuccess"] = "config.yaml successfully generated!"
            };

            // 2. УКРАЇНСЬКА ЛОКАЛІЗАЦІЯ
            Localization["Ukrainian"] = new Dictionary<string, string>
            {
                ["Title"] = "Конфігуратор шаблона fb2cng",
                ["Language"] = "Мова:",
                ["DumpConfig"] = "Завантажити дефолтний config.yaml",
                ["ConfigName"] = "Назва власного шаблона:",
                ["CssEnable"] = "CSS-таблиця стилів",
                ["Fb2Name"] = "Залишити назву fb2 для вихідного файла",
                ["Help"] = "Довідка",
                ["Theme"] = "Тема",
                ["Ok"] = "Зберегти",
                ["Cancel"] = "Скасувати",
                ["ErrTitle"] = "Помилка",
                ["ErrFbc"] = "Помилка: fbc.exe не знайдено в папці з програмою!",
                ["OutNameTitle"] = "Структура назви вихідного файла",
                ["AsFolder"] = "як папка",
                ["Translit"] = "Транслітерувати назву вихідного файлу",
                ["ReaderSize"] = "Розмір екрана рідера (Ш / В / DPI)",
                ["Width"] = "W:",
                ["Height"] = "H:",
                ["Dpi"] = "DPI:",
                ["Item_Empty"] = "[Не вибрано]",
                ["Item_Author"] = "Автор (.Authors)",
                ["Item_Series"] = "Серія (.Series)",
                ["Item_Title"] = "Назва книги (.Title)",
                ["Item_Lang"] = "Мова (.Language)",
                ["Item_Genre"] = "Жанр (.Genres)",
                ["Item_Date"] = "Дата (.Date)",
                ["Item_Source"] = "Базова назва файла (.SourceFile)",
                ["Item_Uuid"] = "UUID книги (.BookID)",
                ["Short_id"] = "Скорочений ID книги (_xx)",
                ["FootnotesMode"] = "Спосіб відображення виносок:",
                ["TocType"] = "Тип навігаційної ієрархії:",
                ["OpenCover"] = "Відкриття книги з титульної сторінки",
                ["FixZip"] = "Вилучити дескриптор даних (Fix ZIP)",
                ["SaveSuccessTitle"] = "Успіх",
                ["SaveErrorTitle"] = "Помилка збереження",
                ["ErrAccessDenied"] = "Помилка доступу: Файл '{0}' заблоковано!\nПеревірте, чи не встановлено атрибут 'Тільки для читання', або чи не відкритий він в іншій програмі.",
                ["SaveSuccess"] = "Конфігурацію успішно збережено у файл {0}!",
                ["YamlTitle"] = "Помилка YAML",
                ["YamlErr"] = "Помилка: Ключ '{0}' не знайдено у файлі config.yaml!",
                ["HelpText"] = "Довідка конфігуратора fb2cng:\n\n1. Налаштуйте необхідні параметри." +
                                                              "\n2. Використовуйте конструктор для створення структури папок та імені." +
                                                              "\n3. Натисніть ОК для збереження." +
                                                              "\n\nСтворено: Jurchos & Gemini" +
                                                              "\nВерсія: 0.3",
                ["GenTitle"] = "Успіх",
                ["GenSuccess"] = "config.yaml успішно згенеровано!"
            };

            // 3. РОСІЙСЬКА ЛОКАЛІЗАЦІЯ
            Localization["Russian"] = new Dictionary<string, string>
            {
                ["Title"] = "Конфигуратор шаблонов fb2cng",
                ["Language"] = "Язык:",
                ["DumpConfig"] = "Загрузить дефолтный config.yaml",
                ["ConfigName"] = "Имя пользовательского шаблона:",
                ["CssEnable"] = "CSS-таблица стилей",
                ["Fb2Name"] = "Сохранить имя fb2 для исходного файла",
                ["Help"] = "Справка",
                ["Theme"] = "Тема",
                ["Ok"] = "ОК",
                ["Cancel"] = "Отмена",
                ["ErrTitle"] = "Ошибка",
                ["ErrFbc"] = "Ошибка: fbc.exe не найден в папке с программой!",
                ["OutNameTitle"] = "Структура имени выходного файла",
                ["AsFolder"] = "как папка",
                ["Translit"] = "Транслитерировать имя выходного файла",
                ["ReaderSize"] = "Размер экрана ридера (Ш/В/DPI)",
                ["Width"] = "W:",
                ["Height"] = "H:",
                ["Dpi"] = "DPI:",
                ["Item_Empty"] = "[Не выбрано]",
                ["Item_Author"] = "Автор (.Authors)",
                ["Item_Series"] = "Серия (.Series)",
                ["Item_Title"] = "Название книги (.Title)",
                ["Item_Lang"] = "Язык (.Language)",
                ["Item_Genre"] = "Жанр (.Genres)",
                ["Item_Date"] = "Дата (.Date)",
                ["Item_Source"] = "Базовое имя файла (.SourceFile)",
                ["Item_Uuid"] = "UUID книги (.BookID)",
                ["Short_id"] = "Сокращенный ID книги (_xx)",
                ["FootnotesMode"] = "Способ отображения сносок:",
                ["TocType"] = "Тип навигационной иерархии:",
                ["OpenCover"] = "Открытие книги с титульной страницы",
                ["FixZip"] = "Удалить дескриптор данных (Fix ZIP)",
                ["SaveSuccessTitle"] = "Успех",
                ["SaveErrorTitle"] = "Ошибка сохранения",
                ["ErrAccessDenied"] = "Ошибка доступа: Файл '{0}' заблокирован!\nПроверьте, не установлен ли атрибут 'Только для чтения', или не открыт ли он в другой программе.",
                ["SaveSuccess"] = "Конфигурация успешно сохранена в файл {0}!",
                ["YamlTitle"] = "Ошибка YAML",
                ["YamlErr"] = "Ошибка: Ключ '{0}' не найден в файле config.yaml!",
                ["HelpText"] = "Справка конфигуратора fb2cng:\n\n1. Настройте необходимые параметры." +
                                                               "\n2. Используйте конструктор для создания структуры папок и имени." +
                                                               "\n3. Нажмите ОК для сохранения." +
                                                               "\n\nСоздано: Jurchos & Gemini" +
                                                               "\nВерсия: 0.3",
                ["GenTitle"] = "Успех",
                ["GenSuccess"] = "config.yaml успешно сгенерирован!"
            };
        }
    }
}
