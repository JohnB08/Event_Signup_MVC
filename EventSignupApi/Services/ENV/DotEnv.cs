
namespace EventSignupApi.Services.ENV
{
    public static class DotEnv
    {
        public static async Task Load(string file)
        {
            if (!File.Exists(file))
            {
                return;
            }
            foreach (var line in await File.ReadAllLinesAsync(file))
            {
                var parts = line.Split("=");
                if (parts.Length != 2) continue;
                Environment.SetEnvironmentVariable(parts[0], parts[1]);
            }
        }
    }
}
