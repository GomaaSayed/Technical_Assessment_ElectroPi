using Moq;
using Technical_Assessment_ElectroPi.Application.Services;
using Technical_Assessment_ElectroPi.Contract;
using Technical_Assessment_ElectroPi.Contract.DTOs;
using Technical_Assessment_ElectroPi.Core.Entities;

namespace UnitTest
{
    public class ProductServiceTest
    {
        //private readonly Mock<IProductRepository> _mockProductRepository;
        //private readonly IProductService _productService;
        //public ProductServiceTest()
        //{
        //    _mockProductRepository = new Mock<IProductRepository>();
        //    _productService = new ProductService(_mockProductRepository.Object);
        //}
        //[Fact]
        //public async Task GetAllProductsAsync_ShouldReturnAllProducts()
        //{
        //    // Arrange
        //    var products = new List<ProductDTO>
        //    {
        //        new ProductDTO { Id = Guid.NewGuid(), Name = "Product 1", Price = 100 },
        //        new ProductDTO { Id =  Guid.NewGuid(), Name = "Product 2", Price = 200 }
        //    };

        //    _mockProductRepository.Setup(repo => repo.GetAllProductAsync()).ReturnsAsync(products);

        //    // Act
        //    var result = await _productService.GetAllProductAsync();

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(2, result.Count());
        //    Assert.Contains(result, p => p.Name == "Product 1");
        //    _mockProductRepository.Verify(repo => repo.GetAllProductAsync(), Times.Once);
        //}

        //[Fact]
        //public async Task GetProductByIdAsync_ShouldReturnProduct_WhenIdExists()
        //{
        //    // Arrange
        //    var id = Guid.NewGuid();
        //    var product = new Product { Id = id, Name = "Product 1", Price = 100 };
        //    _mockProductRepository.Setup(repo => repo.GetByProductIdAsync(id)).ReturnsAsync(product);

        //    // Act
        //    var result = await _productService.GetByProductIdAsync(id);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal("Product 1", result.Name);
        //    _mockProductRepository.Verify(repo => repo.GetByProductIdAsync(id), Times.Once);
        //}

        //[Fact]
        //public async Task AddProductAsync_ShouldAddProduct()
        //{
        //    // Arrange
        //    var product = new ProductDTO { Id = Guid.NewGuid(), Name = "Product 3", Price = 300 };
        //    _mockProductRepository.Setup(repo => repo.AddProductAsync(product));
        //    // Act
        //    await _productService.AddProductAsync(product);
        //    // Assert
        //    _mockProductRepository.Verify(repo => repo.AddProductAsync(product), Times.Once);
        //}
    }
}