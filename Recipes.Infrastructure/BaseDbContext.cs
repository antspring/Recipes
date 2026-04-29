using Microsoft.EntityFrameworkCore;
using Recipes.Domain.Models;
using Recipes.Domain.Models.RecipesRelations;
using Recipes.Domain.Models.UserRelations;
using Recipes.Infrastructure.Models;

namespace Recipes.Infrastructure;

public class BaseDbContext(DbContextOptions<BaseDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
    public DbSet<RecipeStep> RecipeSteps { get; set; }
    public DbSet<RecipeRating> RecipeRatings { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<RecipeImage> RecipeImages { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<UnwantedIngredients> UnwantedIngredients { get; set; }
    public DbSet<Allergens> Allergens { get; set; }
    public DbSet<UserSubscription> UserSubscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => new { u.UserName, u.Email }).IsUnique();
            entity.Property(u => u.UserName).HasColumnType("varchar(50)");
            entity.Property(u => u.Email).HasColumnType("varchar(254)");
            entity.Property(u => u.Name).HasColumnType("varchar(100)");
            entity.Property(u => u.Description).HasColumnType("varchar(1000)");
            entity.Property(u => u.Password).HasColumnType("varchar(100)");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.Property(r => r.Title).HasColumnType("varchar(150)");
            entity.Property(r => r.Description).HasColumnType("varchar(1000)");
        });

        modelBuilder.Entity<RecipeStep>(entity =>
        {
            entity.ToTable("RecipeSteps");
            entity.Property(rs => rs.Description).HasColumnType("varchar(1000)");

            entity.HasOne(rs => rs.Recipe)
                .WithMany(r => r.Steps)
                .HasForeignKey(rs => rs.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(rs => rs.Image)
                .WithMany()
                .HasForeignKey(rs => rs.ImageId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<Comment>(entity => { entity.Property(c => c.Value).HasColumnType("varchar(1000)"); });

        modelBuilder.Entity<Ingredient>(entity => { entity.Property(i => i.Title).HasColumnType("varchar(100)"); });

        modelBuilder.Entity<Like>().HasKey(l => new { l.RecipeId, l.UserId });
        modelBuilder.Entity<Favorite>().HasKey(f => new { f.RecipeId, f.UserId });
        modelBuilder.Entity<RecipeImage>().HasKey(ri => new { ri.RecipeId, ri.ImageId });

        modelBuilder.Entity<RecipeRating>(entity =>
        {
            entity.HasKey(rr => new { rr.RecipeId, rr.UserId });
            entity.ToTable("RecipeRatings");

            entity.HasIndex(rr => rr.UserId);

            entity.HasOne(rr => rr.Recipe)
                .WithMany(r => r.Ratings)
                .HasForeignKey(rr => rr.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(rr => rr.User)
                .WithMany(u => u.RecipeRatings)
                .HasForeignKey(rr => rr.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RecipeIngredient>(entity =>
        {
            entity.HasKey(ri => new { ri.RecipeId, ri.IngredientId });
            entity.ToTable("RecipeIngredients");
            entity.Property(ri => ri.AlternativeWeight).HasColumnType("varchar(50)");

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_RecipeIngredients_Weight_AlternativeWeight",
                "\"Weight\" IS NOT NULL OR \"AlternativeWeight\" IS NOT NULL"
            ));

            entity.HasOne(ri => ri.Recipe)
                .WithMany(r => r.RecipeIngredients)
                .HasForeignKey(ri => ri.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RecipeImage>(entity =>
        {
            entity.ToTable("RecipeImages");

            entity.HasOne(ri => ri.Recipe)
                .WithMany(r => r.RecipeImages)
                .HasForeignKey(ri => ri.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ri => ri.Image)
                .WithMany()
                .HasForeignKey(ri => ri.ImageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RefreshToken>(entity => { entity.HasIndex(e => e.Token).IsUnique(); });

        modelBuilder.Entity<UnwantedIngredients>(entity =>
        {
            entity.HasKey(uui => new { uui.UserId, uui.IngredientId });
            entity.ToTable("UnwantedIngredients");

            entity.HasOne(uui => uui.User)
                .WithMany(u => u.UnwantedIngredients)
                .HasForeignKey(uui => uui.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(uui => uui.Ingredient)
                .WithMany(i => i.UsersUnwantedIngredients)
                .HasForeignKey(uui => uui.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Allergens>(entity =>
        {
            entity.HasKey(al => new { al.UserId, al.IngredientId });
            entity.ToTable("Allergens");

            entity.HasOne(al => al.User)
                .WithMany(u => u.Allergens)
                .HasForeignKey(al => al.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(al => al.Ingredient)
                .WithMany(i => i.UsersAllergens)
                .HasForeignKey(al => al.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserSubscription>(entity =>
        {
            entity.HasKey(us => new { us.SubscriberId, us.SubscribedToId });
            entity.ToTable("UserSubscriptions");

            entity.HasIndex(us => us.SubscribedToId);

            entity.HasOne(us => us.Subscriber)
                .WithMany(u => u.Following)
                .HasForeignKey(us => us.SubscriberId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(us => us.SubscribedTo)
                .WithMany(u => u.Followers)
                .HasForeignKey(us => us.SubscribedToId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasMany(c => c.Images)
                .WithMany(i => i.Comments)
                .UsingEntity<Dictionary<string, object>>(
                    j => j.HasOne<Image>()
                        .WithMany()
                        .HasForeignKey("ImageId")
                        .HasPrincipalKey(i => i.Id)
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Comment>()
                        .WithMany()
                        .HasForeignKey("CommentId")
                        .HasPrincipalKey(c => c.Id)
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j.ToTable("CommentImages")
                );

            entity.HasOne(c => c.Commentator)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.CommentatorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.Recipe)
                .WithMany(r => r.Comments)
                .HasForeignKey(c => c.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
