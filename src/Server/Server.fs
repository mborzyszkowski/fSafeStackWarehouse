module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn
open Shared
open Giraffe
open System
//open Db


// DB Mocks - storages

type SupplierStorage () =
    let suppliers = ResizeArray<Supplier>()

    member __.GetAll() =
        List.ofSeq suppliers

    member __.Get(id: Guid) =
        Some (suppliers.Find(fun s -> s.Id = id))

    member __.Add(supplier: Supplier) =
        suppliers.Add supplier
        
    member __.Update(supplier: Supplier) =
        let supplierToUpdate = suppliers.Find(fun s -> s.Id = supplier.Id)
        suppliers.Remove(supplierToUpdate) |> ignore
        suppliers.Add(supplier)

    member __.Delete(id: Guid) =
        let supplierToDelete = suppliers.Find(fun s -> s.Id = id)
        suppliers.Remove(supplierToDelete) |> ignore

let supplierStorage = SupplierStorage()

let supplierApiMock : ISupplierApi =
    {
        getAll = fun () -> async { return supplierStorage.GetAll() }
        get = fun id -> async { return supplierStorage.Get(id) }
        add = fun supplier -> async { return supplierStorage.Add(supplier) }
        update = fun supplier -> async { return supplierStorage.Update(supplier) }
        delete = fun id -> async { return supplierStorage.Delete(id) }
    }


type WarehouseStorage () =
    let warehouses = ResizeArray<Warehouse>()

    member __.GetAll() =
        List.ofSeq warehouses

    member __.Get(id: Guid) =
        Some (warehouses.Find(fun s -> s.Id = id))

    member __.Add(warehouse: Warehouse) =
        warehouses.Add warehouse
        
    member __.Update(warehouse: Warehouse) =
        let warehouseToUpdate = warehouses.Find(fun s -> s.Id = warehouse.Id)
        warehouses.Remove(warehouseToUpdate) |> ignore
        warehouses.Add(warehouse)

    member __.Delete(id: Guid) =
        let warehouseToDelete = warehouses.Find(fun s -> s.Id = id)
        warehouses.Remove(warehouseToDelete) |> ignore

let warehouseStorage = WarehouseStorage()

let warehouseApiMock : IWarehouseApi =
    {
        getAll = fun () -> async { return warehouseStorage.GetAll() }
        get = fun id -> async { return warehouseStorage.Get(id) }
        add = fun warehouse -> async { return warehouseStorage.Add(warehouse) }
        update = fun warehouse -> async { return warehouseStorage.Update(warehouse) }
        delete = fun id -> async { return warehouseStorage.Delete(id) }
    }


type ProductsStorage () =
    let products = ResizeArray<Product>()

    member __.GetAll() =
        List.ofSeq products

    member __.Get(id: Guid) =
        Some (products.Find(fun s -> s.Id = id))

    member __.Add(product: Product) =
        products.Add product
        
    member __.Update(product: Product) =
        let productToUpdate = products.Find(fun s -> s.Id = product.Id)
        products.Remove(productToUpdate) |> ignore
        products.Add(product)

    member __.Delete(id: Guid) =
        let productToDelete = products.Find(fun s -> s.Id = id)
        products.Remove(productToDelete) |> ignore

let productsStorage = ProductsStorage()

let productApiMock : IProductApi =
    {
        getAll = fun () -> async { return productsStorage.GetAll() }
        get = fun id -> async { return productsStorage.Get(id) }
        add = fun product -> async { return productsStorage.Add(product) }
        update = fun product -> async { return productsStorage.Update(product) }
        delete = fun id -> async { return productsStorage.Delete(id) }
    }



// Real Apis


//let supplierApi : ISupplierApi = 
//    {
//        getAll = fun () -> async { return Db.Suppliers.getAll () }
//        get = fun id -> async { return Db.Suppliers.get id }
//        add = fun supplier -> async { return Db.Suppliers.add supplier }
//        update = fun supplier -> async { return Db.Suppliers.update supplier }
//        delete = fun id -> async { return Db.Suppliers.delete id }
//    }

//let warehouseApi : IWarehouseApi = 
//    {
//        getAll = fun () -> async { return Db.Warehouses.getAll ()}
//        get = fun id -> async { return Db.Warehouses.get id }
//        add = fun warehouse -> async { return Db.Warehouses.add warehouse }
//        update = fun warehouse -> async { return Db.Warehouses.update warehouse }
//        delete = fun id -> async { return Db.Warehouses.delete id }
//    }

//let productApi : IProductApi = 
//    {
//        getAll = fun () -> async { return Db.Products.getAll ()}
//        get = fun id -> async { return Db.Products.get id }
//        add = fun product -> async { return Db.Products.add product }
//        update = fun product -> async { return Db.Products.update product }
//        delete = fun id -> async { return Db.Products.delete id }
//    }

let createApp api =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue api
    |> Remoting.buildHttpHandler

// Real DB
//let supplierApp = createApp supplierApi
//let warehouseApp = createApp warehouseApi
//let productApp = createApp productApi

// DB mock - storages
let supplierApp = createApp supplierApiMock
let warehouseApp = createApp warehouseApiMock
let productApp = createApp productApiMock


let webApp = choose [
    supplierApp
    warehouseApp
    productApp]


let app =
    application {
        url "http://0.0.0.0:8085"
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
    }

run app
