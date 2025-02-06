namespace DiscreetKindnessApp.Services
{
    public interface IPaymentService
    {
        string CreateCheckoutSession(decimal amount, string currency, string successUrl, string cancelUrl);
    }
}
