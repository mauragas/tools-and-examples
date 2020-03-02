# Add blocking hosts file builder

## Description

Console application designed for Debian based Linux operating systems. Application downloads 3 default hosts [files](https://github.com/AdAway/AdAway/wiki/HostsSources), takes local hosts file and combines  names with IP addresses. Downloaded file IP addresses automatically changed to `0.0.0.0` by default.  All hosts files are sorted by name and added to file one per line.

## Dependencies

To be able to build or to run executable file you need to install [dotnet](https://dotnet.microsoft.com/download) 3.1 runtime.

## Installation

Download and install `AdAwayHosts.1.0.0.deb` executable with command `sudo dpkg -i AdAwayHosts.1.0.0.deb` from release list for Debian based Linux OS or run project file `AdAwayHost.Console.csproj` with command `dotnet run`.

Remove application with command (two methods):

```bash
sudo apt remove AdAwayHosts
sudo dpkg -r AdAwayHosts.1.0.0.deb
```

## Configuration

Application contains `appsettings.json` file in `AdAwayHost.Console` project folder or after installation in `/usr/share/AdAwayHosts/appsettings.json`.

| Name            | Description                                                                                                                                                            |
| --------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| LogFilePath     | Path to log file. Disabled by default.                                                                                                                                 |
| IpAddress       | IP address is changed only for new downloaded hosts (e.g by default IP addresses will be changed from `127.0.0.1` to `0.0.0.0`). Leave empty if do not want to change. |
| HostsFilePath   | Local path to `hosts` file on workstation. Leave empty if you want program detect it automatically.                                                                    |
| HostsSourceUrls | URL list of remote hosts files. By default 3 hosts [files](https://github.com/AdAway/AdAway/wiki/HostsSources) are taken.                                              |

### Command line arguments

All configuration variables can be overwritten by command line arguments:

**LogFilePath** (`-l`, `--LogFilePath`)

**IpAddress** (`-i`, `--IpAddress`)

**HostsFilePath** (`-f`, `--HostsFilePath`)

**HostsSourceUrls** (`-u`, `--HostsSourceUrls`)

## Examples

Update local host file `/etc/hosts` with default configurations:

```bash
sudo AdAwayHosts
sudo dotnet run
```

Generate hosts file in home folder `~/hosts`, with IP address `127.0.0.1` , logs file in `~/logs` and using specific hosts source:

```bash
AdAwayHosts -f ~/hosts -i 127.0.0.1 -l ~/logs -u http://sysctl.org/cameleon/hosts
```

**NOTE:** `sudo` is not needed, but you will need manually copy to `/etc/hosts`

If you running application from source code using `dotnet run`, use long arguments:

```bash
dotnet run --HostsFilePath ~/hosts --IpAddress 127.0.0.1 --LogFilePath ~/logs --HostsSourceUrls http://sysctl.org/cameleon/hosts
```

## How to build and install

To build and deploy `.deb` file:

```bash
git clone https://github.com/mauragas/AdAwayHost.git
cd ./AdAwayHost/src/AdAwayHost.Console
dotnet build
dotnet tool install --global dotnet-deb
dotnet deb install
dotnet deb -c Release
```

Deb file should be generated here `./bin/Debug/netcoreapp3.1/AdAwayHosts.1.0.0.deb`.

How to create installation file for other Linux distributions can be found [here](https://github.com/qmfrederik/dotnet-packaging).

## System logs

Application automatically logs into syslog, therefore you can check logs with commands:

```bash
more -f /var/log/syslog | grep AdAwayHosts
journalctl --since="3 days ago" | grep AdAwayHosts
```
