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
        let firstSupplierId = if suppliers.IsEmpty then "" else suppliers.Head.Id.ToString()
        { model with Suppliers = suppliers; ProductForm = { model.ProductForm with SupplierId = firstSupplierId}}, cmd
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
        let cmd = Cmd.OfAsync.perform supplierApi.update { supplier with Id = id } RefreshSuppliers
        { model with SupplierForm = { Name = "" }}, cmd
    | DeleteSupplier id ->
        let cmd = Cmd.OfAsync.perform supplierApi.delete id RefreshSuppliers
        model, cmd

    | GotAllWarehouses warehouses ->
        let cmd = Cmd.OfAsync.perform productApi.getAll () GotAllProducts
        let firstWarehouseId = if warehouses.IsEmpty then "" else warehouses.Head.Id.ToString()
        { model with Warehouses = warehouses; ProductForm = { model.ProductForm with WarehouseId = firstWarehouseId} }, cmd
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
        let cmd = Cmd.OfAsync.perform warehouseApi.update { warehouse with Id = id } RefreshWarehouses
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
        let cmd = Cmd.OfAsync.perform productApi.update { product with Id = id } RefreshWarehouses
        { model with ProductForm = { Name = ""; SupplierId = ""; WarehouseId = "" }}, cmd
    | DeleteProduct id ->
        let cmd = Cmd.OfAsync.perform productApi.delete id RefreshProducts
        model, cmd
        
let anyReferencedProductsToSupplier (products: Product list) (supplierId: Guid) =
    products
    |> List.map (fun p -> p.SupplierId)
    |> List.contains(supplierId)

let anyReferencedProductsToWarehouse (products: Product list) (warehouseId: Guid) =
   products
   |> List.map (fun p -> p.WarehouseId)
   |> List.contains(warehouseId)

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

let containerBox (model : Model) (dispatch : Msg -> unit) =
    Box.box' [ ] [
        Content.content [ ] [
            Control.p [
                Control.Props [
                    Style [
                        FontWeight "bold"
                    ]
                ]
            ] [ str "Suppliers"]
            Content.Ol.ol [ ] [
                for supplier in model.Suppliers do
                    li [ ] [
                        Field.div [ Field.IsGrouped ] [ 
                            Control.p [ ] [ str supplier.Name ]
                            Control.p [ ] [ str (supplier.Id.ToString()) ] 
                            Button.a [
                                Button.Color IsPrimary
                                Button.Disabled (Supplier.isValid model.SupplierForm.Name |> not)
                                Button.OnClick (fun _ -> dispatch (UpdateSupplier supplier.Id))
                            ] [
                                str "Update"
                            ]
                            Button.a [
                                Button.Color IsPrimary
                                Button.Disabled (anyReferencedProductsToSupplier model.Products supplier.Id)
                                Button.OnClick (fun _ -> dispatch (DeleteSupplier supplier.Id))
                                Button.Props [
                                    Style [
                                        MarginLeft """10px"""
                                    ]
                                ]
                            ] [
                                str "Remove"
                            ]
                        ]
                    ]
            ]
            Field.div [ Field.IsGrouped ] [
                Control.p [ Control.IsExpanded ] [
                    Input.text [
                      Input.Value model.SupplierForm.Name
                      Input.Placeholder "Supplier name"
                      Input.OnChange (fun x -> SetSupplierName x.Value |> dispatch) ]
                ]
                Control.p [ ] [
                    Button.a [
                        Button.Color IsPrimary
                        Button.Disabled (Supplier.isValid model.SupplierForm.Name |> not)
                        Button.OnClick (fun _ -> dispatch AddSupplier)
                    ] [
                        str "Add Supplier"
                    ]
                ]
            ]
            Control.p [
                Control.Props [
                    Style [
                        FontWeight "bold"
                    ]
                ]
            ] [ str "Warehouses"]
            Content.Ol.ol [ ] [
                for warehouse in model.Warehouses do
                    li [ ] [
                        Field.div [ Field.IsGrouped ] [ 
                            Control.p [ ] [ str warehouse.Name ]
                            Control.p [ ] [ str (warehouse.Id.ToString()) ] 
                            Button.a [
                                Button.Color IsPrimary
                                Button.Disabled (Supplier.isValid model.WarehouseForm.Name |> not)
                                Button.OnClick (fun _ -> dispatch (UpdateWarehouse warehouse.Id))
                            ] [
                                str "Update"
                            ]
                            Button.a [
                                Button.Color IsPrimary
                                Button.Disabled (anyReferencedProductsToWarehouse model.Products warehouse.Id)
                                Button.OnClick (fun _ -> dispatch (DeleteWarehouse warehouse.Id))
                                Button.Props [
                                    Style [
                                        MarginLeft "10px"
                                    ]
                                ]
                            ] [
                                str "Remove"
                            ]
                        ]
                    ]
            ]
            Field.div [ Field.IsGrouped ] [
                Control.p [ Control.IsExpanded ] [
                    Input.text [
                      Input.Value model.WarehouseForm.Name
                      Input.Placeholder "Warehouse name"
                      Input.OnChange (fun x -> SetWarehouseName x.Value |> dispatch) ]
                ]
                Control.p [ ] [
                    Button.a [
                        Button.Color IsPrimary
                        Button.Disabled (Warehouse.isValid model.WarehouseForm.Name |> not)
                        Button.OnClick (fun _ -> dispatch AddWarehouse)
                    ] [
                        str "Add Warehouse"
                    ]
                ]
            ]
            Control.p [ 
                Control.Props [
                    Style [
                        FontWeight "bold"
                    ]
                ]
            ] [ str "Products"]
            Content.Ol.ol [ ] [
                for product in model.Products do
                    li [ ] [
                        Field.div [ Field.IsGrouped ] [
                            Field.div [] [
                                Control.p [ ] [ str product.Name ]
                                Control.p [ ] [ str ("id: " + product.Id.ToString()) ] 
                                Control.p [ ] [ str ("supplierId: " + product.SupplierId.ToString()) ] 
                                Control.p [ ] [ str ("warehouseId: " + product.WarehouseId.ToString()) ]
                            ]
                            Field.div [
                                Field.IsGrouped
                                Field.Props [
                                    Style [
                                        Display DisplayOptions.Flex
                                        JustifyContent JustifySelfOptions.Center
                                        AlignItems AlignItemsOptions.Center
                                    ]
                                ]
                            ] [
                                Button.a [
                                    Button.Color IsPrimary
                                    Button.Disabled (Supplier.isValid model.WarehouseForm.Name |> not)
                                    Button.OnClick (fun _ -> dispatch (UpdateProduct product.Id))
                                ] [
                                    str "Update"
                                ]
                                Button.a [
                                    Button.Color IsPrimary
                                    Button.OnClick (fun _ -> dispatch (DeleteProduct product.Id))
                                    Button.Props [
                                        Style [
                                            MarginLeft "10px"
                                        ]
                                    ]
                                ] [
                                    str "Remove"
                                ]
                            ]
                        ]
                    ]
            ]
            Field.div [ Field.IsGrouped ] [
                Control.p [ Control.IsExpanded ] [
                    Input.text [
                      Input.Value model.ProductForm.Name
                      Input.Placeholder "Product name"
                      Input.OnChange (fun x -> SetProductName x.Value |> dispatch) ]
                ]
                Control.p [ Control.IsExpanded ] [
                    select [
                        Value model.ProductForm.SupplierId
                        Placeholder "SupplierId"
                        OnChange (fun x -> SetProductSupplierId x.Value |> dispatch) ] 
                        [ 
                            for s in model.Suppliers do
                                option [ Value (s.Id.ToString()) ] [ str (s.Name) ]
                        ]
                ]
                Control.p [ Control.IsExpanded ] [
                    select [
                        Value model.ProductForm.WarehouseId
                        Placeholder "WarehouseId"
                        OnChange (fun x -> SetProductWarehouseId x.Value |> dispatch) ] 
                        [ 
                            for s in model.Warehouses do
                                option [ Value (s.Id.ToString()) ] [ str (s.Name) ]
                        ]
                ]
                Control.p [ ] [
                    Button.a [
                        Button.Color IsPrimary
                        Button.Disabled ((Warehouse.isValid model.ProductForm.Name) |> not)
                        Button.OnClick (fun _ -> dispatch AddProduct)
                    ] [
                        str "Add Product"
                    ]
                ]
            ]

        ]
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
                Container.container [ ] [ ]
            ]
        ]

        Hero.body [ ] [
            Container.container [ ] [
                Column.column [
                    Column.Width (Screen.All, Column.Is6)
                    Column.Offset (Screen.All, Column.Is3)
                ] [
                    Heading.p [ Heading.Modifiers [ Modifier.TextAlignment (Screen.All, TextAlignment.Centered) ] ] [ str "Inventory products" ]
                    containerBox model dispatch
                ]
            ]
        ]
    ]
