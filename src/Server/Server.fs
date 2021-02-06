module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn
open Shared
open Db
open Giraffe

let supplierApi : ISupplierApi = 
    {
        getAll = fun () -> async { return Db.Suppliers.getAll () }
        get = fun id -> async { return Db.Suppliers.get id }
        add = fun supplier -> async { return Db.Suppliers.add supplier }
        update = fun supplier -> async { return Db.Suppliers.update supplier }
        delete = fun id -> async { return Db.Suppliers.delete id }
    }

let warehouseApi : IWarehouseApi = 
    {
        getAll = fun () -> async { return Db.Warehouses.getAll ()}
        get = fun id -> async { return Db.Warehouses.get id }
        add = fun warehouse -> async { return Db.Warehouses.add warehouse }
        update = fun warehouse -> async { return Db.Warehouses.update warehouse }
        delete = fun id -> async { return Db.Warehouses.delete id }
    }

let productApi : IProductApi = 
    {
        getAll = fun () -> async { return Db.Products.getAll ()}
        get = fun id -> async { return Db.Products.get id }
        add = fun product -> async { return Db.Products.add product }
        update = fun product -> async { return Db.Products.update product }
        delete = fun id -> async { return Db.Products.delete id }
    }

let createApp api =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue api
    |> Remoting.buildHttpHandler

let supplierApp = createApp supplierApi

let warehouseApp = createApp warehouseApi
    
let productApp = createApp productApi

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
