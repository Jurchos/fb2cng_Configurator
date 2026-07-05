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
                ["Ok"] = "Save",
                ["Cancel"] = "Cancel",
                ["ErrTitle"] = "Component Missing",
                ["ErrFbc"] = "The GUI program for fb2cng not found: please verify that 'fbc.exe' is present in the application folder!",
                ["ErrGui"] = "Configurator program not found: please verify that 'fb2cng_GUI.exe' is present in the application folder!",
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
                ["Item_Short_Uuid"] = "Shortened ID (_xx), for duplicates",
                ["FootnotesMode"] = "Footnotes display method:",
                ["TocType"] = "Navigation hierarchy type:",
                ["OpenCover"] = "Open book from the cover page",
                ["FixZip"] = "Remove data descriptor (Fix ZIP)",
                ["SaveErrorTitle"] = "Save Error",
                ["ErrReadOnly"] = "Access Denied:\n The file '{0}' is locked!\nPlease check if it is marked as 'Read-Only' or opened in another application.",
                ["ErrAccessDenied"] = "Access Denied:\n Access to the file '{0}' is denied!\nPlease run the application as Administrator.",
                ["SaveSuccessTitle"] = "Success",
                ["SaveSuccess"] = "Configuration successfully saved to {0}!",
                ["YamlTitle"] = "YAML Error",
                ["YamlErr"] = "Error: Key '{0}' not found in template config.yaml!",
                ["HelpText"] = "fb2cng Template Configurator\nDesigned for the fb2cng GUI toolkit." +
                               "\n\nThis application automatically builds a Go template for the fb2cng.exe CLI converter and updates the YAML configuration files." +
                               "\n1. Adjust the required parameters." +
                               "\n2. Use the Constructor to build the folder structure and filename." +
                               "\n3. Click 'Save'." +
                               "\n\nCreated by: Jurchos & Gemini" +
                               "\nVersion: 0.5.1",
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
                ["ErrTitle"] = "Помилка конфігурації",
                ["ErrFbc"] = "Відсутня програма-конвертор: перевірте наявність файлу 'fbc.exe' в папці з програмою!",
                ["ErrGui"] = "Відсутня програма-оболонка для fb2cng: перевірте наявність файлу 'fb2cng_GUI.exe' в папці з програмою!",
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
                ["Item_Short_Uuid"] = "Скорочений ID (_xx), для дублів",
                ["FootnotesMode"] = "Спосіб відображення виносок:",
                ["TocType"] = "Тип навігаційної ієрархії:",
                ["OpenCover"] = "Відкриття книги з титульної сторінки",
                ["FixZip"] = "Вилучити дескриптор даних (Fix ZIP)",
                ["SaveErrorTitle"] = "Помилка збереження",
                ["ErrReadOnly"] = "Помилка доступу:\n Файл '{0}' заблоковано!\nПеревірте, чи не встановлено атрибут 'Тільки для читання', або чи не відкритий він в іншій програмі.",
                ["ErrAccessDenied"] = "Помилка доступу:\n Відмовлено в доступі до файлу '{0}'!\nЗапустіть програму від імені Адміністратора.",
                ["SaveSuccessTitle"] = "Успіх",
                ["SaveSuccess"] = "Конфігурацію успішно збережено у файл {0}!",
                ["YamlTitle"] = "Помилка YAML",
                ["YamlErr"] = "Помилка: Ключ '{0}' не знайдено у файлі config.yaml!",
                ["HelpText"] = "Конфігуратор шаблона fb2cng\nРозроблено для набору інструментів fb2cng GUI." +
                               "\n\nПрограма автоматично збирає Go-шаблон для консольного конвертера fb2cng.exe та модифікує файли конфігурації YAML." +
                               "\n1. Налаштуйте необхідні параметри." +
                               "\n2. Використовуйте конструктор для створення структури папок та імені." +
                               "\n3. Натисніть 'Зберегти.'" +
                               "\n\nСтворено: Jurchos & Gemini" +
                               "\nВерсія: 0.5.1",
                ["GenTitle"] = "Успіх",
                ["GenSuccess"] = "config.yaml успішно згенеровано!"
            };

            // 3. РОСІЙСЬКА ЛОКАЛІЗАЦІЯ
            Localization["Russian"] = new Dictionary<string, string>
            {
                ["Title"] = "Конфигуратор шаблона fb2cng",
                ["Language"] = "Язык:",
                ["DumpConfig"] = "Загрузить дефолтный config.yaml",
                ["ConfigName"] = "Имя пользовательского шаблона:",
                ["CssEnable"] = "CSS-таблица стилей",
                ["Fb2Name"] = "Сохранить имя fb2 для исходного файла",
                ["Help"] = "Справка",
                ["Theme"] = "Тема",
                ["Ok"] = "Сохранить",
                ["Cancel"] = "Отмена",
                ["ErrTitle"] = "Ошибка конфигурации",
                ["ErrFbc"] = "Программа-конвертер не найдена: проверьте наличие файла 'fbc.exe' в папке с программой!",
                ["ErrGui"] = "Программа-оболочка для fb2cng не найдена: проверьте наличие файла 'fb2cng_GUI.exe' в папке с программой!",
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
                ["Item_Short_Uuid"] = "Сокращенный ID (_xx), для дублей",
                ["FootnotesMode"] = "Способ отображения сносок:",
                ["TocType"] = "Тип навигационной иерархии:",
                ["OpenCover"] = "Открытие книги с титульной страницы",
                ["FixZip"] = "Удалить дескриптор данных (Fix ZIP)",
                ["SaveErrorTitle"] = "Ошибка сохранения",
                ["ErrReadOnly"] = "Ошибка доступа:\n Файл '{0}' заблокирован!\nПроверьте, не установлен ли атрибут 'Только для чтения', или не открыт ли он в другой программе.",
                ["ErrAccessDenied"] = "Ошибка доступа:\n Отказано в доступе к файлу '{0}'!\nЗапустите программу от имени Администратора.",
                ["SaveSuccessTitle"] = "Успех",
                ["SaveSuccess"] = "Конфигурация успешно сохранена в файл {0}!",
                ["YamlTitle"] = "Ошибка YAML",
                ["YamlErr"] = "Ошибка: Ключ '{0}' не найден в файле config.yaml!",
                ["HelpText"] = "Конфигуратор шаблона fb2cng\nРазработано для набора инструментов fb2cng GUI." +
                               "\n\nПрограмма автоматически собирает Go-шаблон для консольного конвертера fb2cng.exe и модифицирует файлы конфигурации YAML." +
                               "\n1. Настройте необходимые параметры." +
                               "\n2. Используйте конструктор для создания структуры папок и имени." +
                               "\n3. Нажмите 'Сохранить'." +
                               "\n\nСоздано: Jurchos & Gemini" +
                               "\nВерсия: 0.5.1",
                ["GenTitle"] = "Успех",
                ["GenSuccess"] = "config.yaml успешно сгенерирован!"
            };
        }
    }
}
