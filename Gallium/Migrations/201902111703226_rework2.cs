namespace Gallium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rework2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.PhotoMiniatures", "OriginalImageFullPath");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PhotoMiniatures", "OriginalImageFullPath", c => c.String());
        }
    }
}
