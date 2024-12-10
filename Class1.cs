using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace PlayerJoinInfo
{
    [ApiVersion(2, 1)]
    public class PlayerJoinInfo : TerrariaPlugin
    {
        public override string Author => "FrankV22";
        public override string Description => "Show player information";
        public override string Name => "PlayerGetInfo";
        public override Version Version => new Version(1, 0, 0);

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
            LoadFirstLoginTimes(); // Cargar tiempos de la base de datos o archivo
            ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnPlayerJoin);
            On.OTAPI.Hooks.MessageBuffer.InvokeGetData += this.OnGetData;
            ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreet);

            Commands.ChatCommands.Add(new Command("getinfo.admin", this.InfoCommand, "getinfo", "gi"));
            Commands.ChatCommands.Add(new Command("getinfo.user", this.InfoCommandUser, "getinfouser", "giu"));
        }

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

        private void OnPlayerJoin(GreetPlayerEventArgs args)
        {
            var player = TShock.Players[args.Who];
            if (player != null)
            {
                string playerIP = player.IP;

                if (!firstLoginTimes.ContainsKey(args.Who))
                {
                    // Registrar la primera vez que el jugador se conecta
                    firstLoginTimes[args.Who] = DateTime.UtcNow;
                    SaveFirstLoginTimes();

                    player.SendInfoMessage($"[ + ][ PlayerGetInfo ] by{Author} v{Version} please follow in Youtube Discord etc");
                    player.SendInfoMessage($"[ + ] - Your playtime since registration: [ 00:00 ].");
                    player.SendInfoMessage($"[ + ] - Your ip {playerIP} ");
                }
                else
                {
                    var firstLoginTime = firstLoginTimes[args.Who];
                    var playTime = DateTime.UtcNow - firstLoginTime;

                    string formattedPlayTime = $"{(int)playTime.TotalHours:D2}:{playTime.Minutes:D2}:{playTime.Seconds:D2}";
                    player.SendInfoMessage($"[ + ][ PlayerGetInfo ] by{Author} v{Version} please follow in Youtube Discord etc");
                    player.SendInfoMessage($"[ + ] - Your playtime since registration: [ {formattedPlayTime} ].");
                    player.SendInfoMessage($"[ + ] - Your ip {playerIP} ");
                }
            }
        }

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

                        string playerIP = targetPlayer.IP;
                        string playerGroup = targetPlayer.Group.Name;
                        var platform = InfoTool.GetPlatform(targetPlayer);

                        if (firstLoginTimes.TryGetValue(targetPlayer.Index, out DateTime firstLoginTime))
                        {
                            var playTime = DateTime.UtcNow - firstLoginTime;
                            string formattedPlayTime = $"{(int)playTime.TotalHours:D2}:{playTime.Minutes:D2}:{playTime.Seconds:D2}";
                            args.Player.SendInfoMessage($"[ InfoPlayer ] `{targetPlayer.Name}` From: [ {InfoTool.GetPlatform(targetPlayer)} ] PlayTime since first registration: [ {formattedPlayTime} ]");
                        }
                        else
                        {
                            args.Player.SendInfoMessage($"[ InfoPlayer ] `{targetPlayer.Name}` PlayTime: [ Unknown ]");
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
            private void InfoCommand(CommandArgs args)
        {

            if (args.Parameters.Count == 1)
            {
                var plys = TSPlayer.FindByNameOrID(args.Parameters[0]);
                if (plys.Count > 0)
                {
                    var targetPlayer = plys[0];
                    string playerIP = targetPlayer.IP;
                    string playerGroup = targetPlayer.Group.Name;
                    var platform = InfoTool.GetPlatform(targetPlayer);

                    if (firstLoginTimes.TryGetValue(targetPlayer.Index, out DateTime firstLoginTime))
                    {
                        var playTime = DateTime.UtcNow - firstLoginTime;
                        string formattedPlayTime = $"{(int)playTime.TotalHours:D2}:{playTime.Minutes:D2}:{playTime.Seconds:D2}";
                        args.Player.SendInfoMessage($"[ InfoPlayer ] `{targetPlayer.Name}` From: [ {InfoTool.GetPlatform(targetPlayer)} ] PlayTime [ {formattedPlayTime} ] IP: [ {playerIP} ]");
                    }
                    else
                    {
                        args.Player.SendInfoMessage($"[ InfoPlayer ] `{targetPlayer.Name}` PlayTime: [ Unknown ]");
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

        private void OnGreet(GreetPlayerEventArgs args)
        {
            var player = TShock.Players[args.Who];
            if (player == null)
            {
                return;
            }

            string playerIP = player.IP;
            loginTimes[args.Who] = DateTime.UtcNow; // Registrar tiempo de inicio

            TShock.Log.ConsoleInfo($"[ PlayerGetInfo ] Player {player.Name} joined from: [ {Platforms[args.Who]} ]. IP: [ {playerIP} ]");
        }

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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SaveFirstLoginTimes(); // Guardar antes de desactivar
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnPlayerJoin);
                On.OTAPI.Hooks.MessageBuffer.InvokeGetData -= this.OnGetData;
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreet);
                Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.InfoCommand);
            }
            base.Dispose(disposing);
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