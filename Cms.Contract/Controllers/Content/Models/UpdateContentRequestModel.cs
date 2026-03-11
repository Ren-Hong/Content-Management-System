using System.ComponentModel.DataAnnotations;

namespace Cms.Contract.Controllers.Content.Models
{
    public class UpdateContentRequestModel
    {
        [Required(ErrorMessage = "請提供內容")]
        public Guid ContentId { get; set; }

        [Required(ErrorMessage = "請提供版本")]
        public Guid RevisionId { get; set; }

        [Required(ErrorMessage = "請提供欄位資料")]
        [MinLength(1, ErrorMessage = "請至少填寫一個欄位")]
        public required List<UpdateContentFieldValueModel> FieldValues { get; set; }
    }

    public class UpdateContentFieldValueModel
    {
        [Required(ErrorMessage = "請提供欄位")]
        public Guid FieldId { get; set; }

        public string? FieldValue { get; set; }
    }
}
