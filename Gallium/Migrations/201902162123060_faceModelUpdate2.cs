namespace Gallium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class faceModelUpdate2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DetectedFaces", "Postponed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DetectedFaces", "Postponed");
        }
    }
}
