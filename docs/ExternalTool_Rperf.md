# Rperf

## Setup
To use Rperf as external tool you need a running Rperf Server and Rperf on your system. Setupt instructions can be found [here](https://github.com/stec-zcps/rperf). For detailed description of required arguments, flags and options see help text.

## Usage
```bash
dotnet run -- rperf [FLAGS] [OPTIONS] --serverIp <IP of Rperf server> --port <Port of Rperf server> --interface=<Network interface name> --time <Test duration> --msgs-per-sec <Messages per second> --msg-size <Messages size> --protocol <Protocol [tcp|udp]> --transmissionTechnology <Transmission technology> --comment <Comment>

[FLAGS]
--tap
--owl
--sym-load

[OPTIONS]
--captureInterfaceName <Name of network interface>
--captureInterfaceFriendlyName <Friendly name of network interface>
--cpu-affinity <Comma-seperated list of cpu core ids>
```

Example:
```bash
sudo dotnet run -- rperf --serverIp 192.168.6.10 --interface=enp2s0 --port 22222 --time 30 --msgs-per-sec 2500 --msg-size 64 --protocol udp --transmissionTechnology Ethernet --comment Sample Comment --captureInterfaceName enp3s0 --tap --cpu-affinity 2,3
```
