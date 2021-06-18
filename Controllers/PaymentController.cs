using System.Threading.Tasks;
using FoodDiary.API.Data;
using FoodDiary.API.Models;
using FoodDiary.API.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Stripe;

namespace FoodDiary.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
public class PaymentController : Controller
{
    [HttpPost]
    public IActionResult Post([FromBody]Payment payment)
    {
        StripeConfiguration.ApiKey = "sk_test_51I7gdcKhTDjEMF3pO9H5CkzpCmR32lRTXrsjwZmfy2TyxqYwj9uuNuKvLTmxlYkBvNe5fGLrDmvVy8TgOLDnNNuf00iCI3Btqj";
        // You can optionally create a customer first, and attached this to the CustomerId
        var chargeOptions = new ChargeCreateOptions
        {
            Amount = Convert.ToInt32(payment.Amount * 100), // In cents, not dollars, times by 100 to convert
            Currency = "pln", // or the currency you are dealing with
            Description = "Thanks for donate!",
            Source = payment.Token
        };

        var service = new ChargeService();

        try
        {
            var response = service.Create(chargeOptions);

            // Record or do something with the charge information
        }
        catch (StripeException ex)
        {
            StripeError stripeError = ex.StripeError;
               
            // Handle error
        }

        // Ideally you would put in additional information, but you can just return true or false for the moment.
        return Ok("Thanks for donate!"); 
    }
}
}