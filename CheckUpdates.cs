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
            string url = "https://api.github.com/repos/itsFrankV22/PlayerGetInfo/releases/latest/";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.TryParseAdd("request"); // Set a user agent header

                try
                {
                    var response = await client.GetStringAsync(url);
                    dynamic? latestRelease = JsonConvert.DeserializeObject<dynamic>(response);

                    if (latestRelease == null) return null;

                    string tag = latestRelease.tag_name;

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
                    //TShock.Log.ConsoleInfo($"###################################################");
                    //TShock.Log.ConsoleInfo($"#  An error occurred while checking for updates.  #", ConsoleColor.Red);
                    //TShock.Log.ConsoleInfo($"###################################################");

                    TShock.Log.ConsoleInfo($"###################################################");
                    TShock.Log.ConsoleInfo($"#            Feature still in progress.           #", ConsoleColor.Red);
                    TShock.Log.ConsoleInfo($"###################################################");
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
            if (plugin == null) return;

            TShock.Log.ConsoleInfo($"###################################################");
            TShock.Log.ConsoleInfo($"# #####  ####  #####    ##### ###  ## #### #####  #");
            TShock.Log.ConsoleInfo($"# #      #       #        #   #### ## #    #   #  #");
            TShock.Log.ConsoleInfo($"# #  ##  ##      #   ##   #   ## #### ###  #   #  #");
            TShock.Log.ConsoleInfo($"# #   #  #       #        #   ##  ### #    #   #  #");
            TShock.Log.ConsoleInfo($"# #####  ####    #      ##### ##   ## #    #####  #");
            TShock.Log.ConsoleInfo($"###################################################");
            TShock.Log.ConsoleInfo($"#              Checking for updates...            #", ConsoleColor.Cyan);
            TShock.Log.ConsoleInfo($"###################################################");

            bool isUpToDate = await IsUpToDate(plugin);

            if (isUpToDate)
            {
                //TShock.Log.ConsoleInfo($"###################################################");
                //TShock.Log.ConsoleInfo($"#            Plugin is up to date!!!              #", ConsoleColor.Green);
                //TShock.Log.ConsoleInfo($"###################################################");
            }
            else
            {
            //    TShock.Log.ConsoleInfo($"###################################################");
            //    TShock.Log.ConsoleInfo($"#            Plugin is no up to date!!!           #", ConsoleColor.Green);
            //    TShock.Log.ConsoleInfo($"###################################################");
            //    TShock.Log.ConsoleInfo($"#   Please visit to download the latest version.  #", ConsoleColor.Blue);
            //    TShock.Log.ConsoleInfo($"#  https://github.com/itsFrankV22/PlayerGetInfo   #", ConsoleColor.Blue);
            //    TShock.Log.ConsoleInfo($"###################################################");

                TShock.Log.ConsoleInfo($"###################################################");
                TShock.Log.ConsoleInfo($"#       Visit to see if there is an update        #", ConsoleColor.Blue);
                TShock.Log.ConsoleInfo($"#                  Current: 1.2.0                 #", ConsoleColor.Blue);
                TShock.Log.ConsoleInfo($"#  https://github.com/itsFrankV22/PlayerGetInfo   #", ConsoleColor.Blue);
                TShock.Log.ConsoleInfo($"###################################################");
            }
        }
    }
}
