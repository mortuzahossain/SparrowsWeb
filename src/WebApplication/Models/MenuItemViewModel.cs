using GlobalEntities.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using static GlobalEntities.Enums.GlobalEnums;

namespace WebApplication.Models
{
    public class MenuItemViewModel
    {
        public string Id { get; set; }
        public string IID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Details { get; set; }
        public string CategoryId { get; set; }

        [DisplayName("Menu Category")]
        public List<MenuCategory> CategoryList { get; set; }
        public string IsAvailableVariantId { get; set; }
        public AvailabilityCode IsAvailableVariant { get; set; }
        public string IsAvailableAdonsId { get; set; }
        public AvailabilityCode IsAvailableAdons { get; set; }
        public string Price { get; set; }
        public string DiscountPrice { get; set; }
        public string Image { get; set; }
        public HttpPostedFileBase File { get; set; }
        public StatusCode Status { get; set; }
        public string StatusId { get; set; }
        public string StockPositionId { get; set; }
        public AvailabilityCode StockPosition { get; set; }
    }
}