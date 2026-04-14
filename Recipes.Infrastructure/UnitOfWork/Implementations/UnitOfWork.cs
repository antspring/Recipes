using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models.UserRelations;
using Recipes.Infrastructure.Repositories.Implementations;

namespace Recipes.Infrastructure.UnitOfWork.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly BaseDbContext _dbContext;
    private readonly Dictionary<Type, object> _relationRepositories = new();

    public UnitOfWork(
        BaseDbContext dbContext,
        IRefreshTokenRepository refreshTokens,
        IUserRepository users,
        IRecipeRepository recipes,
        IImageRepository images,
        IRecipeImageRepository recipeImages,
        IRecipeIngredientRepository recipeIngredients,
        IIngredientRepository ingredients)
    {
        _dbContext = dbContext;
        RefreshTokens = refreshTokens;
        Users = users;
        Recipes = recipes;
        Images = images;
        RecipeImages = recipeImages;
        RecipeIngredients = recipeIngredients;
        Ingredients = ingredients;
    }

    public IRefreshTokenRepository RefreshTokens { get; }
    public IUserRepository Users { get; }
    public IRecipeRepository Recipes { get; }
    public IImageRepository Images { get; }
    public IRecipeImageRepository RecipeImages { get; }
    public IRecipeIngredientRepository RecipeIngredients { get; }
    public IIngredientRepository Ingredients { get; }

    public IUserIngredientRelationRepository<T> GetUserIngredientRelationRepository<T>()
        where T : class, IUserIngredientRelation
    {
        var type = typeof(T);

        if (!_relationRepositories.TryGetValue(type, out var repository))
        {
            repository = new UserIngredientRelationRepository<T>(_dbContext);
            _relationRepositories.Add(type, repository);
        }

        return (IUserIngredientRelationRepository<T>)repository;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}