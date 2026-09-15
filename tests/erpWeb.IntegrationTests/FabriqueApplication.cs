using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;

namespace erpWeb.IntegrationTests;

/// <summary>
/// Application complète sur une base SQLite temporaire. L'environnement « Test » évite de charger
/// les secrets utilisateur du poste de développement.
/// </summary>
public sealed class FabriqueApplication : WebApplicationFactory<Program>
{
    public const string EmailAdministrateur = "admin-tests@erpweb.local";
    public const string MotDePasseAdministrateur = "Admin#Tests2026";

    private readonly string _repertoire = Path.Combine(Path.GetTempPath(), "erpWeb-tests-" + Guid.NewGuid().ToString("N"));

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Directory.CreateDirectory(_repertoire);

        builder.UseEnvironment("Test");
        builder.UseSetting("ConnectionStrings:ParDefaut", $"Data Source={Path.Combine(_repertoire, "erpWeb-tests.db")}");
        builder.UseSetting("BaseDeDonnees:AppliquerMigrationsAuDemarrage", "true");
        builder.UseSetting("Administrateur:Email", EmailAdministrateur);
        builder.UseSetting("Administrateur:MotDePasse", MotDePasseAdministrateur);
        builder.UseSetting("Stockage:Dossier", Path.Combine(_repertoire, "documents"));
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            SqliteConnection.ClearAllPools();
            try
            {
                Directory.Delete(_repertoire, recursive: true);
            }
            catch (IOException)
            {
                // Fichier encore verrouillé : le dossier temporaire sera nettoyé par le système.
            }
        }
    }
}
