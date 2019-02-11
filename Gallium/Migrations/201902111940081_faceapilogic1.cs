namespace Gallium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class faceapilogic1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "RemoteGuid", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "RemoteGuid");
        }
    }
}
