namespace LinkTracker.Scrapper.Migrator.Utils;

public static class MigrationPathResolver
{
    public static string Resolve(string basePath, string serviceName)
    {
        var dir = new DirectoryInfo(basePath);

        while (dir != null)
        {
            var migrationsDir = Path.Combine(dir.FullName, "migrations", serviceName);
            if (Directory.Exists(migrationsDir))
            {
                return migrationsDir;
            }

            dir = dir.Parent;
        }

        throw new InvalidOperationException($"Migrations directory 'migrations/{serviceName}' was not found.");
    }
}
