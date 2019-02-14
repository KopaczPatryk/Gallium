namespace Gallium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class photoEntityUpdate2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Photos", "HasFaces");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Photos", "HasFaces", c => c.Boolean(nullable: false));
        }
    }
}
