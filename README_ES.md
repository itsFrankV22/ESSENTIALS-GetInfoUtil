# PlayerGetInfo

> [!NOTE]
> IF YOU SPEAK ANOTHER LANGUAGE, PLEASE GO TO [README_EN](README.md) TO READ THIS IN ENGLISH.

> [!WARNING]
> Debes tener `"EnableGeoIp": true,` en `home/container/tshock/Config.json`

## Descripción
El plugin **PlayerGetInfo** para TShock proporciona a los administradores del servidor información detallada sobre los jugadores cuando se unen al servidor o solicitan información sobre otros jugadores. Esto incluye la plataforma en la que están jugando, su dirección IP y su tiempo de juego total desde la primera vez que se registraron. El plugin ayuda a mejorar la gestión del servidor al proporcionar un acceso rápido a datos específicos de los jugadores.

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


## Características
- Rastrea la hora de primer inicio de sesión de cada jugador.
- Muestra la dirección IP y el tiempo de juego desde el registro al unirse al servidor.
- Permite a los administradores consultar información sobre otros jugadores por nombre.
- Soporta varias plataformas de juego (PC, Xbox, PSN, etc.) e identifica la plataforma.
- Guarda los datos de los jugadores en un archivo JSON (`firstLoginTimes.json`) para persistencia.

## Comandos y Permisos

| Comando                    | Permiso           | Descripción                                   |
|----------------------------|-------------------|-----------------------------------------------|
| `/getinfo <Jugador>` `/gi <Jugador>`      | `getinfo.admin`   | Muestra información detallada sobre un jugador. |
| `/getinfouser <Jugador>` `/giu <Jugador>` | `getinfo.user`    | Muestra el tiempo de juego y la información de la plataforma de un jugador. |

## Detalles de los Comandos

### `/getinfo <Jugador>`
- **Permiso Requerido**: `getinfo.admin`
- **Uso**: `/getinfo <Jugador>`
- **Descripción**: Muestra información detallada sobre un jugador especificado, incluyendo su plataforma, tiempo de juego total y dirección IP.

### `/getinfouser <Jugador>`
- **Permiso Requerido**: `getinfo.user`
- **Uso**: `/getinfouser <Jugador>`
- **Descripción**: Muestra información básica sobre un jugador, como su plataforma y tiempo de juego desde el primer registro.

## Instalación y Configuración
1. Coloca el archivo `.dll` del plugin compilado en el directorio `tshock/plugins`.
2. Reinicia o recarga el servidor para cargar el plugin.
3. Asegúrate de que la configuración del servidor permita asignar los permisos `getinfo.admin` y `getinfo.user` a los grupos de usuarios apropiados.

## Configuración
El plugin guarda los datos en un archivo llamado `firstLoginTimes.json` ubicado en el directorio `tshock/`. Este archivo se usa para almacenar la hora de primer inicio de sesión de los jugadores para el seguimiento del tiempo de juego.

### Formato del Archivo
- El archivo JSON tiene el formato de un diccionario donde cada entrada corresponde al ID de un jugador y su hora de primer inicio de sesión.

## Cómo Funciona
- **Al Unirse un Jugador**: El plugin registra la dirección IP del jugador y guarda su hora de primer inicio de sesión si es la primera vez que se une. Luego, envía mensajes informativos al jugador con su tiempo de juego y dirección IP.
- **Al Ejecutar Comandos**: Los comandos `/getinfo` y `/getinfouser` permiten a los administradores consultar la información de jugadores específicos.
- **Identificación de la Plataforma**: El plugin identifica la plataforma que utiliza un jugador según los datos recibidos del cliente del juego.

## Dependencias
- **API de TShock**: Este plugin utiliza TShock versión 2.1 o superior para la integración.
- **Newtonsoft.Json**: Se usa para la serialización y deserialización de JSON.

## Créditos
- **Autor**: FrankV22
- **Versión**: 1.0.0

## Licencia
Este proyecto está licenciado bajo la [Licencia MIT](LICENSE).
