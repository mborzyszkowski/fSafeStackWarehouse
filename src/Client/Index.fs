module Index

open Elmish
open Fable.Remoting.Client
open Shared
open System

type SupplierForm =
    {
        Name: string
    }

type WarehouseForm =
    {
        Name: string
    }

type ProductForm =
    {
        SupplierId: string
        WarehouseId: string
        Name: string
    }

type Model =
    {
        Suppliers: Supplier list
        Warehouses: Warehouse list
        Products: Product list
        SupplierForm: SupplierForm
        WarehouseForm: WarehouseForm
        ProductForm: ProductForm
    }

type Msg =
    | GotAllSuppliers of Supplier list
    | SetSupplierName of string
    | RefreshSuppliers of unit
    | AddSupplier
    | UpdateSupplier of Guid
    | DeleteSupplier of Guid
    | GotAllWarehouses of Warehouse list
    | SetWarehouseName of string
    | RefreshWarehouses of unit
    | AddWarehouse
    | UpdateWarehouse of Guid
    | DeleteWarehouse of Guid
    | GotAllProducts of Product list
    | SetProductName of string
    | SetProductSupplierId of string
    | SetProductWarehouseId of string
    | RefreshProducts of unit
    | AddProduct
    | UpdateProduct of Guid
    | DeleteProduct of Guid

let supplierApi =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ISupplierApi>

let warehouseApi =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IWarehouseApi>

let productApi =
    Remoting.createApi()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IProductApi>

let init(): Model * Cmd<Msg> =
    let model =
        {
            Suppliers = []
            Warehouses = []
            Products = []
            SupplierForm = { Name = "" }
            WarehouseForm = { Name = "" }
            ProductForm = { Name = ""; SupplierId = ""; WarehouseId = "" }
        }
    let cmd = Cmd.OfAsync.perform supplierApi.getAll () GotAllSuppliers
    model, cmd

let update (msg: Msg) (model: Model): Model * Cmd<Msg> =
    match msg with
    | GotAllSuppliers suppliers ->
        let cmd = Cmd.OfAsync.perform warehouseApi.getAll () GotAllWarehouses
        { model with Suppliers = suppliers }, cmd
    | SetSupplierName name ->
        { model with SupplierForm = { Name = name }}, Cmd.none
    | RefreshSuppliers _ ->
        let cmd = Cmd.OfAsync.perform supplierApi.getAll () GotAllSuppliers
        model, cmd
    | AddSupplier ->
        let newSupplier = Supplier.create model.SupplierForm.Name
        let cmd = Cmd.OfAsync.perform supplierApi.add newSupplier RefreshSuppliers
        { model with SupplierForm = { Name = "" }}, cmd
    | UpdateSupplier id ->
        let supplier = Supplier.create model.SupplierForm.Name
        let cmd = Cmd.OfAsync.perform supplierApi.add { supplier with Id = id } RefreshSuppliers
        { model with SupplierForm = { Name = "" }}, cmd
    | DeleteSupplier id ->
        let cmd = Cmd.OfAsync.perform supplierApi.delete id RefreshSuppliers
        model, cmd

    | GotAllWarehouses warehouses ->
        let cmd = Cmd.OfAsync.perform productApi.getAll () GotAllProducts
        { model with Warehouses = warehouses }, cmd
    | SetWarehouseName name ->
        { model with WarehouseForm = { Name = name }}, Cmd.none
    | RefreshWarehouses _ ->
        let cmd = Cmd.OfAsync.perform warehouseApi.getAll () GotAllWarehouses
        model, cmd
    | AddWarehouse ->
        let newWarehouse = Warehouse.create model.WarehouseForm.Name
        let cmd = Cmd.OfAsync.perform warehouseApi.add newWarehouse RefreshWarehouses
        { model with WarehouseForm = { Name = "" }}, cmd
    | UpdateWarehouse id ->
        let warehouse = Warehouse.create model.WarehouseForm.Name
        let cmd = Cmd.OfAsync.perform warehouseApi.add { warehouse with Id = id } RefreshWarehouses
        { model with WarehouseForm = { Name = "" }}, cmd
    | DeleteWarehouse id ->
        let cmd = Cmd.OfAsync.perform warehouseApi.delete id RefreshWarehouses
        model, cmd

    | GotAllProducts products ->
        { model with Products = products }, Cmd.none
    | SetProductName name ->
        { model with ProductForm = { model.ProductForm with Name = name }}, Cmd.none
    | SetProductSupplierId supplierId ->
        { model with ProductForm = { model.ProductForm with SupplierId = supplierId }}, Cmd.none
    | SetProductWarehouseId warehouseId ->
        { model with ProductForm = { model.ProductForm with WarehouseId = warehouseId }}, Cmd.none
    | RefreshProducts _ ->
           let cmd = Cmd.OfAsync.perform productApi.getAll () GotAllProducts
           model, cmd
    | AddProduct ->
        let newProduct = Product.create model.ProductForm.Name (Guid.Parse model.ProductForm.SupplierId) (Guid.Parse model.ProductForm.WarehouseId)
        let cmd = Cmd.OfAsync.perform productApi.add newProduct RefreshProducts
        { model with ProductForm = { Name = ""; SupplierId = ""; WarehouseId = "" }}, cmd
    | UpdateProduct id ->
        let product = Product.create model.ProductForm.Name (Guid.Parse model.ProductForm.SupplierId) (Guid.Parse model.ProductForm.WarehouseId)
        let cmd = Cmd.OfAsync.perform productApi.add { product with Id = id } RefreshWarehouses
        { model with ProductForm = { Name = ""; SupplierId = ""; WarehouseId = "" }}, cmd

    | DeleteProduct id ->
        let cmd = Cmd.OfAsync.perform productApi.delete id RefreshProducts
        model, cmd

open Fable.React
open Fable.React.Props
open Fulma

let navBrand =
    Navbar.Brand.div [ ] [
        Navbar.Item.a [
            Navbar.Item.Props [ Href "https://safe-stack.github.io/" ]
            Navbar.Item.IsActive true
        ] [
            img [
                Src "/favicon.png"
                Alt "Logo"
            ]
        ]
    ]

    // Build container

let containerBox (model : Model) (dispatch : Msg -> unit) =
    Box.box' [ ] [
        Content.content [ ] [
            Content.Ol.ol [ ] [
                for todo in model.Suppliers do
                    li [ ] [ str todo.Name ]
            ]
        ]
        //Field.div [ Field.IsGrouped ] [
        //    Control.p [ Control.IsExpanded ] [
        //        Input.text [
        //          Input.Value model.Input
        //          Input.Placeholder "What needs to be done?"
        //          Input.OnChange (fun x -> SetInput x.Value |> dispatch) ]
        //    ]
        //    Control.p [ ] [
        //        Button.a [
        //            Button.Color IsPrimary
        //            Button.Disabled (Todo.isValid model.Input |> not)
        //            Button.OnClick (fun _ -> dispatch AddTodo)
        //        ] [
        //            str "Add"
        //        ]
        //    ]
        //]
    ]

let view (model : Model) (dispatch : Msg -> unit) =
    Hero.hero [
        Hero.Color IsPrimary
        Hero.IsFullHeight
        Hero.Props [
            Style [
                Background """linear-gradient(rgba(0, 0, 0, 0.5), rgba(0, 0, 0, 0.5)), url("https://unsplash.it/1200/900?random") no-repeat center center fixed"""
                BackgroundSize "cover"
            ]
        ]
    ] [
        Hero.head [ ] [
            Navbar.navbar [ ] [
                Container.container [ ] [ navBrand ]
            ]
        ]

        Hero.body [ ] [
            Container.container [ ] [
                Column.column [
                    Column.Width (Screen.All, Column.Is6)
                    Column.Offset (Screen.All, Column.Is3)
                ] [
                    Heading.p [ Heading.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ] [ str "fSharpSafeStackWeb" ]
                    containerBox model dispatch
                ]
            ]
        ]
    ]
