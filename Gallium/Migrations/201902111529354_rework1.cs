namespace Gallium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rework1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Miniatures", newName: "PhotoMiniatures");
            RenameTable(name: "dbo.UserPhotoLocations", newName: "PhotoDirectories");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.PhotoDirectories", newName: "UserPhotoLocations");
            RenameTable(name: "dbo.PhotoMiniatures", newName: "Miniatures");
        }
    }
}
