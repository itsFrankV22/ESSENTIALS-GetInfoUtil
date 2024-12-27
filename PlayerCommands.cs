using TShockAPI;
using Newtonsoft.Json;
using System.IO;

namespace InfoPlayers
{
    public partial class PlayerJoinInfo
    {
        private Dictionary<string, PlayerData> _playerData = PlayerData.LoadData(); // Cargar los datos desde el archivo JSON

        private void InfoCommandUser(CommandArgs args)
        {
            if (args.Parameters.Count == 1)
            {
                var players = TSPlayer.FindByNameOrID(args.Parameters[0]);

                if (players.Count > 0)
                {
                    TSPlayer targetPlayer = players[0];

                    if (targetPlayer == null || !targetPlayer.Active)
                    {
                        args.Player.SendErrorMessage("The player is not online!");
                        return;
                    }

                    ShowPlayerInfo(targetPlayer, args.Player, true);
                }
                else
                {
                    // Si el jugador no está en línea, buscar en playerdata.json
                    string playerName = args.Parameters[0];
                    if (_playerData.ContainsKey(playerName))
                    {
                        PlayerData playerData = _playerData[playerName];
                        ShowPlayerInfo(playerData, args.Player, true);
                    }
                    else
                    {
                        args.Player.SendErrorMessage("Player data not found!");
                    }
                }
            }
            else
            {
                args.Player.SendErrorMessage("[ - ] Syntax error.");
                args.Player.SendInfoMessage("/getinfouser <Player name>");
                args.Player.SendInfoMessage("/giu <Player name>");
            }
        }

        private void InfoCommand(CommandArgs args)
        {
            if (args.Parameters.Count == 1)
            {
                var players = TSPlayer.FindByNameOrID(args.Parameters[0]);

                if (players.Count > 0)
                {
                    TSPlayer targetPlayer = players[0];

                    if (targetPlayer == null || !targetPlayer.Active)
                    {
                        args.Player.SendErrorMessage("The player is not online!");
                        return;
                    }

                    ShowPlayerInfo(targetPlayer, args.Player, false);
                }
                else
                {
                    // Si el jugador no está en línea, buscar en playerdata.json
                    string playerName = args.Parameters[0];
                    if (_playerData.ContainsKey(playerName))
                    {
                        PlayerData playerData = _playerData[playerName];
                        ShowPlayerInfo(playerData, args.Player, false);
                    }
                    else
                    {
                        args.Player.SendErrorMessage("Player data not found!");
                    }
                }
            }
            else
            {
                args.Player.SendErrorMessage("[ - ] Syntax error.");
                args.Player.SendInfoMessage("/getinfo <Player name>");
                args.Player.SendInfoMessage("/gi <Player name>");
            }
        }

        private void ShowPlayerInfo(TSPlayer targetPlayer, TSPlayer requestingPlayer, bool isUserInfo)
        {
            string playerCountry = targetPlayer.Country ?? "Unknown";
            string playerSelectedItem = targetPlayer.SelectedItem?.netID.ToString() ?? "None";
            string name = targetPlayer.Name;
            int maxHp = targetPlayer.TPlayer.statLifeMax2;
            int currentHp = targetPlayer.TPlayer.statLife;
            int maxMana = targetPlayer.TPlayer.statManaMax2;
            int currentMana = targetPlayer.TPlayer.statMana;
            int defense = targetPlayer.TPlayer.statDefense;
            var platform = InfoTool.GetPlatform(targetPlayer);
            string playerGroup = targetPlayer.Group.Name;
            string playerGroupPrefix = targetPlayer.Group.Prefix;
            string playerGroupSuffix = targetPlayer.Group.Suffix;

            if (isUserInfo)
            {
                requestingPlayer.SendInfoMessage("");
                requestingPlayer.SendInfoMessage("         U S E R - I N F O       ");
                requestingPlayer.SendInfoMessage($"  [ InfoPlayer ] PLAYER: [ {name} ]");
                requestingPlayer.SendInfoMessage($"  [ + ] - DEVICE: [ {platform} ]");
                requestingPlayer.SendInfoMessage($"  [ + ] - COUNTRY: [ {playerCountry} ]");
                requestingPlayer.SendInfoMessage($"  [ + ] - SELECTED ITEM: [ [i:{playerSelectedItem}] ]");
                requestingPlayer.SendInfoMessage("             S T A T S");
                requestingPlayer.SendInfoMessage($"  [i:29] [c/FF6666:LIFE:] [ {currentHp}/{maxHp} ]");
                requestingPlayer.SendInfoMessage($"  [i:109] [c/00E5FF:MANA:] [ {currentMana}/{maxMana} ]");
                requestingPlayer.SendInfoMessage($"  [i:938] [c/66B2FF:DEFENSE:] [ {defense} ]");
                requestingPlayer.SendInfoMessage($"");
            }
            else
            {
                requestingPlayer.SendInfoMessage("");
                requestingPlayer.SendInfoMessage("       I N F O - P L A Y E R         ");
                requestingPlayer.SendInfoMessage($"  [ InfoPlayer ] PLAYER: [ {name} ]");
                requestingPlayer.SendInfoMessage($"  [ + ] - DEVICE: [ {platform} ]");
                requestingPlayer.SendInfoMessage($"  [ + ] - IP: [ {targetPlayer.IP} ]");
                requestingPlayer.SendInfoMessage($"  [ + ] - UUID: [ {targetPlayer.UUID} ]");
                requestingPlayer.SendInfoMessage($"  [ + ] - COUNTRY: [ {playerCountry} ]");
                requestingPlayer.SendInfoMessage($"  [ + ] - TEAM: [ {targetPlayer.Team} ]");
                requestingPlayer.SendInfoMessage($"  [ + ] - S&P: [ {playerGroupPrefix}{playerGroupSuffix} ]");
                requestingPlayer.SendInfoMessage($"  [ + ] - GROUP: [ {targetPlayer.Group} ]");
                requestingPlayer.SendInfoMessage($"  [ + ] - SELECTED ITEM: [ [i:{playerSelectedItem}] ]");
                requestingPlayer.SendInfoMessage("             S T A T S");
                requestingPlayer.SendInfoMessage($"  [c/FF6666:LIFE:] [ Current: {currentHp} / Max: {maxHp} ]");
                requestingPlayer.SendInfoMessage($"  [c/00E5FF:MANA:] [ Current: {currentMana} / Max: {maxMana} ]");
                requestingPlayer.SendInfoMessage($"  [c/66B2FF:DEFENSE:] [ {defense} ]");
                requestingPlayer.SendInfoMessage("");
            }
        }

        private void ShowPlayerInfo(PlayerData playerData, TSPlayer requestingPlayer, bool isUserInfo)
        {
            if (isUserInfo)
            {
                requestingPlayer.SendInfoMessage("");
                requestingPlayer.SendInfoMessage("         U S E R - I N F O       ");
                requestingPlayer.SendInfoMessage($"  [ InfoPlayer ] PLAYER: [ {playerData.PlayerName} ]");
                requestingPlayer.SendInfoMessage($"  [ + ] - DEVICE: [ {playerData.Platform} ]");
                requestingPlayer.SendInfoMessage($"  [ + ] - COUNTRY: [ {playerData.Country} ]");
                requestingPlayer.SendInfoMessage($"  [ + ] - SELECTED ITEM: [ [i:{playerData.SelectedItem}] ]");
                requestingPlayer.SendInfoMessage("             S T A T S");
                requestingPlayer.SendInfoMessage($"  [i:29] [c/FF6666:LIFE:] [ {playerData.CurrentHp}/{playerData.MaxHp} ]");
                requestingPlayer.SendInfoMessage($"  [i:109] [c/00E5FF:MANA:] [ {playerData.CurrentMana}/{playerData.MaxMana} ]");
                requestingPlayer.SendInfoMessage($"  [i:938] [c/66B2FF:DEFENSE:] [ {playerData.Defense} ]");
                requestingPlayer.SendInfoMessage("");
            }
            else
            {
                requestingPlayer.SendInfoMessage("");
                requestingPlayer.SendInfoMessage("       I N F O - P L A Y E R         ");
                requestingPlayer.SendInfoMessage($"  [ InfoPlayer ] PLAYER: [ {playerData.PlayerName} ]");
                requestingPlayer.SendInfoMessage($"  [ + ] - DEVICE: [ {playerData.Platform} ]");
                requestingPlayer.SendInfoMessage($"  [ + ] - COUNTRY: [ {playerData.Country} ]");
                requestingPlayer.SendInfoMessage($"  [ + ] - TEAM: [ {playerData.Team} ]");
                requestingPlayer.SendInfoMessage($"  [ + ] - S&P: [ {playerData.GroupPrefix}{playerData.GroupSuffix} ]");
                requestingPlayer.SendInfoMessage($"  [ + ] - GROUP: [ {playerData.Group} ] S&P: {playerData.GroupPrefix} {playerData.GroupSuffix})");
                requestingPlayer.SendInfoMessage($"  [ + ] - SELECTED ITEM: [ [i:{playerData.SelectedItem}] ]");
                requestingPlayer.SendInfoMessage("             S T A T S");
                requestingPlayer.SendInfoMessage($"  [c/FF6666:LIFE:] [ Current: {playerData.CurrentHp} / Max: {playerData.MaxHp} ]");
                requestingPlayer.SendInfoMessage($"  [c/00E5FF:MANA:] [ Current: {playerData.CurrentMana} / Max: {playerData.MaxMana} ]");
                requestingPlayer.SendInfoMessage($"  [c/66B2FF:DEFENSE:] [ {playerData.Defense} ]");
                requestingPlayer.SendInfoMessage("");
            }
        }
    }
}
