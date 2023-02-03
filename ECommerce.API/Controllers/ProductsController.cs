using ECommerce.API.Model;
using ECommerce.ProductCatalog.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {

        private readonly IProductCatalogService _service;

        public ProductsController() {
            var proxyFactory = new ServiceProxyFactory(
                c => new FabricTransportServiceRemotingClientFactory());

            _service = proxyFactory.CreateServiceProxy<IProductCatalogService>(
                    new Uri("fabric:/ECommerce/ECommerce.ProductCatalog"),
                    new ServicePartitionKey(0)); 
        }

        [HttpGet]
        public async Task<IEnumerable<ApiProduct>> GetAsync() {
            IEnumerable<Product> allProducts = await _service.GetAllProductsAsync();
            return allProducts.Select(p => new ApiProduct
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                IsAvailable = p.Availability > 0
            });
        }

        [HttpPost]
        public async Task PostAsync([FromBody] ApiProduct product) {
            var newProduct = new Product()
            {
                Id = Guid.NewGuid(),
                Name = product.Name,
                Price= product.Price,
                Description = product.Description,
                Availability = 100
            };
            await _service.AddProductAsync(newProduct);
        }
    } 
}