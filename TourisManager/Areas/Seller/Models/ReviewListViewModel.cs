using System;
using System.Collections.Generic;
using TourisManager.Core.Entities;

namespace TourisManager.Areas.Seller.Models
{
    public class ReviewListViewModel
    {
        public Location? Location { get; set; }
        public List<Review> Reviews { get; set; } = new();
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
        public int Count5Star { get; set; }
        public int Count4Star { get; set; }
        public int Count3Star { get; set; }
        public int Count2Star { get; set; }
        public int Count1Star { get; set; }
        public int WithCommentsCount { get; set; }

        public int? SelectedRating { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }

        public int GetRatingPercentage(int star)
        {
            if (TotalReviews == 0) return 0;
            var count = star switch
            {
                5 => Count5Star,
                4 => Count4Star,
                3 => Count3Star,
                2 => Count2Star,
                1 => Count1Star,
                _ => 0
            };
            return (int)Math.Round((double)count * 100 / TotalReviews);
        }

        public int PositiveRatingPercentage
        {
            get
            {
                if (TotalReviews == 0) return 100;
                return (int)Math.Round((double)(Count4Star + Count5Star) * 100 / TotalReviews);
            }
        }
    }
}
