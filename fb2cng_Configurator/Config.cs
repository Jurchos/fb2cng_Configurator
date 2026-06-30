using System;
using System.Collections.Generic;
using System.IO;

namespace fb2cng_Configurator
{
    public static class Config
    {
        public static string CurrentLanguage { get; set; } = "English";
        public static bool IsDarkTheme { get; set; } = false;

        // Зберігатимемо відсоток від ширини та висоти екрана (дефолтні пропорції)
        public static float WindowWidthPct { get; set; } = 0.35f;  // ~35% ширини екрана
        public static float WindowHeightPct { get; set; } = 0.70f; // ~70% висоти екрана

        private static string settingsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_settings.txt");

        // Безпечне збереження налаштувань у папку користувача, якщо немає прав доступу до Program Files
        public static void SaveSettings()
        {
            try
            {
                // Повертаємо використання вашої рідної змінної settingsFile
                File.WriteAllLines(settingsFile, new string[] {
                    CurrentLanguage,
                    IsDarkTheme.ToString(),
                    WindowWidthPct.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    WindowHeightPct.ToString(System.Globalization.CultureInfo.InvariantCulture)
                });
            }
            catch { }
        }

        public static void LoadSettings()
        {
            try
            {
                // Повертаємо використання вашої рідної змінної settingsFile
                if (File.Exists(settingsFile))
                {
                    string[] lines = File.ReadAllLines(settingsFile);
                    if (lines != null && lines.Length >= 4)
                    {
                        if (!string.IsNullOrEmpty(lines[0])) CurrentLanguage = lines[0];
                        if (bool.TryParse(lines[1], out bool dark)) IsDarkTheme = dark;

                        // Виправлено простір імен: використовуємо NumberStyles замість Formatting
                        if (float.TryParse(lines[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float w)) WindowWidthPct = w;
                        if (float.TryParse(lines[3], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float h)) WindowHeightPct = h;
                    }
                }
            }
            catch
            {
                CurrentLanguage = "English";
                IsDarkTheme = false;
                WindowWidthPct = 0.35f;
                WindowHeightPct = 0.70f;
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
                ["ConfigName"] = "Config File Name:",
                ["CssEnable"] = "Use Custom CSS Stylesheet",
                ["Browse"] = "Browse...",
                ["Fb2Name"] = "Use input fb2 filename for output",
                ["Help"] = "Help",
                ["Theme"] = "Theme",
                ["Ok"] = "OK",
                ["Cancel"] = "Cancel",
                ["ErrFbc"] = "Error: fbc.exe not found in the application folder!",
                ["OutNameTitle"] = "Output File Name Structure",
                ["AsFolder"] = "Fold",
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
                ["FootnotesMode"] = "Footnotes display method:",
                ["TocType"] = "Navigation hierarchy type:",
                ["OpenCover"] = "Open book from the cover page",
                ["FixZip"] = "Remove data descriptor (Fix ZIP)",
                ["SaveSuccess"] = "Configuration successfully saved to {0}!",
                ["YamlErr"] = "Error: Key '{0}' not found in template config.yaml!",
                ["HelpText"] = "fb2cng Configurator Help:\n\n1. Select your options.\n2. Use the Name Constructor to build the file structure.\n3. Click OK to save.",
                ["GenSuccess"] = "config.yaml successfully generated!"
            };

            // 2. УКРАЇНСЬКА ЛОКАЛІЗАЦІЯ (Виправлено ключ на Ukrainian)
            Localization["Ukrainian"] = new Dictionary<string, string>
            {
                ["Title"] = "Конфігуратор шаблонів fb2cng",
                ["Language"] = "Мова:",
                ["DumpConfig"] = "Завантажити дефолтний config.yaml",
                ["ConfigName"] = "Назва файлу налаштувань:",
                ["CssEnable"] = "CSS-таблиця стилів",
                ["Browse"] = "Огляд...",
                ["Fb2Name"] = "Використовувати ім'я файлу fb2 для вихідного",
                ["Help"] = "Довідка",
                ["Theme"] = "Тема",
                ["Ok"] = "ОК",
                ["Cancel"] = "Скасувати",
                ["ErrFbc"] = "Помилка: fbc.exe не знайдено в папці з програмою!",
                ["OutNameTitle"] = "Структура назви вихідного файла",
                ["AsFolder"] = "Папка",
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
                ["FootnotesMode"] = "Спосіб відображення виносок:",
                ["TocType"] = "Тип навігаційної ієрархії:",
                ["OpenCover"] = "Відкриття книги з титульної сторінки",
                ["FixZip"] = "Вилучити дескриптор даних (Fix ZIP)",
                ["SaveSuccess"] = "Конфігурацію успішно збережено у файл {0}!",
                ["YamlErr"] = "Помилка: Ключ '{0}' не знайдено у файлі config.yaml!",
                ["HelpText"] = "Довідка конфігуратора fb2cng:\n\n1. Налаштуйте необхідні параметри.\n2. Використовуйте конструктор для створення структури папок та імені.\n3. Натисніть ОК для збереження.",
                ["GenSuccess"] = "config.yaml успішно згенеровано!"
            };

            // 3. РОСІЙСЬКА ЛОКАЛІЗАЦІЯ
            Localization["Russian"] = new Dictionary<string, string>
            {
                ["Title"] = "Конфигуратор шаблонов fb2cng",
                ["Language"] = "Язык:",
                ["DumpConfig"] = "Загрузить дефолтный config.yaml",
                ["ConfigName"] = "Название файла настроек:",
                ["CssEnable"] = "CSS-таблица стилей",
                ["Browse"] = "Обзор...",
                ["Fb2Name"] = "Использовать имя файла fb2 для выходного",
                ["Help"] = "Справка",
                ["Theme"] = "Тема",
                ["Ok"] = "ОК",
                ["Cancel"] = "Отмена",
                ["ErrFbc"] = "Ошибка: fbc.exe не найден в папке с программой!",
                ["OutNameTitle"] = "Структура названия выходного файла",
                ["AsFolder"] = "Папка",
                ["Translit"] = "Транслитерировать название выходного файла",
                ["ReaderSize"] = "Размер экрана ридера (Ш / В / DPI)",
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
                ["Item_Source"] = "Базовое название файла (.SourceFile)",
                ["Item_Uuid"] = "UUID книги (.BookID)",
                ["FootnotesMode"] = "Способ отображения сносок:",
                ["TocType"] = "Тип навигационной иерархии:",
                ["OpenCover"] = "Открытие книги с титульной страницы",
                ["FixZip"] = "Удалить дескриптор данных (Fix ZIP)",
                ["SaveSuccess"] = "Конфигурация успешно сохранена в файл {0}!",
                ["YamlErr"] = "Ошибка: Ключ '{0}' не найден в файле config.yaml!",
                ["HelpText"] = "Справка конфигуратора fb2cng:\n\n1. Настройте необходимые параметры.\n2. Используйте конструктор для создания структуры папок и имени.\n3. Нажмите ОК для сохранения.",
                ["GenSuccess"] = "config.yaml успешно сгенерирован!"
            };
        }
    }
}
