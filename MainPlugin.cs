using NuGet.Protocol.Plugins;
using ReLogic.OS;
using System;
using System.Collections.Generic;
using System.Data;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.Hooks;

namespace InfoPlayers
{
    [ApiVersion(2, 1)]
    public partial class PlayerJoinInfo : TerrariaPlugin
    {
        public static PlayerJoinInfo Instance { get; private set; }
        public override string Author => "FrankV22";
        public override string Description => "Show Info and better command info";
        public override string Name => "ESSENTIALS GetInfoUtil";
        public override Version Version => new Version(1, 6, 0);

        public static IDbConnection Db { get; private set; }
        private Dictionary<int, int> playerKills = new();
        private Dictionary<int, DateTime> firstLoginTimes = new();
        private Dictionary<int, DateTime> loginTimes = new();
        public static PlatformType[] Platforms { get; set; } = new PlatformType[256];
        

        public PlayerJoinInfo(Main game) : base(game)
        {
            base.Order = int.MinValue;
            Instance = this;
        }

        public override void Initialize()
        {
            ConfigManager.Initialize(); // Inicializa la configuración
            ConfigManager.Instance.LoadConfig();


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

                ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnPlayerJoin);
                ServerApi.Hooks.GamePostInitialize.Deregister(this, OnPostInitialize);
                On.OTAPI.Hooks.MessageBuffer.InvokeGetData -= this.OnGetData;
                ServerApi.Hooks.NetGreetPlayer.Deregister(this, this.OnGreet);
                GeneralHooks.ReloadEvent -= OnReload;
                Commands.ChatCommands.RemoveAll(x => x.CommandDelegate == this.InfoCommand);
            }
            base.Dispose(disposing);
        }

        private void OnReload(ReloadEventArgs reloadEventArgs)
        {
            ConfigManager.Instance.LoadConfig();
            TShock.Log.ConsoleInfo("[ GetInfoUtil ] Reloaded!");

        }
        private async void OnPostInitialize(EventArgs e)
        {
            await CheckForPluginUpdates();
        }

        public async Task CheckForPluginUpdates()
        {
            await CheckUpdates.CheckUpdates.CheckUpdateVerbose(this);
        }
    }
}
