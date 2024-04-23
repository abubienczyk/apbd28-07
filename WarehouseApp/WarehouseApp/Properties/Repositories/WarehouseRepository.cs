using Microsoft.Data.SqlClient;
using WarehouseApp.Properties.Models.DTO_s;

namespace WarehouseApp.Properties.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly IConfiguration _configuration;
    
    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    //done czy istanieje taki produkt
    public async Task<bool> DoesProductExists(int id)
    {
        var query = "SELECT 1 FROM Product WHERE ID = @ID";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }
    
    //done czy istanieje taki magazyn 
    public async Task<bool> DoesWarehouseExists(int id)
    {
        var query = "SELECT 1 FROM Warehouse WHERE ID = @ID";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }
    
    //czy istnieje takie zamowienie
    public async Task<bool> DoesOredrExists(int id)
    {
        var query = "SELECT CreatedAt, FullfilledAt FROM Warehouse WHERE IDPRODUCT = @ID";
        //spr czy data jest wieksza czy nie 
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }

    public async Task<AddProductDTO> AddProduct(AddProductDTO product)
    {
        throw new NotImplementedException();
    }
    
    //done --> spr czy zamowienie zostalo zrealizowane
    public async Task<bool> WasOrderRealized(int id)
    {
        var query = "SELECT 1t FROM Product_Warehouse WHERE IDORDER = @ID";
        //spr czy data jest wieksza czy nie 
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }
    
    // done --> aktualizacja daty wykonania zamowienia
    public  async Task UpdateDate(int id)
    {
        DateTime d=DateTime.Now;
        var query = @"UPDATE ORDER SET FullfilledAt=@d WHERE ID = @ID";

        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@d", d);
        command.Parameters.AddWithValue("@ID", id);
        
        await connection.OpenAsync();
        
        await command.ExecuteNonQueryAsync();
    }

    public Task<InsertIntoProduct_WarehouseDTO> InsertIntoProduct_Warehouse(InsertIntoProduct_WarehouseDTO data)
    {
        throw new NotImplementedException();
    }
}