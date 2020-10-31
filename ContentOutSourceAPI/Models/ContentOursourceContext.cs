using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ContentOutSourceAPI.Models
{
    public partial class ContentOursourceContext : DbContext
    {

        public ContentOursourceContext(DbContextOptions<ContentOursourceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PostHistory> PostHistory { get; set; }
        public virtual DbSet<TblKeywords> TblKeywords { get; set; }
        public virtual DbSet<TblPostStatus> TblPostStatus { get; set; }
        public virtual DbSet<TblPosts> TblPosts { get; set; }
        public virtual DbSet<TblPostsHavingKeywords> TblPostsHavingKeywords { get; set; }
        public virtual DbSet<TblRoles> TblRoles { get; set; }
        public virtual DbSet<TblUsers> TblUsers { get; set; }
        public virtual DbSet<TblUsersHavingPosts> TblUsersHavingPosts { get; set; }
        public virtual DbSet<TransactionHistory> TransactionHistory { get; set; }
        public virtual DbSet<UsersHavingKeywords> UsersHavingKeywords { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PostHistory>(entity =>
            {
                entity.Property(e => e.HistoryDate).HasColumnType("date");

                entity.Property(e => e.Username).HasMaxLength(100);

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.PostHistory)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("FK__PostHisto__PostI__6383C8BA");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.PostHistory)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK__PostHisto__Statu__6477ECF3");

                entity.HasOne(d => d.UsernameNavigation)
                    .WithMany(p => p.PostHistory)
                    .HasForeignKey(d => d.Username)
                    .HasConstraintName("FK__PostHisto__Usern__628FA481");
            });

            modelBuilder.Entity<TblKeywords>(entity =>
            {
                entity.ToTable("tblKeywords");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(100);
            });

            modelBuilder.Entity<TblPostStatus>(entity =>
            {
                entity.ToTable("tblPostStatus");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<TblPosts>(entity =>
            {
                entity.ToTable("tblPosts");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<TblPostsHavingKeywords>(entity =>
            {
                entity.ToTable("tblPostsHavingKeywords");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Keyword)
                    .WithMany(p => p.TblPostsHavingKeywords)
                    .HasForeignKey(d => d.KeywordId)
                    .HasConstraintName("FK__tblPostsH__Keywo__1BFD2C07");

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.TblPostsHavingKeywords)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("FK__tblPostsH__PostI__1B0907CE");
            });

            modelBuilder.Entity<TblRoles>(entity =>
            {
                entity.ToTable("tblRoles");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<TblUsers>(entity =>
            {
                entity.HasKey(e => e.Username)
                    .HasName("PK__tblUsers__536C85E5C270D5FC");

                entity.ToTable("tblUsers");

                entity.Property(e => e.Username).HasMaxLength(100);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.TblUsers)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__tblUsers__Role__30F848ED");
            });

            modelBuilder.Entity<TblUsersHavingPosts>(entity =>
            {
                entity.ToTable("tblUsersHavingPosts");

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.Property(e => e.Username).HasMaxLength(100);

                entity.HasOne(d => d.Post)
                    .WithMany(p => p.TblUsersHavingPosts)
                    .HasForeignKey(d => d.PostId)
                    .HasConstraintName("FK__tblUsersH__PostI__34C8D9D1");

                entity.HasOne(d => d.UsernameNavigation)
                    .WithMany(p => p.TblUsersHavingPosts)
                    .HasForeignKey(d => d.Username)
                    .HasConstraintName("FK__tblUsersH__Usern__33D4B598");
            });

            modelBuilder.Entity<TransactionHistory>(entity =>
            {
                entity.Property(e => e.Giver).HasMaxLength(100);

                entity.Property(e => e.Receiver).HasMaxLength(100);

                entity.Property(e => e.TransactionDate).HasColumnType("datetime");

                entity.HasOne(d => d.GiverNavigation)
                    .WithMany(p => p.TransactionHistoryGiverNavigation)
                    .HasForeignKey(d => d.Giver)
                    .HasConstraintName("FK__Transacti__Giver__6754599E");

                entity.HasOne(d => d.ReceiverNavigation)
                    .WithMany(p => p.TransactionHistoryReceiverNavigation)
                    .HasForeignKey(d => d.Receiver)
                    .HasConstraintName("FK__Transacti__Recei__68487DD7");
            });

            modelBuilder.Entity<UsersHavingKeywords>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Username).HasMaxLength(100);

                entity.HasOne(d => d.Keyword)
                    .WithMany()
                    .HasForeignKey(d => d.KeywordId)
                    .HasConstraintName("FK__UsersHavi__Keywo__6FE99F9F");

                entity.HasOne(d => d.UsernameNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.Username)
                    .HasConstraintName("FK__UsersHavi__Usern__6EF57B66");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
