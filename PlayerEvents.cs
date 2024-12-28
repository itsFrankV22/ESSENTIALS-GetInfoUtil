using System;
using System.Threading.Tasks;
using TerrariaApi.Server;
using TShockAPI;
using Terraria;
using TShockAPI.Hooks;
using IL.Terraria.Utilities;
using System.Net.Sockets;
using System.Net;
using System.Threading.Channels;


namespace InfoPlayers
{
    public partial class PlayerJoinInfo
    {
        private ConfigManager _configManager;


        private PingData[] PlayerPing { get; set; }
        public class PingData
        {
            public TimeSpan? LastPing;
            internal PingDetails?[] RecentPings = new PingDetails?[Terraria.Main.item.Length];
        }
        internal class PingDetails
        {
            internal Channel<int>? Channel;
            internal DateTime Start = DateTime.Now;
            internal DateTime? End = null;
        }

        public async Task<TimeSpan> Ping(TSPlayer player)
        {
            return await Ping(player, new CancellationTokenSource(1000).Token);
        }

        public async Task<TimeSpan> Ping(TSPlayer player, CancellationToken token)
        {
            var pingdata = PlayerPing[player.Index];
            if (pingdata == null) return TimeSpan.MaxValue;

            var inv = -1;
            for (var i = 0; i < Terraria.Main.item.Length; i++)
                if (Terraria.Main.item[i] != null)
                    if (!Terraria.Main.item[i].active || Terraria.Main.item[i].playerIndexTheItemIsReservedFor == 255)
                    {
                        if (pingdata.RecentPings[i]?.Channel == null)
                        {
                            inv = i;
                            break;
                        }
                    }

            if (inv == -1) return TimeSpan.MaxValue;

            var pd = pingdata.RecentPings[inv] ??= new PingDetails();

            pd.Channel ??= Channel.CreateBounded<int>(new BoundedChannelOptions(30)
            {
                SingleReader = true,
                SingleWriter = true
            });


            Terraria.NetMessage.TrySendData((int)PacketTypes.RemoveItemOwner, player.Index, -1, null, inv);

            await pd.Channel.Reader.ReadAsync(token);
            pd.Channel = null;

            return (pingdata.LastPing = pd.End!.Value - pd.Start).Value;
        }

        private void Hook_Ping_GetData(GetDataEventArgs args)
        {
            if (args.Handled) return;
            if (args.MsgID != PacketTypes.ItemOwner) return;
            var user = TShock.Players[args.Msg.whoAmI];
            if (user == null) return;
            using (BinaryReader date = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
            {
                int iid = date.ReadInt16();
                int pid = date.ReadByte();
                if (pid != 255) return;
                var pingresponse = PlayerPing[args.Msg.whoAmI];
                var ping = pingresponse?.RecentPings[iid];
                if (ping != null)
                {
                    ping.End = DateTime.Now;
                    ping.Channel!.Writer.TryWrite(iid);
                }
            }
        }
        private string GetLocalIPAddress()
        {
            foreach (var ip in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }





        private void OnPlayerJoin(GreetPlayerEventArgs args)
        {

            var player = TShock.Players[args.Who];
            if (player != null)
            {
                ServerApi.Hooks.NetGreetPlayer.Register(this, this.OnGreet);

                player.SilentJoinInProgress = true; // Evitar mensaje predeterminado

                var playerDataDictionary = PlayerData.LoadData();

                // Verificar si ya existe el jugador en los datos cargados
                if (!playerDataDictionary.ContainsKey(player.Name))
                {
                    // Si no existe, crear y agregar una nueva entrada
                    var playerData = PlayerData.CreateFromPlayer(player);
                    playerDataDictionary.Add(player.Name, playerData);
                    PlayerData.SaveData(playerDataDictionary); // Guardar los nuevos datos
                }
                else
                {
                    // Si ya existe, actualizar los datos (puedes modificar lo que desees)
                    var playerData = playerDataDictionary[player.Name];
                    playerData.CurrentHp = player.TPlayer.statLife;
                    playerData.CurrentMana = player.TPlayer.statMana;
                    playerData.SelectedItem = player.SelectedItem?.netID.ToString() ?? "None";
                    PlayerData.SaveData(playerDataDictionary); // Guardar los datos actualizados
                }

                if (ConfigManager.Instance.ShowStatsOnPlayerJoin)
                {

                    Task.Delay(1000).ContinueWith(_ =>
                    {
                        string name = player.Name;
                        int maxHp = player.TPlayer.statLifeMax2;  // Vida máxima
                        int currentHp = player.TPlayer.statLife; // Vida actual
                        int maxMana = player.TPlayer.statManaMax2; // Maná máximo
                        int currentMana = player.TPlayer.statMana; // Maná actual
                        int defense = player.TPlayer.statDefense; // Defensa actual

                        string playerCountry = player.Country ?? "Unknown";

                        player.SendInfoMessage($"[ GetInfoUtil ] by {Author} v{Version} please follow on YouTube and Discord");
                        player.SendInfoMessage("-  Y O U R  -  S T A T S -");
                        player.SendInfoMessage($"  [i:29] [c/FF6666:LIFE:] [ {currentHp}/{maxHp} ]");
                        player.SendInfoMessage($"  [i:109] [c/00E5FF:MANA:] [ {currentMana}/{maxMana} ]");
                        player.SendInfoMessage($"  [i:938] [c/66B2FF:DEFENSE:] [ {defense} ]");
                        player.SendInfoMessage($"  [i:4080] [c/FFCC66:COUNTRY:] [ {playerCountry} ]");
                    });
                }



                if (ConfigManager.Instance.ShowJoinCustomMessage)
                {
                    string customMessage = ConfigManager.Instance.JoinCustomMessage
                        .Replace("{difficulty}", diff[player.Difficulty])
                        .Replace("{playerName}", player.Name)
                        .Replace("{country}", player.Country ?? "Unknown")
                        .Replace("{platform}", InfoTool.GetPlatform(player));

                    var messageColor = new Microsoft.Xna.Framework.Color(255, 255, 255);
                    TSPlayer.All.SendMessage(customMessage, messageColor);
                }

            }
        }

        private HashSet<int> greetedPlayers = new HashSet<int>();

        private void OnGreet(GreetPlayerEventArgs args)
        {
            var player = TShock.Players[args.Who];
            if (player == null || greetedPlayers.Contains(args.Who))
            {
                return;
            }

            greetedPlayers.Add(args.Who);

            if (ConfigManager.Instance.ShowJoinMessageInConsole)
            {

                TShock.Log.ConsoleInfo("╔═══════════════════════════════════════════════╗");
                TShock.Log.ConsoleInfo("║             J O I N E D - P L A Y E R         ║");
                TShock.Log.ConsoleInfo("╚═══════════════════════════════════════════════╝");
                TShock.Log.ConsoleInfo($"  [InfoPlayer] PLAYER: [ {player.Name} ]");
                TShock.Log.ConsoleInfo($"  [ + ] - DEVICE: [ {Platforms[args.Who]} ]");
                TShock.Log.ConsoleInfo($"  [ + ] - IP: [ {player.IP} ]");
                TShock.Log.ConsoleInfo($"  [ + ] - COUNTRY: [ {player.Country} ]");
                TShock.Log.ConsoleInfo($"  [ + ] - TEAM: [ {player.Team} ]");
                TShock.Log.ConsoleInfo($"  [ + ] - GROUP: [ {player.Group.Name} ]");
                TShock.Log.ConsoleInfo($"  [ + ] - SELECTED ITEM: [ [i:{player.SelectedItem?.netID}] ]");
                TShock.Log.ConsoleInfo($"  [Stats] LIFE: {player.TPlayer.statLife}/{player.TPlayer.statLifeMax2}");
                TShock.Log.ConsoleInfo($"  [Stats] MANA: {player.TPlayer.statMana}/{player.TPlayer.statManaMax2}");
                TShock.Log.ConsoleInfo($"  [Stats] DEFE: {player.TPlayer.statDefense}");
                TShock.Log.ConsoleInfo("╚═══════════════════════════════════════════════╝");
            }
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



        public enum PlatformType : byte
        {
            MOBILE = 0,
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