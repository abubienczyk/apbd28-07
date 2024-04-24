using WarehouseApp.Properties.Models.DTO_s;

namespace WarehouseApp.Properties.Repositories;

public interface IWarehouseRepository
{
    Task<bool> DoesProductExists(int id);
    Task<bool> DoesWarehouseExists(int id);
    Task<bool> DoesOrderExists(int id, int amount, DateTime created);
    Task<int> GetOrderId(int id);
    Task<bool> WasOrderRealized(int id);
    Task UpdateDate(int id);
    Task<double> GetPrice(int id);
    Task<int> InsertIntoProduct_Warehouse(InsertIntoProduct_WarehouseDTO data, double cena, int id);
    
    Task<int> InsertIntoProduct_Warehouse_With_Procedure(InsertIntoProduct_WarehouseDTO data);
}