using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductAPI.Application.DTOs;
using ProductAPI.Application.DTOs.Conversions;
using ProductAPI.Application.Interfaces;

namespace ProductAPI.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProductRepository productRepository) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProducts()
        {
            // Get all products from repo
            var products = await productRepository.GetAllAsync();
            if (!products.Any())
                return NotFound("No product detected in the database");

            // convert data from entity to DTO and return
            var (_, list) = ProductConversions.FromEntity(null!, products);
            return list!.Any() ? Ok(list) : NotFound("No product found");
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            // Get single product from the repo
            var product = await productRepository.FindByIdAsync(id);
            if (product == null) 
                return NotFound("Product requested not found");

            // convert from entity to DTO and return
            var (_product, _) = ProductConversions.FromEntity(product, null!);
            return _product != null ? Ok(_product) : NotFound("Product not found");
        }

        [HttpPost]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO product)
        {
            // check model state is all data annotations are passed
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // convert to entity
            var getEntity = ProductConversions.ToEntity(product);
            var response = await productRepository.CreateAsync(getEntity);
            return response.Flag == true ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
        {
            // check model state is all data annotations are passed
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // convert to entity
            var getEntity = ProductConversions.ToEntity(product);
            var response = await productRepository.UpdateAsync(getEntity);
            return response.Flag == true ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDTO product)
        {
            // convert to entity
            var getEntity = ProductConversions.ToEntity(product);
            var response = await productRepository.DeleteAsync(getEntity);
            return response.Flag == true ? Ok(response) : BadRequest(response);
        }
    }
}
