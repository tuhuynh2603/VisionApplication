using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics.Eventing.Reader;
using System.Security.Principal;
using System.Windows.Markup;

namespace VisionApplication.Model

{
    public class DatabaseContext : DbContext
    {

        public static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddFilter(DbLoggerCategory.Query.Name, LogLevel.Information);
            builder.AddConsole();

        });




        private const string _connectionString = "server=localhost;user=root;database=db1;port=3306;password=gacon05637";

        public static void CreateDatabase(DatabaseContext dbcontext)
        {
            //string dbname = dbcontext.Database.GetDbConnection().Database;
            //var kq = dbcontext.Database.EnsureCreated();
            //if (kq)
            //    Console.WriteLine($"Tao Database {dbname} thanh cong");
            //else
            //    Console.WriteLine($"Tao Database {dbname} fail");

        }

        public static void DropDatabase(DatabaseContext dbcontext)
        {
            string dbname = dbcontext.Database.GetDbConnection().Database;
            var kq = dbcontext.Database.EnsureDeleted();
            if (kq)
                Console.WriteLine($"delete Database {dbname} thanh cong");
            else
                Console.WriteLine($"delete Database {dbname} fail");
        }


        public static List<CategoryVisionParameter> GetAllTrackVisionParam(DatabaseContext dbcontext)
        {
            var teachParam = (from p in dbcontext.categoryTeachParametersModel where p.cameraID == 1 select p).FirstOrDefault();
            if (teachParam == null)
                return new List<CategoryVisionParameter>();

            var e = dbcontext.Entry(teachParam);
            e.Collection(c=>c.listVisionParam).Load();
            if (teachParam.listVisionParam != null)
            {
                teachParam.listVisionParam.ForEach(user => user.Printinfor());

                return teachParam.listVisionParam;
            };

            return new List<CategoryVisionParameter>();
        }

        //public static RectanglesModel GetRectangleModel(DatabaseContext dbcontext)
        //{
        //    var rec = (from p in dbcontext.categoryTeachParametersModel where p.Id == 2 select p).FirstOrDefault();
        //    if (rec == null)
        //        return new RectanglesModel();

        //    var e = dbcontext.Entry(rec);
        //    e.Reference(c => c.).Load();
        //    if (teachParam.listVisionParam != null)
        //    {
        //        teachParam.listVisionParam.ForEach(user => user.Printinfor());

        //        return teachParam.listVisionParam;
        //    };

        //    return new RectanglesModel();
        //}


        public DbSet<RectanglesModel> rectanglesModel { get; set; }
        public DbSet<CategoryTeachParameter> categoryTeachParametersModel { get; set; }
        public DbSet<CategoryVisionParameter> categoryVisionParametersModel { get; set; }

        public DbSet<CameraParameter> cameraParameterModel { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionssBuilder)
        {
            base.OnConfiguring(optionssBuilder);
            optionssBuilder.UseLoggerFactory(loggerFactory);
            optionssBuilder.UseMySQL(_connectionString);
            //optionssBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CategoryVisionParameter>(
                entity =>
                {
                    entity.HasKey(e => new { e.cameraID, e.areaID});
                }
                );

            modelBuilder.Entity<CategoryTeachParameter>(
            entity =>
            {
                entity.HasKey(e => new { e.cameraID });
            }
            );

            modelBuilder.Entity<CameraParameter>(
            entity =>
            {
                entity.HasKey(e => new { e.cameraID });
            }
            );

        }

    }
}
