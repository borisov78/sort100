namespace Sort100.Interfaces
{
    /// <summary>
    /// Обработчик второй фазы сортировки - принимает на вход множества предварительно отсортированных объектов Entry
    /// и пишет их в результирующий файл в порядке возрастания.
    /// </summary>
    public interface IExternalMergeSort
    {
        void Sort(EntryReader[] sortedChunksReaders);
    }
}