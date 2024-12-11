# PlayerGetInfo Plugin README

> [!NOTE]
> SI HABLAS OTRO IDIOMA PORFAVOR VE A [README_ES](README_ES.md) PARA LEER ESTO IDIOMA ESPAÑOL

> [!WARNING]
> You must have `"EnableGeoIp": true,` in `home/container/tshock/Config.json`

## Overview
The **PlayerGetInfo** plugin for TShock provides server administrators with detailed information about players when they join the server or request information about other players. This includes the platform they are playing on, their IP address, and their total playtime since registration. The plugin helps enhance server management by providing quick access to player-specific data.

### EXAMPLE: Join Message
```PowerShell
###################################################
#          J O I N E D - P L A Y E R              #
###################################################
[ InfoPlayer ] PLAYER: FrankV22MVS ]
[ + ] - PlayTime [ 03:40:32 ]
[ + ] - DEVICE: [ PC ]
[ + ] - IP: [ 0.0.0.0 ]
[ + ] - COUNTRY: [ CSharpLand ]
[ + ] - TEAM: [ 0 ]
[ + ] - GROUP: [ guest ]
[ + ] - SELECTED ITEM: [ [i:3827]
[ + ] - LIFE: [ 100 ]
[ + ] - MANA: [ 20 ]
[ + ] - CURRENT LIFE: [ 100]
[ + ] - CURRENT MANA: [ 20 ]
[ + ] - TILES CREATED: [ 0 ]
[ + ] - TILES DESTROYED: [ 0 ]
###################################################
```

## Features
- Tracks the first login time of each player.
- Shows a player's IP address and playtime since registration upon joining.
- Allows administrators to query information about other players by name.
- Supports various gaming platforms (PC, Xbox, PSN, etc.) and identifies them.
- Saves player data in a JSON file (`firstLoginTimes.json`) for persistence.

## Commands and Permissions

| Command                | Permission        | Description                              |
|------------------------|-------------------|------------------------------------------|
| `/getinfo <Player>` `/gi <Player>`    | `getinfo.admin`   | Displays detailed information about a player. |
| `/getinfouser <Player>` `/giu <Player>`| `getinfo.user`    | Displays a user's playtime and platform info. |

## Command Details

### `/getinfo <Player>`
- **Permission Required**: `getinfo.admin`
- **Usage**: `/getinfo <Player>`
- **Description**: Shows detailed information about a specified player, including their platform, total playtime, and IP address.

### `/getinfouser <Player>`
- **Permission Required**: `getinfo.user`
- **Usage**: `/getinfouser <Player>`
- **Description**: Displays basic information about a player, such as their platform and playtime since first registration.

## Installation and Setup
1. Place the compiled plugin `.dll` file in the `tshock/plugins` directory.
2. Restart or reload the server to load the plugin.
3. Ensure that the server configuration allows the permissions for `getinfo.admin` and `getinfo.user` to be assigned to the appropriate user groups.

## Configuration
The plugin saves data in a file named `firstLoginTimes.json` located in the `tshock/` directory. This file is used to store the first login time of players for tracking playtime.

### File Format
- The JSON file is formatted as a dictionary where each entry corresponds to a player’s ID and their first login time.

## How It Works
- **On Player Join**: The plugin logs the player's IP address and records their first login time if they are joining for the first time. It then sends informational messages to the player with their playtime and IP address.
- **On Command Execution**: The `/getinfo` and `/getinfouser` commands allow administrators to check the information of specific players.
- **Platform Identification**: The plugin identifies the platform a player is using based on data received from the game client.

## Dependencies
- **TShock API**: This plugin uses TShock version 2.1 or newer for integration.
- **Newtonsoft.Json**: Used for handling JSON serialization and deserialization.

## Credits
- **Author**: FrankV22
- **Version**: 1.0.0

## License
This project is licensed under the [MIT License](LICENSE).
