using LibraryManagement.Tests.Helpers;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Repositories;
using LibraryManagement.Web.Services;
using Moq;

namespace LibraryManagement.Tests.Services;

public class CouponServiceTests
{
    private static CouponService CreateService(Coupon coupon)
    {
        var repository = new Mock<ICouponRepository>();
        repository.Setup(r => r.Query()).Returns(AsyncQueryable.Create(new List<Coupon> { coupon }));
        repository.Setup(r => r.GetByCodeAsync(It.IsAny<string>())).ReturnsAsync((string code) =>
            coupon.Code.Equals(code, StringComparison.OrdinalIgnoreCase) ? coupon : null);

        return new CouponService(repository.Object);
    }

    [Fact]
    public async Task ValidateCouponAsync_WhenCouponIsValid_CalculatesDiscount()
    {
        var service = CreateService(new Coupon
        {
            Code = "SAVE10",
            DiscountType = "Percentage",
            DiscountValue = 10,
            MinimumOrderAmount = 100,
            IsActive = true,
            ExpirationDate = DateTime.Today.AddDays(1),
            UsageLimit = 10,
            UsedCount = 0
        });

        var result = await service.ValidateCouponAsync("SAVE10", 500);

        Assert.True(result.Success);
        Assert.Equal(50, result.DiscountAmount);
    }

    [Fact]
    public async Task ValidateCouponAsync_WhenCouponExpired_ReturnsFalse()
    {
        var service = CreateService(new Coupon
        {
            Code = "OLD",
            DiscountType = "FixedAmount",
            DiscountValue = 50,
            IsActive = true,
            ExpirationDate = DateTime.Today.AddDays(-1)
        });

        var result = await service.ValidateCouponAsync("OLD", 500);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task ValidateCouponAsync_WhenUsageLimitReached_ReturnsFalse()
    {
        var service = CreateService(new Coupon
        {
            Code = "LIMIT",
            DiscountType = "FixedAmount",
            DiscountValue = 50,
            IsActive = true,
            UsageLimit = 2,
            UsedCount = 2
        });

        var result = await service.ValidateCouponAsync("LIMIT", 500);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task ValidateCouponAsync_WhenMinimumAmountNotMet_ReturnsFalse()
    {
        var service = CreateService(new Coupon
        {
            Code = "MIN500",
            DiscountType = "FixedAmount",
            DiscountValue = 50,
            MinimumOrderAmount = 500,
            IsActive = true
        });

        var result = await service.ValidateCouponAsync("MIN500", 300);

        Assert.False(result.Success);
    }
}
