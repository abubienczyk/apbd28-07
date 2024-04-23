using System.ComponentModel.DataAnnotations;

namespace WarehouseApp.Properties.Models.DTO_s;

public class AddProductDTO
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }
    [Required]
    [MaxLength(200)]
    public string Description { get; set; }
    [Required]
    [MinLength(2)]
    [MaxLength(25)]
    public int Price { get; set; }
}