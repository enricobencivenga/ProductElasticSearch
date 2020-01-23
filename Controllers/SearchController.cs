using ProductElasticSearch.Models;
using ProductElasticSearch.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nest;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ProductElasticSearch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IElasticClient _elasticClient;
        private readonly IOptionsSnapshot<ProductSettings> _settings;
        private readonly ILogger _logger;

        public SearchController(IProductService productService, IElasticClient elasticClient, IOptionsSnapshot<ProductSettings> settings)
        {
            _productService = productService;
            _elasticClient = elasticClient;
            _settings = settings;
        }

        [HttpGet("find")]
        public async Task<IActionResult> Find(string query, int page = 1, int pageSize = 5)
        {
            var response = await _elasticClient.SearchAsync<Product>(
                 s => s.Query(q => q.QueryString(d => d.Query('*' + query + '*')))
                     .From((page - 1) * pageSize)
                     .Size(pageSize));

            if (!response.IsValid)
            {
                // We could handle errors here by checking response.OriginalException 
                //or response.ServerError properties
                _logger.LogError("Failed to search documents");
                return Ok(new Product[] { });
            }

            return Ok(response.Documents);
        }

        //Only for development purpose
        [HttpGet("reindex")]
        public async Task<IActionResult> ReIndex()
        {
            await _elasticClient.DeleteByQueryAsync<Product>(q => q.MatchAll());

            var allProducts = (await _productService.GetProducts(int.MaxValue)).ToArray();

            foreach (var product in allProducts)
            {
                await _elasticClient.IndexDocumentAsync(product);
            }

            return Ok($"{allProducts.Length} product(s) reindexed");
        }
    }
}