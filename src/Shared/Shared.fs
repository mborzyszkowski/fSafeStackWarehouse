namespace Shared

open System

type Supplier = 
    {
        Id: Guid
        Name: string
    }

type Warehouse = 
    {
        Id: Guid
        Name: string
    }

type Product = 
    {
        Id: Guid
        SupplierId: Guid
        WarehouseId: Guid
        Name: string
    }


module Supplier = 
    let isValid (name: string) = 
        String.IsNullOrWhiteSpace name |> not

    let create (name: string) : Supplier = 
    {
        Id = Guid.NewGuid()
        Name = name
    }

module Warehouse = 
    let isValid (name: string) = 
        String.IsNullOrWhiteSpace name |> not

    let create (name: string) : Warehouse = 
    {
        Id = Guid.NewGuid()
        Name = name
    }

module Product = 
    let isValid (name: string) = 
        String.IsNullOrWhiteSpace name |> not

    let create (name: string) (supplierId: Guid) (warehouseId: Guid) = 
    {
        Id = Guid.NewGuid()
        SupplierId = supplierId
        WarehouseId = warehouseId
        Name = name
    }

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

type ISupplierApi = 
    {
        getAll : unit -> Async<Supplier list>
        get : Guid -> Async<Supplier option>
        add : Supplier -> Async<unit>
        update : Supplier -> Async<unit>
        delete : Guid -> Async<unit>
    }

type IWarehouseApi = 
    {
        getAll : unit -> Async<Warehouse list>
        get : Guid -> Async<Warehouse option>
        add : Warehouse -> Async<unit>
        update : Warehouse -> Async<unit>
        delete : Guid -> Async<unit>
    }

type IProductApi = 
    {
        getAll : unit -> Async<Product list>
        get : Guid -> Async<Product option>
        add : Product -> Async<unit>
        update : Product -> Async<unit>
        delete : Guid -> Async<unit>
    }
