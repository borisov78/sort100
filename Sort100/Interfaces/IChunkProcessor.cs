using System.Threading;

namespace Sort100.Interfaces
{
    /// <summary>
    /// Обработчик первой фазы сортировки - принимает небольшое множество объектов (Chunk), асинхронно сортирует
    /// их в памяти и записывает на диск во временное хранилище.
    /// </summary>
    public interface IChunkProcessor
    {
        /// <summary>
        /// Готовность к обработке очередной пачки объектов.
        /// </summary>
        public WaitHandle IsReady { get; }

        /// <summary>
        /// Начать обработку пачки объектов (происходит в отдельном потоке). 
        /// </summary>
        public void StartProcess(Chunk chunk);
    }
}