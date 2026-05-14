using FoodOrderQuality.Core.Interfaces;
using FoodOrderQuality.Core.Models;

namespace FoodOrderQuality.Core.Services;

public sealed class OrderPricingService
{
    public const decimal MinimumDeliverySubtotal = 75m;
    public const decimal MaximumDeliveryDistanceKm = 10m;
    public const int MaximumQuantityPerLine = 20;

    private readonly DeliveryFeeCalculator _deliveryFeeCalculator;
    private readonly IDiscountRepository _discountRepository;
    private readonly IOpeningHoursPolicy _openingHoursPolicy;
    private readonly IClock _clock;

    public OrderPricingService(
        DeliveryFeeCalculator deliveryFeeCalculator,
        IDiscountRepository discountRepository,
        IOpeningHoursPolicy openingHoursPolicy,
        IClock clock)
    {
        _deliveryFeeCalculator = deliveryFeeCalculator;
        _discountRepository = discountRepository;
        _openingHoursPolicy = openingHoursPolicy;
        _clock = clock;
    }

    public OrderResult Calculate(OrderRequest? request)
    {
        if (request is null)
        {
            return OrderResult.Rejected(0m, 0m, 0m, ["ORDER_REQUEST_MISSING"]);
        }

        var errors = new List<string>();
        var items = request.Items?.ToArray() ?? [];

        if (items.Length == 0)
        {
            errors.Add("BASKET_EMPTY");
        }

        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item.MenuItemId))
            {
                errors.Add("ITEM_ID_MISSING");
            }

            if (string.IsNullOrWhiteSpace(item.Name))
            {
                errors.Add("ITEM_NAME_MISSING");
            }

            if (item.UnitPrice <= 0m)
            {
                errors.Add("ITEM_PRICE_INVALID");
            }

            if (item.Quantity < 1 || item.Quantity > MaximumQuantityPerLine)
            {
                errors.Add("ITEM_QUANTITY_OUT_OF_RANGE");
            }
        }

        var subtotal = items
            .Where(item => item.UnitPrice > 0m && item.Quantity > 0)
            .Sum(item => item.UnitPrice * item.Quantity);

        if (errors.Count > 0)
        {
            return OrderResult.Rejected(subtotal, 0m, 0m, errors.Distinct());
        }

        if (subtotal < MinimumDeliverySubtotal)
        {
            errors.Add("MINIMUM_ORDER_NOT_REACHED");
        }

        if (request.DeliveryDistanceKm < 0m)
        {
            errors.Add("DISTANCE_NEGATIVE");
        }
        else if (request.DeliveryDistanceKm > MaximumDeliveryDistanceKm)
        {
            errors.Add("DISTANCE_TOO_FAR");
        }

        var deliveryTime = request.RequestedDeliveryTime ?? _clock.Now;
        if (!_openingHoursPolicy.IsOpenAt(deliveryTime))
        {
            errors.Add("OUTSIDE_OPENING_HOURS");
        }

        if (errors.Count > 0)
        {
            return OrderResult.Rejected(subtotal, 0m, 0m, errors);
        }

        var deliveryFee = _deliveryFeeCalculator.Calculate(request.DeliveryDistanceKm);
        var discountAmount = 0m;
        string? appliedDiscountCode = null;

        if (!string.IsNullOrWhiteSpace(request.DiscountCode))
        {
            var discount = _discountRepository.FindByCode(request.DiscountCode.Trim());

            if (discount is null)
            {
                return OrderResult.Rejected(subtotal, deliveryFee, 0m, ["DISCOUNT_CODE_UNKNOWN"]);
            }

            var today = DateOnly.FromDateTime(deliveryTime.DateTime);
            if (discount.ExpiresOn.HasValue && discount.ExpiresOn.Value < today)
            {
                return OrderResult.Rejected(subtotal, deliveryFee, 0m, ["DISCOUNT_CODE_EXPIRED"]);
            }

            if (subtotal < discount.MinimumSubtotal)
            {
                return OrderResult.Rejected(subtotal, deliveryFee, 0m, ["DISCOUNT_MINIMUM_NOT_REACHED"]);
            }

            appliedDiscountCode = discount.Code;

            switch (discount.Kind)
            {
                case DiscountKind.Percentage:
                    discountAmount = Math.Round(subtotal * discount.Value / 100m, 2, MidpointRounding.AwayFromZero);
                    break;

                case DiscountKind.FreeDelivery:
                    deliveryFee = 0m;
                    break;

                case DiscountKind.FixedAmount:
                    discountAmount = Math.Min(discount.Value, subtotal);
                    break;

                default:
                    return OrderResult.Rejected(subtotal, deliveryFee, 0m, ["DISCOUNT_TYPE_UNSUPPORTED"]);
            }
        }

        var total = subtotal + deliveryFee - discountAmount;
        return new OrderResult(true, subtotal, deliveryFee, discountAmount, total, appliedDiscountCode, []);
    }
}