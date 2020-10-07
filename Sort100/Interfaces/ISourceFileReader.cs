namespace Sort100.Interfaces
{
    /// <summary>
    /// Обработчик исходного файла для первой фазы сортировки - читает небольшие пачки неотсортированных объектов
    /// и отдает их на асинхронную сортировку.
    /// </summary>
    public interface ISourceFileReader
    {
        /// <summary>
        /// Прочитать кусками исходный файл и дождаться обработки каждой части. 
        /// </summary>
        void ReadAndSplitChunks();

        /// <summary>
        /// Зарегистрировать обработчик для частей исходного файла. 
        /// </summary>
        void RegisterChunkProcessor(IChunkProcessor processor);
    }
}