using System.ComponentModel.DataAnnotations;

namespace APBD7.Models.DTOs;

public class WarehouseDTO
{
    [Required]
    private int IdWarehouse { get; set; }
    [MaxLength(200)]
    private string Name { get; set; }
    [MaxLength(200)]
    private string Address { get; set; }
}