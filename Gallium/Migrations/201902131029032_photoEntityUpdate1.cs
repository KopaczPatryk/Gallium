namespace Gallium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class photoEntityUpdate1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Photos", "HasFacesChecked", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Photos", "HasFacesChecked");
        }
    }
}
