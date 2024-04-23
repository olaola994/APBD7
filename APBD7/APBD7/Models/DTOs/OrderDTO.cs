using System.ComponentModel.DataAnnotations;

namespace APBD7.Models.DTOs;

public class OrderDTO
{
    [Required] 
    private int IdOrder { get; set; }
    private int IdProduct { get; set; }
    private int Amount { get; set; }
    private DateTime CreatedAt { get; set; }
    private DateTime FulfilledAt { get; set; }
}