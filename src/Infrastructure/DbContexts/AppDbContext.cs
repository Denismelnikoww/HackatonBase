using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DbContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Session> UserSessions { get; set; }
        public DbSet<Token> UserTokens { get; set; }
        public DbSet<ApiKey> ApiKeys { get; set; }
        public DbSet<Terminal> Terminals { get; set; }
        public DbSet<Entry> Entries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("hackaton");

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new SessionConfiguration());
            modelBuilder.ApplyConfiguration(new TokenConfiguration());
            modelBuilder.ApplyConfiguration(new TerminalConfiguration());
            modelBuilder.ApplyConfiguration(new EntryConfiguration());
            modelBuilder.ApplyConfiguration(new ApiKeyConfiguration());

            base.OnModelCreating(modelBuilder);
        }


        public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
        {
            public void Configure(EntityTypeBuilder<ApiKey> builder)
            {
                builder.ToTable("api_keys");

                builder.HasKey(x => x.Id);

                builder.Property(x => x.Id)
                    .HasColumnType("uuid")
                    .ValueGeneratedNever();

                builder.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("varchar(50)");
                
                builder.Property(x => x.Value)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("varchar(50)");
             
                builder.Property(x => x.IsRevoked)
                    .HasDefaultValue(false);
                
                builder.Property(x => x.IsDeleted)
                    .HasDefaultValue(false);
                
                builder.Property(x => x.UserId)
                    .IsRequired()
                    .HasColumnType("uuid");

                builder.HasOne(x => x.User);
            }
        }

        public class UserConfiguration : IEntityTypeConfiguration<User>
        {
            public void Configure(EntityTypeBuilder<User> builder)
            {
                builder.ToTable("users");

                builder.HasKey(x => x.Id);

                builder.Property(x => x.Id)
                    .HasColumnType("uuid")
                    .ValueGeneratedNever();

                builder.Property(x => x.Login)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("varchar(50)");

                builder.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("varchar(50)");

                builder.Property(x => x.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnType("varchar(255)");

                builder.Property(x => x.Email)
                    .HasMaxLength(100)
                    .HasColumnType("varchar(100)");

                builder.Property(x => x.IsEmailConfirmed)
                    .HasDefaultValue(false);

                builder.Property(x => x.IsDeleted)
                    .HasDefaultValue(false);

                builder.Property(x => x.IsBanned)
                    .HasDefaultValue(false);

                builder.Property(x => x.RegistrationDate)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp with time zone");

                builder.Property(x => x.PasswordChangeDate)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP")
                  .HasColumnType("timestamp with time zone");

                builder.HasIndex(x => x.Login)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Login");

                builder.HasIndex(x => x.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Email")
                    .HasFilter("\"Email\" IS NOT NULL");

                builder.HasIndex(x => new { x.IsDeleted, x.IsBanned })
                    .HasDatabaseName("IX_Users_Status");
            }
        }

        public class SessionConfiguration : IEntityTypeConfiguration<Session>
        {
            public void Configure(EntityTypeBuilder<Session> builder)
            {
                builder.ToTable("sessions");

                builder.HasKey(x => x.Id);

                builder.Property(x => x.Id)
                    .HasColumnType("uuid")
                    .ValueGeneratedNever();

                builder.Property(x => x.UserId)
                    .IsRequired()
                    .HasColumnType("uuid");

                builder.Property(x => x.TokenId)
                    .IsRequired()
                    .HasColumnType("uuid");

                builder.Property(x => x.LoginDate)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp with time zone");

                builder.Property(x => x.LastActivity)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp with time zone");

                builder.Property(x => x.LogoutDate)
                    .HasColumnType("timestamp with time zone");

                builder.Property(x => x.IsActive)
                    .HasDefaultValue(true);

                builder.HasOne(x => x.User)
                    .WithMany(u => u.Sessions)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(x => x.Token)
                    .WithOne(t => t.Session)
                    .HasForeignKey<Token>(x => x.SessionId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasIndex(x => x.UserId)
                    .HasDatabaseName("IX_Sessions_UserId");

                builder.HasIndex(x => new { x.LastActivity })
                    .HasDatabaseName("IX_Sessions_LastActivity");
            }
        }

        public class TokenConfiguration : IEntityTypeConfiguration<Token>
        {
            public void Configure(EntityTypeBuilder<Token> builder)
            {
                builder.ToTable("tokens");

                builder.HasKey(x => x.Id);

                builder.Property(x => x.Id)
                    .HasColumnType("uuid")
                    .ValueGeneratedNever();

                builder.Property(x => x.SessionId)
                    .IsRequired()
                    .HasColumnType("uuid");

                builder.Property(x => x.RefreshToken)
                    .IsRequired()
                    .HasMaxLength(500)
                    .HasColumnType("varchar(500)");

                builder.Property(x => x.CreatedAt)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP")
                    .HasColumnType("timestamp with time zone");

                builder.Property(x => x.IsRevoked)
                    .HasDefaultValue(false);

                builder.HasOne(x => x.Session)
                    .WithOne(s => s.Token)
                    .HasForeignKey<Token>(x => x.SessionId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasIndex(x => x.RefreshToken)
                    .IsUnique()
                    .HasDatabaseName("IX_Tokens_RefreshToken");

                builder.HasIndex(x => x.SessionId)
                    .IsUnique()
                    .HasDatabaseName("IX_Tokens_SessionId");
            }
        }

        public class TerminalConfiguration : IEntityTypeConfiguration<Terminal>
        {
            public void Configure(EntityTypeBuilder<Terminal> builder)
            {
                builder.ToTable("terminals");

                builder.HasKey(x => x.Id);

                builder.Property(x => x.Id)
                    .HasColumnType("uuid")
                    .ValueGeneratedNever();

                builder.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnType("varchar(100)");

                builder.Property(x => x.IsDeleted)
                    .HasDefaultValue(false);
            }
        }

        public class EntryConfiguration : IEntityTypeConfiguration<Entry>
        {
            public void Configure(EntityTypeBuilder<Entry> builder)
            {
                builder.ToTable("entries");

                builder.HasKey(x => x.Id);

                builder.Property(x => x.Id)
                    .HasColumnType("uuid")
                    .ValueGeneratedNever();

                builder.Property(x => x.Time)
                    .IsRequired()
                    .HasColumnType("timestamp with time zone");

                builder.Property(x => x.UserId)
                    .IsRequired()
                    .HasColumnType("uuid");

                builder.Property(x => x.TerminalId)
                    .IsRequired()
                    .HasColumnType("uuid");

                builder.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.NoAction);

                builder.HasOne(x => x.Terminal)
                    .WithMany()
                    .HasForeignKey(x => x.TerminalId)
                    .OnDelete(DeleteBehavior.NoAction);

                builder.HasIndex(x => x.UserId)
                    .HasDatabaseName("IX_Entries_UserId");

                builder.HasIndex(x => x.TerminalId)
                    .HasDatabaseName("IX_Entries_TerminalId");
            }
        }
    }
}