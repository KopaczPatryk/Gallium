namespace Gallium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class faceModelUpdate1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DetectedFaces", "IsValidFace", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DetectedFaces", "IsValidFace");
        }
    }
}
