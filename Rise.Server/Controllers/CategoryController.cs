using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Helpers;
using Rise.Shared.Machineries;
using Serilog;

namespace Rise.Server.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
	private readonly ICategoryService categoryService = categoryService;

    [HttpGet]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<IEnumerable<CategoryDto.Detail>> Get([FromQuery] CategoryQueryObject query)
	{
		var categories = await categoryService.GetCategoriesAsync(query);
        Log.Information("Categories retrieved");
        return categories;
	}

    [HttpGet("{id}")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<CategoryDto.Detail> Get(int id)
    {
        var category = await categoryService.GetCategoryAsync(id);
        Log.Information("Category retrieved by id");
        return category;
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<CategoryDto.Detail>> Post(CategoryDto.Create category)
    {
        var newCategory = await categoryService.CreateCategoryAsync(category);
        Log.Information("Category created");
        return CreatedAtAction(nameof(Get), new { id = newCategory.Id }, newCategory);
    }

    [HttpPut]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<CategoryDto.Detail>> Put(CategoryDto.Update category)
    {
        var updatedCategory = await categoryService.UpdateCategoryAsync(category.Id, category);
        Log.Information("Category updated");
        return updatedCategory;
    }

    [HttpDelete ("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult> Delete(int id)
    {
        await categoryService.DeleteCategoryAsync(id);
        Log.Information("Category deleted");
        return NoContent();
    }

}
