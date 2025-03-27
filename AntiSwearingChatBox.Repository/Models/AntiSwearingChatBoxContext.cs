﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AntiSwearingChatBox.Repository.Models;

public partial class AntiSwearingChatBoxContext : DbContext
{
    public AntiSwearingChatBoxContext()
    {
    }

    public AntiSwearingChatBoxContext(DbContextOptions<AntiSwearingChatBoxContext> options)
        : base(options)
    {
    }

    public virtual DbSet<FilteredWord> FilteredWords { get; set; }

    public virtual DbSet<MessageHistory> MessageHistories { get; set; }

    public virtual DbSet<Thread> Threads { get; set; }

    public virtual DbSet<ThreadParticipant> ThreadParticipants { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserWarning> UserWarnings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=KAINOTE\\SQLEXPRESS;Database=AntiSwearingChatBox;User Id=sa;Password=123456;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FilteredWord>(entity =>
        {
            entity.HasKey(e => e.WordId).HasName("PK__Filtered__2C20F0663F2E9EA9");

            entity.HasIndex(e => e.Word, "UQ__Filtered__95B50108668C1993").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SeverityLevel).HasDefaultValue(1);
            entity.Property(e => e.Word).HasMaxLength(100);
        });

        modelBuilder.Entity<MessageHistory>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__MessageH__C87C0C9C336D7AD2");

            entity.ToTable("MessageHistory");

            entity.HasIndex(e => e.ThreadId, "IX_MessageHistory_ThreadId");

            entity.HasIndex(e => e.UserId, "IX_MessageHistory_UserId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Thread).WithMany(p => p.MessageHistories)
                .HasForeignKey(d => d.ThreadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MessageHistory_Threads");

            entity.HasOne(d => d.User).WithMany(p => p.MessageHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MessageHistory_Users");
        });

        modelBuilder.Entity<Thread>(entity =>
        {
            entity.HasKey(e => e.ThreadId).HasName("PK__Threads__68835684F7B229EB");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastMessageAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<ThreadParticipant>(entity =>
        {
            entity.HasKey(e => new { e.ThreadId, e.UserId });

            entity.HasIndex(e => e.ThreadId, "IX_ThreadParticipants_ThreadId");

            entity.HasIndex(e => e.UserId, "IX_ThreadParticipants_UserId");

            entity.Property(e => e.JoinedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Thread).WithMany(p => p.ThreadParticipants)
                .HasForeignKey(d => d.ThreadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ThreadParticipants_Threads");

            entity.HasOne(d => d.User).WithMany(p => p.ThreadParticipants)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ThreadParticipants_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C2E69730F");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E455A64883").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105346DE313B2").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastPasswordChange).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
            entity.Property(e => e.RefreshToken).HasMaxLength(500);
            entity.Property(e => e.ResetToken).HasMaxLength(100);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasDefaultValue("User");
            entity.Property(e => e.TrustScore)
                .HasDefaultValue(1.00m)
                .HasColumnType("decimal(3, 2)");
            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.VerificationToken).HasMaxLength(100);
        });

        modelBuilder.Entity<UserWarning>(entity =>
        {
            entity.HasKey(e => e.WarningId).HasName("PK__UserWarn__21457158B6C801D1");

            entity.HasIndex(e => e.ThreadId, "IX_UserWarnings_ThreadId");

            entity.HasIndex(e => e.UserId, "IX_UserWarnings_UserId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.WarningMessage).HasMaxLength(500);

            entity.HasOne(d => d.Thread).WithMany(p => p.UserWarnings)
                .HasForeignKey(d => d.ThreadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserWarnings_Threads");

            entity.HasOne(d => d.User).WithMany(p => p.UserWarnings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserWarnings_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
