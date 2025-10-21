using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LabWork
{
    class Program
    {
        static void Main(string[] args)
        {
            // Великий приклад тексту українською з кількома випадковими IP-адресами
            string sampleText = @"Це тестовий текст для демонстрації пошуку IP-адрес. 
Деякі рядки містять IP: 127.0.0.1, інші — 192.168.0.100 або 10.0.0.5.
Інші приклади: 8.8.8.8; 255.255.255.255 — крайній випадок, а також 172.16.254.1.
Можна вставити IP всередині слова likeabc123.45.67.89xyz — але такий не має відповідати, 
тому що повинен бути виділений пробілами або пунктуацією. Також додаємо кілька випадкових чисел 300.300.300.300 та 256.256.256.256, 
які НЕ є валідними IPv4-адресами і мають бути відфільтровані правилом.
Ще кілька випадків у різних місцях: (123.45.67.89),
і на новому рядку: 1.2.3.4; кінець тесту
01:23:45.";

            var finder = new IpFinder();
            var matches = finder.FindIPv4Addresses(sampleText);

            Console.WriteLine("Знайдені валідні IPv4-адреси:");
            foreach (var ip in matches)
            {
                Console.WriteLine(ip);
            }

            // Перевірка — чи містить текст якийсь час у форматі HH:MM або HH:MM:SS
            bool hasTime = ContainsTime(sampleText);
            Console.WriteLine();
            Console.WriteLine($"Чи містить текст час? {(hasTime ? "Так" : "Ні")}");

            // Вивід знайдених часів (якщо є)
            var times = FindTimes(sampleText);
            if (times.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine("Знайдені часи:");
                foreach (var t in times)
                {
                    Console.WriteLine(t);
                }
            }
        }

        // Метод перевіряє наявність часу у форматах 0-23:00-59 або з секундами 00-59
        private static bool ContainsTime(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;

            // Простий регекс для часу: годинник 00-23, хвилини і секунди 00-59
            // Формати: H:MM, HH:MM, HH:MM:SS
            var timeRegex = new Regex(@"\b(?:[01]?\d|2[0-3]):[0-5]\d(?::[0-5]\d)?\b", RegexOptions.Compiled);
            return timeRegex.IsMatch(text);
        }

        // Повертає всі знайдені збіги часу у тексті у вигляді списку рядків
        private static IReadOnlyList<string> FindTimes(string text)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(text)) return list.AsReadOnly();

            var timeRegex = new Regex(@"\b(?:[01]?\d|2[0-3]):[0-5]\d(?::[0-5]\d)?\b", RegexOptions.Compiled);
            var matches = timeRegex.Matches(text);
            foreach (Match m in matches)
            {
                list.Add(m.Value);
            }

            return list.AsReadOnly();
        }
    }

    /// <summary>
    /// Допоміжний клас для пошуку валідних IPv4 адрес у тексті
    /// Використовує System.Text.RegularExpressions.Regex
    /// </summary>
    sealed class IpFinder
    {
        // Регекс, який знаходить потенційні IPv4 у тексті. Потім ми додатково перевіряємо кожен октет 0-255.
        // Пояснення: \b - межа слова, (?:\d{1,3}\.){3}\d{1,3} - чотири числа по 1-3 цифри, розділені крапками
        private static readonly Regex CandidateRegex = new Regex(@"\b(?:(?:25[0-5]|2[0-4]\d|[01]?\d?\d).){3}(?:25[0-5]|2[0-4]\d|[01]?\d?\d)\b", RegexOptions.Compiled);

        public IReadOnlyList<string> FindIPv4Addresses(string text)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(text)) return list.AsReadOnly();

            var matches = CandidateRegex.Matches(text);
            foreach (Match m in matches)
            {
                if (IsValidIPv4(m.Value))
                {
                    list.Add(m.Value);
                }
            }

            return list.AsReadOnly();
        }

        // Перевіряємо, що кожен октет у межах 0-255
        private bool IsValidIPv4(string candidate)
        {
            var parts = candidate.Split('.');
            if (parts.Length != 4) return false;
            foreach (var p in parts)
            {
                // Запобігти провалу через провідні нулі - дозволяємо їх як валідні (наприклад 01 -> 1)
                if (!int.TryParse(p, out int val)) return false;
                if (val < 0 || val > 255) return false;
            }
            return true;
        }
    }
}
