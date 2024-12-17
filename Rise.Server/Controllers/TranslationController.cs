using Auth0.ManagementApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Helpers;
using Rise.Shared.Translations;
using System.Security.Claims;
using Serilog;

namespace Rise.Server.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator")]
public class TranslationController(ITranslationService translationService, IManagementApiClient managementApiClient) : ControllerBase
{
    private readonly ITranslationService translationService = translationService;
	private readonly IManagementApiClient managementApiClient = managementApiClient;

	[HttpGet]
    public async Task<IEnumerable<TranslationDto.Index>> Get()
    {
        var translations = await translationService.GetTranslationsAsync();
        Log.Information("Translations retrieved");
        return translations;
    }

    [HttpGet]
    [Route("accepted")]
    public async Task<IEnumerable<TranslationDto.Index>> GetAccepted([FromQuery] TranslationQueryObject query)
    {
        var translations = await translationService.GetAcceptedTranslationsAsync(query);
        Log.Information("Accepted translations retrieved");
        return translations;
    }

    [HttpGet]
    [Route("unaccepted")]
    public async Task<IEnumerable<TranslationDto.Index>> GetUnaccepted([FromQuery] UnacceptedTranslationQueryObject query)
    {
        var translations = await translationService.GetUnacceptedTranslationsAsync(query);
        Log.Information("Unaccepted translations retrieved");
        return translations;
    }

    [HttpGet("{id}")]
    public async Task<TranslationDto.Index> Get(int id)
    {
        var translation = await translationService.GetTranslationAsync(id);
        Log.Information("Translation retrieved by id");
        return translation;
    }

    [HttpGet("totalAccepted")]
    public async Task<int> GetTotalAccepted()
    {
        Log.Information("Total accepted translations retrieved");
        return await translationService.GetTotalAcceptedTranslationsAsync();
    }
    [HttpGet("totalUnaccepted")]
    public async Task<int> GetTotalUnaccepted()
    {
        Log.Information("Total unaccepted translations retrieved");
        return await translationService.GetTotalUnacceptedTranslationsAsync();
    }

    [HttpPost]
    public async Task<ActionResult<TranslationDto.Index>> Post(TranslationDto.Index translationDto)
    {
		var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
        var user = await managementApiClient.Users.GetAsync(userId);
		var newTranslation = await translationService.CreateTranslationAsync(translationDto, user.Email);
        Log.Information("Translation created");
        return CreatedAtAction(nameof(Get), new { id = newTranslation.Id }, newTranslation);
    }

    [HttpPut]
    public async Task<ActionResult<TranslationDto.Index>> Put(TranslationDto.Index translationDto)
    {
		var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
        var user = await managementApiClient.Users.GetAsync(userId);
		var updatedTranslation = await translationService.UpdateTranslationAsync(translationDto, user.Email);
        Log.Information("Translation updated");
        return updatedTranslation;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await translationService.DeleteTranslationAsync(id);
        Log.Information("Translation deleted");
        return NoContent();
    }

    [HttpPost("translate")]
    public async Task<ActionResult<string>> Translate([FromBody] string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            Log.Warning("Text to translate is required.");
            return BadRequest("Text to translate is required.");
        }

		var translatedText = await translationService.TranslateText(text);
        Log.Information("Text translated");
        return Ok(translatedText);
    }


}




