﻿namespace FreeCourse.Web.Models.BasketDtos;

public class BasketViewModel
{
    public BasketViewModel()
    {
        _basketItems = new List<BasketItemViewModel>();
    }

    public string UserId { get; set; } = "";

    public string DiscountCode { get; set; } = "";
    
    public int? DiscountRate { get; set; }

    private List<BasketItemViewModel> _basketItems;

    public List<BasketItemViewModel> BasketItems
    {
        get
        {
            if (HasDiscount)
            {
                _basketItems.ForEach(x =>
                {
                    var discountPrice = x.Price * ((decimal)DiscountRate.Value / 100);
                    x.AppliedDiscount(Math.Round(x.Price - discountPrice, 2));
                });
            }

            return _basketItems;
        }
        set
        {
            _basketItems = value;
        }
    }

    public decimal TotalPrice
    {
        get => BasketItems.Sum(x => x.GetCurrentPrice);
    }

    public bool HasDiscount
    {
        get => !string.IsNullOrEmpty(DiscountCode) && DiscountRate.HasValue;
    }
    
}   