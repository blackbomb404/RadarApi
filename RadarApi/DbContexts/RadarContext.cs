using Microsoft.EntityFrameworkCore;
using RadarApi.Models;
using RadarApi.Models.DTO;

namespace RadarApi.DbContexts
{
    public class RadarContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public RadarContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var connectionString = Configuration.GetConnectionString("WebApiDb");
            options.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region News Models Config

            modelBuilder.Entity<News>()
                .HasCheckConstraint("CK_Type", "\"Type\" IN ('Music', 'Entertainment', 'Lifestyle', 'Mark', 'Opinion')")
                .Property(e => e.Type)
                .HasConversion<string>();

            modelBuilder.Entity<News>()
                .Property(e => e.Id)
                .UseIdentityAlwaysColumn();

            modelBuilder.Entity<News>()
                .Property(e => e.PostedAt)
                .HasColumnType("TIMESTAMP")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<News>()
                .Property(e => e.Views)
                .HasDefaultValue(0);
            modelBuilder.Entity<News>()
                .Property(e => e.Comments)
                .HasDefaultValue(0);
            #endregion

            #region SideNews Models Config

            modelBuilder.Entity<SideNews>()
                .Property(e => e.Id)
                .UseIdentityAlwaysColumn();

            modelBuilder.Entity<SideNews>()
                .Property(e => e.PostedAt)
                .HasColumnType("TIMESTAMP")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            modelBuilder.Entity<SideNews>()
                .Property(e => e.AuthorName)
                .HasMaxLength(30);

            #endregion
        }

        public DbSet<News> News { get; set; }
        public DbSet<SideNews> SideNews { get; set; }
    }
}
