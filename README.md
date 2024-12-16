# PlayerGetInfo Plugin README

> [!NOTE]
> SI HABLAS OTRO IDIOMA PORFAVOR VE A [README_ES](README_ES.md) PARA LEER ESTO IDIOMA ESPAÑOL

> [!WARNING]
> You must have `"EnableGeoIp": true,` in `home/container/tshock/Config.json`

> When installing this update from 1.3.0 you must manually delete the plugin called `PlayerGetInfo`

## Overview
The **PlayerGetInfo** plugin for TShock provides server administrators with detailed information about players when they join the server or request information about other players. This includes the platform they are playing on, their IP address, and their total playtime since registration. The plugin helps enhance server management by providing quick access to player-specific data.



![image](https://github.com/user-attachments/assets/06f8bfed-2e9a-44be-a3a0-fa22f1d1cbc4) ![image](https://github.com/user-attachments/assets/2134aaef-6e9b-43ae-ad9e-c989e03281c6)

![image](https://github.com/user-attachments/assets/d3904363-9602-4592-ab1b-0e90b1d4c367)

![image](https://github.com/user-attachments/assets/2c811e60-dc0d-4de9-9c21-4a8ca29b561f)

![image](https://github.com/user-attachments/assets/eec21800-9d8a-4f03-887b-3b3509f0cb06)

![image](https://github.com/user-attachments/assets/bea21da4-2036-4638-a2b2-8888229cab8d)


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
- **Version**: 1.4.0

## License
This project is licensed under the [MIT License](LICENSE).
