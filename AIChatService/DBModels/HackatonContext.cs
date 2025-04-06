using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AIChatService.DBModels;

public partial class HackatonContext : DbContext
{
    public HackatonContext()
    {
    }

    public HackatonContext(DbContextOptions<HackatonContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<Startup> Startups { get; set; }

    public virtual DbSet<StartupInvestor> StartupInvestors { get; set; }

    public virtual DbSet<StartupScientist> StartupScientists { get; set; }

    public virtual DbSet<StartupWorker> StartupWorkers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserSession> UserSessions { get; set; }

    public virtual DbSet<UsersSkill> UsersSkills { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("host=localhost;port=5432;username=postgres;password=new_password;database=hackaton");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("chats_pkey");

            entity.ToTable("chats");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.StartupId).HasColumnName("startup_id");

            entity.HasOne(d => d.Startup).WithMany(p => p.Chats)
                .HasForeignKey(d => d.StartupId)
                .HasConstraintName("chats_startup_id_fkey");

            entity.HasMany(d => d.Users).WithMany(p => p.Chats)
                .UsingEntity<Dictionary<string, object>>(
                    "ChatUser",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("chat_users_user_id_fkey"),
                    l => l.HasOne<Chat>().WithMany()
                        .HasForeignKey("ChatId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("chat_users_chat_id_fkey"),
                    j =>
                    {
                        j.HasKey("ChatId", "UserId").HasName("chat_users_pkey");
                        j.ToTable("chat_users");
                        j.IndexerProperty<int>("ChatId").HasColumnName("chat_id");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                    });
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("messages_pkey");

            entity.ToTable("messages");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AttachmentType)
                .HasMaxLength(50)
                .HasColumnName("attachment_type");
            entity.Property(e => e.AttachmentUrl).HasColumnName("attachment_url");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("sent_at");
            entity.Property(e => e.StartupId).HasColumnName("startup_id");
            entity.Property(e => e.Text).HasColumnName("text");

            entity.HasOne(d => d.Sender).WithMany(p => p.Messages)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("messages_sender_id_fkey");

            entity.HasOne(d => d.Startup).WithMany(p => p.Messages)
                .HasForeignKey(d => d.StartupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("messages_startup_id_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.Name, "roles_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("skills_pkey");

            entity.ToTable("skills");

            entity.HasIndex(e => e.Name, "skills_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Startup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("startups_pkey");

            entity.ToTable("startups");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");

            entity.HasOne(d => d.Owner).WithMany(p => p.Startups)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("startups_owner_id_fkey");
        });

        modelBuilder.Entity<StartupInvestor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("startup_investors_pkey");

            entity.ToTable("startup_investors");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.InvestorId).HasColumnName("investor_id");
            entity.Property(e => e.StartupId).HasColumnName("startup_id");

            entity.HasOne(d => d.Investor).WithMany(p => p.StartupInvestors)
                .HasForeignKey(d => d.InvestorId)
                .HasConstraintName("startup_investors_investor_id_fkey");

            entity.HasOne(d => d.Startup).WithMany(p => p.StartupInvestors)
                .HasForeignKey(d => d.StartupId)
                .HasConstraintName("startup_investors_startup_id_fkey");
        });

        modelBuilder.Entity<StartupScientist>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("startup_scientists_pkey");

            entity.ToTable("startup_scientists");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.StartupId).HasColumnName("startup_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Startup).WithMany(p => p.StartupScientists)
                .HasForeignKey(d => d.StartupId)
                .HasConstraintName("startup_scientists_startup_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.StartupScientists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("startup_scientists_user_id_fkey");
        });

        modelBuilder.Entity<StartupWorker>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("startup_workers_pkey");

            entity.ToTable("startup_workers");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.StartupId).HasColumnName("startup_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Startup).WithMany(p => p.StartupWorkers)
                .HasForeignKey(d => d.StartupId)
                .HasConstraintName("startup_workers_startup_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.StartupWorkers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("startup_workers_user_id_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FcmToken).HasColumnName("fcm_token");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("last_name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.ThirdName)
                .HasMaxLength(50)
                .HasColumnName("third_name");

            entity.HasOne(d => d.RoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.Role)
                .HasConstraintName("users_role_fkey");
        });

        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_sessions_pkey");

            entity.ToTable("user_sessions");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.LastAccessedAt)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_accessed_at");
            entity.Property(e => e.LastAccessedIp)
                .HasMaxLength(50)
                .HasColumnName("last_accessed_ip");
            entity.Property(e => e.LastAccessedUserAgent)
                .HasMaxLength(255)
                .HasColumnName("last_accessed_user_agent");
            entity.Property(e => e.Token)
                .HasMaxLength(88)
                .HasColumnName("token");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserSessions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("user_sessions_user_id_fkey");
        });

        modelBuilder.Entity<UsersSkill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_skills_pkey");

            entity.ToTable("users_skills");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.SkillId).HasColumnName("skill_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Skill).WithMany(p => p.UsersSkills)
                .HasForeignKey(d => d.SkillId)
                .HasConstraintName("users_skills_skill_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.UsersSkills)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("users_skills_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
