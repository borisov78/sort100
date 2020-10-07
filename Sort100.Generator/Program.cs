using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Sort100.Common;

namespace Sort100.Generator
{
    class Program
    {
        static void Main(string[] args)
        {
            var cmdParser = new CmdParser(args);
            if (cmdParser.IsEmpty)
            {
                Console.WriteLine($"Usage: sort100gen {ParamNames.Output} = <output_file> [--{ParamNames.Size}=<file size in Gb>]");
                Console.WriteLine($"Sample: sort100gen {ParamNames.Output}=c:/testdata.txt --{ParamNames.Size}=1");
                return;
            }

            var filePathParam = cmdParser.Get<string>(ParamNames.Output);
            var fileSizeInGb = cmdParser.GetOrDefault(ParamNames.Size, 1);

            var sw = Stopwatch.StartNew();
            GenerateTestFile(filePathParam, fileSizeInGb);
            Console.WriteLine($"File '{filePathParam}' with size {fileSizeInGb} Gb created. Elapsed time: {sw.ElapsedMilliseconds} ms.");
        }
        
        // для генерации символьной части строки будем использовать "радужную таблицу" - список английских слов,
        // заранее закодированных в байты в нужной кодировке
        private static List<byte[]> InitWordsRainbowTable(Encoding encoding)
        {
            var allWords = ResourceUtil.GetStringFromResources(typeof(Program).Assembly, "words.txt");
            var words = new HashSet<string>(allWords.Split(new[] {"\r\n", "\n"}, StringSplitOptions.RemoveEmptyEntries));
            // для повышения скорости генерации предварительно преобразуем слова в байты
            var result = new List<byte[]>(words.Count);
            result.AddRange(words.Select(encoding.GetBytes));
            return result;
        }

        // для генерации числовой части строки будем использовать "радужную таблицу" случайных чисел, заканее
        // закодированных в байты в нужной кодировке
        private static List<byte[]> InitNumericsRainbowTable(Encoding encoding, int tableSize)
        {
            var result = new List<byte[]>(tableSize);
            for (var i = 0; i < tableSize; i++)
            {
                var randomNum = RandomUtil.Next(tableSize);
                result.Add(encoding.GetBytes(randomNum.ToString(CultureInfo.InvariantCulture)));    
            }
            return result;
        }
        
        // Генерация тестового файла
        private static void GenerateTestFile(string filePath, int fileSizeInGb)
        {
            const int maxPhraseLength = 8;
            const int repetitionsCount = 10;
            const int numbersTableSize = 1000000;
            var encoding = Encoding.UTF8;
            
            var wordsTable = InitWordsRainbowTable(encoding);
            var numbersTable = InitNumericsRainbowTable(encoding, numbersTableSize);
            var delimiters = new Delimiters(encoding);
            
            var fileSizeInBytes = fileSizeInGb * Consts.OneGb;
            long bytesWritten = 0;

            using var outputFile = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 32, FileOptions.SequentialScan);
            
            // По условиям задачи, в файле должно присутствовать несколько повторов
            using (var memoryStream = new MemoryStream())
            {
                // Генерируем несколько строк в память 
                for (var curRepetitions = 0; curRepetitions < repetitionsCount; curRepetitions++)
                {
                    WriteOneLine(maxPhraseLength, wordsTable, numbersTable, delimiters, memoryStream);
                }
                // Затем пишем повторы в результирующий файл 2 раза.                    
                memoryStream.Position = 0;
                var repetitions = memoryStream.ToArray();
                outputFile.Write(repetitions);
                outputFile.Write(repetitions);
                bytesWritten += repetitions.Length * 2;
                outputFile.Flush();
            }
            
            // Далее начинаем генерировать уникальный контент :)
            do
            {
                bytesWritten += WriteOneLine(maxPhraseLength, wordsTable, numbersTable, delimiters, outputFile);
            } while (bytesWritten < fileSizeInBytes);
            outputFile.Flush();
        }

        // Генерация одной строки
        private static int WriteOneLine(int maxPhraseLength, IReadOnlyList<byte[]> words, 
            IReadOnlyList<byte[]> numbers, Delimiters delimiters, Stream output)
        {
            var bytesWritten = 0;
            // Пишем числовую часть
            var numberPart = numbers[RandomUtil.Next(numbers.Count - 1)];
            output.Write(numberPart);
            bytesWritten += numberPart.Length;
            
            // Пишем разделитель численной и строковой части
            output.Write(delimiters.PartsDelimiterBytes);
            bytesWritten += delimiters.PartsDelimiterBytes.Length;
            
            // Пишем строковую часть
            var phraseLength = 1 + RandomUtil.Next(maxPhraseLength - 1);
            for (var i = 0; i < phraseLength; i++)
            {
                output.Write(delimiters.WordDelimiterBytes);
                bytesWritten += delimiters.WordDelimiterBytes.Length;
                var word = words[RandomUtil.Next(words.Count - 1)];
                output.Write(word);
                bytesWritten += word.Length;
            }
            
            // Пишем разделитель строк
            output.Write(delimiters.LineDelimiterBytes);
            bytesWritten += delimiters.LineDelimiterBytes.Length;

            return bytesWritten;
        }
    }
}