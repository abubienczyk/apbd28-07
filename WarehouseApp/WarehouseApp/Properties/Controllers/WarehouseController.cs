using Microsoft.AspNetCore.Mvc;
using WarehouseApp.Properties.Models;
using WarehouseApp.Properties.Models.DTO_s;
using WarehouseApp.Properties.Repositories;

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
    public async Task<IActionResult> AddProdcut(InsertIntoProduct_WarehouseDTO p)
    {
        //punkt 1 -> czy istnieje magazyn czy istnieje produkt i czy amount>0
        if (!await _warehouseRepository.DoesProductExists(p.IdProduct))
            return NotFound();
        if (!await _warehouseRepository.DoesWarehouseExists(p.IdWarehouse))
            return NotFound();
        if (p.Amount <= 0)
            return BadRequest();
        //punkt 2 --> czy jest takie zamowienie z id produktu i amount i czy data<cretedOredr
        if (!await _warehouseRepository.DoesOrderExists(p.IdProduct, p.Amount, p.CreatedAt))
            return NotFound();
        
        //punkt 3 czy jest takie zamowienie w warehouse
        int id = await _warehouseRepository.GetOrderId(p.IdProduct);
        if (await _warehouseRepository.WasOrderRealized(id))
            return Conflict();
        //punkt 4 aktualizacja Fullfiledat
        await  _warehouseRepository.UpdateDate(id);
        //punkt 5 wstawienie nowego produktu
        double cena = await _warehouseRepository.GetPrice(p.IdProduct);
        cena *= p.Amount;
        
        int newID=await _warehouseRepository.InsertIntoProduct_Warehouse(p, cena, id);
        return Ok(newID);
    }

    [HttpPost]
    public async Task<IActionResult> AddProdcutWithProcedure(InsertIntoProduct_WarehouseDTO p)
    {
        var index = await _warehouseRepository.InsertIntoProduct_Warehouse_With_Procedure(p);
        return Ok(index);
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