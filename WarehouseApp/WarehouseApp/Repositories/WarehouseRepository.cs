using System.Data.Common;
using Microsoft.Data.SqlClient;
using WarehouseApp.Models;

namespace WarehouseApp.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly IConfiguration _configuration;
    
    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
   
    public async Task<bool> DoesProductExist(int id)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT 1 FROM PRODUCT WHERE IDPRODUCT=@ID;";
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        return result is not null;
    }

    public async Task<bool> DoesWarehouseExist(int id)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT 1 FROM WAREHOUSE WHERE IDWAREHOUSE=@ID;";
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        return result is not null;
    }

    public async Task<bool> DoesOrderExist(int id, int amount)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT 1 FROM [Order] WHERE IDPRODUCT=@ID AND AMOUNT=@AMOUNT;";
        command.Parameters.AddWithValue("@ID", id);
        command.Parameters.AddWithValue("@AMOUNT", amount);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        return result is not null;
    }

    public async Task<bool> WasOrderRealizde(int id)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT 1 FROM PRODUCT_WAREHOUSE WHERE IDORDER=@ID; ";
        command.Parameters.AddWithValue("@ID", id);
        

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        return result is not null;
    }

    public async  Task<int> GetOredrID(int id, int amount)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT IDORDER FROM [Order] WHERE IDPRODUCT=@ID AND AMOUNT=@AMOUNT";
        command.Parameters.AddWithValue("@ID", id);
        command.Parameters.AddWithValue("@AMOUNT", amount);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        
        return (int)result;
    }

    public async Task<DateTime> GetOrderDate(int id, int amount)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT CreatedAt FROM [Order] WHERE IDPRODUCT=@ID AND AMOUNT=@AMOUNT";
        command.Parameters.AddWithValue("@ID", id);
        command.Parameters.AddWithValue("@AMOUNT", amount);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        
        return (DateTime)result;
    }

    public async Task UpdateOrder(int id)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "UPDATE [Order] SET FULFILLEDAT=@DATE WHERE IDORDER=@ID";
        command.Parameters.AddWithValue("@ID", id);
        command.Parameters.AddWithValue("@FULFILLEDAT", DateTime.Now);

        await connection.OpenAsync();
       // await command.ExecuteScalarAsync();
        await command.ExecuteNonQueryAsync();

    }

    public async Task<int> GetPrice(int id)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "SELECT price FROM product WHERE IDPRODUCT=@ID";
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        
        return (int)result;
    }


    public async Task<int> addProduct(AddProductDTO data, int orderId, int price)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        var cena = price * data.Amount;
        command.Connection = connection;
        command.CommandText = "INSERT INTO PRODUCT_WAREOUSE VALUES(@WAREHOUSEID, @PRODUCTID, @ORDERID, @AMOUNT, @PRICE, @DATE); SELECT @@IDENTITY AS ID;";
        command.Parameters.AddWithValue("@WAREHOUSEID", data.IdWarehouse);
        command.Parameters.AddWithValue("@PRODUCTEID", data.IdProduct);
        command.Parameters.AddWithValue("@ORDERID", orderId);
        command.Parameters.AddWithValue("@AMOUNT", data.Amount);
        command.Parameters.AddWithValue("@PRICE", cena);
        command.Parameters.AddWithValue("@DATE", DateTime.Now);

        await connection.OpenAsync();
        var result=await command.ExecuteScalarAsync();
        return (int)result;
    }

    public async Task<int> addProductProcedure(AddProductDTO data)
    {
        int IdProductFromDb=0;
        int IdOrder=0;
        double Price=0;
        var query1 =
            "SELECT IdProduct,  Price FROM Product   WHERE IdProduct = @IdProduct;";
            //"SELECT TOP 1 @IdOrder = o.IdOrder  FROM \"Order\" o   \r\n LEFT JOIN Product_Warehouse pw ON o.IdOrder=pw.IdOrder  \r\n WHERE o.IdProduct=@IdProduct AND o.Amount=@Amount AND pw.IdProductWarehouse IS NULL AND  \r\n o.CreatedAt<@CreatedAt;  ";
            var query2 =
                "SELECT 1 FROM Warehouse WHERE IdWarehouse = @IdWarehouse;";
            //"SELECT @IdProductFromDb=Product.IdProduct, @Price=Product.Price FROM Product WHERE IdProduct=@IdProduct  ";
            var query3 =
                "    SELECT TOP 1 @IdOrder = o.IdOrder\n    FROM [Order] o\n             LEFT JOIN Product_Warehouse pw ON o.IdOrder = pw.IdOrder\n    WHERE o.IdProduct = @IdProduct\n      AND o.Amount = @Amount\n      AND pw.IdProductWarehouse IS NULL\n      AND o.CreatedAt < @CreatedAt\n    ORDER BY o.CreatedAt DESC;";
            //"UPDATE \"Order\" SET  \r\n FulfilledAt=@CreatedAt  \r\n WHERE IdOrder=@IdOrder;  ";
            var query4 =
                "SELECT 1 FROM Product_Warehouse WHERE IdOrder = @IdOrder;";
            //"INSERT INTO Product_Warehouse(IdWarehouse,   \r\n IdProduct, IdOrder, Amount, Price, CreatedAt)  \r\n VALUES(@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Amount*@Price, @CreatedAt); ";
            var query5 =
                "UPDATE [Order]\n    SET FulfilledAt = @CreatedAt\n    WHERE IdOrder = @IdOrder;";
            //" SELECT @@IDENTITY AS NewId;";
            var query6 =
                "INSERT INTO Product_Warehouse(IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)\n    VALUES(@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Amount * @Price, @CreatedAt);";
            var query7=" SELECT @@IDENTITY AS NewId;";
            
            
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;

        await connection.OpenAsync();
        
        var transaction = await connection.BeginTransactionAsync();
        command.Transaction = transaction as SqlTransaction;

        try
        {
            // Execution of the first command
            command.Parameters.Clear();
            command.CommandText = query1;
            command.Parameters.AddWithValue("@IdProduct", data.IdProduct);
            
            var read =  await command.ExecuteReaderAsync();
            var idOrdinal = read.GetOrdinal("IdProduct");
            var priceOrdinal = read.GetOrdinal("Price");
            while (await  read.ReadAsync())
            {
                IdProductFromDb = read.GetInt32(idOrdinal);
                Price = read.GetDouble(priceOrdinal);
            }

            if (IdProductFromDb == null)
                throw new Exception("IdProduct does not exist");
            
            
            // IdOrder =(int)await command.ExecuteScalarAsync();
            // if (IdOrder == 0)
            //     throw new Exception("Brak zamowienia");
            //
            
            //await command.ExecuteNonQueryAsync();
            
            // Execution of the second command
            command.Parameters.Clear();
            command.CommandText = query2;
            command.Parameters.AddWithValue("@IdWarehouse", data.IdWarehouse);

            var res = await command.ExecuteScalarAsync();
            if(res==null)
                throw new Exception("Warehouse does not exist'");
            
            if(data.Amount <= 0)
                throw new Exception("Data have to be grater than zero");
            //if (IdProductFromDb == null) throw new Exception();
            
            //await command.ExecuteNonQueryAsync();
            
            // Execution of the third command
            command.Parameters.Clear();
            command.CommandText = query3;
            command.Parameters.AddWithValue("@Product", IdOrder);
            command.Parameters.AddWithValue("@Amount", data.Amount);
            command.Parameters.AddWithValue("@CreatedAt", data.CreatedAt);
            IdOrder = (int)await command.ExecuteScalarAsync();
            
            if(IdOrder==null)
                throw new Exception("no order to fulfill");
            //await command.ExecuteNonQueryAsync();
            
            // Execution of the fourth command
            command.Parameters.Clear();
            command.CommandText = query4;
            command.Parameters.AddWithValue("@IdOrder", IdOrder);

            var res1 = await command.ExecuteScalarAsync();
            if(res1==null)
                throw new Exception("order was fulfill");
            
            //Execution of fifth command
            command.Parameters.Clear();
            command.CommandText = query5;
            command.Parameters.AddWithValue("@CreatedAt", data.CreatedAt);
            command.Parameters.AddWithValue("@IdOrder", IdOrder);

            await command.ExecuteNonQueryAsync();
            
            //Execution of sixth command
            command.Parameters.Clear();
            command.CommandText = query6;
            command.Parameters.AddWithValue("@IdWarehouse", data.IdWarehouse);
            command.Parameters.AddWithValue("@IdProduct", data.IdProduct);
            command.Parameters.AddWithValue("@IdOrder", IdOrder);
            command.Parameters.AddWithValue("@Amount", data.Amount);
            command.Parameters.AddWithValue("@Price", Price);
            command.Parameters.AddWithValue("@CreatedAt", data.CreatedAt);
            
            await command.ExecuteNonQueryAsync();
            
            // Execution of the seventh command
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