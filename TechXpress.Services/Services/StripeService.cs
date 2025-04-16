using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using TechXpress.Data.Entities;
using TechXpress.Services.Interfaces;
using TechXpress.Services.Services;

namespace TechXpress.Services
{
    public class StripeService : IStripeService
    {
        private readonly StripeSettings _stripeSettings;

        public StripeService(IOptions<StripeSettings> stripeSettings)
        {
            _stripeSettings = stripeSettings.Value;
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }
        public async Task<Session> CreateCheckoutSessionAsync(string userId, Cart cart)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = cart.Items.Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100),
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                        },
                    },
                    Quantity = item.Quantity,
                }).ToList(),
                Mode = "payment",
                SuccessUrl = "https://localhost:7251/Order/StripeSuccess?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = "https://localhost:7251/Cart",
                Metadata = new Dictionary<string, string>
        {
            { "UserId", userId }
        }
            };

            var service = new SessionService();
            return await service.CreateAsync(options);
        }
        public async Task<Session> GetSessionDetailsAsync(string sessionId)
        {
            var service = new SessionService();
            return await service.GetAsync(sessionId);
        }



        public async Task<string> GetPaymentStatusAsync(string sessionId)
        {
            var service = new SessionService();
            var session = await service.GetAsync(sessionId);
            return session.PaymentStatus; // e.g., "paid", "unpaid"
        }
    }
}
