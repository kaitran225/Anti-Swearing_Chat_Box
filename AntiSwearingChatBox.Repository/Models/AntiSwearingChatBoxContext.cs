﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

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

    public virtual DbSet<ChatThread> ChatThreads { get; set; }

    public virtual DbSet<FilteredWord> FilteredWords { get; set; }

    public virtual DbSet<MessageHistory> MessageHistories { get; set; }

    public virtual DbSet<ThreadParticipant> ThreadParticipants { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserWarning> UserWarnings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Get the executing assembly's directory
            string? currentDirectory = Path.GetDirectoryName(typeof(AntiSwearingChatBoxContext).Assembly.Location);
            
            // Look for the Service project directory (either as a direct parent or as a sibling)
            string serviceDirectory = Path.Combine(currentDirectory ?? "", "..", "AntiSwearingChatBox.Service");
            
            // If not found as direct parent, try looking for it as sibling in the parent directory
            if (!Directory.Exists(serviceDirectory))
            {
                serviceDirectory = Path.Combine(currentDirectory ?? "", "..", "..", "AntiSwearingChatBox.Service");
            }
            
            // If still not found, try to get the solution root
            if (!Directory.Exists(serviceDirectory))
            {
                // Try to find the solution root (looking for parent directories)
                string? solutionRoot = currentDirectory;
                while (solutionRoot != null && !File.Exists(Path.Combine(solutionRoot, "AntiSwearingChatBox.sln")))
                {
                    solutionRoot = Path.GetDirectoryName(solutionRoot);
                }
                
                if (solutionRoot != null)
                {
                    serviceDirectory = Path.Combine(solutionRoot, "AntiSwearingChatBox.Service");
                }
            }
            
            // Fallback to current directory if all attempts fail
            if (!Directory.Exists(serviceDirectory))
            {
                serviceDirectory = currentDirectory ?? "";
            }
            
            // Create configuration
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(serviceDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
                
            var strConn = config["ConnectionStrings:AntiSwearingChatBox"];
            optionsBuilder.UseSqlServer(strConn);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatThread>(entity =>
        {
            entity.HasKey(e => e.ThreadId).HasName("PK__ChatThre__688356845E42936A");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.LastMessageAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ModerationEnabled).HasDefaultValue(true);
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<FilteredWord>(entity =>
        {
            entity.HasKey(e => e.WordId).HasName("PK__Filtered__2C20F06639322A22");

            entity.HasIndex(e => e.Word, "UQ__Filtered__95B501084AE8F06F").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SeverityLevel).HasDefaultValue(1);
            entity.Property(e => e.Word).HasMaxLength(100);
        });

        modelBuilder.Entity<MessageHistory>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__MessageH__C87C0C9CDC014970");

            entity.ToTable("MessageHistory");

            entity.HasIndex(e => e.ThreadId, "IX_MessageHistory_ThreadId");

            entity.HasIndex(e => e.UserId, "IX_MessageHistory_UserId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Thread).WithMany(p => p.MessageHistories)
                .HasForeignKey(d => d.ThreadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MessageHistory_ChatThreads");

            entity.HasOne(d => d.User).WithMany(p => p.MessageHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MessageHistory_Users");
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
                .HasConstraintName("FK_ThreadParticipants_ChatThreads");

            entity.HasOne(d => d.User).WithMany(p => p.ThreadParticipants)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ThreadParticipants_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C037765CB");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4D8DA359D").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105343506988F").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(256);
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
            entity.HasKey(e => e.WarningId).HasName("PK__UserWarn__21457158A1C6A6EF");

            entity.HasIndex(e => e.ThreadId, "IX_UserWarnings_ThreadId");

            entity.HasIndex(e => e.UserId, "IX_UserWarnings_UserId");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.WarningMessage).HasMaxLength(500);

            entity.HasOne(d => d.Thread).WithMany(p => p.UserWarnings)
                .HasForeignKey(d => d.ThreadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserWarnings_ChatThreads");

            entity.HasOne(d => d.User).WithMany(p => p.UserWarnings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserWarnings_Users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
