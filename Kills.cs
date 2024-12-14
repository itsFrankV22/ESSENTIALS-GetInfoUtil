using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace ESSENTIALS_GetInfoUtil
{
    //public class KillCounter
    //{
    //    private static string filePath = "container/home/tshock/ESSENTIALS/GetInfoUtil/playerkills.json";
    //    public static Dictionary<string, int> PlayerKills { get; private set; } = LoadKills();

    //    public static Dictionary<string, int> LoadKills()
    //    {
    //        if (File.Exists(filePath))
    //        {
    //            string json = File.ReadAllText(filePath);
    //            return JsonConvert.DeserializeObject<Dictionary<string, int>>(json) ?? new Dictionary<string, int>();
    //        }
    //        return new Dictionary<string, int>();
    //    }

    //    public static void SaveKills()
    //    {
    //        string json = JsonConvert.SerializeObject(PlayerKills, Formatting.Indented);
    //        File.WriteAllText(filePath, json);
    //    }

    //    public static void RegisterKill(string playerName)
    //    {
    //        if (PlayerKills.ContainsKey(playerName))
    //        {
    //            PlayerKills[playerName]++;
    //        }
    //        else
    //        {
    //            PlayerKills[playerName] = 1;
    //        }
    //        SaveKills();
    //    }
    //}
}
