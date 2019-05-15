using System;
using System.Collections.Generic;
using System.Linq;
using Library.Publisher.Data.Entities;
using Library.Publisher.Data.Repositories;
using Library.Publisher.Service.Requests;
using Moq;
using Xunit;

namespace Library.Publisher.Service.Tests.PublisherServiceTests
{
    public class PublisherServiceTest
    {
        private readonly Mock<IPublisherRepository> _publisherRepositoryMock;
        private PublisherService _sut;
        
        public PublisherServiceTest()
        {
            _publisherRepositoryMock = new Mock<IPublisherRepository>();
            _sut = new PublisherService(_publisherRepositoryMock.Object);

        }

        [Fact] 
        public void When_PublisherRepository_ReturnsNullById_ObjectShouldNotBeEmpty()
        {
            _publisherRepositoryMock.Setup(t => t.SelectAll()).Returns(() => null);

            var actual = _sut.Select(Guid.NewGuid().ToString());

            Assert.NotNull(actual.Publishers);
        }
        
        [Fact]
        public void When_PublisherRepository_ReturnsWithDataById_ObjectShouldNotBeEmpty()
        {
            _publisherRepositoryMock.Setup(t => t.Select(It.IsAny<string>())).Returns(() => new EPublisher()
                                                                                            {
                                                                                                Id = "Id",
                                                                                                Name = "name",
                                                                                                Series = null,
                                                                                                CreateDateTime = DateTime.Now,
                                                                                                UpdateDateTime = null
                                                                                            });

            var actual = _sut.Select(Guid.NewGuid().ToString());

            Assert.NotNull(actual.Publishers);
            Assert.Equal(1, actual.Total);
            Assert.Equal("Id",actual.Publishers.FirstOrDefault().Id);            
            Assert.Equal("name",actual.Publishers.FirstOrDefault().Name);
            Assert.Null(actual.Publishers.FirstOrDefault().Series);

        }
        [Fact]
        public void When_DeleteStorage_Completed_VerifyStorageRepository()
        {
            _sut.DeletePublisher(Guid.NewGuid().ToString());
            _publisherRepositoryMock.Verify(t => t.Delete(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void When_InsertStorage_Completed_VerifyStorageRepository()
        {
            _sut.InsertPublisher(new InsertPublisherServiceRequest()
                                 {
                                     Name = "publisher",
                                     Series = new []{ "series"}
                                 });
            _publisherRepositoryMock.Verify(t => t.Insert(It.IsAny<EPublisher>()), Times.Once);
        }
        
        [Fact]
        public void When_PublisherRepository_ReturnsNull_ObjectShouldNotBeEmpty()
        {
            _publisherRepositoryMock.Setup(t => t.SelectAll()).Returns(() => null);

            var actual = _sut.SelectAll(0, 10);

            Assert.NotNull(actual.Publishers);
        }
        
        [Fact]
        public void When_PublisherRepository_ReturnsWithData_ObjectShouldNotBeEmpty()
        {
            _publisherRepositoryMock.Setup(t => t.SelectAll()).Returns(() => new List<EPublisher>
                                                                           {
                                                                               new EPublisher(){
                                                                                                   Id = "Id",
                                                                                                   Name = "name",
                                                                                                   Series = null,
                                                                                                   CreateDateTime = DateTime.Now,
                                                                                                   UpdateDateTime = null
                                                                                               }
                                                                           });


            var actual = _sut.SelectAll(0, 10);

            Assert.NotNull(actual.Publishers);
            Assert.Equal(1, actual.Total);
            Assert.NotNull(actual.Publishers.SingleOrDefault());
        }
        
        [Fact]
        public void When_UpdatePublisher_Completed_VerifyStorageRepository()
        {
            _sut.UpdatePublisher(new UpdatePublisherServiceRequest()
                                 {
                                     Id = "Id",
                                     Name = "name",
                                     Series = new []{"series1", "series2"}
                                 });
            _publisherRepositoryMock.Verify(t => t.Update(It.IsAny<EPublisher>()), Times.Once);
        }
        
    }
}