using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace LabWork
{
    // Даний проект є шаблоном для виконання лабораторних робіт
    // з курсу "Об'єктно-орієнтоване програмування та патерни проектування"
    // Необхідно змінювати і дописувати код лише в цьому проекті
    // Відео-інструкції щодо роботи з github можна переглянути 
    // за посиланням https://www.youtube.com/@ViktorZhukovskyy/videos 
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
і на новому рядку: 1.2.3.4; кінець тесту.";

            var finder = new IpFinder();
            var matches = finder.FindIPv4Addresses(sampleText);

            Console.WriteLine("Знайдені валідні IPv4-адреси:");
            foreach (var ip in matches)
            {
                Console.WriteLine(ip);
            }
        }
    }

    /// <summary>
    /// Допоміжний клас для пошуку валідних IPv4 адрес у тексті
    /// Використовує System.Text.RegularExpressions.Regex
    /// </summary>
    class IpFinder
    {
        // Регекс, який знаходить потенційні IPv4 у тексті. Потім ми додатково перевіряємо кожен октет 0-255.
        // Пояснення: \b - межа слова, (?:\d{1,3}\.){3}\d{1,3} - чотири числа по 1-3 цифри, розділені крапками
        private static readonly Regex CandidateRegex = new Regex(@"\b(?:\d{1,3}\.){3}\d{1,3}\b", RegexOptions.Compiled);

        public IEnumerable<string> FindIPv4Addresses(string text)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(text)) return list;

            var matches = CandidateRegex.Matches(text);
            foreach (Match m in matches)
            {
                if (IsValidIPv4(m.Value))
                {
                    list.Add(m.Value);
                }
            }

            return list;
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
