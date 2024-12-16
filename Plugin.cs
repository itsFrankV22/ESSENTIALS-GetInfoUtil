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
//using ESSENTIALS_GetInfoUtil;

namespace InfoPlayers
{
    [ApiVersion(2, 1)]
    public class PlayerJoinInfo : TerrariaPlugin
    {
        public override string Author => "FrankV22";
        public override string Description => "Show Info and better comand info";
        public override string Name => "ESSENTIALS GetInfoUtil";
        public override Version Version => new Version(1, 5, 0);

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

            ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnPlayerJoin, 9);

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
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnPlayerJoin);
                On.OTAPI.Hooks.MessageBuffer.InvokeGetData -= this.OnGetData;
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreet);
                Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.InfoCommand);
            }
            base.Dispose(disposing);
        }


        // #####################################################################################################
        // #####################################################################################################
        //                          R E L O A D    -   C O M M A N D
        // #####################################################################################################
        // #####################################################################################################

        private void OnReload(ReloadEventArgs reloadEventArgs)
        {
            TShock.Log.ConsoleInfo($"╔════════════════════════════════════════════════╗");
            TShock.Log.ConsoleInfo($"║ █████  ████  █████    █████ ███  ██ ████ █████ ║");
            TShock.Log.ConsoleInfo($"║ █      █       █        █   ████ ██ █    █   █ ║");
            TShock.Log.ConsoleInfo($"║ █  ██  ██      █   ██   █   ██ ████ ███  █   █ ║");
            TShock.Log.ConsoleInfo($"║ █   █  █       █        █   ██  ███ █    █   █ ║");
            TShock.Log.ConsoleInfo($"║ █████  ████    █      █████ ██   ██ █    █████ ║");
            TShock.Log.ConsoleInfo($"║                                                ║");
            TShock.Log.ConsoleInfo($"║     GetInfoPlugin - RELOADED   - GOD LUCK!     ║");
            TShock.Log.ConsoleInfo($"╚════════════════════════════════════════════════╝");
        }


        private async void OnPostInitialize(EventArgs e)
        {

            await CheckForPluginUpdates();
        }

        public async Task CheckForPluginUpdates()
        {
            await CheckUpdates.CheckUpdates.CheckUpdateVerbose(this);
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
            args.Handled = true;
        }




        private void OnPlayerJoin(GreetPlayerEventArgs args)
        {
            var player = TShock.Players[args.Who];
            if (player != null)
            {
                player.SilentJoinInProgress = true; // Evitar mensaje predeterminado

                // Esperar 1 segundo antes de obtener estadísticas
                Task.Delay(1000).ContinueWith(_ =>
                {
                    string name = player.Name;
                    int maxHp = player.TPlayer.statLifeMax2;  // Vida máxima
                    int currentHp = player.TPlayer.statLife; // Vida actual
                    int maxMana = player.TPlayer.statManaMax2; // Maná máximo
                    int currentMana = player.TPlayer.statMana; // Maná actual
                    int defense = player.TPlayer.statDefense; // Defensa actual

                    string playerCountry = player.Country ?? "Unknown";

                    // Mensajes al jugador
                    player.SendInfoMessage($"║  Y O U R  -  S T A T S ║");
                    player.SendInfoMessage($"  [i:29] [c/FF6666:LIFE:] [ {currentHp}/{maxHp} ]");
                    player.SendInfoMessage($"  [i:109] [c/00E5FF:MANA:] [ {currentMana}/{maxMana} ]");
                    player.SendInfoMessage($"  [i:938] [c/66B2FF:DEFENSE:] [ {defense} ]");
                    player.SendInfoMessage($"  [i:4080] [c/FFCC66:COUNTRY:] [ {playerCountry} ]");
                });


                string name = player.Name;
                string playerCountry  = player.Country;

                // Mensaje personalizado para todos los jugadores
                string customMessage =
                    $"[ + ] [ PLAYER JOINED!! ]\n" +
                    $"[ {InfoTool.GetPlatform(player)} ][ {name} ] From: [c/FFB000:{playerCountry}] Welcome!";

                var messageColor = new Microsoft.Xna.Framework.Color(0, 255, 255);
                TSPlayer.All.SendMessage(customMessage, messageColor);
            }
        }


        // #####################################################################################################
        // #####################################################################################################
        //                          P L A Y E R S    -   U S E R  C O M A N D
        // #####################################################################################################
        // #####################################################################################################
        private void InfoCommandUser(CommandArgs args)
        {
            if (args.Parameters.Count == 1) // Validar que el comando tenga un parámetro
            {
                var plys = TSPlayer.FindByNameOrID(args.Parameters[0]); // Buscar jugadores por nombre o ID

                if (plys.Count > 0) // Verificar si se encontró al menos un jugador
                {
                    TSPlayer targetPlayer = plys[0]; // Asignar el primer jugador encontrado

                    if (targetPlayer == null || !targetPlayer.Active) // Verificar si el jugador está activo
                    {
                        args.Player.SendErrorMessage("The player is not online!");
                        return;
                    }

                    // Obtener información del jugador objetivo
                    string playerCountry = targetPlayer.Country ?? "Unknown"; // País del jugador
                    string playerSelectedItem = targetPlayer.SelectedItem?.netID.ToString() ?? "None"; // Objeto seleccionado
                    string name = targetPlayer.Name;
                    int maxHp = targetPlayer.TPlayer.statLifeMax2;  // Vida máxima
                    int currentHp = targetPlayer.TPlayer.statLife; // Vida actual
                    int maxMana = targetPlayer.TPlayer.statManaMax2; // Maná máximo
                    int currentMana = targetPlayer.TPlayer.statMana; // Maná actual
                    int defense = targetPlayer.TPlayer.statDefense; // Defensa actual
                    var platform = InfoTool.GetPlatform(targetPlayer); // Dispositivo del jugador

                    // Mensajes para mostrar información al jugador que ejecutó el comando
                    args.Player.SendInfoMessage($"╔═══════════════════════════════════════════════╗");
                    args.Player.SendInfoMessage($"║  // USER - INFO                               ║");
                    args.Player.SendInfoMessage($"╚═══════════════════════════════════════════════╝");
                    args.Player.SendInfoMessage($"  [ InfoPlayer ] PLAYER: [ {name} ]");
                    args.Player.SendInfoMessage($"  [ + ] - DEVICE: [ {platform} ]");
                    args.Player.SendInfoMessage($"  [ + ] - COUNTRY: [ {playerCountry} ]");
                    args.Player.SendInfoMessage($"  [ + ] - SELECTED ITEM: [ [i:{playerSelectedItem}] ]");
                    args.Player.SendInfoMessage($"════════════════════════════════════════════════");
                    args.Player.SendInfoMessage($"  [i:29] [c/FF6666:LIFE:] [ {currentHp}/{maxHp} ]");
                    args.Player.SendInfoMessage($"  [i:109] [c/00E5FF:MANA:] [ {currentMana}/{maxMana} ]");
                    args.Player.SendInfoMessage($"  [i:938] [c/66B2FF:DEFENSE:] [ {defense} ]");
                    args.Player.SendInfoMessage($"╚═══════════════════════════════════════════════╝");
                }
                else
                {
                    args.Player.SendErrorMessage("The player is not online!");
                }
            }
            else
            {
                // Mensaje de error en caso de sintaxis incorrecta
                args.Player.SendErrorMessage("[ - ] Syntax error.");
                args.Player.SendInfoMessage("/getinfouser <Player name>");
                args.Player.SendInfoMessage("/giu <Player name>");
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
                    TSPlayer targetPlayer = plys[0];

                    if (targetPlayer == null || !targetPlayer.Active) // Validar si el jugador está activo
                    {
                        args.Player.SendErrorMessage("The player is not online!");
                        return;
                    }

                    // Información del jugador
                    string playerName = targetPlayer.Name;
                    string playerCountry = targetPlayer.Country ?? "Unknown";
                    string playerIP = targetPlayer.IP ?? "Hidden";
                    string playerUUID = targetPlayer.UUID ?? "Unavailable";
                    string playerGroup = targetPlayer.Group.Name ?? "None";
                    string playerGroupPrefix = targetPlayer.Group.Prefix ?? "None";
                    string playerTeam = targetPlayer.Team.ToString();
                    string playerSelectedItem = targetPlayer.SelectedItem?.netID.ToString() ?? "None";
                    int currentHp = targetPlayer.TPlayer.statLife;
                    int maxHp = targetPlayer.TPlayer.statLifeMax2;
                    int currentMana = targetPlayer.TPlayer.statMana;
                    int maxMana = targetPlayer.TPlayer.statManaMax2;
                    int defense = targetPlayer.TPlayer.statDefense;
                    var platform = InfoTool.GetPlatform(targetPlayer);

                    // Mostrar mensajes al jugador que ejecutó el comando
                    args.Player.SendInfoMessage($"╔═══════════════════════════════════════════════╗");
                    args.Player.SendInfoMessage($"║               I N F O - P L A Y E R           ║");
                    args.Player.SendInfoMessage($"╚═══════════════════════════════════════════════╝");
                    args.Player.SendInfoMessage($"  [ InfoPlayer ] PLAYER: [ {playerName} ]");
                    args.Player.SendInfoMessage($"  [ + ] - DEVICE: [ {platform} ]");
                    args.Player.SendInfoMessage($"  [ + ] - IP: [ {playerIP} ]");
                    args.Player.SendInfoMessage($"  [ + ] - UUID: [ {playerUUID} ]");
                    args.Player.SendInfoMessage($"  [ + ] - COUNTRY: [ {playerCountry} ]");
                    args.Player.SendInfoMessage($"  [ + ] - TEAM: [ {playerTeam} ]");
                    args.Player.SendInfoMessage($"  [ + ] - GROUP: [ {playerGroup} ] (Prefix: {playerGroupPrefix})");
                    args.Player.SendInfoMessage($"  [ + ] - SELECTED ITEM: [ [i:{playerSelectedItem}] ]");
                    args.Player.SendInfoMessage($"════════════════════════════════════════════════");
                    args.Player.SendInfoMessage($"  [ STATS ]");
                    args.Player.SendInfoMessage($"  [c/FF6666:LIFE:] [ Current: {currentHp} / Max: {maxHp} ]");
                    args.Player.SendInfoMessage($"  [c/00E5FF:MANA:] [ Current: {currentMana} / Max: {maxMana} ]");
                    args.Player.SendInfoMessage($"  [c/66B2FF:DEFENSE:] [ {defense} ]");
                    args.Player.SendInfoMessage($"╚═══════════════════════════════════════════════╝");
                }
                else
                {
                    args.Player.SendErrorMessage("The player is not online!");
                }
            }
            else
            {
                // Error de sintaxis
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
            // Obtener el jugador
            var player = TShock.Players[args.Who];
            if (player == null)
            {
                return;
            }

            // Información básica del jugador
            string playerName = player.Name;
            string playerIP = player.IP;
            string playerUUID = player.UUID;
            string playerCountry = player.Country;
            string playerTeam = player.Team.ToString();
            string playerGroup = player.Group.Name;
            string groupPrefix = player.Group.Prefix;
            string playerSelectedItem = player.SelectedItem.netID.ToString();
            int defense = player.TPlayer.statDefense; // Defensa actual

            // Estadísticas de vida y maná
            int maxLife = player.TPlayer.statLifeMax2;
            int currentLife = player.TPlayer.statLife;
            int maxMana = player.TPlayer.statManaMax2;
            int currentMana = player.TPlayer.statMana;

            // Registrar el tiempo de inicio
            loginTimes[args.Who] = DateTime.UtcNow;

            // Registro en consola
            TShock.Log.ConsoleInfo($"╔═══════════════════════════════════════════════╗");
            TShock.Log.ConsoleInfo($"║             J O I N E D - P L A Y E R         ║");
            TShock.Log.ConsoleInfo($"╚═══════════════════════════════════════════════╝");
            TShock.Log.ConsoleInfo($"  [InfoPlayer] PLAYER: [ {playerName} ]");
            TShock.Log.ConsoleInfo($"  [ + ] - DEVICE: [ {Platforms[args.Who]} ]");
            TShock.Log.ConsoleInfo($"  [ + ] - IP: [ {playerIP} ]");
            TShock.Log.ConsoleInfo($"  [ + ] - COUNTRY: [ {playerCountry} ]");
            TShock.Log.ConsoleInfo($"  [ + ] - TEAM: [ {playerTeam} ]");
            TShock.Log.ConsoleInfo($"  [ + ] - GROUP: [ {playerGroup} ]");
            TShock.Log.ConsoleInfo($"  [ + ] - SELECTED ITEM: [ [i:{playerSelectedItem}] ]");

            // Sección de estadísticas de vida y maná
            TShock.Log.ConsoleInfo($"  [Stats] LIFE: {currentLife}/{maxLife}");
            TShock.Log.ConsoleInfo($"  [Stats] MANA: {currentMana}/{maxMana}");
            TShock.Log.ConsoleInfo($"  [Stats] DEFE: {defense}");
            TShock.Log.ConsoleInfo($"╚═══════════════════════════════════════════════╝");
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