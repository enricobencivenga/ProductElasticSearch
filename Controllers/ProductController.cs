using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ProductElasticSearch.Models;
using ProductElasticSearch.Services;

namespace ProductElasticSearch.Controllers
{
    public class ProductController : Controller
    {
        private IProductService _productService;
        private IOptionsSnapshot<ProductSettings> _settings;

        public ProductController(IProductService productService, IOptionsSnapshot<ProductSettings> settings)

        {
            _productService = productService;
            _settings = settings;
        }

        [Route("/{page:int?}")]
        public async Task<IActionResult> Index([FromRoute]int page = 0)
        {
            var products = await _productService.GetProducts(_settings.Value.ProductsPerPage, _settings.Value.ProductsPerPage * page);
            ViewData["Title"] = _settings.Value.Name + " - A blog about ASP.NET & Visual Studio";
            ViewData["Description"] = _settings.Value.Description;
            ViewData["prev"] = $"/{page + 1}/";
            ViewData["next"] = $"/{(page <= 1 ? null : page - 1 + "/")}";
            return View(products);
        }

        [Route("/edit/{id?}")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return View(new Product());
            }

            var product = await _productService.GetProductById(id);

            if (product != null)
            {
                return View(product);
            }

            return NotFound();
        }    

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
