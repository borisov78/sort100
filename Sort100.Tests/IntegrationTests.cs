using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Sort100.Common;
using Sort100.Impl;
using Sort100.Interfaces;
using Sort100.Params;
using Sort100.Tests.Stub;

namespace Sort100.Tests
{
    [TestFixture]
    public class IntegrationTests
    {
        private StreamsFactoryStub _streams;
        private IOParams _ioParams;
        private Sort _sort;
        private ServiceProvider _serviceProvider;
        
        [SetUp]
        public void Setup()
        {
            _streams = new StreamsFactoryStub();
            
            var serviceCollection = new ServiceCollection();
            _ioParams = new IOParams {SourceFilePath = "input", ResultFilePath = "output"};
            serviceCollection.AddSingleton(_ioParams);
            
           
            var algParams = new AlgParams
            {
                Encoding = Encoding.Default,
                WorkersCount = 4,
                ChunkSizeInChars = 40,
                ChunkCapacity = 40
            };
            serviceCollection.AddSingleton(algParams);
            serviceCollection.AddSingleton<IIOStreamsFactory>(_streams);
            serviceCollection.AddSingleton<ISortedChunksStorage, SortedChunksStorage>();
            serviceCollection.AddSingleton<ISourceFileReader, SourceFileReader>();
            serviceCollection.AddTransient<IInMemoryChunkSort, InMemoryChunkHybridSort>();
            serviceCollection.AddTransient<IChunkProcessor, ChunkProcessor>();
            serviceCollection.AddTransient<IComparer<Entry>, EntryComparer>();
            serviceCollection.AddSingleton<IExternalMergeSort, ExternalMergeSort>();
            serviceCollection.AddSingleton<Sort>();

            _serviceProvider = serviceCollection.BuildServiceProvider();
            _sort = _serviceProvider.GetService<Sort>();
        }

        [TearDown]
        public void TearDown()
        {
            _serviceProvider?.Dispose();
        }

        [Test]
        public void RealWorldTest()
        {
            //Arrange
            var input = ResourceUtil.GetStringFromResources(typeof(IntegrationTests).Assembly, "TestInput.txt");
            _streams.SetUpInput(_ioParams.SourceFilePath, input);
            
            // Act
            _sort.DoSort();
            
            // Assert
            var expectedOutput = ResourceUtil.GetStringFromResources(typeof(IntegrationTests).Assembly,"TestOutput.txt");
            _streams.AssertOutput(_ioParams.ResultFilePath, expectedOutput);
        }
    }
}