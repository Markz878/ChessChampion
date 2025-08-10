namespace ChessChampion.Installers;

public sealed class HealthChecksInstaller : IInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks();
    }
}
