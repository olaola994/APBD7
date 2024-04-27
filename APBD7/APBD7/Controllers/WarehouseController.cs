using System.Data.SqlClient;
using APBD7.Models.DTOs;
using APBD7.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace APBD7.Controllers;

[ApiController]
[Route("/api/warehouse")]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseRepository _warehouseRepository;
    public WarehouseController(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    [HttpPost("AddProductToWarehouse")]
    public async Task<IActionResult> AddProductToWarehouse([FromBody] AddProductToWarehouseDTO request)
    {
        // czy produkt o podanym identyfikatorze istnieje
        bool doesProductExist = await _warehouseRepository.DoesProductExist(request.IdProduct);
        if (!doesProductExist)
        {
            return NotFound("Product not found.");
        }
        // czy magazyn o podanym identyfikatorze istnieje
        bool doesWarehouseExist = await _warehouseRepository.DoesWarehouseExist(request.IdWarehouse);
        if (!doesWarehouseExist)
        {
            return NotFound("Warehouse not found.");
        }
        // Wartość ilości przekazana w żądaniu powinna być większa niż 0
        if (request.Amount <= 0)
        {
            return BadRequest("Amount should be greater than 0.");
        }
        // Czy w tabeli Order istnieje rekord z IdProduktu i Ilością (Amount), które odpowiadają naszemu żądaniu
        bool doesOrderExist =
            await _warehouseRepository.DoesOrderExist(request.IdProduct, request.Amount, request.CreatedAt);
        if (!doesOrderExist)
        {
            return BadRequest("No existing purchase order for the product.");
        }
        int IdOrder = await _warehouseRepository.GetOrderId(request.IdProduct, request.Amount, request.CreatedAt);
        if (IdOrder == null)
        {
            return BadRequest("No existing order.");
        }
        // czy zamówienie zostało przypadkiem zrealizowane
        bool isOrderFulfilled = await _warehouseRepository.IsOrderFulfilled(IdOrder);
        if (isOrderFulfilled)
        {
            return BadRequest("Order is fulfilled");
        }
        
        // Aktualizujemy kolumnę FullfilledAt zamówienia na aktualną datę i godzinę
        await _warehouseRepository.UpdateOrder(IdOrder);
        // Wstawiamy rekord do tabeli Product_Warehouse. W wyniku operacji zwracamy wartość klucza głównego wygenerowanegodla rekordu wstawionego
        var key = await _warehouseRepository.AddProductToWarehouse(request);
        

        return Ok("Product " + key + " added to warehouse successfully.");
    }

    [HttpPost("AddProductToWarehouse/StoredProc")]
    public async Task<IActionResult> AddProductToWarehouseUsingStoredProc([FromBody] AddProductToWarehouseDTO request)
    {
        try
        {
            int newId = await _warehouseRepository.AddProductToWarehouseUsingStoredProc(request.IdProduct, request.IdWarehouse, request.Amount, request.CreatedAt);
            return Ok(new { Id = newId });
            
        }catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}