using System.IO;

namespace Sort100.Interfaces
{
    /// <summary>
    /// Фабрика для получения стримов ввода-вывода. Для реальной обработки используются файловые стримы,
    /// для тестов - стримы в памяти. 
    /// </summary>
    public interface IIOStreamsFactory
    {
        Stream GetForRead(string filePath, bool useCompression = false);
        Stream GetForWrite(string filePath, bool useCompression = false);
    }
}