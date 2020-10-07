using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Sort100.Common;
using Sort100.Helpers;
using Sort100.Impl;
using Sort100.Interfaces;
using Sort100.Params;

namespace Sort100
{
    internal static class Program
    {
        private static IOParams CreateIOParams(string sourceFilePath, string destinationFilePath)
        {
            destinationFilePath = destinationFilePath ?? sourceFilePath;
            var workDir = Path.GetDirectoryName(destinationFilePath);
            var datePart = DateTime.Now.ToString("yyyy-MM-dd-HHmmss");
            var sourceExtensions = Path.GetExtension(destinationFilePath);
            var sourceFileNameWithoutExtensions = Path.GetFileNameWithoutExtension(destinationFilePath);
            return new IOParams
            {
                SourceFilePath = sourceFilePath,
                ResultFilePath = Path.Combine(workDir,
                    sourceFileNameWithoutExtensions + "_sorted_" + datePart + sourceExtensions),
                TempDir = Path.Combine(workDir, "tmp_" + datePart)
            };
        }

        private static AlgParams CreateAlgParams(IOParams ioParams)
        {
            var encoding = FileHelper.DetectEncoding(ioParams.SourceFilePath);
            // Важные параметры, влияющие на потребление памяти алгоритмом.
            // Рассчитываем, что будем параллелить сортировку чанков по числу ядер процессора (WorkersCount).
            //
            // Размер одного чанка (ChunkSizeInChars) - 64Mb. Экспериментальным путем было выяснено,
            // что на больших размерностях сортировка работает менее эфективно, а на меньших
            // - создается слишком много  временных файлов.
            //
            // Количество объектов в чанке (ChunkCapacity) - эвристика, рассчитанная на основе средней длины строки (20 символов)
            // При превышении ChunkSizeInChars или ChunkCapacity - текущий чанк закрывается. 
            const ulong chunkSizeInBytes = 1024 * 1024 * 64; // 64 Mb
            var chunkSizeInChars = (int) (encoding.IsSingleByte ? chunkSizeInBytes : chunkSizeInBytes / 2);
            return new AlgParams
            {
                Encoding = encoding,
                WorkersCount = Environment.ProcessorCount,
                ChunkSizeInChars = chunkSizeInChars,
                ChunkCapacity = chunkSizeInChars / 60
            };
        }
        
        private static void Main(string[] args)
        {
            var cmdParser = new CmdParser(args);
            if (cmdParser.IsEmpty)
            {
                Console.WriteLine("Usage: sort100 input=<input_file> [output=<output_file>]");
                return;
            }

            var inputFilePathParam = cmdParser.Get<string>("input");
            var outputFilePathParam = cmdParser.GetOrDefault<string>("output", null);
            var ioParams = CreateIOParams(inputFilePathParam, outputFilePathParam);

            // Создаем и наполняем DI-контейнер
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(ioParams);
            serviceCollection.AddSingleton(CreateAlgParams(ioParams));
            serviceCollection.AddSingleton<IIOStreamsFactory, IOStreamsFactory>();
            serviceCollection.AddSingleton<ISortedChunksStorage, SortedChunksStorage>();
            serviceCollection.AddSingleton<ISourceFileReader, SourceFileReader>();
            serviceCollection.AddTransient<IInMemoryChunkSort, InMemoryChunkHybridSort>();
            serviceCollection.AddTransient<IChunkProcessor, ChunkProcessor>();
            serviceCollection.AddTransient<IComparer<Entry>, EntryComparer>();
            serviceCollection.AddSingleton<IExternalMergeSort, ExternalMergeSort>();
            serviceCollection.AddSingleton<Sort>();

            // Компилируем ServiceProvider, при этом происходит разрешение всех зависимостей
            using var serviceProvider = serviceCollection.BuildServiceProvider();
            
            // Получаем корневой объект для выполнения сортировки
            var sort = serviceProvider.GetService<Sort>();
            var sw = Stopwatch.StartNew();
            try
            {
                // Запускаме сортировку и замеряем время выполнения
                sort.DoSort();
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception:" + e.Message + Environment.NewLine + e.StackTrace);
            }
            Console.WriteLine($"File '{ioParams.ResultFilePath}'. Elapsed time: {sw.ElapsedMilliseconds} ms.");
        }
    }
}