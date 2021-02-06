module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Giraffe
open Saturn
open System
open Shared
open Db

type Storage () =
    let todos = ResizeArray<_>()

    member __.GetTodos () =
        List.ofSeq todos

    member __.AddTodo (todo: Todo) =
        if Todo.isValid todo.Description then
            todos.Add todo
            Ok ()
        else Error "Invalid todo"

let storage = Storage()

storage.AddTodo(Todo.create "Create new SAFE project") |> ignore
storage.AddTodo(Todo.create "Write your app") |> ignore
storage.AddTodo(Todo.create "Ship it !!!") |> ignore

let todosApi =
    { getTodos = fun () -> async { return storage.GetTodos() }
      addTodo =
        fun todo -> async {
            match storage.AddTodo todo with
            | Ok () -> return todo
            | Error e -> return failwith e
        } }

let webApp =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue todosApi
    |> Remoting.buildHttpHandler

let supplierApi : ISupplierApi = 
    {
        getAll = fun () -> async { return Db.Suppliers.getAll }
        get = fun id -> async { return Db.Suppliers.get id }
        add = fun supplier -> async { return Db.Suppliers.add supplier }
        update = fun supplier -> async { return Db.Suppliers.update supplier }
        delete = fun id -> async { return Db.Suppliers.delete id }
    }

let warehouseApi : IWarehouseApi = 
    {
        getAll = fun () -> async { return Db.Warehouses.getAll }
        get = fun id -> async { return Db.Warehouses.get id }
        add = fun warehouse -> async { return Db.Warehouses.add warehouse }
        update = fun warehouse -> async { return Db.Warehouses.update warehouse }
        delete = fun id -> async { return Db.Warehouses.delete id }
    }

let productApi : IProductApi = 
    {
        getAll = fun () -> async { return Db.Warehouses.getAll }
        get = fun id -> async { return Db.Warehouses.get id }
        add = fun product -> async { return Db.Warehouses.add product }
        update = fun product -> async { return Db.Warehouses.update product }
        delete = fun id -> async { return Db.Warehouses.delete id }
    }

let supplierApp = 
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue supplierApi
    |> Remoting.buildHttpHandler

let warehouseApp = 
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue warehouseApi
    |> Remoting.buildHttpHandler

let productApp = 
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue productApi
    |> Remoting.buildHttpHandler

// let webApp = choose [
//     supplierApp
//     warehouseApp
//     productApp]

let app =
    application {
        url "http://0.0.0.0:8085"
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
    }

run app
