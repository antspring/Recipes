using Recipes.Application.Options.Interfaces;

namespace Recipes.Infrastructure.Options;

public class SmtpOptions : ISmtpOptions
{
    public string Host { get; init; } = null!;
    public int Port { get; init; }
    public string? UserName { get; init; }
    public string? Password { get; init; }
    public string FromEmail { get; init; } = null!;
    public string FromName { get; init; } = null!;
    public bool UseSsl { get; init; }
}
