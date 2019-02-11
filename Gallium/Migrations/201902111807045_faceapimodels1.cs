namespace Gallium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class faceapimodels1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LargeFaceLists",
                c => new
                    {
                        LargeFaceListId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        UserData = c.String(),
                    })
                .PrimaryKey(t => t.LargeFaceListId);
            
            CreateTable(
                "dbo.LargePersonGroups",
                c => new
                    {
                        LargePersonGroupId = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        UserData = c.String(),
                    })
                .PrimaryKey(t => t.LargePersonGroupId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.LargePersonGroups");
            DropTable("dbo.LargeFaceLists");
        }
    }
}
