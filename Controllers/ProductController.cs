using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Models;
using System.Linq;

namespace Shop.Controllers
{
    [Route("products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<Product>>> Get([FromServices] DataContext context)
        {
            var products = await context
            .Products
            .Include(x => x.Category)//Executa um Join na tabela categoria e traz as suas informações por referência
            .AsTracking()
            .ToListAsync();
            return products;
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<Product>> GetById([FromServices] DataContext context, int id)
        {

            var product = await context
                .Products
                .Include(x => x.Category)//Incluí a categoria dentro do produto
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CategoryId == id);
            return Ok(product);
        }

        [HttpGet]//product/categories/1
        [Route("categories/{id:int}")]
        public async Task<ActionResult<List<Product>>> GetByCategory([FromServices] DataContext context, int id)
        {

            var products = await context
                .Products
                .Include(x => x.Category)
                .AsNoTracking()
                .Where(x => x.Category.Id == id)
                .ToListAsync();

            return Ok(products);
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<Product>> Post(
            [FromServices] DataContext context,
            [FromBody] Product model
        )
        {
            if (ModelState.IsValid)
            {
                context.Products.Add(model);
                await context.SaveChangesAsync();
                return Ok(new { message = "Cadastro realizado com sucesso!" });
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
    }
}