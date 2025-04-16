using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stripe.Checkout;
using TechXpress.Data.Entities;


namespace TechXpress.Services.Interfaces
{
    public interface IStripeService
    {
        Task<Session> CreateCheckoutSessionAsync(string userId, Cart cart);
        Task<string> GetPaymentStatusAsync(string sessionId);
        Task<Session> GetSessionDetailsAsync(string sessionId);
    }
}
