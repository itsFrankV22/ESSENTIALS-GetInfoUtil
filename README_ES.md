# PlayerGetInfo

> [!NOTE]
> IF YOU SPEAK ANOTHER LANGUAGE, PLEASE GO TO [README_EN](README.md) TO READ THIS IN ENGLISH.

> [!WARNING]
> Debes tener `"EnableGeoIp": true,` en `home/container/tshock/Config.json`

> Si vas a actualizar desde la 1.3.0 debes borrar manualmente el anterior plugin llamado `PlayerGetInfo`

## Descripción
El plugin **PlayerGetInfo** para TShock proporciona a los administradores del servidor información detallada sobre los jugadores cuando se unen al servidor o solicitan información sobre otros jugadores. Esto incluye la plataforma en la que están jugando, su dirección IP y su tiempo de juego total desde la primera vez que se registraron. El plugin ayuda a mejorar la gestión del servidor al proporcionar un acceso rápido a datos específicos de los jugadores.

**EXAMPLE WELCOME MESSAGE to ALL PLAYERS**

![image](https://github.com/user-attachments/assets/70c05245-c736-4da4-85b9-0e454a8d5b78)

**EXAMPLE JOIN MESSAGE CONSLE**

![image](https://github.com/user-attachments/assets/cfd9f394-a0f8-43e0-a7b4-23646529e00f)

**EXAMPLE UPDATED PLUGIN CONSOLE**

![image](https://github.com/user-attachments/assets/97d356e6-26a1-446b-a737-332fa655a03d)

**EXAMPLE OUTDATED PLUGIN CONSOLE**

![image](https://github.com/user-attachments/assets/c131aab7-68c7-4688-a7ee-9845afbf0919)


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
- **Versión**: 1.4.0

## Licencia
Este proyecto está licenciado bajo la [Licencia MIT](LICENSE).
