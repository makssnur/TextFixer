using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace TextFixer
{
    class Program
    {
        // Словарь ошибок: правильное слово -> список неправильных вариантов
        static Dictionary<string, List<string>> errorWords = new Dictionary<string, List<string>>();

        static void Main(string[] args)
        {
            Console.WriteLine(" ИСПРАВЛЕНИЕ ОШИБОК В ФАЙЛАХ \n");

            // Заполняем словарь ошибок
            FillErrorDictionary();

            while (true)
            {
                Console.WriteLine("\n МЕНЮ ");
                Console.WriteLine("1 - Показать словарь ошибок");
                Console.WriteLine("2 - Исправить ошибки в папке");
                Console.WriteLine("3 - Исправить номера телефонов в папке");
                Console.WriteLine("4 - Сделать всё сразу (ошибки + телефоны)");
                Console.WriteLine("0 - Выход");
                Console.Write("Выбери: ");

                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    ShowDictionary();
                }
                else if (choice == "2")
                {
                    FixAllFiles();
                }
                else if (choice == "3")
                {
                    FixPhoneNumbersInFolder();
                }
                else if (choice == "4")
                {
                    FixAllFiles();
                    FixPhoneNumbersInFolder();
                    Console.WriteLine("\n Готово! Ошибки и телефоны исправлены.");
                }
                else if (choice == "0")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Такого пункта нет, попробуй ещё раз");
                }
            }

            Console.WriteLine("Пока!");
        }

        //  ЗАПОЛНЯЕМ СЛОВАРЬ ОШИБОК 
        static void FillErrorDictionary()
        {
            // Правильное слово "привет" и его неправильные варианты
            errorWords["привет"] = new List<string> { "првиет", "пирвет", "припет", "превед" };

            // Правильное слово "пока" и его неправильные варианты
            errorWords["пока"] = new List<string> { "покаа", "поке", "поко" };

            // Правильное слово "здравствуйте"
            errorWords["здравствуйте"] = new List<string> { "здраствуйте", "здравствути", "здрасте" };

            // Можно добавить свои слова
            errorWords["спасибо"] = new List<string> { "спасиба", "спс", "спосибо" };
            errorWords["пожалуйста"] = new List<string> { "пажалуйста", "пожалйста", "пожалуста" };
        }

        //  ПОКАЗАТЬ СЛОВАРЬ 
        static void ShowDictionary()
        {
            Console.WriteLine("\n СЛОВАРЬ ОШИБОК \n");
            foreach (var pair in errorWords)
            {
                Console.WriteLine($"Правильно: {pair.Key}");
                Console.Write("   Ошибки: ");
                Console.WriteLine(string.Join(", ", pair.Value));
                Console.WriteLine();
            }
        }

        //  ИСПРАВИТЬ ОШИБКИ В ОДНОМ ФАЙЛЕ 
        static string FixErrorsInText(string text)
        {
            string fixedText = text;

            // Для каждого правильного слова
            foreach (var pair in errorWords)
            {
                string correctWord = pair.Key;
                List<string> wrongWords = pair.Value;

                // Для каждого неправильного варианта
                foreach (string wrong in wrongWords)
                {
                    // Заменяем все вхождения неправильного слова на правильное
                    // (независимо от регистра)
                    fixedText = Regex.Replace(fixedText, wrong, correctWord, RegexOptions.IgnoreCase);
                }
            }

            return fixedText;
        }

        //  ИСПРАВИТЬ ВСЕ ФАЙЛЫ В ПАПКЕ 
        static void FixAllFiles()
        {
            Console.Write("\nВведи путь к папке: ");
            string folderPath = Console.ReadLine();

            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("Папка не найдена!");
                return;
            }

            // Находим все .txt файлы
            string[] files = Directory.GetFiles(folderPath, "*.txt");

            if (files.Length == 0)
            {
                Console.WriteLine("В этой папке нет текстовых файлов (.txt)");
                return;
            }

            Console.WriteLine($"\nНайдено {files.Length} файлов. Начинаю исправлять...\n");

            int fixedCount = 0;

            foreach (string file in files)
            {
                string content = File.ReadAllText(file);
                string newContent = FixErrorsInText(content);

                if (content != newContent)
                {
                    File.WriteAllText(file, newContent);
                    Console.WriteLine($" Исправлен: {Path.GetFileName(file)}");
                    fixedCount++;
                }
                else
                {
                    Console.WriteLine($" Без изменений: {Path.GetFileName(file)}");
                }
            }

            Console.WriteLine($"\nГотово! Исправлено файлов: {fixedCount}");
        }

        // ===== ИСПРАВИТЬ НОМЕРА ТЕЛЕФОНОВ =====
        static string FixPhoneNumbersInText(string text)
        {
            // Шаблон для поиска номеров (012) 345-67-89
            // \\( - это открывающая скобка
            // (\\d{3}) - три цифры
            // \\) - закрывающая скобка
            // \\s - пробел
            // (\\d{3}) - три цифры
            // - - дефис
            // (\\d{2}) - две цифры
            // - - дефис
            // (\\d{2}) - две цифры
            string pattern = @"\((\d{3})\)\s(\d{3})-(\d{2})-(\d{2})";

            // Замена: +380 12 345 67 89
            // $1, $2, $3, $4 - это группы цифр из шаблона
            string replacement = "+380 $1 $2 $3 $4";

            // Заменяем все вхождения
            string newText = Regex.Replace(text, pattern, replacement);

            return newText;
        }

        //  ИСПРАВИТЬ ТЕЛЕФОНЫ ВО ВСЕХ ФАЙЛАХ В ПАПКЕ 
        static void FixPhoneNumbersInFolder()
        {
            Console.Write("\nВведи путь к папке: ");
            string folderPath = Console.ReadLine();

            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("Папка не найдена!");
                return;
            }

            // Находим все .txt файлы
            string[] files = Directory.GetFiles(folderPath, "*.txt");

            if (files.Length == 0)
            {
                Console.WriteLine("В этой папке нет текстовых файлов (.txt)");
                return;
            }

            Console.WriteLine($"\nНайдено {files.Length} файлов. Исправляю номера телефонов...\n");

            int fixedCount = 0;

            foreach (string file in files)
            {
                string content = File.ReadAllText(file);
                string newContent = FixPhoneNumbersInText(content);

                if (content != newContent)
                {
                    File.WriteAllText(file, newContent);
                    Console.WriteLine($" Телефоны исправлены: {Path.GetFileName(file)}");
                    fixedCount++;
                }
                else
                {
                    Console.WriteLine($" Телефонов не найдено: {Path.GetFileName(file)}");
                }
            }

            Console.WriteLine($"\nГотово! Обработано файлов: {fixedCount}");
        }
    }
}
//Готово
