using NUnit.Framework;
using Moq;
using ItemManagementApp.Services;
using ItemManagementLib.Repositories;
using ItemManagementLib.Models;
using System.Collections.Generic;
using System.Linq;

namespace ItemManagement.Tests
{
    [TestFixture]
    public class ItemServiceTests
    {
        // Field to hold the mock repository and the service being tested
        private ItemService _itemService;
        private Mock<IItemRepository> _mockItemRepository;

        [SetUp]
        public void Setup()
        {
            // Arrange: Create a mock instance of IItemRepository
            _mockItemRepository = new Mock<IItemRepository>();
            // Instantiate ItemService with the mocked repository
            _itemService = new ItemService(_mockItemRepository.Object);
            
        }

        [Test]
        public void AddItem_ShouldCallAddItemOnRepository()
        {
            // Arrange
            var item = new Item { Name = "Test" };
            _mockItemRepository.Setup(x => x.AddItem(It.IsAny<Item>()));
            // Act
            _itemService.AddItem(item.Name);
            // Assert
            _mockItemRepository.Verify(x => x.AddItem(It.IsAny<Item>()), Times.Once());
            
        }

        [Test]
        public void AddItem_ShouldCallArgumentExceptionIfNameTooLongOrNull()
        {
            // Arrange
            string invalidName = "";
            _mockItemRepository
                .Setup(x => x.AddItem(It.IsAny<Item>()))
                .Throws<ArgumentException>();
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _itemService.AddItem(invalidName));
            _mockItemRepository.Verify(x => x.AddItem(It.IsAny<Item>()), Times.Once());

        }

        [Test]
        public void GetAllItems_ShouldReturnAllItems()
        {
            //Arrange
            var items = new List<Item>() { new Item { Id = 1, Name = "Sample" } };
            _mockItemRepository.Setup(x => x.GetAllItems()).Returns(items);
            //Act
            var result = _itemService.GetAllItems();
            //Assert
            Assert.NotNull(result);
            Assert.That(result.Count, Is.EqualTo(1));
            _mockItemRepository.Verify(x => x.GetAllItems(), Times.Once());
        }

        [Test]
        public void GetItemById_ShouldReturnItemByIdIfItemExists()
        {
            //Arrange
            var item = new Item { Id = 1, Name = "Single Item" };
            _mockItemRepository.Setup(x => x.GetItemById(item.Id)).Returns(item);
            // Act
            var result = _itemService.GetItemById(item.Id);
            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(item.Name, result.Name);
            Assert.That(item.Id, Is.EqualTo(item.Id));
            _mockItemRepository.Verify(x => x.GetItemById(item.Id), Times.Once());
            
        }

        [Test]
        public void GetItemById_ShouldReturnNull_IfItemDoesNotExists()
        {
            //Arrange
            _mockItemRepository.Setup(x => x.GetItemById(It.IsAny<int>())).Returns<Item>(null);
            // Act
            var result = _itemService.GetItemById(132); // random int hc
            // Assert
            Assert.Null(result);
            _mockItemRepository.Verify(x => x.GetItemById(It.IsAny<int>()), Times.Once());
        }

        [Test]
        public void UpdateItem_ShouldNotUpdateItem_IfItemDoesntExist()
        {
            //arrange
            var nonExisting = 1;
            _mockItemRepository
                .Setup(x => x.GetItemById(nonExisting))
                .Returns<Item>(null);
            _mockItemRepository
                .Setup(x => x.UpdateItem(It.IsAny<Item>()));

            //act
            _itemService.UpdateItem(nonExisting, "DoesNotExist");

            //Assert
            _mockItemRepository
                .Verify(x => x.GetItemById(nonExisting)
                , Times.Once());
            _mockItemRepository
                .Verify(x => x.UpdateItem(It.IsAny<Item>()), Times.Never);
        }

        [Test]
        public void UpdateItem_ShouldThrowException_IfItemNameIsInvalid()
        {
            //arrange
            var item = new Item { Name = "Sample", Id = 1 };
            _mockItemRepository
                .Setup(x => x.GetItemById(item.Id))
                .Returns(item);
            _mockItemRepository
                .Setup(x => x.UpdateItem(It.IsAny<Item>())).Throws<ArgumentException>();

            //act
            Assert.Throws<ArgumentException>(() => _itemService.UpdateItem(item.Id, ""));

            //Assert
            _mockItemRepository
                .Verify(x => x.GetItemById(item.Id)
                , Times.Once());
            _mockItemRepository
                .Verify(x => x.UpdateItem(It.IsAny<Item>()), Times.Once());
        }

        [Test]
        public void UpdateItem_ShoulUpdateItem_IfItemNameIsValid()
        {
            //arrange
            var item = new Item { Name = "Sample", Id = 1 };
            _mockItemRepository
                .Setup(x => x.GetItemById(item.Id))
                .Returns(item);
            _mockItemRepository
                .Setup(x => x.UpdateItem(It.IsAny<Item>()));

            //act
            _itemService.UpdateItem(item.Id, "Sample Updated");

            //Assert
            _mockItemRepository
                .Verify(x => x.GetItemById(item.Id)
                , Times.Once());
            _mockItemRepository
                .Verify(x => x.UpdateItem(It.IsAny<Item>()), Times.Once());
        }

        [Test]
        public void DeleteItem_ShouldDeleteItem()
        {
            //Arrange
            var itemID = 5;
            _mockItemRepository.Setup(x => x.DeleteItem(itemID));

            // Act
            _itemService.DeleteItem(itemID);

            // Assert
            _mockItemRepository.Verify(x => x.DeleteItem(itemID), Times.Once());
        }

        [TestCase("", false)]
        [TestCase(null, false)]
        [TestCase("duaishdasuidsdhauisdhsauidhasui", false)]
        [TestCase("A", true)]
        [TestCase("Sample", true)]
        [TestCase("SampleName", true)]
        public void ValidateItemName_ShouldReturnCorrectAsnwer_IfItemNameIsValid(string name, bool isValid)
        {
            
            // Act
            var result = _itemService.ValidateItemName(name);

            // Assert
            Assert.That(result, Is.EqualTo(isValid));
        }
    }
}