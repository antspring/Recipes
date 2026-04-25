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
    public DbSet<Image> Images { get; set; }
    public DbSet<RecipeImage> RecipeImages { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<UnwantedIngredients> UnwantedIngredients { get; set; }
    public DbSet<Allergens> Allergens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => new { u.UserName, u.Email }).IsUnique();

        modelBuilder.Entity<RecipeIngredient>(entity =>
        {
            entity.ToTable("RecipeIngredients");

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

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comments");

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
