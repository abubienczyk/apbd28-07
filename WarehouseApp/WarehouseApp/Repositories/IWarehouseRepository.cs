using WarehouseApp.Models;


namespace WarehouseApp.Repositories;

public interface IWarehouseRepository
{
    public Task<bool> DoesProductExist(int id);
    public Task<bool> DoesWarehouseExist(int id);
    public Task<bool> DoesOrderExist(int id, int amount);
    public Task<bool> WasOrderRealizde(int id);

    public Task<int> GetOredrID(int id, int amount);

    public Task<DateTime> GetOrderDate(int id, int amount);

    public Task UpdateOrder(int id);

    public Task<int> addProduct(AddProductDTO data, int orderId, int price);
    public Task<int> GetPrice(int id);

    public Task<int> addProductProcedure(AddProductDTO data);
}