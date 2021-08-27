# Packet Capturing

## Setup

### Linux
For network packet capturing on Linux systems libpcap needs to be installed:
```bash
sudo apt-get install libpcap-dev 
```
Depending on your system configuration **sudo may be required** for packet capturing.

## Usage
To enable packet capturing a network interface name must be provided. The specified network interface will be used for 
packet capturing:
```bash
--captureInterfaceName eth0
```
If the Kunbus TAP CURIOUS is used for packet capturing the `--tap` flag must be set. In this case the timestamp
of the Kunbus TAP CURIOUS will be used which nanoseconds precision.
