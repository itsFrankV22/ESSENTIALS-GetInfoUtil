using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;
using System.Data;
using System.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using Microsoft.Data.Sqlite;
using CheckUpdates;
using System.Security.Cryptography.X509Certificates;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace InfoPlayers
{
    [ApiVersion(2, 1)]
    public class PlayerJoinInfo : TerrariaPlugin
    {
        public override string Author => "FrankV22";
        public override string Description => "Show player information";
        public override string Name => "InfoPlayers";
        public override Version Version => new Version(1, 2, 0);

        public static IDbConnection Db { get; private set; }


        private Dictionary<int, int> playerKills = new Dictionary<int, int>();
        private Dictionary<int, DateTime> firstLoginTimes = new();
        private Dictionary<int, DateTime> loginTimes = new(); // Diccionario para tiempos de inicio

        public PlayerJoinInfo(Main game) : base(game)
        {
            base.Order = int.MinValue;
        }

        public static PlatformType[] Platforms { get; set; } = new PlatformType[256];

        public static string GetPlatform(TSPlayer plr)
        {
            return Platforms[plr.Index].ToString();
        }

        public override void Initialize()
        {
            ServerApi.Hooks.GamePostInitialize.Register(this, OnPostInitialize);

            LoadFirstLoginTimes(); // Cargar tiempos de la base de datos o archivo

            ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnPlayerJoin, 10);

            On.OTAPI.Hooks.MessageBuffer.InvokeGetData += this.OnGetData;
            ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreet);
            GeneralHooks.ReloadEvent += OnReload;

            Commands.ChatCommands.Add(new Command("getinfo.admin", this.InfoCommand, "getinfo", "gi"));
            Commands.ChatCommands.Add(new Command("getinfo.user", this.InfoCommandUser, "getinfouser", "giu"));
        }

        protected override void Dispose(bool disposing)
        {


            if (disposing)
            {
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnGreetPlayer);


                ServerApi.Hooks.GamePostInitialize.Deregister(this, OnPostInitialize);
                SaveFirstLoginTimes(); // Guardar antes de desactivar
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnPlayerJoin);
                On.OTAPI.Hooks.MessageBuffer.InvokeGetData -= this.OnGetData;
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreet);
                Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.InfoCommand);
            }
            base.Dispose(disposing);
        }


        private void OnReload(ReloadEventArgs reloadEventArgs)
        {
            TShock.Log.ConsoleInfo($"###################################################");
            TShock.Log.ConsoleInfo($"# #####  ####  #####    ##### ###  ## #### #####  #");
            TShock.Log.ConsoleInfo($"# #      #       #        #   #### ## #    #   #  #");
            TShock.Log.ConsoleInfo($"# #  ##  ##      #   ##   #   ## #### ###  #   #  #");
            TShock.Log.ConsoleInfo($"# #   #  #       #        #   ##  ### #    #   #  #");
            TShock.Log.ConsoleInfo($"# #####  ####    #      ##### ##   ## #    #####  #");
            TShock.Log.ConsoleInfo($"###################################################");
            TShock.Log.ConsoleInfo($"#     GetInfoPlugin - RE-Loaded  - GodLuck!       #");
            TShock.Log.ConsoleInfo($"###################################################");
        }


        private async void OnPostInitialize(EventArgs e)
        {


            TShock.Log.ConsoleInfo($"##################################################");
            TShock.Log.ConsoleInfo($"#     GetInfoPlugin - Installed  - GodLuck!       #");
            TShock.Log.ConsoleInfo($"###################################################");

            await CheckForPluginUpdates();
        }

        public async Task CheckForPluginUpdates()
        {
            await CheckUpdates.CheckUpdates.CheckUpdateVerbose(this);
        }



        // #####################################################################################################
        // #####################################################################################################
        //                          P L A Y E R S    -   K I L L  C O U N T
        // #####################################################################################################
        // #####################################################################################################




        // #####################################################################################################
        // #####################################################################################################
        //                          P L A Y E R S    -   P L A Y T I M E  C O U N T
        // #####################################################################################################
        // #####################################################################################################

        private void LoadFirstLoginTimes()
        {
            if (File.Exists("tshock/firstLoginTimes.json"))
            {
                var json = File.ReadAllText("tshock/firstLoginTimes.json");
                firstLoginTimes = JsonConvert.DeserializeObject<Dictionary<int, DateTime>>(json) ?? new();
            }
        }

        private void SaveFirstLoginTimes()
        {
            var json = JsonConvert.SerializeObject(firstLoginTimes, Formatting.Indented);
            File.WriteAllText("tshock/firstLoginTimes.json", json);
        }


        // #####################################################################################################
        // #####################################################################################################
        //                          P L A Y E R S    -   J O I N  M E S S A G G E S
        // #####################################################################################################
        // #####################################################################################################


        private void OnGreetPlayer(GreetPlayerEventArgs args)
        {
            var player = TShock.Players[args.Who];
            if (player == null)
            {
                args.Handled = true;
                return;
            }

            string name = player.Name;
            var PlayerCountry = player.Country ?? "Unknown";

            //// Mensaje personalizado
            //string customMessage =
            //    $"[ INFO PLAYER ] [ PLAYER JOINED! ]\n" +
            //    $"[ {name} ] Joined: from {PlayerCountry} WELCOME!";

            //var messageColor = new Microsoft.Xna.Framework.Color(0, 255, 255);

            //// Envía el mensaje personalizado
            //TSPlayer.All.SendMessage(customMessage, messageColor);

            args.Handled = true;
        }


        private void OnPlayerJoin(GreetPlayerEventArgs args)
        {
        

            var player = TShock.Players[args.Who];
            if (player != null)
            {
                player.SilentJoinInProgress = true; // Evita el mensaje predeterminado


                string name = player.Name;
                var hp = player.TPlayer.statLifeMax2;
                var mana = player.TPlayer.statManaMax2;
                var currentHp = player.TPlayer.statLife;
                var currentMana = player.TPlayer.statMana;
                var groupName = player.Group.Name;
                var groupPrefix = player.Group.Prefix;

                var PlayerTilesCreated = player.TilesCreated.Count;
                var PlayerTilesDestroyed = player.TilesDestroyed.Count;

                string PlayerTeam = player.Team.ToString();
                string PlayerSelectedItem = player.SelectedItem.netID.ToString();
                string PlayerCountry = player.Country;
                string playerIP = player.IP;
                string playerGroup = player.Group.Name;
                string playerUUID = player.UUID;
                string playerID = player.UUID;
                if (!firstLoginTimes.ContainsKey(args.Who))
                {
                    // Registrar la primera vez que el jugador se conecta
                    firstLoginTimes[args.Who] = DateTime.UtcNow;
                    SaveFirstLoginTimes();

                    player.SendInfoMessage($"[ + ][ PlayerGetInfo ] by{Author} v{Version} please follow in Youtube Discord etc");
                    player.SendInfoMessage($"[ + ] - PlayTime [ NULL ]");
                    player.SendInfoMessage($"[ + ] - DEVICE: [ {InfoTool.GetPlatform(player)} ]");
                    player.SendInfoMessage($"[ + ] - COUNTRY: [ {PlayerCountry} ]");
                    player.SendInfoMessage($"[ + ] - SELECTED ITEM: [ [i:{PlayerSelectedItem}] ]");
                    player.SendInfoMessage($"#  [ + ] - TILES CREATED: [ {PlayerTilesCreated}] ]");
                    player.SendInfoMessage($"#  [ + ] - TILES DESTROYED: [ {PlayerTilesDestroyed}] ]");

                    // Mensaje personalizado
                    string customMessage =
                        $"[ INFO PLAYER ] [ PLAYER JOINED! ]\n" +
                        $"[ {name} ] Joined: from {PlayerCountry} WELCOME!";

                    var messageColor = new Microsoft.Xna.Framework.Color(0, 255, 255);

                    // Envía el mensaje personalizado
                    TSPlayer.All.SendMessage(customMessage, messageColor);
                }
                else
                {
                    var firstLoginTime = firstLoginTimes[args.Who];
                    var playTime = DateTime.UtcNow - firstLoginTime;

                    string formattedPlayTime = $"{(int)playTime.TotalHours:D2}:{playTime.Minutes:D2}:{playTime.Seconds:D2}";
                    player.SendInfoMessage($"[ + ][ PlayerGetInfo ] by{Author} v{Version} please follow in Youtube Discord etc");
                    player.SendInfoMessage($"[ + ] - PlayTime [ {formattedPlayTime} ]");
                    player.SendInfoMessage($"[ + ] - DEVICE: [ {InfoTool.GetPlatform(player)} ]");
                    player.SendInfoMessage($"[ + ] - COUNTRY: [ {PlayerCountry} ]");
                    player.SendInfoMessage($"[ + ] - SELECTED ITEM: [ [i:{PlayerSelectedItem}] ]");
                    player.SendInfoMessage($"#  [ + ] - TILES CREATED: [ {PlayerTilesCreated}] ]");
                    player.SendInfoMessage($"#  [ + ] - TILES DESTROYED: [ {PlayerTilesDestroyed}] ]");

                    // Mensaje personalizado
                    string customMessage =
                        $"[ INFO PLAYER ] [ PLAYER JOINED! ]\n" +
                        $"[ {name} ] Joined: from {PlayerCountry} WELCOME!";

                    var messageColor = new Microsoft.Xna.Framework.Color(0, 255, 255);

                    // Envía el mensaje personalizado
                    TSPlayer.All.SendMessage(customMessage, messageColor);
                }
            }
        }

        // #####################################################################################################
        // #####################################################################################################
        //                          P L A Y E R S    -   U S E R  C O M A N D
        // #####################################################################################################
        // #####################################################################################################
        private void InfoCommandUser(CommandArgs args)
        {
            if (args.Parameters.Count == 1)
            {
                if (args.Parameters.Count == 1)
                {
                    var plys = TSPlayer.FindByNameOrID(args.Parameters[0]);
                    if (plys.Count > 0)
                    {
                        var targetPlayer = plys[0];
                        string name = targetPlayer.Name;
                        var hp = targetPlayer.TPlayer.statLifeMax2;
                        var mana = targetPlayer.TPlayer.statManaMax2;
                        var currentHp = targetPlayer.TPlayer.statLife;
                        var currentMana = targetPlayer.TPlayer.statMana;
                        var groupName = targetPlayer.Group.Name;
                        var groupPrefix = targetPlayer.Group.Prefix;



                        var PlayerTilesCreated = targetPlayer.TilesCreated.Count;
                        var PlayerTilesDestroyed = targetPlayer.TilesDestroyed.Count;

                        string PlayerSelectedItem = targetPlayer.SelectedItem.netID.ToString();
                        string PlayerCountry = targetPlayer.Country;
                        string playerIP = targetPlayer.IP;
                        string playerGroup = targetPlayer.Group.Name;
                        var platform = InfoTool.GetPlatform(targetPlayer);

                        if (firstLoginTimes.TryGetValue(targetPlayer.Index, out DateTime firstLoginTime))
                        {
                            var playTime = DateTime.UtcNow - firstLoginTime;
                            string formattedPlayTime = $"{(int)playTime.TotalHours:D2}:{playTime.Minutes:D2}:{playTime.Seconds:D2}";


                            args.Player.SendInfoMessage($"#####################################");
                            args.Player.SendInfoMessage($"#          USER - INFO              #");
                            args.Player.SendInfoMessage($"#####################################");
                            args.Player.SendInfoMessage($"#  [ InfoPlayer ] PLAYER: {targetPlayer.Name} ]");
                            args.Player.SendInfoMessage($"#  [ + ] - PlayTime [ {formattedPlayTime} ]");
                            args.Player.SendInfoMessage($"#  [ + ] - DEVICE: [ {InfoTool.GetPlatform(targetPlayer)} ]");
                            args.Player.SendInfoMessage($"#  [ + ] - IP: [ {playerIP} ]");
                            args.Player.SendInfoMessage($"#  [ + ] - COUNTRY: [ {PlayerCountry} ]");
                            args.Player.SendInfoMessage($"#  [ + ] - GROUP: [ {playerGroup} ]");
                            args.Player.SendInfoMessage($"#  [ + ] - SELECTED ITEM: [ [i:{PlayerSelectedItem}]");
                            args.Player.SendInfoMessage($"#  [ + ] - LIFE: [ {hp} ]");
                            args.Player.SendInfoMessage($"#  [ + ] - LIFE: [ {mana} ]");
                            args.Player.SendInfoMessage($"#  [ + ] - CURRENT LIFE: [ {currentHp} ]");
                            args.Player.SendInfoMessage($"#  [ + ] - CURRENT MANA: [ {mana} ]");
                            args.Player.SendInfoMessage($"#  [ + ] - TILES CREATED: [ {PlayerTilesCreated}] ]");
                            args.Player.SendInfoMessage($"#  [ + ] - TILES DESTROYED: [ {PlayerTilesDestroyed}] ]");
                            args.Player.SendInfoMessage($"######################################");
                        }
                        else
                        {
                            args.Player.SendInfoMessage($"#####################################");
                            args.Player.SendInfoMessage($"#          USER - INFO              #");
                            args.Player.SendInfoMessage($"#####################################");
                            args.Player.SendInfoMessage($"#  [ InfoPlayer ] PLAYER: {targetPlayer.Name} ]");
                            args.Player.SendInfoMessage($"#  [ + ] - PlayTime [ 00:00:00 ]");
                            args.Player.SendInfoMessage($"#  [ + ] - DEVICE: [ {InfoTool.GetPlatform(targetPlayer)} ]");
                            args.Player.SendInfoMessage($"#  [ + ] - IP: [ {playerIP} ]");
                            args.Player.SendInfoMessage($"#  [ + ] - COUNTRY: [ {PlayerCountry} ]");
                            args.Player.SendInfoMessage($"#  [ + ] - GROUP: [ {playerGroup} ]");
                            args.Player.SendInfoMessage($"#  [ + ] - SELECTED ITEM: [ [i:{PlayerSelectedItem}]");
                            args.Player.SendInfoMessage($"#  [ + ] - LIFE: [ {hp} ]");
                            args.Player.SendInfoMessage($"#  [ + ] - LIFE: [ {mana} ]");
                            args.Player.SendInfoMessage($"#  [ + ] - CURRENT LIFE: [ {currentHp} ]");
                            args.Player.SendInfoMessage($"#  [ + ] - CURRENT MANA: [ {mana} ]");
                            args.Player.SendInfoMessage($"#  [ + ] - TILES CREATED: [ {PlayerTilesCreated}] ]");
                            args.Player.SendInfoMessage($"#  [ + ] - TILES DESTROYED: [ {PlayerTilesDestroyed}] ]");
                            args.Player.SendInfoMessage($"######################################");
                        }
                    }
                    else
                    {
                        args.Player.SendErrorMessage("The player is not online!");
                    }
                }
                else
                {
                    args.Player.SendErrorMessage("[ - ] Syntax error.");
                    args.Player.SendInfoMessage("/getinfouser <Player name>");
                    args.Player.SendInfoMessage("/giu <Player name>");
                }
            }
        }

        // #####################################################################################################
        // #####################################################################################################
        //                          P L A Y E R S    -   A D M I N C O M M A N D
        // #####################################################################################################
        // #####################################################################################################
        private void InfoCommand(CommandArgs args)
        {

            if (args.Parameters.Count == 1)
            {
                var plys = TSPlayer.FindByNameOrID(args.Parameters[0]);
                if (plys.Count > 0)
                {

                    var targetPlayer = plys[0];
                    var platform = InfoTool.GetPlatform(targetPlayer);


                    string name = targetPlayer.Name;
                    var hp = targetPlayer.TPlayer.statLifeMax2;
                    var mana = targetPlayer.TPlayer.statManaMax2;
                    var currentHp = targetPlayer.TPlayer.statLife;
                    var currentMana = targetPlayer.TPlayer.statMana;
                    var groupName = targetPlayer.Group.Name;
                    var groupPrefix = targetPlayer.Group.Prefix;

                    var PlayerTilesCreated = targetPlayer.TilesCreated.Count;
                    var PlayerTilesDestroyed = targetPlayer.TilesDestroyed.Count;

                    string playerCountry = targetPlayer.Country;
                    string playerTeam = targetPlayer.Team.ToString();
                    string playerSelectedItem = targetPlayer.SelectedItem.netID.ToString();
                    string playerName = targetPlayer.Name;
                    string playerGroup = targetPlayer.Group.Name;
                    string playerUUID = targetPlayer.UUID;
                    string playerIP = targetPlayer.IP;

                    if (firstLoginTimes.TryGetValue(targetPlayer.Index, out DateTime firstLoginTime))
                    {

                        var playTime = DateTime.UtcNow - firstLoginTime;
                        string formattedPlayTime = $"{(int)playTime.TotalHours:D2}:{playTime.Minutes:D2}:{playTime.Seconds:D2}";


                        args.Player.SendInfoMessage($"###################################################");
                        args.Player.SendInfoMessage($"#            I N F O  - P L A Y E R               #");
                        args.Player.SendInfoMessage($"###################################################");
                        args.Player.SendInfoMessage($"  [ InfoPlayer ] PLAYER: [ {playerName} ]");
                        args.Player.SendInfoMessage($"  [ + ] - PlayTime [ {formattedPlayTime} ]");
                        args.Player.SendInfoMessage($"  [ + ] - DEVICE: [ {InfoTool.GetPlatform(targetPlayer)} ]");
                        args.Player.SendInfoMessage($"  [ + ] - IP: [ {playerIP} ]");
                        args.Player.SendInfoMessage($"  [ + ] - COUNTRY: [ {playerCountry} ]");
                        args.Player.SendInfoMessage($"  [ + ] - TEAM: [ {playerTeam} ]");
                        args.Player.SendInfoMessage($"  [ + ] - GROUP: [ {playerGroup} ]");
                        args.Player.SendInfoMessage($"  [ + ] - SELECTED ITEM: [ [i:{playerSelectedItem} ]");
                        args.Player.SendInfoMessage($"  [ + ] - LIFE: [ {hp} ]");
                        args.Player.SendInfoMessage($"  [ + ] - LIFE: [ {mana} ]");
                        args.Player.SendInfoMessage($"  [ + ] - CURRENT LIFE: [ {currentHp} ]");
                        args.Player.SendInfoMessage($"  [ + ] - CURRENT MANA: [ {mana} ]");
                        args.Player.SendInfoMessage($"  [ + ] - TILES CREATED: [ {PlayerTilesCreated}] ]");
                        args.Player.SendInfoMessage($"  [ + ] - TILES DESTROYED: [ {PlayerTilesDestroyed}] ]");
                        args.Player.SendInfoMessage($"###################################################");
                    }
                    else
                    {

                        args.Player.SendInfoMessage($"###################################################");
                        args.Player.SendInfoMessage($"#            I N F O  - P L A Y E R               #");
                        args.Player.SendInfoMessage($"###################################################");
                        args.Player.SendInfoMessage($"  [ InfoPlayer ] PLAYER: [ {playerName} ]");
                        args.Player.SendInfoMessage($"  [ + ] - PlayTime [ 00:00:00 ]");
                        args.Player.SendInfoMessage($"  [ + ] - DEVICE: [ {InfoTool.GetPlatform(targetPlayer)} ]");
                        args.Player.SendInfoMessage($"  [ + ] - IP: [ {playerIP} ]");
                        args.Player.SendInfoMessage($"  [ + ] - COUNTRY: [ {playerCountry} ]");
                        args.Player.SendInfoMessage($"  [ + ] - TEAM: [ {playerTeam} ]");
                        args.Player.SendInfoMessage($"  [ + ] - GROUP: [ {playerGroup} ]");
                        args.Player.SendInfoMessage($"  [ + ] - SELECTED ITEM: [ [i:{playerSelectedItem}] ]");
                        args.Player.SendInfoMessage($"  [ + ] - LIFE: [ {hp} ]");
                        args.Player.SendInfoMessage($"  [ + ] - LIFE: [ {mana} ]");
                        args.Player.SendInfoMessage($"  [ + ] - CURRENT LIFE: [ {currentHp} ]");
                        args.Player.SendInfoMessage($"  [ + ] - CURRENT MANA: [ {mana} ]");
                        args.Player.SendInfoMessage($"  [ + ] - TILES CREATED: [ {PlayerTilesCreated}] ]");
                        args.Player.SendInfoMessage($"  [ + ] - TILES DESTROYED: [ {PlayerTilesDestroyed} ]");
                        args.Player.SendInfoMessage($"###################################################");
                    }
                }
                else
                {
                    args.Player.SendErrorMessage("The player is not online!");
                }
            }
            else
            {
                args.Player.SendErrorMessage("[ - ] Syntax error.");
                args.Player.SendInfoMessage("/getinfo <Player name>");

                args.Player.SendInfoMessage("/gi <Player name>");
            }
        }

        // #####################################################################################################
        // #####################################################################################################
        //               P L A Y E R S    -   P L A Y E R  J O I N M E S S A G E S  C O N S O L E
        // #####################################################################################################
        // #####################################################################################################
        private void OnGreet(GreetPlayerEventArgs args)
        {

            var player = TShock.Players[args.Who];
            if (player == null)
            {
                return;
            }


            string name = player.Name;
            var hp = player.TPlayer.statLifeMax2;
            var mana = player.TPlayer.statManaMax2;
            var currentHp = player.TPlayer.statLife;
            var currentMana = player.TPlayer.statMana;
            var groupName = player.Group.Name;
            var groupPrefix = player.Group.Prefix;
            var PlayerTilesCreated = player.TilesCreated.Count;
            var PlayerTilesDestroyed = player.TilesDestroyed.Count;
            string playerCountry = player.Country;
            string playerTeam = player.Team.ToString();
            string playerSelectedItem = player.SelectedItem.netID.ToString();
            string playerName = player.Name;
            string playerGroup = player.Group.Name;
            string playerUUID = player.UUID;
            string playerIP = player.IP;

            if (firstLoginTimes.TryGetValue(player.Index, out DateTime firstLoginTime))
            {

                var playTime = DateTime.UtcNow - firstLoginTime;
                string formattedPlayTime = $"{(int)playTime.TotalHours:D2}:{playTime.Minutes:D2}:{playTime.Seconds:D2}";


                loginTimes[args.Who] = DateTime.UtcNow; // Registrar tiempo de inicio
                TShock.Log.ConsoleInfo($"###################################################");
                TShock.Log.ConsoleInfo($"#          J O I N E D - P L A Y E R               #");
                TShock.Log.ConsoleInfo($"###################################################");
                TShock.Log.ConsoleInfo($"[ InfoPlayer ] PLAYER: {playerName} ]");
                TShock.Log.ConsoleInfo($"[ + ] - PlayTime [ {formattedPlayTime} ]");
                TShock.Log.ConsoleInfo($"[ + ] - DEVICE: [ {Platforms[args.Who]} ]");
                TShock.Log.ConsoleInfo($"[ + ] - IP: [ {playerIP} ]");
                TShock.Log.ConsoleInfo($"[ + ] - COUNTRY: [ {playerCountry} ]");
                TShock.Log.ConsoleInfo($"[ + ] - TEAM: [ {playerTeam} ]");
                TShock.Log.ConsoleInfo($"[ + ] - GROUP: [ {playerGroup} ]");
                TShock.Log.ConsoleInfo($"[ + ] - SELECTED ITEM: [ [i:{playerSelectedItem}]");
                TShock.Log.ConsoleInfo($"[ + ] - LIFE: [ {hp} ]");
                TShock.Log.ConsoleInfo($"[ + ] - MANA: [ {mana} ]");
                TShock.Log.ConsoleInfo($"[ + ] - CURRENT LIFE: [ {currentHp} ]");
                TShock.Log.ConsoleInfo($"[ + ] - CURRENT MANA: [ {mana} ]");
                TShock.Log.ConsoleInfo($"[ + ] - TILES CREATED: [ {PlayerTilesCreated} ]");
                TShock.Log.ConsoleInfo($"[ + ] - TILES DESTROYED: [ {PlayerTilesDestroyed} ]");
                TShock.Log.ConsoleInfo($"###################################################");
            }
            else
            {

                loginTimes[args.Who] = DateTime.UtcNow; // Registrar tiempo de inicio
                TShock.Log.ConsoleInfo($"###################################################");
                TShock.Log.ConsoleInfo($"#          J O I N E D - P L A Y E R               #");
                TShock.Log.ConsoleInfo($"###################################################");
                TShock.Log.ConsoleInfo($"[ InfoPlayer ] PLAYER: {playerName} ]");
                TShock.Log.ConsoleInfo($"[ + ] - PlayTime [ 00:00:00 ]");
                TShock.Log.ConsoleInfo($"[ + ] - DEVICE: [ {Platforms[args.Who]} ]");
                TShock.Log.ConsoleInfo($"[ + ] - IP: [ {playerIP} ]");
                TShock.Log.ConsoleInfo($"[ + ] - COUNTRY: [ {playerCountry} ]");
                TShock.Log.ConsoleInfo($"[ + ] - TEAM: [ {playerTeam} ]");
                TShock.Log.ConsoleInfo($"[ + ] - GROUP: [ {playerGroup} ]");
                TShock.Log.ConsoleInfo($"[ + ] - SELECTED ITEM: [ [i:{playerSelectedItem}]");
                TShock.Log.ConsoleInfo($"[ + ] - LIFE: [ {hp} ]");
                TShock.Log.ConsoleInfo($"[ + ] - MANA: [ {mana} ]");
                TShock.Log.ConsoleInfo($"[ + ] - CURRENT LIFE: [ {currentHp} ]");
                TShock.Log.ConsoleInfo($"[ + ] - CURRENT MANA: [ {mana} ]");
                TShock.Log.ConsoleInfo($"[ + ] - TILES CREATED: [ {PlayerTilesCreated} ]");
                TShock.Log.ConsoleInfo($"[ + ] - TILES DESTROYED: [ {PlayerTilesDestroyed} ]");
                TShock.Log.ConsoleInfo($"###################################################");
            }
        }

        
        // #####################################################################################################
        // #####################################################################################################
        //                          P L A Y E R S    -   P L A T T F O R M  D E T E C T O R
        // #####################################################################################################
        // #####################################################################################################
        private bool OnGetData(On.OTAPI.Hooks.MessageBuffer.orig_InvokeGetData orig, MessageBuffer instance, ref byte packetId, ref int readOffset, ref int start, ref int length, ref int messageType, int maxPackets)
        {
            try
            {
                if (messageType == 1)
                {
                    Platforms[instance.whoAmI] = PlatformType.PC;
                }

                if (messageType == 150)
                {
                    instance.ResetReader();
                    instance.reader.BaseStream.Position = start + 1;
                    var PlayerSlot = instance.reader.ReadByte();
                    var Platform = instance.reader.ReadByte();
                    Platforms[instance.whoAmI] = (PlatformType)Platform;
                }
            }
            catch { }

            return orig(instance, ref packetId, ref readOffset, ref start, ref length, ref messageType, maxPackets);
        }

        

        public enum PlatformType : byte
        {
            PE = 0,
            Stadia = 1,
            XBOX = 2,
            PSN = 3,
            Editor = 4,
            Switch = 5,
            PC = 233
        }
    }

    public static class InfoTool
    {
        public static string GetPlatform(this TSPlayer plr)
        {
            return PlayerJoinInfo.Platforms[plr.Index].ToString();
        }
    }
}