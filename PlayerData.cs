using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TShockAPI;

namespace InfoPlayers
{
    public class PlayerData
    {
        public string PlayerName { get; set; }
        public string Country { get; set; }
        public string SelectedItem { get; set; }
        public int MaxHp { get; set; }
        public int CurrentHp { get; set; }
        public int MaxMana { get; set; }
        public int CurrentMana { get; set; }
        public int Defense { get; set; }
        public string Platform { get; set; }
        public string IP { get; set; }
        public string UUID { get; set; }
        public string Group { get; set; }
        public string GroupPrefix { get; set; }
        public string GroupSuffix { get; set; }
        public string Team { get; set; }

        private static readonly string FilePath = Path.Combine(TShock.SavePath, "GetInfoUtilData.json");

        // Método para cargar los datos del archivo JSON
        public static Dictionary<string, PlayerData> LoadData()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var jsonData = File.ReadAllText(FilePath);
                    return JsonConvert.DeserializeObject<Dictionary<string, PlayerData>>(jsonData) ?? new Dictionary<string, PlayerData>();
                }
            }
            catch (Exception ex)
            {
                TShockAPI.TShock.Log.Error($"Error loading player data: {ex.Message}");
            }
            return new Dictionary<string, PlayerData>();
        }

        // Método para guardar o actualizar la información del jugador en el archivo JSON
        public static void SaveData(Dictionary<string, PlayerData> playerDataDictionary)
        {
            try
            {
                var updatedJson = JsonConvert.SerializeObject(playerDataDictionary, Formatting.Indented);
                File.WriteAllText(FilePath, updatedJson);
            }
            catch (Exception ex)
            {
                TShockAPI.TShock.Log.Error($"Error saving player data: {ex.Message}");
            }
        }

        // Método para crear una instancia con la información del jugador
        public static PlayerData CreateFromPlayer(TSPlayer player)
        {
            return new PlayerData
            {
                PlayerName = player.Name,
                Country = player.Country ?? "Unknown",
                SelectedItem = player.SelectedItem?.netID.ToString() ?? "None",
                MaxHp = player.TPlayer.statLifeMax2,
                CurrentHp = player.TPlayer.statLife,
                MaxMana = player.TPlayer.statManaMax2,
                CurrentMana = player.TPlayer.statMana,
                Defense = player.TPlayer.statDefense,
                Platform = InfoTool.GetPlatform(player),
                IP = player.IP ?? "Hidden",
                UUID = player.UUID ?? "Unavailable",
                Group = player.Group.Name ?? "None",
                GroupPrefix = player.Group.Prefix ?? "None",
                GroupSuffix = player.Group.Suffix ?? "None",
                Team = player.Team.ToString()
            };
        }

        // Método para actualizar los datos cada vez que un jugador se une
        public static void OnPlayerJoin(TSPlayer player)
        {
            var playerDataDictionary = LoadData(); // Cargar los datos previos

            // Crear o actualizar los datos del jugador
            var playerData = CreateFromPlayer(player);
            if (playerDataDictionary.ContainsKey(player.Name))
            {
                playerDataDictionary[player.Name] = playerData; // Si ya existe, actualizamos
            }
            else
            {
                playerDataDictionary.Add(player.Name, playerData); // Si no, agregamos un nuevo registro
            }

            // Guardar los datos actualizados
            SaveData(playerDataDictionary);
        }
    }
}