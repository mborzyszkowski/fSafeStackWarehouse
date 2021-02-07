CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE suppliers (
    id uuid DEFAULT uuid_generate_v4(),
    name VARCHAR,
    PRIMARY KEY (id)
);


CREATE TABLE warehouses (
    id uuid DEFAULT uuid_generate_v4(),
    name VARCHAR,
    PRIMARY KEY (id)
);

CREATE TABLE products (
    id uuid DEFAULT uuid_generate_v4(),
	supplier_id uuid NOT NULL,
	warehouse_id uuid NOT NULL,
    name VARCHAR,
    PRIMARY KEY (id)
);
