using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Serilog;

using Rise.Shared.Orders;
using Rise.Shared.Helpers;
using Rise.Shared.Quotes;

namespace Rise.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController(IOrderService orderService) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<IEnumerable<OrderDto.Index>> Get([FromQuery] OrderQueryObject query)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

        if (User.IsInRole("Verkoper"))
        {
            Log.Information("Orders retrieved for salesperson");
            return await orderService.GetOrdersForSalespersonAsync(userId, query);
        }

        Log.Information("Orders retrieved for admin");
        return await orderService.GetOrdersAsync(query);
    
    }

    [HttpGet("{orderNumber}")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<OrderDto.Detail> Get(string orderNumber)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

        if (User.IsInRole("Verkoper"))
        {
            Log.Information("Order retrieved for salesperson");
            return await orderService.GetOrderForSalespersonAsync(userId, orderNumber);
        }

        Log.Information("Order retrieved for admin");
        return await orderService.GetOrderAsync(orderNumber);
    }

    [HttpGet("HasOrder/{orderNumber}")]
    [Authorize(Roles = "Administrator,Verkoper")]
    public async Task<bool> GetHasOrder(string orderNumber)
    {
        Log.Information("Checking if quote has order");
        return await orderService.CheckIfQuoteHasOrder(orderNumber);
    }

    [HttpPost]
    [Authorize(Roles = "Verkoper")]
    public async Task<ActionResult<OrderDto.Index>> Post(OrderDto.Create order)
    {
        var newOrder = await orderService.CreateOrderAsync(order);
        Log.Information("Order created");
        return CreatedAtAction(nameof(Get), new { id = newOrder.Id }, newOrder);
    }

    [HttpGet("Total")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<int> GetTotalOrders()
    {

        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
        if (User.IsInRole("Verkoper"))
        {
            Log.Information("Total orders retrieved for salesperson");
            return await orderService.GetTotalOrdersForSalesperson(userId);
        }

        Log.Information("Total orders retrieved for admin");
        return await orderService.GetTotalOrders();
    }

    [HttpPut("{orderNumber}")]
    [Authorize(Roles = "Verkoper")]
    public async Task<OrderDto.Detail> Put(string orderNumber, OrderDto.Update order)
    {
        Log.Information("Order updated");
        return await orderService.UpdateOrderAsync(orderNumber, order);
    }

    [HttpGet("android/{orderNumber}")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<OrderDto.Android> GetAndroid(string orderNumber)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)!.Value;

        if (User.IsInRole("Verkoper"))
        {
            Log.Information("Order retrieved for salesperson");
            return await orderService.GetOrderForSalespersonAsyncAndroid(userId, orderNumber);
        }

        Log.Information("Order retrieved for admin");
        return await orderService.GetOrderAsyncAndroid(orderNumber); 

    }

    /// <summary>
    /// Generates a PDF for the specified order number.
    /// </summary>
    /// <param name="orderNumber">The order number.</param>
    /// <returns>The PDF file.</returns>
    [HttpGet("{orderNumber}/pdf")]
    [Authorize(Roles = "Administrator, Verkoper")]
    public async Task<IActionResult> GetQuotePdf(string orderNumber)
    {
        try
        {
            var pdfBytes = await orderService.GeneratePdf(orderNumber);

            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                Log.Warning("PDF generation failed: received empty byte array.");
                throw new Exception("PDF generation failed: received empty byte array.");
            }

            Log.Information("PDF generated");
            return File(pdfBytes, "application/pdf", $"{orderNumber}.pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Er is een fout opgetreden tijdens het genereren van de PDF: {ex.Message}");
        }
    }

}
