using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CCSystem.Presentation.Models.Category
{
    public class CategoryResponse
    {
        [JsonPropertyName("categoryId")]
        public int CategoryId { get; set; }
        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; } = string.Empty;
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; } // Trả về URL ảnh
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
        [JsonPropertyName("createdDate")]
        public DateTime CreatedDate { get; set; }
        [JsonPropertyName("updatedDate")]
        public DateTime UpdatedDate { get; set; }


    }
}