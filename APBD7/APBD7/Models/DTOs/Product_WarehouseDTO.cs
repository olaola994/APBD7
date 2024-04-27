using System.ComponentModel.DataAnnotations;

namespace APBD7.Models.DTOs;

public class Product_WarehouseDTO
{
    [Required] 
    private int IdProductWarehouse { get; set; }
    private int IdWarehouse { get; set; }
    private int IdProduct { get; set; }
    
    private int IdOrder { get; set; }
    private int Amount { get; set; }
    private double Price { get; set; }
    private DateTime CreatedAt { get; set; }
}