namespace Sort100.Params
{
    /// <summary>
    /// Параметры ввода-вывода.
    /// </summary>
    public sealed class IOParams
    {
        /// <summary>
        /// Полный путь к исходному файлу.
        /// </summary>
        public string SourceFilePath { get; set; }
        
        /// <summary>
        /// Каталог для хранения временных файлов сортировки.
        /// </summary>
        public string TempDir { get; set; }
        
        /// <summary>
        /// Полный путь к файлу-результату.
        /// </summary>
        public string ResultFilePath { get; set; }
        
        /// <summary>
        /// Размер буфера для стримов чтения.
        /// </summary>
        public int ReadBufferSizeInBytes { get; set; } = 1024 * 32;
        
        /// <summary>
        /// Размер буфера для стримов записи. 
        /// </summary>
        public int WriteBufferSizeInBytes { get; set; } = 1024 * 32;
    }
}