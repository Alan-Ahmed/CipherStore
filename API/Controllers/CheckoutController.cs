using Application.DTOs;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CheckoutController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IConfiguration _config;

    public CheckoutController(IOrderService orderService, IConfiguration config)
    {
        _orderService = orderService;
        _config = config;
        StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
    }

    // 1. SKAPA SESSIONEN
    [HttpPost]
    public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateOrderDto dto)
    {
        try
        {
            var order = await _orderService.CreateOrderAsync(dto);

            var options = new SessionCreateOptions
            {
                Metadata = new Dictionary<string, string>
                {
                    {"orderId", order.Id.ToString()}
                },
                LineItems = dto.Items.Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.UnitPrice * 100),
                        Currency = "sek",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.ProductName ?? "Produkt",
                        },
                    },
                    Quantity = item.Quantity,
                }).ToList(),
                Mode = "payment",
                // Skicka med session_id tillbaka till frontend
                SuccessUrl = "http://localhost:5173/success?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = "http://localhost:5173/cancel",
            };

            var service = new SessionService();
            var session = service.Create(options);

            return Ok(new { sessionId = session.Id, redirectUrl = session.Url });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    // 2. VERIFIERA BETALNINGEN (Denna anropas av din SuccessPage)
    [HttpGet("verify")]
    public async Task<IActionResult> VerifyPayment(string sessionId)
    {
        try
        {
            var service = new SessionService();
            var session = service.Get(sessionId);

            if (session.PaymentStatus == "paid")
            {
                if (session.Metadata.TryGetValue("orderId", out var orderIdStr) && int.TryParse(orderIdStr, out int orderId))
                {
                    // ÄNDRA STATUS TILL PAID
                    await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Paid.ToString());
                    return Ok(new { status = "Paid", orderId = orderId });
                }
            }

            return BadRequest("Betalning ej godkänd.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}