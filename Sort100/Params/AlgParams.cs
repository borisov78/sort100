using System.Text;

namespace Sort100.Params
{
    /// <summary>
    /// Параметры алгоритма.
    /// </summary>
    public sealed class AlgParams
    {
        /// <summary>
        /// Кодировка исходного файла (сохраняется на всех этапах обработки).
        /// </summary>
        public Encoding Encoding { get; set; }
        
        /// <summary>
        /// Количество InMemory сортировщиков небольших пачек данных. Для нераспареллеливаемых алгоритмов сортировки
        /// целесообразно задавать равным количеству ядер процессора.
        /// </summary>
        public int WorkersCount { get; set; }
        
        /// <summary>
        /// Максимальное количество симоволов в чанке (используется для эффективного использования памяти
        /// и предотвращения ухода системы в swap) 
        /// </summary>
        public int ChunkSizeInChars { get; set; }
        
        /// <summary>
        /// Максимальное количество объектов с чанке. Используется для эффективного распределения памяти для хранения чанка.
        /// </summary>
        public int ChunkCapacity { get; set; }
    }
}