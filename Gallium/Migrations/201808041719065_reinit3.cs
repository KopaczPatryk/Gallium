namespace Gallium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reinit3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Photos", "HasFaces", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Photos", "HasFaces");
        }
    }
}
