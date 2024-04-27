using System.ComponentModel.DataAnnotations;

namespace APBD7.Models.DTOs;

public class ProductDTO
{
    [Required] 
    private int IdProduct { get; set; }
    [MaxLength(200)]
    private string Name { get; set; }
    [MaxLength(200)]
    private string Description { get; set; }
    private double Price { get; set; }
}