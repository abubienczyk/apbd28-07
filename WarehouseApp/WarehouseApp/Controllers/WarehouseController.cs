using Microsoft.AspNetCore.Mvc;
using WarehouseApp.Models;

using WarehouseApp.Repositories;

namespace WarehouseApp.Properties.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseRepository _warehouseRepository;
    public WarehouseController(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository =warehouseRepository;
    }
    [HttpPost]
    public async Task<IActionResult> PostProduct(AddProductDTO data)
    {
        //punkt 1
        if (!await _warehouseRepository.DoesProductExist(data.IdProduct))
            return NotFound("no product found");
        if (!await _warehouseRepository.DoesWarehouseExist(data.IdWarehouse))
            return NotFound("no warehouse found");
        if (data.Amount <= 0)
            return Conflict("amount should be grater than zero");
        //punkt 2
        if (! await _warehouseRepository.DoesOrderExist(data.IdProduct, data.Amount))
            return NotFound("no oredr found");
        var orderDate = await _warehouseRepository.GetOrderDate(data.IdProduct, data.Amount);
        if (orderDate < data.CreatedAt)
            return Conflict("date should be younger");
        //punkt 3
        var orderId = await _warehouseRepository.GetOredrID(data.IdProduct, data.Amount);
        if (await _warehouseRepository.WasOrderRealizde(orderId))
            return Conflict("order wasnt realized");
        //punkt 4
        await _warehouseRepository.UpdateOrder(orderId);
        //punkt 5
        var price = await _warehouseRepository.GetPrice(data.IdProduct);
        var id = await _warehouseRepository.addProduct(data, orderId, price);
        //punkt 6
        return Created("",id);
    }

    [HttpPost]
    public async Task<IActionResult> addProductProcedure(AddProductDTO data)
    {
        var id = await _warehouseRepository.addProductProcedure(data);
        return Created("",id);
    }
    // robic na taskach !!
    //wstrzykiwianie zaleznosci --> dependency injection
    
    //ExectuteScalar -> zwraca obkiet lub null , jedna kolumna 
    
    
    //pobieranie danych --> wazne bedzie na kolosie
    //zamiast kluczy obcych zwracamy oelny obiekt 
    
    //jesli takie same nazwy kolumn to zrobic aliasy np name AS productName
    
    // jsk tworzymy obiekt to zwracamy DTO
    
    //haslo yourStrong@Password
    
    // TransactionScope - w jakim zakresie ma zialac transakcja 
    
    //zadanie 2 zdjecie
    
    // laczenie z baza manuall
    // name SA paswor yourStrong@Password
    
}