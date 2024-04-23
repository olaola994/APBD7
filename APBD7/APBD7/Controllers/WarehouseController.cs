using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

namespace APBD7.Controllers;

[ApiController]
[Route("/api/warehouse")]
public class WarehouseController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public  WarehouseController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("{id}")]
    public IActionResult IsProductExists()
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT * FROM Animal";
        
        var reader = command.ExecuteReader();
    }
    [HttpGet("{id}")]
    public IActionResult IsWarehouseExists()
    {
        
    }
}