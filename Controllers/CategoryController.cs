using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;



//Endpoint => URL
//https://localhost:5001/categories

[Route("categories")]
public class CategoryController : ControllerBase
{

    //https://localhost:5001/categories
    [HttpGet]
    [Route("")]
    public async Task<ActionResult<List<Category>>> Get(
        [FromServices]DataContext context
    )
    {
        //Neste caso estamos usando o AsNoTracking porque precisamos apenas dos dados da categoria e não 
        //da sua referência para excluir ou interar sobre ela. Desta forma economizamos memória.
        var categories = await context.Categories.AsNoTracking().ToListAsync();//ToListAsync executa de fato o processo no banco
            return Ok(categories);
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<Category>> GetById(
        int id,
        [FromServices]DataContext context
    )
    {
        var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return Ok(category);
    }

    [HttpPost]
    [Route("")]
    public async Task<ActionResult<List<Category>>> Post(
        [FromBody] Category model,
        [FromServices] DataContext context
    )
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);//Já traz as anotações feitas nos models (Erros)

        try
        {
            //Utiliza o contexto que vem pela injeção de dependencia para adicionar um model
            context.Categories.Add(model);
            //Salva as alteraçoes de modo assíncrono no nosso banco de dados.
            await context.SaveChangesAsync();
            return Ok(model);
        }
        catch
        {

            return BadRequest(new { message = "Não foi possível criar a categoria" });
        }
    }

    [HttpPut]
    [Route("{id:int}")]
    public async Task<ActionResult<List<Category>>> Put(
    int id,
    [FromBody] Category model,
    [FromServices] DataContext context)
    {


        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            context.Entry<Category>(model).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return Ok(model);
        }
        //Verifica se foi um erro de concorrência e gera uma exception personalizada
        catch (DbUpdateConcurrencyException)
        {
            return BadRequest(new { message = "Este registro já foi atualizado"});
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Não foi possível atualizar a categoria"});
        }

    }

    [HttpDelete]
    [Route("{id:int}")]
    public async Task<ActionResult<List<Category>>> Delete(
        int id,
        [FromServices]DataContext context
    )
    {
        //Pega a categoria no BD e armazena em varíavel, tranzendo todos os proxys criados pelo Entity
        //Só usamos o FirsOrDefaultAsync porque de fato precisamos da categoria para remove-la
        var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if(category == null)
                return NotFound(new { message = "categoria não encontrada"});

        try
        {
            context.Categories.Remove(category);
            await context.SaveChangesAsync();
            return Ok(new { message = "Categoria removida com sucesso !"});
        }
        catch (Exception)
        {
            return BadRequest(new { message = "Não foi possível remover esta categoria"});
        }

    }


}
