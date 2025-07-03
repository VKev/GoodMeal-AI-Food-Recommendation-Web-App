using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    [Table("food_image", Schema = "public")]
    public class FoodImage
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("food_name")]
        public string FoodName { get; set; } = string.Empty;

        [Required]
        [Column("image_url")]
        public string ImageUrl { get; set; } = string.Empty;

        [Column("create_at")]
        public DateTime CreatedAt { get; set; }
    }
}