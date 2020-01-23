using Bogus;
using Elasticsearch;
using ProductElasticSearch.Models;
using ProductElasticSearch.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace ProductElasticSearch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private IProductService _productService;

        public ProductsController(IProductService productService)

        {
            _productService = productService;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            var existing = await _productService.GetProductById(id);

            if (existing != null)
            {
                await _productService.SaveSingleAsync(existing);
                return Ok();
            }

            return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var existing = await _productService.GetProductById(id);

            if (existing != null)
            {
                await _productService.DeleteAsync(existing);
                return Ok();
            }

            return NotFound();
        }

        [HttpGet("fakeimport/{count}")]
        public async Task<ActionResult> Import(int count = 0)
        {
            var productFaker = new Faker<Product>()
                   .CustomInstantiator(f => new Product())
                   .RuleFor(p => p.Id, f => f.IndexFaker)
                   .RuleFor(p => p.Ean, f => f.Commerce.Ean13())
                   .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                   .RuleFor(p => p.Description, f => f.Lorem.Sentence(f.Random.Int(5, 20)))
                   .RuleFor(p => p.Brand, f => f.Company.CompanyName())
                   .RuleFor(p => p.Category, f => f.Commerce.Categories(1).First())
                   .RuleFor(p => p.Price, f => f.Commerce.Price(1, 1000, 2, "€"))
                   .RuleFor(p => p.Quantity, f => f.Random.Int(0, 1000))
                   .RuleFor(p => p.Rating, f => f.Random.Float(0, 1))
                   .RuleFor(p => p.ReleaseDate, f => f.Date.Past(2));


            var products = productFaker.Generate(count);
            await _productService.SaveManyAsync(products.ToArray());

            return Ok();
        }
    }
}
