[![Build Status](https://travis-ci.org/Gaulomatic/Logixware.SoftEther.Client.svg?branch=master)](https://travis-ci.org/Gaulomatic/Logixware.SoftEther.Client)

# Logixware SoftEther Client

A daemon for SoftEther VPN client.

Since SoftEther is lacking a daemon for macOS and Linux, this is a wrapper around the client provided by SoftEther.
It starts the VPN client, handles IP address, static route management and `vpnclient` service & connection failures. 

## Features

- Implemented in C# on .NET Core 2.1
- Background service for macOS (10.13 or higher)
- Background daemon for Linux (Ubuntu 18.04 or higher)
- Automatic IP address assignment on virtual TAP devices
- Automatic assignment and release of static routes associated with a VPN connection
- Automatic restart of the `vpnclient` service in cases of failures
- Configuration via .NET Core `appsettings.json` file

## Installation

### Prerequisites

#### macOS

- `tuntaposx` is required. You can download it [here](http://tuntaposx.sourceforge.net)

### Installer

- There is a `.pkg` package [available](https://github.com/Gaulomatic/Logixware.SoftEther.Client/releases) for installation on macOS.

### Manual installation on macOS

 1. Compile the daemon by executing `./publish.sh` in the folder `src/Logixware.SoftEther.Client.Daemon`. 
 2. Copy the output from `src/Logixware.SoftEther.Client.Daemon/bin/Release/netcoreapp2.1/osx.10.13-x64/publish` to
 `/Library/Application Support/Logixware/Logixware.SoftEther.Client.Daemon`
 3. Copy `src/Logixware.SoftEther.Client.Daemon/de.logixware.SoftEther.Client.Daemon.plist` to `/Library/LaunchDaemons`
 4. Copy `src/Logixware.SoftEther.Client.Daemon/appsettings.example.macos.json` to
 `/Library/Preferences/Logixware/Logixware.SoftEther.Client.Daemon` and rename to `appsettings.json`
 5. Download the official client from the [website](http://www.softether.org/5-download)
 6. Compile it and copy the output to `/Library/Application Support/Softether/vpnclient`
 
 ### Installation on Linux
 
  1. Compile the daemon by executing `./publish.sh` in the folder `src/Logixware.SoftEther.Client.Daemon`. 
  2. Copy the output from `src/Logixware.SoftEther.Client.Daemon/bin/Release/netcoreapp2.1/ubuntu.18.04-x64/publish` to
  `/opt/logixware/Logixware.SoftEther.Client.Daemon`
  3. Copy `src/Logixware.SoftEther.Client.Daemon/vpnclient.service` to `/lib/systemd/system`
  4. Copy `src/Logixware.SoftEther.Client.Daemon/appsettings.example.linux.json` to
  `/etc/logixware/Logixware.SoftEther.Client.Daemon` and rename to `appsettings.json`
  5. Download the official client from the [website](http://www.softether.org/5-download)
  6. Compile it and copy the output to `/opt/softether/vpnclient`

## Configuration

Open the config file in your favorite text editor and add configurations. The network name must match the connection
name in the `vpnclient` service.

- The location on macOS is `/Library/Preferences/Logixware/Logixware.SoftEther.Client.Daemon/appsettings.json`
- The location on Linux is `/etc/logixware/Logixware.SoftEther.Client.Daemon/appsettings.json`

### Sample configuration:

```json5
    {
        "Logging":
        {
            "LogLevel":
            {
                "Default": "Information"
            }
        },
        "AllowedHosts": "*",
        "VPN":
        {
            "CommandLineInterface":
            {
                "PathToClient": "/Library/Application Support/SoftEther/vpnclient/vpnclient",
                "PathToCli": "/Library/Application Support/SoftEther/vpnclient/vpncmd",
                "CliPassword": ""
            },
    
            "Platform":
            {
                "Mac":
                {
                    "TapKextIdentifier": "net.sf.tuntaposx.tap",
                    "PathToTapKext": "/Library/Extensions/tap.kext"
                }
            },
    
            "ConnectionAttemptsBeforeClientRestart": 5,
    
            "Networks":
            [
                {
                    "Name": "logixware",
                    "ConnectionTestHost": "192.168.0.254",
                    "AlwaysOn": true,
                    "IPv4":
                    {
                        "Address": "192.168.0.1",
                        "Mask": "255.255.255.0",
                        "Routes":
                        [
                            {
                                "Network": "192.168.1.0",
                                "Prefix": "24",
                                "Gateway": "192.168.0.100"
                            }
                        ]
    
                    },
                    "IPv6":
                    {
                        "Address": "fdd3:82c4:f610:c08f:fdd3:82c4:f610:1",
                        "Prefix": 64
                    }
                }
            ]
        }
    }
```

## Credits

- Icons made by [Roundicons](https://www.flaticon.com/authors/roundicons). [Roundicons](https://www.flaticon.com/authors/roundicons) from [Flaticon](https://www.flaticon.com/) is licensed by [Creative Commons BY 3.0](http://creativecommons.org/licenses/by/3.0/)


__Please feel free to download, fork and/or provide any feedback!__

