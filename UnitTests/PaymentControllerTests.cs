using DiscreetKindnessApp.Controllers;
using DiscreetKindnessApp.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DiscreetKindnessApp.UnitTests
{
    public class PaymentControllerTests
    {
        private readonly Mock<IPaymentService> _mockPaymentService;
        private readonly PaymentController _controller;
        private readonly Mock<IUrlHelper> _mockUrlHelper;

        public PaymentControllerTests()
        {
            _mockPaymentService = new Mock<IPaymentService>();
            _mockUrlHelper = new Mock<IUrlHelper>();

            _controller = new PaymentController(_mockPaymentService.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() // Mock HttpContext for response headers
            };
            _controller.Url = _mockUrlHelper.Object;
        }

        [Fact]
        public void Donate_Returns303WithLocationHeader()
        {
            // Arrange
            string successUrl = "https://example.com/success";
            string cancelUrl = "https://example.com/cancel";
            string expectedSessionUrl = "https://stripe.com/checkout/session";

            _mockUrlHelper.Setup(u => u.Action("Success", "Payment", null, "https")).Returns(successUrl);
            _mockUrlHelper.Setup(u => u.Action("Cancel", "Payment", null, "https")).Returns(cancelUrl);
            _mockPaymentService.Setup(p => p.CreateCheckoutSession(2.00m, "usd", successUrl, cancelUrl)).Returns(expectedSessionUrl);

            // Act
            var result = _controller.Donate() as StatusCodeResult;
            var locationHeader = _controller.Response.Headers["Location"];

            // Assert
            Assert.NotNull(result);
            Assert.Equal(303, result.StatusCode);
            Assert.Equal(expectedSessionUrl, locationHeader);
        }

        [Fact]
        public void Donate_ReturnsBadRequest_WhenSessionCreationFails()
        {
            // Arrange
            _mockPaymentService.Setup(p => p.CreateCheckoutSession(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(string.Empty);

            // Act
            var result = _controller.Donate() as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Failed to create Stripe session.", result.Value);
        }
    }
}