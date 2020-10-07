namespace Sort100.Interfaces
{
    /// <summary>
    /// Обработчик второй фазы сортивки - принимает на вход множества предварительно отсортированных объектов
    /// и сливает их в результирующий файл.
    /// </summary>
    public interface IExternalMergeSort
    {
        void Sort(EntryReader[] sortedChunksReaders);
    }
}