using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lab14
{
    internal class HashDictionary
    {
        private readonly int _blockCapacity;

        private readonly Dictionary<int, string> _dictionary = new Dictionary<int, string>();

        private readonly string currentPath = Directory.GetCurrentDirectory();

        public HashDictionary(int blockCapacity, IEnumerable<int> hashKeys)
        {
            _blockCapacity = blockCapacity;
            foreach (var key in hashKeys)
            {
                _dictionary.Add(key, currentPath + $"\\key({key})");
            }
        }

        public void AddValue(string value)
        {
            WriteToBlock(GetHashKey(value), value, 1);
        }

        public void DeleteValue(string value)
        {
            DeleteFromBlock(GetHashKey(value), value);
        }

        public string SearchValue(string value)
        {
            return SearchFromBlock(GetHashKey(value), value);
        }

        private string SearchFromBlock(int haskKey, string value)
        {
            var pathToBlocks = _dictionary[haskKey];
            var firstFilePath = pathToBlocks + $"\\{1}.txt";
            if (!Directory.Exists(pathToBlocks) || !File.Exists(firstFilePath))
            {
                return "Такого значения не существует в словаре";
            }

            return SearchFromFile(haskKey, value, firstFilePath);

            string SearchFromFile(int hashKey, string value, string fileName)
            {
                var lines = File.ReadAllLines(fileName);

                for (int i = 0; i < lines.Length && i < _blockCapacity; i++)
                {
                    if (lines[i].Equals(value))
                    {
                        return $"Значение находится в файле {fileName.Replace(_dictionary[haskKey] + "\\", string.Empty)}. HashKey: {hashKey}";
                    }
                }

                if (lines.Length == 4)
                {
                    return SearchFromFile(hashKey, value, lines[^1]);
                }

                return "Такого значения не существует в словаре";
            }
        }

        private void DeleteFromBlock(int haskKey, string value)
        {
            var pathToBlocks = _dictionary[haskKey];
            var firstFilePath = pathToBlocks + $"\\{1}.txt";
            if (!Directory.Exists(pathToBlocks) || !File.Exists(firstFilePath))
            {
                return;
            }

            DeleteFromFile(value, firstFilePath);

            void DeleteFromFile(string value, string fileName)
            {
                var lines = File.ReadAllLines(fileName);

                for (int i = 0; i < lines.Length && i < _blockCapacity; i++)
                {
                    if (lines[i].Equals(value))
                    {
                        lines[i] = string.Empty;

                        using var sw = new StreamWriter(fileName, false);
                        foreach (string line in lines)
                        {
                            sw.WriteLine(line);
                        }

                        return;
                    }
                }

                DeleteFromFile(value, lines[^1]);
            }
        }

        private void WriteToBlock(int haskKey, string value, int number)
        {
            var pathToBlocks = _dictionary[haskKey];
            if (!Directory.Exists(pathToBlocks))
            {
                Directory.CreateDirectory(pathToBlocks);
            }

            WriteValue(value, pathToBlocks + $"\\{1}.txt");

            void WriteValue(string value, string fileName)
            {
                if (!File.Exists(fileName))
                {
                    using var file = File.AppendText(fileName);
                    file.WriteLine(value);
                    return;
                }

                var lines = File.ReadAllLines(fileName);

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Equals(string.Empty))
                    {
                        lines[i] = value;
                        WriteToFile(lines, fileName);
                        return;
                    }
                }

                if (lines.Length < _blockCapacity)
                {
                    var newList = new List<string>(lines)
                    {
                        value
                    };

                    WriteToFile(newList, fileName);
                }
                else
                {
                    if (lines.Length == _blockCapacity)
                    {
                        var newList = new List<string>(lines)
                        {
                            pathToBlocks + $"\\{number + 1}.txt"
                        };

                        WriteToFile(newList, fileName);

                        lines = newList.ToArray();
                    }


                    WriteValue(value, lines[3]);
                }
            }
        }

        private void WriteToFile(IEnumerable<string> values, string path)
        {
            using var sw = new StreamWriter(path, false);
            foreach (string line in values)
            {
                sw.WriteLine(line);
            }
        }
    
        private int GetHashKey(string value)
        {
            var chars = value.ToCharArray();
            int validHash = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                validHash += chars[i] - '0';
            }

            var hashKey = validHash  % _dictionary.Count + 1;
            return hashKey;
        }
    }
}
