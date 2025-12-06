using FluentMigrator;

namespace Infrastructure.Persistence.Migrations;

[Migration(version: 2, description: "Add extra order history kinds for external processing events")]
public sealed class MigrationVersion2 : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    ALTER TYPE order_history_item_kind ADD VALUE 'approval_received';
                    ALTER TYPE order_history_item_kind ADD VALUE 'packing_started';
                    ALTER TYPE order_history_item_kind ADD VALUE 'packing_finished';
                    ALTER TYPE order_history_item_kind ADD VALUE 'delivery_started';
                    ALTER TYPE order_history_item_kind ADD VALUE 'delivery_finished';
                    """);
    }

    public override void Down() { }
}