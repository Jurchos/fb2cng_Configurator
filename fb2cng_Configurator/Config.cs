using System;
using System.Collections.Generic;
using System.IO;

namespace fb2cng_Configurator
{
    public static class Config
    {
        public static string CurrentLanguage { get; set; } = "English";
        public static bool IsDarkTheme { get; set; } = false;

        private static string settingsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_settings.txt");

        // Метод збереження налаштувань самої оболонки конфігуратора
        public static void SaveSettings()
        {
            try
            {
                File.WriteAllLines(settingsFile, new string[] { CurrentLanguage, IsDarkTheme.ToString() });
            }
            catch { }
        }

        // Захищений метод завантаження налаштувань, повністю сумісний з C# 7.3
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

                        bool dark;
                        if (bool.TryParse(lines[1], out dark))
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

        public static Dictionary<string, Dictionary<string, string>> Localization = new Dictionary<string, Dictionary<string, string>>
        {
            { "English", new Dictionary<string, string> {
                { "Title", "fb2cng Template Configurator" },
                { "Language", "Language" },
                { "DumpConfig", "Load Default config.yaml" },
                { "ConfigName", "Config File Name" },
                { "CssEnable", "Use CSS Stylesheet" },
                { "Browse", "Browse..." },
                { "Fb2Name", "Use input fb2 filename for output" },
                { "Help", "Help" },
                { "Theme", "Theme" },
                { "Ok", "OK" },
                { "Cancel", "Cancel" },
                { "ErrFbc", "Error: fbc.exe not found in the application folder!" },
                { "OutNameTitle", "Output File Name Structure" },
                { "AsFolder", "as folder" },
                { "Translit", "Transliterate output filename" },
                { "ReaderSize", "Reader screen size" },
                { "Width", "Width:" },
                { "Height", "Height:" },
                { "Dpi", "DPI:" },
                { "Item_Empty", "[Not selected]" },
                { "Item_Author", "Author (.Authors)" },
                { "Item_Series", "Series (.Series)" },
                { "Item_Title", "Book Title (.Title)" },
                { "Item_Lang", "Language (.Language)" },
                { "Item_Genre", "Genre (.Genres)" },
                { "Item_Date", "Date (.Date)" },
                { "Item_Source", "Source File (.SourceFile)" },
                { "Item_Uuid", "Book UUID (.BookID)" },
                { "CoverMode", "Footnotes display method:" },
                { "NotesMode", "Navigation hierarchy type:" },
                { "OpenFromCover", "Open book from the cover page" },
                { "FixZip", "Remove data descriptor (Fix ZIP)" },
                { "EmbedFonts", "Embed fonts into book" },
                { "SaveSuccess", "Configuration successfully saved to {0}!" }
            }},
            { "Ukrainan", new Dictionary<string, string> {
                { "Title", "Конфігуратор шаблонів fb2cng" },
                { "Language", "Мова" },
                { "DumpConfig", "Завантажити config.yaml" },
                { "ConfigName", "Назва файлу налаштувань" },
                { "CssEnable", "CSS-таблиця стилів" },
                { "Browse", "Огляд..." },
                { "Fb2Name", "Використовувати ім'я файлу fb2 для вихідного" },
                { "Help", "Довідка" },
                { "Theme", "Тема" },
                { "Ok", "ОК" },
                { "Cancel", "Скасувати" },
                { "ErrFbc", "Помилка: fbc.exe не знайдено в папці з програмою!" },
                { "OutNameTitle", "Назва вихідного файла" },
                { "AsFolder", "як папка" },
                { "Translit", "Транслітерувати назву вихідного файлу" },
                { "ReaderSize", "Розмір екрана рідера" },
                { "Width", "Ширина:" },
                { "Height", "Висота:" },
                { "Dpi", "DPI:" },
                { "Item_Empty", "[Не вибрано]" },
                { "Item_Author", "Автор (.Authors)" },
                { "Item_Series", "Серія (.Series)" },
                { "Item_Title", "Назва книги (.Title)" },
                { "Item_Lang", "Мова (.Language)" },
                { "Item_Genre", "Жанр (.Genres)" },
                { "Item_Date", "Дата (.Date)" },
                { "Item_Source", "Базова назва файла (.SourceFile)" },
                { "Item_Uuid", "UUID книги (.BookID)" },
                { "CoverMode", "Спосіб відображення виносок:" },
                { "NotesMode", "Тип навігаційної ієрархії:" },
                { "OpenFromCover", "Відкриття книги з типульної сторінки" },
                { "FixZip", "Вилучити дескриптор даних" },
                { "EmbedFonts", "Вбудовувати шрифти в книгу" },
                { "SaveSuccess", "Конфігурацію успішно збережено у файл {0}!" }
            }},
            { "Russian", new Dictionary<string, string> {
                { "Title", "Конфигуратор шаблонов fb2cng" },
                { "Language", "Язык" },
                { "DumpConfig", "Загрузить config.yaml" },
                { "ConfigName", "Название файла настроек" },
                { "CssEnable", "CSS-таблица стилей" },
                { "Browse", "Обзор..." },
                { "Fb2Name", "Использовать имя файла fb2 для выходного" },
                { "Help", "Справка" },
                { "Theme", "Тема" },
                { "Ok", "ОК" },
                { "Cancel", "Отмена" },
                { "ErrFbc", "Ошибка: fbc.exe не найден в папке с программой!" },
                { "OutNameTitle", "Название выходного файла" },
                { "AsFolder", "как папка" },
                { "Translit", "Транслитерировать название выходного файла" },
                { "ReaderSize", "Размер экрана ридера" },
                { "Width", "Ширина:" },
                { "Height", "Высота:" },
                { "Dpi", "DPI:" },
                { "Item_Empty", "[Не выбрано]" },
                { "Item_Author", "Автор (.Authors)" },
                { "Item_Series", "Серия (.Series)" },
                { "Item_Title", "Название книги (.Title)" },
                { "Item_Lang", "Язык (.Language)" },
                { "Item_Genre", "Жанр (.Genres)" },
                { "Item_Date", "Дата (.Date)" },
                { "Item_Source", "Базовое имя файла (.SourceFile)" },
                { "Item_Uuid", "UUID книги (.BookID)" },
                { "CoverMode", "Способ отображения сносок:" },
                { "NotesMode", "Тип навигационной иерархии:" },
                { "OpenFromCover", "Открытие книги с титульной страницы" },
                { "FixZip", "Удалить дескриптор данных" },
                { "EmbedFonts", "Встраивать шрифты в книгу" },
                { "SaveSuccess", "Конфигурация успешно сохранена в файл {0}!" }
            }}
        };
    }
}