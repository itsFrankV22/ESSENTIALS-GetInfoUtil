# PlayerGetInfo

> [!NOTE]
> IF YOU SPEAK ANOTHER LANGUAGE, PLEASE GO TO [README_EN](README.md) TO READ THIS IN ENGLISH.

> [!WARNING]
> Debes tener `"EnableGeoIp": true,` en `home/container/tshock/Config.json`

## Descripci�n
El plugin **PlayerGetInfo** para TShock proporciona a los administradores del servidor informaci�n detallada sobre los jugadores cuando se unen al servidor o solicitan informaci�n sobre otros jugadores. Esto incluye la plataforma en la que est�n jugando, su direcci�n IP y su tiempo de juego total desde la primera vez que se registraron. El plugin ayuda a mejorar la gesti�n del servidor al proporcionar un acceso r�pido a datos espec�ficos de los jugadores.

### EJEMPLO: Join Message
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


## Caracter�sticas
- Rastrea la hora de primer inicio de sesi�n de cada jugador.
- Muestra la direcci�n IP y el tiempo de juego desde el registro al unirse al servidor.
- Permite a los administradores consultar informaci�n sobre otros jugadores por nombre.
- Soporta varias plataformas de juego (PC, Xbox, PSN, etc.) e identifica la plataforma.
- Guarda los datos de los jugadores en un archivo JSON (`firstLoginTimes.json`) para persistencia.

## Comandos y Permisos

| Comando                    | Permiso           | Descripci�n                                   |
|----------------------------|-------------------|-----------------------------------------------|
| `/getinfo <Jugador>` `/gi <Jugador>`      | `getinfo.admin`   | Muestra informaci�n detallada sobre un jugador. |
| `/getinfouser <Jugador>` `/giu <Jugador>` | `getinfo.user`    | Muestra el tiempo de juego y la informaci�n de la plataforma de un jugador. |

## Detalles de los Comandos

### `/getinfo <Jugador>`
- **Permiso Requerido**: `getinfo.admin`
- **Uso**: `/getinfo <Jugador>`
- **Descripci�n**: Muestra informaci�n detallada sobre un jugador especificado, incluyendo su plataforma, tiempo de juego total y direcci�n IP.

### `/getinfouser <Jugador>`
- **Permiso Requerido**: `getinfo.user`
- **Uso**: `/getinfouser <Jugador>`
- **Descripci�n**: Muestra informaci�n b�sica sobre un jugador, como su plataforma y tiempo de juego desde el primer registro.

## Instalaci�n y Configuraci�n
1. Coloca el archivo `.dll` del plugin compilado en el directorio `tshock/plugins`.
2. Reinicia o recarga el servidor para cargar el plugin.
3. Aseg�rate de que la configuraci�n del servidor permita asignar los permisos `getinfo.admin` y `getinfo.user` a los grupos de usuarios apropiados.

## Configuraci�n
El plugin guarda los datos en un archivo llamado `firstLoginTimes.json` ubicado en el directorio `tshock/`. Este archivo se usa para almacenar la hora de primer inicio de sesi�n de los jugadores para el seguimiento del tiempo de juego.

### Formato del Archivo
- El archivo JSON tiene el formato de un diccionario donde cada entrada corresponde al ID de un jugador y su hora de primer inicio de sesi�n.

## C�mo Funciona
- **Al Unirse un Jugador**: El plugin registra la direcci�n IP del jugador y guarda su hora de primer inicio de sesi�n si es la primera vez que se une. Luego, env�a mensajes informativos al jugador con su tiempo de juego y direcci�n IP.
- **Al Ejecutar Comandos**: Los comandos `/getinfo` y `/getinfouser` permiten a los administradores consultar la informaci�n de jugadores espec�ficos.
- **Identificaci�n de la Plataforma**: El plugin identifica la plataforma que utiliza un jugador seg�n los datos recibidos del cliente del juego.

## Dependencias
- **API de TShock**: Este plugin utiliza TShock versi�n 2.1 o superior para la integraci�n.
- **Newtonsoft.Json**: Se usa para la serializaci�n y deserializaci�n de JSON.

## Cr�ditos
- **Autor**: FrankV22
- **Versi�n**: 1.0.0

## Licencia
Este proyecto est� licenciado bajo la [Licencia MIT](LICENSE).
