namespace Sort100.Interfaces
{
    /// <summary>
    /// Интерфейс сортировщика небольшой пачки объектов.
    /// </summary>
    public interface IInMemoryChunkSort
    {
        public void Sort(Chunk chunk);
    }
}