using System.ComponentModel.DataAnnotations;

namespace Cms.Contract.Controllers.ContentType.Models
{
    public class DeleteContentTypeRequestModel
    {
        [Required(ErrorMessage = "請提供問卷")]
        public Guid TypeId { get; set; }
    }
}
