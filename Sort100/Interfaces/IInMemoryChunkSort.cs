namespace Sort100.Interfaces
{
    /// <summary>
    /// Интерфейс сортировщика небольшой пачки объектов. Сортировка выполняется в памяти.
    /// </summary>
    public interface IInMemoryChunkSort
    {
        public void Sort(Chunk chunk);
    }
}