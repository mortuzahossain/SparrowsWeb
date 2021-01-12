using System.Web;
using static GlobalEntities.Enums.GlobalEnums;

namespace WebApplication.Models
{
    public class MenuCategoryViewModel
    {
        public string Id { get; set; }
        public string IID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Details { get; set; }
        public StatusCode Status { get; set; }
        public string StatusId { get; set; }
        public string Image { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}