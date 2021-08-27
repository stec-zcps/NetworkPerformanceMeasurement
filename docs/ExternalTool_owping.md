# owping

## Setup
To use owping as external tool [installation of Perfsonar tools](http://docs.perfsonar.net/install_debian.html) is required.

### Installation on Debian-based systems
```bash
cd /etc/apt/sources.list.d/
sudo wget http://downloads.perfsonar.net/debian/perfsonar-release.list
wget -qO - http://downloads.perfsonar.net/debian/perfsonar-official.gpg.key | sudo apt-key add -
sudo add-apt-repository universe
sudo apt-get update
sudo apt-get install perfsonar-tools
```

## Usage
Sample command:
```bash
dotnet run -- owping ping-pong --serverIp 127.0.0.1 --clientIp 127.0.0.1 --port 11111 --time 1 --msgs-per-sec 100,500 --msg-size 500 --transmissionTechnology "Loopback" --comment "Sample Comment"
```
