using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using DormMVC.Model;

namespace DormInfrastucture.Model;

public partial class DBDormContext : DbContext
{
    public DBDormContext()
    {
    }

    public DBDormContext(DbContextOptions<DBDormContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Accommodation> Accommodations { get; set; }
    public virtual DbSet<Department> Departments { get; set; }
    public virtual DbSet<Dorm> Dorms { get; set; }
    public virtual DbSet<Faculty> Faculties { get; set; }
    public virtual DbSet<Room> Rooms { get; set; }
    public virtual DbSet<Student> Students { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=dorm_db;Username=yosha;Password=yosha;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Accommodation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("accommodation_pkey");

            entity.ToTable("accommodation");

            entity.HasIndex(e => e.KiId, "idx_accommodation_ki_id");
            entity.HasIndex(e => e.StId, "idx_accommodation_st_id");

            entity.Property(e => e.Id).HasColumnName("pr_id");
            entity.Property(e => e.KiId).HasColumnName("ki_id");
            entity.Property(e => e.StId).HasColumnName("st_id");
            entity.Property(e => e.PrDataVysel).HasColumnName("pr_data_vysel");
            entity.Property(e => e.PrDataZasel).HasColumnName("pr_data_zasel");

            entity.HasOne(d => d.Ki).WithMany(p => p.Accommodations)
                .HasForeignKey(d => d.KiId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("accommodation_ki_id_fkey");

            entity.HasOne(d => d.St).WithMany(p => p.Accommodations)
                .HasForeignKey(d => d.StId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("accommodation_st_id_fkey");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("department_pkey");

            entity.ToTable("department");

            entity.HasIndex(e => e.FaId, "idx_department_fa_id");

            entity.Property(e => e.Id).HasColumnName("ka_id");
            entity.Property(e => e.FaId).HasColumnName("fa_id");
            entity.Property(e => e.KaInformation).HasColumnName("ka_information");
            entity.Property(e => e.KaName)
                .HasMaxLength(200)
                .HasColumnName("ka_name");
            entity.Property(e => e.KaTelefon)
                .HasMaxLength(13)
                .HasColumnName("ka_telefon");
            entity.Property(e => e.KaZaviduvach)
                .HasMaxLength(100)
                .HasColumnName("ka_zaviduvach");

            entity.HasOne(d => d.Fa).WithMany(p => p.Departments)
                .HasForeignKey(d => d.FaId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("department_fa_id_fkey");
        });

        modelBuilder.Entity<Dorm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("dorm_pkey");

            entity.ToTable("dorm");

            entity.Property(e => e.Id).HasColumnName("gu_id");
            entity.Property(e => e.GuAdresa)
                .HasMaxLength(200)
                .HasColumnName("gu_adresa");
            entity.Property(e => e.GuInformation).HasColumnName("gu_information");
            entity.Property(e => e.GuKilkistPoverh).HasColumnName("gu_kilkist_poverh");
            entity.Property(e => e.GuKomendant)
                .HasMaxLength(100)
                .HasColumnName("gu_komendant");
            entity.Property(e => e.GuNomer)
                .HasMaxLength(10)
                .HasColumnName("gu_nomer");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("faculty_pkey");

            entity.ToTable("faculty");

            entity.Property(e => e.Id).HasColumnName("fa_id");
            entity.Property(e => e.FaDekan)
                .HasMaxLength(100)
                .HasColumnName("fa_dekan");
            entity.Property(e => e.FaInformation).HasColumnName("fa_information");
            entity.Property(e => e.FaKorpus)
                .HasMaxLength(50)
                .HasColumnName("fa_korpus");
            entity.Property(e => e.FaName)
                .HasMaxLength(200)
                .HasColumnName("fa_name");
            entity.Property(e => e.FaTelefon)
                .HasMaxLength(13)
                .HasColumnName("fa_telefon");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("room_pkey");

            entity.ToTable("room");

            entity.HasIndex(e => e.GuId, "idx_room_gu_id");

            entity.Property(e => e.Id).HasColumnName("ki_id");
            entity.Property(e => e.GuId).HasColumnName("gu_id");
            entity.Property(e => e.KiInformation).HasColumnName("ki_information");
            entity.Property(e => e.KiMistkist).HasColumnName("ki_mistkist");
            entity.Property(e => e.KiNomer)
                .HasMaxLength(10)
                .HasColumnName("ki_nomer");
            entity.Property(e => e.KiPoverh).HasColumnName("ki_poverh");

            entity.HasOne(d => d.Gu).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.GuId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("room_gu_id_fkey");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("student_pkey");

            entity.ToTable("student");

            entity.HasIndex(e => e.FaId, "idx_student_fa_id");
            entity.HasIndex(e => e.KaId, "idx_student_ka_id");

            entity.Property(e => e.Id).HasColumnName("st_id");
            entity.Property(e => e.FaId).HasColumnName("fa_id");
            entity.Property(e => e.KaId).HasColumnName("ka_id");
            entity.Property(e => e.StDataNarodz).HasColumnName("st_data_narodz");
            entity.Property(e => e.StKurs).HasColumnName("st_kurs");
            entity.Property(e => e.StPib)
                .HasMaxLength(200)
                .HasColumnName("st_pib");
            entity.Property(e => e.StTelefon)
                .HasMaxLength(13)
                .HasColumnName("st_telefon");

            entity.HasOne(d => d.Fa).WithMany(p => p.Students)
                .HasForeignKey(d => d.FaId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("student_fa_id_fkey");

            entity.HasOne(d => d.Ka).WithMany(p => p.Students)
                .HasForeignKey(d => d.KaId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("student_ka_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}