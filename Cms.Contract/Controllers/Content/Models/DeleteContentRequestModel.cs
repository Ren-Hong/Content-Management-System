using System.ComponentModel.DataAnnotations;

namespace Cms.Contract.Controllers.Content.Models
{
    public class DeleteContentRequestModel
    {
        [Required(ErrorMessage = "請提供內容")]
        public Guid ContentId { get; set; }
    }
}
