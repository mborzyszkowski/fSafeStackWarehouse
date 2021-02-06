namespace Db

#r @".\bin\Debug\netcoreapp3.1\FSharp.Data.SqlProvider.dll"
#r @".\bin\Debug\netcoreapp3.1\Npgsql.dll" 
open FSharp.Data.Sql
open Shared

module Db = 

    [<Literal>]
    let connectionString = @"Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=password;"

    type Sql = SqlProvider<Common.DatabaseProviderTypes.POSTGRESQL, connectionString, UseOptionTypes=true>
    type DbContext = Sql.dataContext
    let ctx: DbContext = Sql.GetDataContext connectionString

    // Supplier Entity
    module Suppliers =
        let private suppliers = ctx.Public.Suppliers

        let private mapSupplier (dbRecord:DbContext.``public.suppliersEntity``) : Supplier = 
            {
                Id = dbRecord.Id
                Name = dbRecord.Name
            }

        let getAll (): Supplier list = 
            suppliers
            |> Seq.map (fun x -> x.MapTo<Supplier>())
            |> Seq.toList

        let get (id: Guid): Supplier option = 
            query {
                for supplier in suppliers do
                    where (supplier.Id = id)
                    select supplier
            }
            |> Seq.map (fun x -> x.MapTo<Supplier>)
            |> Seq.tryHead

        let add (supplier: Supplier) =
            let newSupplier = suppliers.Create()
            newSupplier.Id <- supplier.Id
            newSupplier.Name <- supplier.Name
            ctx.SubmitUpdates()

        let update (supplier: Supplier) =
            query {
                for supplier in suppliers do
                    where (supplier.Id = id)
                    select supplier
            }
            |> Seq.iter (fun sup -> 
                sup.Name <- supplier.Name
            )
            ctx.SubmitUpdates()

        let delete (id: Guid) =
            query {
                for supplier in suppliers do
                    where (supplier.Id = id)
                    select supplier
            }
            |> Seq.iter (fun sup -> sup.Delete())
            ctx.SubmitUpdates()

    
    // Warehouse Entity
    module Warehouses = 
        let private warehouses = ctx.Public.Warehouses

        let private mapWarehouse (dbRecord:DbContext.``public.warehousesEntity``) : Warehouse = 
            {
                Id = dbRecord.Id
                Name = dbRecord.Name
            }
        
        let getAll (): Warehouse list = 
            warehouses
            |> Seq.map (fun x -> x.MapTo<Warehouse>())
            |> Seq.toList

        let get (id: Guid): Warehouse option = 
            query {
                for warehouse in warehouses do
                    where (warehouse.Id = id)
                    select warehouse
            }
            |> Seq.map (fun x -> x.MapTo<Warehouse>)
            |> Seq.tryHead

        let add (warehouse: Warehouse) =
            let newWarehouse = warehouses.Create()
            newWarehouse.Id <- warehouse.Id
            newWarehouse.Name <- warehouse.Name
            ctx.SubmitUpdates()

        let update (warehouse: Warehouse) =
            query {
                for warehouse in warehouses do
                    where (warehouse.Id = id)
                    select warehouse
            }
            |> Seq.iter (fun wh -> 
                wh.Name <- warehouse.Name
            )
            ctx.SubmitUpdates()
            
        let delete (id: Guid) =
            query {
                for warehouse in warehouses do
                    where (warehouse.Id = id)
                    select warehouse
            }
            |> Seq.iter (fun wh -> wh.Delete())
            ctx.SubmitUpdates()

    // Product Entity
    module Products = 
        let private products = ctx.Public.Products

        let private mapProduct (dbRecord:DbContext.``public.productsEntity``) : Product = 
            {
                Id = dbRecord.Id
                SupplierId = dbRecord.SupplierId
                WarehouseId = dbRecord.WarehouseId
                Name = dbRecord.Name
            }
        
        let getAll (): Product list = 
            products
            |> Seq.map (fun x -> x.MapTo<Product>())
            |> Seq.toList

        let get (id: Guid): Product option = 
            query {
                for product in products do
                    where (product.Id = id)
                    select product
            }
            |> Seq.map (fun x -> x.MapTo<Product>)
            |> Seq.tryHead

        let add (product: Product) =
            let newProduct = products.Create()
            newProduct.Id <- product.Id
            newProduct.Name <- product.Name
            newProduct.SupplierId <- product.SupplierId
            newProduct.WarehouseId <- product.WarehouseId
            ctx.SubmitUpdates()

        let update (product: Product) =
            query {
                for product in products do
                    where (product.Id = id)
                    select product
            }
            |> Seq.iter (fun p -> 
                p.Name <- product.Name
                p.SupplierId <- product.SupplierId
                p.WarehouseId <- product.WarehouseId
            )
            ctx.SubmitUpdates()
            
        let delete (id: Guid) =
            query {
                for product in products do
                    where (product.Id = id)
                    select product
            }
            |> Seq.iter (fun p -> p.Delete())
            ctx.SubmitUpdates()