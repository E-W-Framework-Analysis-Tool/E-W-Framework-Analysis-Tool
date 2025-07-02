using FluentValidation;

namespace EwFrameworkAnalysis.Blazor.Models;

public class DataElementProviderConfigurationModel
{
    public bool EnableUserInput { get; set; }
    public bool EnableEdFiDatabase { get; set; }
    public string? EdFiConnectionString { get; set; }
    public bool EnableEdFiApi { get; set; }
    public string? EdFiApiBaseUrl { get; set; }
    public string? EdFiApiClientId { get; set; }
    public string? EdFiApiClientSecret { get; set; }
    public string? EdFiApiTemporaryAccessToken { get; set; }
    public bool EnableCedsDW { get; set; }
    public string? CedsDWConnectionString { get; set; }
}

public class DataElementProviderConfigurationValidator : AbstractValidator<DataElementProviderConfigurationModel>
{
    public DataElementProviderConfigurationValidator()
    {
        RuleFor(x => x.EdFiApiBaseUrl)
            .NotEmpty()
            .When(x => x.EnableEdFiApi)
            .WithMessage("Ed-Fi API Base URL must be a valid URL.");

        RuleFor(x => x.EdFiApiClientId)
            .NotEmpty()
            .When(x => x.EnableEdFiApi && string.IsNullOrWhiteSpace(x.EdFiApiTemporaryAccessToken))
            .WithMessage("Client ID is required if Temporary Access Token is not provided.");

        RuleFor(x => x.EdFiApiClientSecret)
            .NotEmpty()
            .When(x => x.EnableEdFiApi && string.IsNullOrWhiteSpace(x.EdFiApiTemporaryAccessToken))
            .WithMessage("Client Secret is required if Temporary Access Token is not provided.");

        RuleFor(x => x.EdFiApiTemporaryAccessToken)
            .NotEmpty()
            .When(x => x.EnableEdFiApi && string.IsNullOrWhiteSpace(x.EdFiApiClientId) && string.IsNullOrWhiteSpace(x.EdFiApiClientSecret))
            .WithMessage("Temporary Access Token is required if Client ID and Client Secret are not provided.");

        RuleFor(x => x.EdFiConnectionString)
            .NotEmpty()
            .When(x => x.EnableEdFiDatabase)
            .WithMessage("Connection string is required when Ed-Fi Database is enabled.");
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        var result = await ValidateAsync(ValidationContext<DataElementProviderConfigurationModel>.CreateWithOptions((DataElementProviderConfigurationModel)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return Array.Empty<string>();
        return result.Errors.Select(e => e.ErrorMessage);
    };
}
