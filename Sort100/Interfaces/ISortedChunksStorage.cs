namespace Sort100.Interfaces
{
    /// <summary>
    /// Хранилище подмножеств отсортированных объектов (Chunk).
    /// </summary>
    public interface ISortedChunksStorage
    {
        void StoreSortedChunk(Chunk chunk);

        EntryReader[] GetStoredChunkReaders();
    }
}