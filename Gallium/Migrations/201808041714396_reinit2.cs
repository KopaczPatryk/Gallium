namespace Gallium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reinit2 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.DetectedFace1", newName: "DetectedFaces");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.DetectedFaces", newName: "DetectedFace1");
        }
    }
}
