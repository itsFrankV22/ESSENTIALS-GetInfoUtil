using InfoPlayers;
using Newtonsoft.Json;
using TShockAPI;

public class ConfigManager
{
    // Instancia estática para acceso global
    public static ConfigManager Instance { get; private set; }
    public static void Initialize()
    {
        if (Instance == null)
        {
            Instance = new ConfigManager();
        }
    }

    Version pluginVersion = PlayerJoinInfo.Instance.Version; // Obtener la versión del plugin
    public string Version { get; set; }
    public bool ShowJoinMessageInConsole { get; set; }
    public bool ShowJoinCustomMessage { get; set; }
    public string JoinCustomMessage { get; set; }
    public bool ShowStatsOnPlayerJoin { get; set; }

    // Comandos configurables
    public string GetInfoCommandName { get; set; }
    public string GetInfoCommandShort { get; set; }
    public string GetInfoUserCommandName { get; set; }
    public string GetInfoUserCommandShort { get; set; }
    public string PingCommandName { get; set; }
    public string PingCommandShort { get; set; }

    private static string ConfigPath => Path.Combine(TShock.SavePath, "GetinfoUtilConfig.json");

    public ConfigManager()
    {
        // Valores por defecto
        Version = "1.6.0";
        ShowJoinMessageInConsole = true;
        ShowStatsOnPlayerJoin = true;
        JoinCustomMessage = "[ [{difficulty} {platform} ][ {playerName} ] Joined From: [c/FFB000:{country}] Welcome!";

        // Valores por defecto para los comandos
        GetInfoCommandName = "getinfo";
        GetInfoCommandShort = "gi";
        GetInfoUserCommandName = "getinfouser";
        GetInfoUserCommandShort = "giu";
        PingCommandName = "gping";
        PingCommandShort = "gp";
    }

    public void LoadConfig()
    {
        Version pluginVersion = PlayerJoinInfo.Instance.Version; // Obtener la versión del plugin

        if (File.Exists(ConfigPath))
        {
            try
            {
                string json = File.ReadAllText(ConfigPath);
                var config = JsonConvert.DeserializeObject<ConfigManager>(json);

                // Comparar versiones
                if (config.Version == null || new Version(config.Version) < pluginVersion)
                {
                    Console.WriteLine($"[ GetInfoUtil ] Outdated config version detected. Updating config from version {config.Version ?? "unknown"} to {pluginVersion}.");
                    LoadDefaults(); // Cargar valores predeterminados
                    Version = pluginVersion.ToString(); // Actualizar a la versión del plugin
                    SaveConfig(); // Guardar la nueva configuración
                }
                else
                {
                    // Copiar valores desde el archivo cargado si está actualizado
                    Version = config.Version;
                    ShowJoinMessageInConsole = config.ShowJoinMessageInConsole;
                    ShowJoinCustomMessage = config.ShowJoinCustomMessage;
                    ShowStatsOnPlayerJoin = config.ShowStatsOnPlayerJoin;
                    JoinCustomMessage = config.JoinCustomMessage;

                    // Cargar comandos configurables
                    GetInfoCommandName = config.GetInfoCommandName;
                    GetInfoCommandShort = config.GetInfoCommandShort;
                    GetInfoUserCommandName = config.GetInfoUserCommandName;
                    GetInfoUserCommandShort = config.GetInfoUserCommandShort;
                    PingCommandName = config.PingCommandName;
                    PingCommandShort = config.PingCommandShort;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ GetInfoUtil ] Error loading config: {ex.Message}");
                LoadDefaults();
                Version = pluginVersion.ToString(); // Asegurarse de usar la versión del plugin
                SaveConfig(); // Guardar los valores predeterminados si hay un error
            }
        }
        else
        {
            Console.WriteLine("[ GetInfoUtil ] Config file not found. Generating default config.");
            LoadDefaults();
            Version = pluginVersion.ToString(); // Asegurarse de usar la versión del plugin
            SaveConfig(); // Crear el archivo de configuración con valores predeterminados
        }
    }

    public void SaveConfig()
    {
        try
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath));
            File.WriteAllText(ConfigPath, json);
            Console.WriteLine("[ GetInfoUtil ] Config file saved.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ GetInfoUtil ] Error saving config: {ex.Message}");
        }
    }

    private void LoadDefaults()
    {
        Version = PlayerJoinInfo.Instance.Version.ToString(); // Usar la versión actual del plugin
        ShowJoinMessageInConsole = true;
        ShowJoinCustomMessage = true;
        ShowStatsOnPlayerJoin = true;
        JoinCustomMessage = " [ {difficulty} ][ {platform} ][ {playerName} ] Joined From: [c/FFB000:{country}] Welcome!";

        // Valores por defecto para los comandos
        GetInfoCommandName = "getinfo";
        GetInfoCommandShort = "gi";
        GetInfoUserCommandName = "getinfouser";
        GetInfoUserCommandShort = "giu";
        PingCommandName = "gping";
        PingCommandShort = "gp";
    }
}