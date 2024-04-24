using System.Data.Common;
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
    public async Task<bool> DoesOrderExists(int id, int amount, DateTime created)
    {
        var query = "SELECT 1 FROM Warehouse WHERE IDPRODUCT = @ID AND AMOUNT=@AMOUNT AND CreatedAt < @DATA";
        //spr czy data jest wieksza czy nie 
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);
        command.Parameters.AddWithValue("@AMOUNT", amount);
        command.Parameters.AddWithValue("@DATA", created);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();
        
        return res is not null;
    }

    public async Task<int> GetOrderId(int id)
    {
        var query = @"SELECT IdOrder
                      FROM Order 
                      IDPRODUCT = @ID";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);
        
        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();
        
        await reader.ReadAsync();

        if (!reader.HasRows) throw new Exception();
        var IDOrdinal = reader.GetOrdinal("IdOrder");
        int ID;
        {
            ID = reader.GetInt32(IDOrdinal);
        };

        return ID;
    }
    
    //done --> spr czy zamowienie zostalo zrealizowane
    public async Task<bool> WasOrderRealized(int id)
    {
        var query = "SELECT 1 FROM Product_Warehouse WHERE IDORDER = @ID";
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
        var query = "UPDATE ORDER SET FullfilledAt=@d WHERE ID = @ID";

        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@d", d);
        command.Parameters.AddWithValue("@ID", id);
        
        await connection.OpenAsync();
        
        await command.ExecuteNonQueryAsync();
    }

    public async Task<double> GetPrice(int id)
    {
        var query = @"SELECT Price
                      FROM Product
                      IDPRODUCT = @ID";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);
        
        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();
        
        await reader.ReadAsync();

        if (!reader.HasRows) throw new Exception();
        var PriceOrdinal = reader.GetOrdinal("Price");
        double cena;
        {
            cena = reader.GetDouble(PriceOrdinal);
        };

        return cena;
    }

    public async Task<int> InsertIntoProduct_Warehouse(InsertIntoProduct_WarehouseDTO data, double cena, int id)
    {
        DateTime aktulana = DateTime.Now;
        var query =
            "INSERT INTO Product_Warehouse VALUES(@idWarehouse,@idProduct, @IdOredr,@amount, @price,@createdAt )"+"SELECT @@IDENTITY";
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        // Definicja commanda
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@idWarehouse", data.IdWarehouse);
        command.Parameters.AddWithValue("@idProduct", data.IdProduct);
        command.Parameters.AddWithValue("@idOredr", id);
        command.Parameters.AddWithValue("@amounte", data.Amount);
        command.Parameters.AddWithValue("@price", cena);
        command.Parameters.AddWithValue("@createdAt", aktulana);


        // Wykonanie commanda
        //command.ExecuteNonQuery();
        
        int newId = Convert.ToInt32(await command.ExecuteScalarAsync());

        return newId;
    }

    public async Task<int> InsertIntoProduct_Warehouse_With_Procedure(InsertIntoProduct_WarehouseDTO data)
    {
        int IdProductFromDb=0;
        int IdOrder=0;
        double Price=0;
        var query1 =
            "SELECT TOP 1 @IdOrder = o.IdOrder  FROM \"Order\" o   \r\n LEFT JOIN Product_Warehouse pw ON o.IdOrder=pw.IdOrder  \r\n WHERE o.IdProduct=@IdProduct AND o.Amount=@Amount AND pw.IdProductWarehouse IS NULL AND  \r\n o.CreatedAt<@CreatedAt;  ";
        var query2 =
            "SELECT @IdProductFromDb=Product.IdProduct, @Price=Product.Price FROM Product WHERE IdProduct=@IdProduct  ";
        var query3 = "UPDATE \"Order\" SET  \r\n FulfilledAt=@CreatedAt  \r\n WHERE IdOrder=@IdOrder;  ";
        var query4 =
            "INSERT INTO Product_Warehouse(IdWarehouse,   \r\n IdProduct, IdOrder, Amount, Price, CreatedAt)  \r\n VALUES(@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Amount*@Price, @CreatedAt); ";
        var query5 = " SELECT @@IDENTITY AS NewId;";
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;

        await connection.OpenAsync();
        
        DbTransaction transaction = await connection.BeginTransactionAsync();
        command.Transaction = (SqlTransaction)transaction;

        try
        {
            // Execution of the first command
            command.Parameters.Clear();
            command.CommandText = query1;
            command.Parameters.AddWithValue("@IdProduct", data.IdProduct);
            command.Parameters.AddWithValue("@Amount", data.Amount);
            command.Parameters.AddWithValue("@CreatedAt", data.CreatedAt);
            
            IdOrder =(int)await command.ExecuteScalarAsync();
            if (IdOrder == 0)
                throw new Exception("Brak zamowienia");
            
            
            await command.ExecuteNonQueryAsync();
            
            // Execution of the second command
            command.Parameters.Clear();
            command.CommandText = query2;
            command.Parameters.AddWithValue("@IdProduct", data.IdProduct);
            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            {
                if (reader.Read())
                {
                   
                    IdProductFromDb = reader.GetInt32(0);
                    Price = reader.GetDouble(1);
                }
            }

            if (IdProductFromDb == null) throw new Exception();
            
            await command.ExecuteNonQueryAsync();
            
            // Execution of the third command
            command.Parameters.Clear();
            command.CommandText = query3;
            command.Parameters.AddWithValue("@IdOrder", IdOrder);
            command.Parameters.AddWithValue("@CreatedAt", data.CreatedAt);
            
            await command.ExecuteNonQueryAsync();
            // Execution of the fourth command
            double cena = Price * data.Amount;
            command.Parameters.Clear();
            command.CommandText = query4;
            command.Parameters.AddWithValue("@IdWarehouse", data.IdWarehouse);
            command.Parameters.AddWithValue("@IdProduct", data.IdProduct);
            command.Parameters.AddWithValue("@IdOrder", IdOrder);
            command.Parameters.AddWithValue("@Amount", data.Amount);
            command.Parameters.AddWithValue("@Price", cena);
            command.Parameters.AddWithValue("@CreatedAt", data.CreatedAt);
            
            await command.ExecuteNonQueryAsync();
            
            // Execution of the fifth command
            command.Parameters.Clear();
            command.CommandText = query5;
            int newId = Convert.ToInt32(await command.ExecuteScalarAsync());

            await transaction.CommitAsync();
            
           
            return newId;
            
            
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
}