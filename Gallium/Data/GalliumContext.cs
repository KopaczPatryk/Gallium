namespace Gallium.Data
{
    using Gallium.Models;
    using Gallium.Models.FaceApi;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class GalliumContext : DbContext
    {
        // Your context has been configured to use a 'GalliumContext' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'Gallium.Data.GalliumContext' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'GalliumContext' 
        // connection string in the application configuration file.
        public GalliumContext()
            : base("name=GalliumContext")
        {
        }

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<PhotoDirectories> Directories { get; set; }
        public virtual DbSet<Photo> Photos { get; set; }
        public virtual DbSet<DetectedFace> DetectedFaces { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<PhotoMiniature> MiniatureLocations { get; set; }
    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}