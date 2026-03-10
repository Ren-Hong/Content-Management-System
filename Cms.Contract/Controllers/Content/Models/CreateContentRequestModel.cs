using System.ComponentModel.DataAnnotations;

namespace Cms.Contract.Controllers.Content.Models
{
    public class CreateContentRequestModel
    {
        [Required(ErrorMessage = "請提供表單種類")]
        public Guid TypeId { get; set; }

        [Required(ErrorMessage = "請提供欄位資料")]
        [MinLength(1, ErrorMessage = "請至少填寫一個欄位")]
        public required List<CreateContentFieldValueModel> FieldValues { get; set; }
    }

    public class CreateContentFieldValueModel
    {
        [Required(ErrorMessage = "請提供欄位")]
        public Guid FieldId { get; set; }

        public string? FieldValue { get; set; }
    }
}
