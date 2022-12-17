using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lab14
{
    internal class Program
    {
        static readonly List<int> defaultHashes = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        static readonly string defaultSettingsPath = Directory.GetCurrentDirectory() + "\\settings.txt";

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            CreateDefaultSettings();

            var dictionary = CreateDictionary();
            dictionary.AddValue("SomeAwesome Value");
            dictionary.SearchValue("SomeAwesome Value");
            dictionary.SearchValue("CreateDictionary");
            dictionary.DeleteValue("SomeAwesome Value");
        }

        static void CreateDefaultSettings(int countOfHashes = 9, bool isNeedToCreate = false, int blockCapacity = 3)
        {
            if (!(File.Exists(defaultSettingsPath)) || isNeedToCreate)
            {
                var hashes = defaultHashes.Take(countOfHashes);

                using var sw = File.AppendText(defaultSettingsPath);

                sw.WriteLine(blockCapacity);
                sw.WriteLine();
                foreach (var item in hashes)
                {
                    sw.WriteLine(item);
                }
            }
            
        }

        static HashDictionary CreateDictionary()
        {
            var settings = ReadSettingsFromFile();

            return new HashDictionary(settings[0], settings.Skip(2));
        }

        static List<int> ReadSettingsFromFile()
        {
            using var reader = new StreamReader(defaultSettingsPath);
            string line;
            var list = new List<int>();
            while ((line = reader.ReadLine()) != null)
            {
                if (!int.TryParse(line, out int result))
                {
                    list.Add(-1);
                }
                else
                {
                    list.Add(result);
                }
            }

            return list;
        }
    }
}
