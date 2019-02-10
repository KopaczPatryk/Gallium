namespace Gallium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class faceVerification : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DetectedFaces", "HumanVerified", c => c.Boolean(nullable: false));
            AddColumn("dbo.DetectedFaces", "AssignedByHuman", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DetectedFaces", "AssignedByHuman");
            DropColumn("dbo.DetectedFaces", "HumanVerified");
        }
    }
}
