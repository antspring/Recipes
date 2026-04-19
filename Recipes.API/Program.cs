using Recipes.API.Endpoints;
using Recipes.API.ServiceCollectionExtension;
using Recipes.Application.Mappings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext(builder.Configuration);

builder.Services.AddSwagger();

builder.Services.AddConfigure(builder.Configuration);
builder.Services.AddDependencyInjections();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<UserProfile>();
    cfg.AddProfile<IngredientProfile>();
    cfg.AddProfile<RecipeProfile>();
}, typeof(Program).Assembly);

builder.Services.AddJwtAuthentication();

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 52428800;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithAuth();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapRecipeEndpoints();
app.MapUnwantedIngredientsEndpoints();
app.MapAllergenEndpoints();
app.MapCommentEndpoints();

app.Run();