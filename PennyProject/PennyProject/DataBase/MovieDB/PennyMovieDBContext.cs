using Microsoft.EntityFrameworkCore;

namespace PennyProject.DataBase.MovieDB;

public partial class PennyMovieDBContext : DbContext
{
    public PennyMovieDBContext()
    {
    }

    public PennyMovieDBContext(DbContextOptions<PennyMovieDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MovieInfo> MovieInfos { get; set; }

    public virtual DbSet<UserFavorite> UserFavorites { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MovieInfo>(entity =>
        {
            entity.HasKey(e => e.MovieId);

            entity.ToTable("MovieInfo");

            entity.Property(e => e.MovieId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreateDateTime).HasColumnType("datetime");
            entity.Property(e => e.Director).HasMaxLength(50);
            entity.Property(e => e.ImgName)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MovieChinessName).HasMaxLength(50);
            entity.Property(e => e.MovieEngName).HasMaxLength(50);
            entity.Property(e => e.ReleaseDateTime).HasColumnType("datetime");
            entity.Property(e => e.UpdateDateTime)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<UserFavorite>(entity =>
        {
            entity.HasKey(e => new { e.MemberId, e.MovieId });

            entity.Property(e => e.CreateDateTime).HasColumnType("datetime");
            entity.Property(e => e.MemberId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MovieId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.MovieName)
                .HasMaxLength(50)
                .IsUnicode(false);
           
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("UserRole");

            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CreateDateTime).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDateTime).HasColumnType("datetime");
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
