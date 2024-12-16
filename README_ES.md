# PlayerGetInfo

> [!NOTE]
> IF YOU SPEAK ANOTHER LANGUAGE, PLEASE GO TO [README_EN](README.md) TO READ THIS IN ENGLISH.

> [!WARNING]
> Debes tener `"EnableGeoIp": true,` en `home/container/tshock/Config.json`

> Si vas a actualizar desde la 1.3.0 debes borrar manualmente el anterior plugin llamado `PlayerGetInfo`

## Descripción
El plugin **PlayerGetInfo** para TShock proporciona a los administradores del servidor información detallada sobre los jugadores cuando se unen al servidor o solicitan información sobre otros jugadores. Esto incluye la plataforma en la que están jugando, su dirección IP y su tiempo de juego total desde la primera vez que se registraron. El plugin ayuda a mejorar la gestión del servidor al proporcionar un acceso rápido a datos específicos de los jugadores.

![image](https://github.com/user-attachments/assets/06f8bfed-2e9a-44be-a3a0-fa22f1d1cbc4) ![image](https://github.com/user-attachments/assets/2134aaef-6e9b-43ae-ad9e-c989e03281c6)



![image](https://github.com/user-attachments/assets/bea21da4-2036-4638-a2b2-8888229cab8d) ![image](https://github.com/user-attachments/assets/eec21800-9d8a-4f03-887b-3b3509f0cb06)


![image](https://github.com/user-attachments/assets/2c811e60-dc0d-4de9-9c21-4a8ca29b561f)  ![image](https://github.com/user-attachments/assets/d3904363-9602-4592-ab1b-0e90b1d4c367)


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
