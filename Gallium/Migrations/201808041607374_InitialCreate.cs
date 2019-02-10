namespace Gallium.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DetectedFace1",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FaceId = c.Guid(nullable: false),
                        FaceRectangle_Width = c.Int(nullable: false),
                        FaceRectangle_Height = c.Int(nullable: false),
                        FaceRectangle_Left = c.Int(nullable: false),
                        FaceRectangle_Top = c.Int(nullable: false),
                        PhotoId = c.Int(nullable: false),
                        FaceOwner_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.FaceOwner_Id)
                .ForeignKey("dbo.Photos", t => t.PhotoId, cascadeDelete: true)
                .Index(t => t.PhotoId)
                .Index(t => t.FaceOwner_Id);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateOfBirth = c.DateTime(),
                        Name = c.String(),
                        LastName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Photos",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        Name = c.String(),
                        Miniature_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Miniatures", t => t.Miniature_Id)
                .Index(t => t.Miniature_Id);
            
            CreateTable(
                "dbo.Miniatures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OriginalImageFullPath = c.String(),
                        OriginalImageFileName = c.String(),
                        MiniatureFullPath = c.String(),
                        MiniatureFileName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserPhotoLocations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Path = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Photos", "Miniature_Id", "dbo.Miniatures");
            DropForeignKey("dbo.DetectedFace1", "PhotoId", "dbo.Photos");
            DropForeignKey("dbo.DetectedFace1", "FaceOwner_Id", "dbo.People");
            DropIndex("dbo.Photos", new[] { "Miniature_Id" });
            DropIndex("dbo.DetectedFace1", new[] { "FaceOwner_Id" });
            DropIndex("dbo.DetectedFace1", new[] { "PhotoId" });
            DropTable("dbo.UserPhotoLocations");
            DropTable("dbo.Miniatures");
            DropTable("dbo.Photos");
            DropTable("dbo.People");
            DropTable("dbo.DetectedFace1");
        }
    }
}
