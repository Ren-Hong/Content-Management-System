using System.ComponentModel.DataAnnotations;

namespace Cms.Contract.Controllers.ContentType.Models
{
    public class CreateContentTypeRequestModel
    {
        [Required(ErrorMessage = "請提供部門")]
        public Guid DepartmentId { get; set; }

        [Required(ErrorMessage = "請輸入表單種類名稱")]
        public required string TypeName { get; set; }

        [Required(ErrorMessage = "請至少新增一個欄位")]
        [MinLength(1, ErrorMessage = "請至少新增一個欄位")]
        public required List<CreateContentTypeFieldModel> Fields { get; set; }
    }

    public class CreateContentTypeFieldModel
    {
        [Required(ErrorMessage = "請輸入欄位名稱")]
        public required string FieldName { get; set; }

        [Required(ErrorMessage = "請選擇欄位型別")]
        public required string FieldType { get; set; }

        public bool IsRequired { get; set; }
    }
}
