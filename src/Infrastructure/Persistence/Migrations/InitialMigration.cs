using FluentMigrator;

namespace Infrastructure.Persistence.Migrations;

[Migration(version: 1, description: "Initial migration")]
public class InitialMigration : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    create table products
                    (
                        product_id    bigint primary key generated always as identity,
                    
                        product_name  text  not null,
                        product_price money not null
                    );
                    
                    create type order_state as enum ('created', 'processing', 'completed', 'cancelled');
                    
                    create table orders
                    (
                        order_id         bigint primary key generated always as identity,
                    
                        order_state      order_state              not null,
                        order_created_at timestamp with time zone not null,
                        order_created_by text                     not null
                    );
                    
                    create table order_items
                    (
                        order_item_id       bigint primary key generated always as identity,
                        order_id            bigint  not null references orders (order_id),
                        product_id          bigint  not null references products (product_id),
                    
                        order_item_quantity int     not null,
                        order_item_deleted  boolean not null
                    );
                    
                    create type order_history_item_kind as enum ('created', 'item_added', 'item_removed', 'state_changed');
                    
                    create table order_history
                    (
                        order_history_item_id         bigint primary key generated always as identity,
                        order_id                      bigint                   not null references orders (order_id),
                    
                        order_history_item_created_at timestamp with time zone not null,
                        order_history_item_kind       order_history_item_kind  not null,
                        order_history_item_payload    jsonb                    not null
                    );
                    """);
    }

    public override void Down()
    {
        Execute.Sql("""
                    drop table if exists order_history;
                    drop table if exists order_items;
                    drop table if exists orders;
                    drop table if exists products;
                    drop type if exists order_history_item_kind;
                    drop type if exists order_state;
                    """);
    }
}