# erpWeb.Infrastructure — implémentations techniques

Seul projet autorisé à utiliser EF Core SQLite, MailKit, ClosedXML et le système de fichiers.

## Données

- `Donnees/AppDbContext` implémente `IAppDbContext` ; exposer chaque nouveau `DbSet` via `Set<T>()`.
- Une classe `IEntityTypeConfiguration<T>` par entité dans `Donnees/Configurations/` : nom de table en français, longueurs maximales explicites, index utiles, `ConfigurerChampsAudit()` pour les entités `EntiteAuditable`.
- `IntercepteurAudit` trace toutes les entités : ajouter toute nouvelle propriété sensible (secret, hash) à `_proprietesExclues`.
- `InitialisateurDonnees` est idempotent : il synchronise rôles et permissions à chaque démarrage.

## Migrations

```bash
dotnet ef migrations add <NomEnFrancais> -p src/erpWeb.Infrastructure -s src/erpWeb.Web -o Donnees/Migrations
```

- Une migration par Pull Request, nom en PascalCase français (`AjoutTableClients`).
- Relire `Up`/`Down` : signaler toute perte de données (`DropColumn`, `DropTable`, changement de type).
- SQLite reconstruit la table pour la plupart des `ALTER` : vérifier les migrations sur une copie de base réaliste.
- Ne jamais modifier une migration présente sur `main` (hook `garde-migrations`) ; en cas de conflit de snapshot, utiliser l'agent `expert-migrations`.

## Services

- Enregistrer les implémentations dans `DependencyInjection.AddInfrastructure` (Scoped par défaut, Singleton seulement si sans état).
- Options typées : `services.Configure<T>(configuration.GetSection(T.Section))`.
