using Microsoft.EntityFrameworkCore;
using Recipes.Application.Auth;
using Recipes.Domain.Models;
using Recipes.Domain.Models.RecipesRelations;
using Recipes.Domain.Models.UserRelations;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => new { u.UserName, u.Email }).IsUnique();

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Password)
                .HasConversion(
                    v => BCrypt.Net.BCrypt.HashPassword(v),
                    v => v);
        });

        modelBuilder.Entity<RecipeIngredient>(entity =>
        {
            entity.ToTable("RecipeIngredients");

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_RecipeIngredients_Weight_AlternativeWeight",
                "\"Weight\" IS NOT NULL OR \"AlternativeWeight\" IS NOT NULL"
            ));
        });

        modelBuilder.Entity<RefreshToken>(entity => { entity.HasIndex(e => e.Token).IsUnique(); });
    }
}