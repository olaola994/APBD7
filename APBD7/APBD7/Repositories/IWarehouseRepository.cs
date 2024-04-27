using APBD7.Models.DTOs;

namespace APBD7.Repositories;

public interface IWarehouseRepository
{
    Task<bool> DoesProductExist(int idProduct);
    Task<bool> DoesWarehouseExist(int idWarehouse);
    Task<bool> DoesOrderExist(int productId, int amount, DateTime createdAt);
    Task<int> GetOrderId(int productId, int amount, DateTime createdAt);
    Task<bool> IsOrderFulfilled(int IdOrder);
    Task UpdateOrder(int IdOrder);
    Task<int> AddProductToWarehouse(AddProductToWarehouseDTO request);
    Task<int> AddProductToWarehouseUsingStoredProc(int idProduct, int idWarehouse, int amount, DateTime createdAt);
}