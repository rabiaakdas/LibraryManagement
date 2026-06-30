using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.Web.Entity;
using LibraryManagement.Web.Models;
using LibraryManagement.Web.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Web.Services
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _coupons;
        private readonly ILogger<CouponService> _logger;

        public CouponService(ICouponRepository coupons, ILogger<CouponService> logger = null)
        {
            _coupons = coupons;
            _logger = logger;
        }

        public async Task<List<Coupon>> GetAllAsync()
        {
            return await _coupons.Query()
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Coupon> GetByIdAsync(int id)
        {
            return await _coupons.GetByIdAsync(id);
        }

        public async Task CreateAsync(Coupon coupon)
        {
            coupon.Code = coupon.Code.Trim().ToUpper();
            coupon.CreatedAt = DateTime.Now;
            _coupons.Add(coupon);
            await _coupons.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, Coupon coupon)
        {
            var existingCoupon = await _coupons.GetByIdAsync(id);
            if (existingCoupon == null)
            {
                return false;
            }

            existingCoupon.Code = coupon.Code.Trim().ToUpper();
            existingCoupon.DiscountType = coupon.DiscountType;
            existingCoupon.DiscountValue = coupon.DiscountValue;
            existingCoupon.MinimumOrderAmount = coupon.MinimumOrderAmount;
            existingCoupon.IsActive = coupon.IsActive;
            existingCoupon.ExpirationDate = coupon.ExpirationDate;
            existingCoupon.UsageLimit = coupon.UsageLimit;

            await _coupons.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var coupon = await _coupons.GetByIdAsync(id);
            if (coupon == null)
            {
                return false;
            }

            coupon.IsActive = false;
            await _coupons.SaveChangesAsync();
            return true;
        }

        public async Task<CouponResultViewModel> ValidateCouponAsync(string code, decimal subTotal)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return Invalid("Kupon kodu boş olamaz.", code);
            }

            var coupon = await _coupons.GetByCodeAsync(code);
            if (coupon == null || !coupon.IsActive)
            {
                return Invalid("Kupon bulunamadı veya aktif değil.", code);
            }

            if (coupon.ExpirationDate.HasValue && coupon.ExpirationDate.Value.Date < DateTime.Today)
            {
                return Invalid("Kuponun süresi dolmuş.", code);
            }

            if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit.Value)
            {
                return Invalid("Kupon kullanım limiti dolmuş.", code);
            }

            if (subTotal < coupon.MinimumOrderAmount)
            {
                return Invalid("Kupon için minimum sipariş tutarı sağlanmıyor.", code);
            }

            var discountAmount = CalculateDiscount(coupon, subTotal);
            _logger?.LogInformation(
                "Kupon uygulandı. CouponCode: {CouponCode}, SubTotal: {SubTotal}, DiscountAmount: {DiscountAmount}",
                coupon.Code,
                subTotal,
                discountAmount);

            return new CouponResultViewModel
            {
                Success = true,
                Message = "Kupon başarıyla uygulandı.",
                CouponCode = coupon.Code,
                DiscountAmount = discountAmount
            };
        }

        public async Task MarkAsUsedAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return;
            }

            var coupon = await _coupons.GetByCodeAsync(code);
            if (coupon == null)
            {
                return;
            }

            coupon.UsedCount++;
            await _coupons.SaveChangesAsync();
        }

        private CouponResultViewModel Invalid(string message, string code)
        {
            _logger?.LogInformation("Geçersiz kupon denendi. CouponCode: {CouponCode}, Reason: {Reason}", code, message);
            return new CouponResultViewModel { Success = false, Message = message, CouponCode = code ?? string.Empty };
        }

        private static decimal CalculateDiscount(Coupon coupon, decimal subTotal)
        {
            var discountAmount = coupon.DiscountType == "Percentage"
                ? subTotal * coupon.DiscountValue / 100
                : coupon.DiscountValue;

            return Math.Min(discountAmount, subTotal);
        }
    }
}
