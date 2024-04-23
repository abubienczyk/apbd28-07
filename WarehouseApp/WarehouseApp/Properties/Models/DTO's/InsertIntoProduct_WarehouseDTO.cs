using System.ComponentModel.DataAnnotations;

namespace WarehouseApp.Properties.Models.DTO_s;

public class InsertIntoProduct_WarehouseDTO
{   
    [Required]
    public int IdProduct { get; set; }
    [Required]
    public int IdWarehouse { get; set; }
    [Required]
    public int Amount { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
}