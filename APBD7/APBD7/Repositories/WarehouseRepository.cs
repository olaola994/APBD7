using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using APBD7.Models.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;

namespace APBD7.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly IConfiguration _configuration;
    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> DoesProductExist(int id)
    {
        var query = "SELECT 1 FROM Product WHERE IdProduct = @ID";

        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);
        
        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }
    public async Task<bool> DoesWarehouseExist(int id)
    {
        var query = "SELECT 1 FROM Warehouse WHERE IdWarehouse = @ID";

        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);
        
        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }

    public async Task<bool> DoesOrderExist(int productId, int amount, DateTime createdAt)
    {
        var query = @"SELECT COUNT(*) FROM [Order] WHERE IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt < @CreatedAt";

        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@IdProduct", productId);
        command.Parameters.AddWithValue("@Amount", amount);
        command.Parameters.AddWithValue("@CreatedAt", createdAt);

        await connection.OpenAsync();

        var result = await command.ExecuteScalarAsync();
        int orderCount = Convert.ToInt32(result);

        return orderCount > 0;
    }
    public async Task<int> GetOrderId(int productId, int amount, DateTime createdAt)
    {
        var query = @"SELECT IdOrder FROM [Order] WHERE IdProduct = @IdProduct AND Amount = @Amount AND CreatedAt < @CreatedAt";

        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@IdProduct", productId);
        command.Parameters.AddWithValue("@Amount", amount);
        command.Parameters.AddWithValue("@CreatedAt", createdAt);

        await connection.OpenAsync();

        var result = await command.ExecuteScalarAsync();
        int insertedId = result != null ? Convert.ToInt32(result) : 0;
        return insertedId;
    }
    
    public async Task<bool> IsOrderFulfilled(int IdOrder)
    {
        var query = "SELECT COUNT(*) FROM Product_Warehouse WHERE IdOrder = @IdOrder";
    
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@IdOrder", IdOrder);
        await connection.OpenAsync();

        var result = await command.ExecuteScalarAsync();
        int rowCount = Convert.ToInt32(result);

        return rowCount > 0;
    }

    public async Task UpdateOrder(int IdOrder)
    {
        var query = @"UPDATE [Order] SET FulfilledAt = GETDATE() WHERE IdOrder = @IdOrder";
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand(query, connection);

        command.Parameters.AddWithValue("@IdOrder", IdOrder);
        await connection.OpenAsync();
        await command.ExecuteNonQueryAsync();
    }
    

    public async Task<int> AddProductToWarehouse(AddProductToWarehouseDTO request)
    {
        decimal productPrice = await GetProductPrice(request.IdProduct);
        int IdOrder = await GetOrderId(request.IdProduct, request.Amount, request.CreatedAt);

        var query = @"INSERT INTO Product_Warehouse (IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) 
                  VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt);
                    SELECT SCOPE_IDENTITY();";
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand(query, connection);
        
        command.Parameters.AddWithValue("@IdWarehouse", request.IdWarehouse);
        command.Parameters.AddWithValue("@IdProduct", request.IdProduct);
        command.Parameters.AddWithValue("@IdOrder", IdOrder);
        command.Parameters.AddWithValue("@Amount", request.Amount);
        command.Parameters.AddWithValue("@Price", request.Amount * productPrice);
        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
        await connection.OpenAsync();
        int insertedId = Convert.ToInt32(await command.ExecuteScalarAsync());
    
        return insertedId;
    }
    private async Task<decimal> GetProductPrice(int productId)
    {
        var query = "SELECT Price FROM Product WHERE IdProduct = @IdProduct";
    
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand(query, connection);
    
        command.Parameters.AddWithValue("@IdProduct", productId);
    
        await connection.OpenAsync();
        
        object result = await command.ExecuteScalarAsync();
        
        int insertedId = result != null ? Convert.ToInt32(result) : 0;
    
        return insertedId;
    }
    
}