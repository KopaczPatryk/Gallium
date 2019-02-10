namespace Gallium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Pupa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DetectedFaces", "FaceFile", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DetectedFaces", "FaceFile");
        }
    }
}
