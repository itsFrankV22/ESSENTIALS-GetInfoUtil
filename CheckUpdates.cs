using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using TerrariaApi.Server;
using TShockAPI;
using InfoPlayers;

namespace CheckUpdates
{
    public class CheckUpdates
    {
        public static async Task<Version?> RequestLatestVersion()
        {
            string url = "https://api.github.com/repos/itsFrankV22/PlayerGetInfo/releases/latest";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.TryParseAdd("request"); // Set a user agent header

                try
                {
                    var response = await client.GetStringAsync(url);
                    dynamic? latestRelease = JsonConvert.DeserializeObject<dynamic>(response);

                    if (latestRelease == null) return null;

                    string tag = latestRelease.name;

                    tag = tag.Trim('v');
                    string[] nums = tag.Split('.');

                    Version version = new Version(int.Parse(nums[0]),
                                                  int.Parse(nums[1]),
                                                  int.Parse(nums[2])
                                                  );
                    return version;
                }
                catch
                {
                    TShock.Log.ConsoleInfo($"╔════════════════════════════════════════════════╗");
                    TShock.Log.ConsoleInfo($"║  An error occurred while checking for updates. ║");
                    TShock.Log.ConsoleInfo($"╚════════════════════════════════════════════════╝");
                }
            }

            return null;
        }

        public static async Task<bool> IsUpToDate(TerrariaPlugin plugin)
        {
            Version? latestVersion = await RequestLatestVersion();
            Version curVersion = plugin.Version;

            return latestVersion != null && curVersion >= latestVersion;
        }

        public static async Task CheckUpdateVerbose(TerrariaPlugin? plugin)
        {
            var VersionInstalled = await RequestLatestVersion();
            var AvilableVersion = plugin.Version;

            if (plugin == null) return;

            TShock.Log.ConsoleInfo($"╔════════════════════════════════════════════════╗");
            TShock.Log.ConsoleInfo($"║ █████  ████  █████    █████ ███  ██ ████ █████ ║");
            TShock.Log.ConsoleInfo($"║ █      █       █        █   ████ ██ █    █   █ ║");
            TShock.Log.ConsoleInfo($"║ █  ██  ██      █   ██   █   ██ ████ ███  █   █ ║");
            TShock.Log.ConsoleInfo($"║ █   █  █       █        █   ██  ███ █    █   █ ║");
            TShock.Log.ConsoleInfo($"║ █████  ████    █      █████ ██   ██ █    █████ ║");
            TShock.Log.ConsoleInfo($"║                                                ║");
            TShock.Log.ConsoleInfo($"║              Checking for updates...           ║");
            TShock.Log.ConsoleInfo($"╚════════════════════════════════════════════════╝");

            bool isUpToDate = await IsUpToDate(plugin);


            if (isUpToDate)
            {
                TShock.Log.ConsoleInfo($"╔════════════════════════════════════════════════╗");
                TShock.Log.ConsoleInfo($"║   [GetInfoPlayers] Plugin is up to date!!      ║");
                TShock.Log.ConsoleInfo($"╚════════════════════════════════════════════════╝");
            }
            else
            {
                TShock.Log.ConsoleInfo($"╔════════════════════════════════════════════════╗");
                TShock.Log.ConsoleError($"║ [ GetInfoPlayers ] Plugin is no up to date!!!  ║");
                TShock.Log.ConsoleInfo($"║      INSTALLED: {AvilableVersion}                          ║");
                TShock.Log.ConsoleInfo($"║      AVAILABLE: {VersionInstalled}                          ║");
                TShock.Log.ConsoleInfo($"║                                                ║");
                TShock.Log.ConsoleInfo($"║  Please visit to download the latest version.  ║");
                TShock.Log.ConsoleInfo($"║  https://github.com/itsFrankV22/PlayerGetInfo  ║");
                TShock.Log.ConsoleInfo($"╚════════════════════════════════════════════════╝");
            }
        }
    }
}
