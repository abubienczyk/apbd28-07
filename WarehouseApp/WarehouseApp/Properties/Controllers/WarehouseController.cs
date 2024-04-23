using Microsoft.AspNetCore.Mvc;
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