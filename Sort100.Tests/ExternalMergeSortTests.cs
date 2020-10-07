using System.Text;
using NUnit.Framework;
using Sort100.Impl;
using Sort100.Params;
using Sort100.Tests.Helpers;
using Sort100.Tests.Stub;

namespace Sort100.Tests
{
    [TestFixture]
    public class ExternalMergeSortTests
    {
        private StreamsFactoryStub _streams;
        private ExternalMergeSort _sort;
        private IOParams _ioParams;

        [SetUp]
        public void Setup()
        {
            _streams = new StreamsFactoryStub();
            var algParams = new AlgParams
            {
                Encoding = Encoding.Default,
                WorkersCount = 4,
                ChunkSizeInChars = 5,
                ChunkCapacity = 5
            };
            _ioParams = new IOParams {SourceFilePath = "input", ResultFilePath = "output"};
            _sort = new ExternalMergeSort(_ioParams, algParams, new EntryComparer(), _streams);
        }
        
        //Add_EmptyString_ReturnsZero
        [Test]
        public void when_chunks_is_sequential_returns_sorted()
        {
            // Arrange
            var readers = _streams.SetUpInputs(
                EntryHelper.Generate(0, 1, 2),
                EntryHelper.Generate(3, 4, 5),
                EntryHelper.Generate(6, 7, 8),
                EntryHelper.Generate(9, 10, 11)
            );
            
            // Act
            _sort.Sort(readers);
            
            //Assert
            _streams.AssertOutput(_ioParams.ResultFilePath,
                EntryHelper.Generate(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)); 
        }
        
        [Test]
        public void when_chunks_is_jumbled_returns_sorted()
        {
            // Arrange
            var readers = _streams.SetUpInputs(
                EntryHelper.Generate(0, 4, 8),
                EntryHelper.Generate(1, 5, 9),
                EntryHelper.Generate(2, 6, 10),
                EntryHelper.Generate(3, 7, 11)
            );
            
            // Act
            _sort.Sort(readers);
            
            //Assert
            _streams.AssertOutput(_ioParams.ResultFilePath,
                EntryHelper.Generate(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)); 
        }
        
        [Test]
        public void when_first_chunk_is_longest_returns_sorted()
        {
            // Arrange
            var readers = _streams.SetUpInputs(
                EntryHelper.Generate(0, 4, 7, 8, 11),
                EntryHelper.Generate(1, 5, 9),
                EntryHelper.Generate(2, 6, 10),
                EntryHelper.Generate(3)
            );
            
            // Act
            _sort.Sort(readers);
            
            //Assert
            _streams.AssertOutput(_ioParams.ResultFilePath,
                EntryHelper.Generate(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)); 
        }
        
        [Test]
        public void when_last_chunk_is_longest_returns_sorted()
        {
            // Arrange
            var readers = _streams.SetUpInputs(
                EntryHelper.Generate(3),
                EntryHelper.Generate(1, 5, 9),
                EntryHelper.Generate(2, 6, 10),
                EntryHelper.Generate(0, 4, 7, 8, 11)
            );
            
            // Act
            _sort.Sort(readers);
            
            //Assert
            _streams.AssertOutput(_ioParams.ResultFilePath,
                EntryHelper.Generate(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)); 
        }
        
        [Test]
        public void when_middle_chunk_is_longest_returns_sorted()
        {
            // Arrange
            var readers = _streams.SetUpInputs(
                EntryHelper.Generate(3),
                EntryHelper.Generate(0, 4, 7, 8, 11),
                EntryHelper.Generate(1, 5, 9),
                EntryHelper.Generate(2, 6, 10)
            );
            
            // Act
            _sort.Sort(readers);
            
            //Assert
            _streams.AssertOutput(_ioParams.ResultFilePath,
                EntryHelper.Generate(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)); 
        }
        
        
        [Test]
        public void when_only_one_chunk_returns_sorted()
        {
            // Arrange
            var readers = _streams.SetUpInputs(
                EntryHelper.Generate(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11));
            
            // Act
            _sort.Sort(readers);
            
            //Assert
            _streams.AssertOutput(_ioParams.ResultFilePath,
                EntryHelper.Generate(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11)); 
        }
        
        [Test]
        public void when_empty_returns_empty()
        {
            // Arrange
            var readers = _streams.SetUpInputs(string.Empty);
            
            // Act
            _sort.Sort(readers);
            
            //Assert
            _streams.AssertOutput(_ioParams.ResultFilePath, string.Empty);
        }
        
    }
}