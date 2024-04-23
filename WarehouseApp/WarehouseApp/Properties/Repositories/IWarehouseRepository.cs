using WarehouseApp.Properties.Models.DTO_s;

namespace WarehouseApp.Properties.Repositories;

public interface IWarehouseRepository
{
    Task<bool> DoesProductExists(int id);
    Task<bool> DoesWarehouseExists(int id);
    Task<bool> DoesOredrExists(int id);
    Task<AddProductDTO> AddProduct(AddProductDTO product);
    Task<bool> WasOrderRealized(int id);
    Task UpdateDate(int id);
    Task<InsertIntoProduct_WarehouseDTO> InsertIntoProduct_Warehouse(InsertIntoProduct_WarehouseDTO data);
}