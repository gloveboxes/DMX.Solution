# Setting up the DMX Light Control Server

MQTT Based ENTTEC DMX Server (Windows and Mono on Raspberry Pi), Touch Colour Client

## Design

1. DMX Light Server - MQTT Subscriber to json colour messages.

2. DMX Client - MQTT Publisher of a json colour message. See Appendix.

3. MQTT Broker. [Mosquiotto](http://mosquitto.org/) running on the DMX Light Server.

![Architecture](https://raw.githubusercontent.com/gloveboxes/DMX.Solution/master/Resources/Architecture.jpg)

## Hardware Required

1. [ENTTEC DMX USB PRO Mk2](http://www.enttec.com/?main_menu=Products&pn=70314) or better.

2. [Raspberry Pi with WiFi](https://www.raspberrypi.org/) or [Beaglebone Black Wireless](https://beagleboard.org/black-wireless)
    - A Raspberry Pi 2 or better with a ARMv7 MCU is required for .NET Core. This excludes the RPi Zero which has a ARMv6 MCU.

3. DMX Lights, cables etc

4. Alternatively a Windows or macOS Device. See the [D2XX Direct Drivers](https://www.ftdichip.com/Drivers/D2XX.htm) instructions.

## Connect to your Raspberry Pi or Beaglebone Black

### Raspberry Pi with WiFi

```bash
    ssh pi@raspberrypi.local
```

### Beaglebone Black Wireless

```bash
    ssh debian@beaglebone.lan
```

## Install ENTTEC Driver

[ENTTEC DMX Driver](http://www.ftdichip.com/Drivers/D2XX.htm). Tested on Raspberry Pi 2 and better (ARMv7 devices), Beaglebone Black Wireless, and Windows.

### Installation on Linux

```bash
    export libftd_version=1.4.8

    wget https://www.ftdichip.com/Drivers/D2XX/Linux/libftd2xx-arm-v7-hf-$libftd_version.gz

    tar xfvz libftd2xx-arm-v7-hf-$libftd_version.gz
    cd release/build
    sudo cp libftd2xx.* /usr/local/lib
    sudo chmod 0755 /usr/local/lib/libftd2xx.so.$libftd_version
    sudo rm /usr/local/lib/libftd2xx.so
    sudo ln -sf /usr/local/lib/libftd2xx.so.$libftd_version /usr/local/lib/libftd2xx.so
```

### Installation on Windows

1. [Windows ENTECC Driver](https://www.ftdichip.com/Drivers/D2XX.htm)
2. [Installation Instructions](https://www.ftdichip.com/Support/Documents/InstallGuides.htm)

## Install Mosquitto Broker on Linux

```bash
    sudo apt install mosquitto
```

On Windows install in the Windows Subsystem for Linux.

## Autostart DMX Server

### Raspberry Pi

```bash
    sudo /etc/rc.local
```

before exit on the last line add the following

    sudo rmmod ftdi_sio
    sudo rmmod usbserial
    /home/pi/DMX.NETCore.Server/DMX.NETCore.Server test &

Ctrl-x to save and exit nano

Reboot the Raspberry Pi

```bash
    sudo reboot
```

### Beaglebone Black Wireless

The DMX Server cannot be started before the USB drivers loaded. The following instructions are a combination of information found from the following links:

- [Linux: Start daemon on connected USB-serial dongle](https://stackoverflow.com/questions/18463755/linux-start-daemon-on-connected-usb-serial-dongle)

- [Linux : How to add rc.local in Debian 9](https://www.itechlounge.net/2017/10/linux-how-to-add-rc-local-in-debian-9/)

### Create a udev rule

```bash
    echo \
    'KERNEL=="ttyUSB0", ENV{SYSTEMD_WANTS}="serialdaemon.service"' \
    | sudo tee -a /etc/udev/rules.d/95-serialdaemon.rules
```

### Create the systemd ***serialdaemon.service*** service.

```bash
    echo \
    '[Unit]
    Description=USB serial to socket bridge
    After=remote-fs.target
    After=syslog.target

    [Service]
    Type=forking
    ExecStart=/etc/rc.local start
    TimeoutSec=0
    StandardOutput=tty
    RemainAfterExit=yes
    SysVStartPriority=99

    [Install]
    WantedBy=multi-user.target' \
    | sudo tee -a /etc/systemd/system/serialdaemon.service
```

### Enable the systemd service

```bash
    sudo systemctl enable serialdaemon.service
```

### Create /etc/rc.local

```bash
    echo \
    '#!/bin/sh -e
    rmmod ftdi_sio
    rmmod usbserial
    /home/debian/DMX.NETCore.Server/DMX.NETCore.Server test &
    exit 0' \
    | sudo tee -a /etc/rc.local
```

### Set rc.local to executable

```bash
    sudo chmod +x /etc/rc.local
```

## Deploying the DMX Server

### Visual Studio Code

The DMX Server needs to run with sudo privileges. For interactive debugging from Visual Studio Code I found the easiest way to debug with elevated privileges is to add pi to root. Make sure you reboot after added user to root.

On Raspberry Pi.

```bash
sudo adduser pi root
```

On Beaglebone.

```bash
sudo adduser debian root
```

Then reboot.

```bash
sudo reboot
```

To delete the added user from root

```bash
deluser pi root
```

Then reboot.

### Compile and Deploy from Visual Studio Code

1. Open the DMX.NETCore.Server project with Visual Studio Code.
2. Select Raspberry or Beaglebone from Debug activity bar.
3. Press F5 to Build and rsync copy the executable file to the device.

### Unpack Precompiled

tbc

### Docker

tbc

## Appendix

## Client JSON Messages

### Declariative mode. 

The client just specifies the colours required. This colours are mapped server side to the correct DMX Channels. See universe.json.

This example would send Red and Green colour information to fixtures 1, 4, and 8. The DMX Server maps the colour data to the correct DMX channel. This makes is simple for a client to send  colour information to a fixture without being concerned about the channel mappings for the fixture.

    {"id":[1,2],"Red":200,"Green":150}

additional examples

    {"id":[1,4,8],"Red":200,"Green":150,"Blue":20, "White":0}

    {"id":[1],"Red"255}

```bash
mosquitto_pub -h localhost -t "dmx/data" -m "{"id":[1,2],"Red":200,"Green":150}"
```

### Imperative mode

The client sends DMX data values that corrospond to the channels on the fixture. The DMX Server maps this data for fixtures 1, 2, and 3. This allows full control of the fixture capabilites such as rotate and strobe effects.  

This example would send the channel data (base 64 encoded) to fixtures 1,2 and 3 as defined in the DMX Server side fixtures.json definition file.

```json
{"id":[1,2,3],"data":"/6T1AAAAAA=="}
```

```bash
mosquitto_pub -h localhost -t "dmx/data" -t "{"id":[1,2,3],"data":"/6T1AAAAAA=="}"
```

## DMX Server Stats

Allows the client to subscript to DMX Server statistics.

```bash
mosquitto_sub -h localhost -t "dmx/status"
```

## DMX Server Config

File: config.json

```json
{
  "dmxRefreshRateMilliseconds": 50,
  "autoPlayEnabled": "true",
  "autoPlayTimeout": 1000,
  "autoPlayCycleMode": "synced",
  "autoPlayIntensity": 1,
  "mqttBroker": "localhost",
  "mqttDataTopic": "dmx/data/#",
  "mqttStatusTopic": "dmx/status"
}
```

## DMX Universe definition

File: universe.json

```json
[
  {
    "desc": "LED PAR 12 RGBW Light",
    "fixtureId": 1,
    "startChannel": 37,
    "numberOfChannels": 8,
    "initialChannelMask": [ 0, 0, 0, 255, 0, 0, 0, 0 ],
    "redChannels": [ 5 ],
    "greenChannels": [ 6 ],
    "blueChannels": [ 7 ],
    "whiteChannels": [ 8 ],
    "autoPlayId": "rgbLight"
  },
  {
    "desc": "LED PAR 12 RGBW Light",
    "fixtureId": 2,
    "startChannel": 45,
    "numberOfChannels": 8,
    "initialChannelMask": [ 0, 0, 0, 255, 0, 0, 0, 0 ],
    "redChannels": [ 5 ],
    "greenChannels": [ 6 ],
    "blueChannels": [ 7 ],
    "whiteChannels": [ 8 ],
    "autoPlayId": "rgbLight"
  }
]
```

## DMX Server Autoplay Config

File: autoplay.json

```json
[
  {
    "autoPlayId": "rgbwLight",
    "type": "rgbw",
    "data": [
      [ 255, 0, 0, 0 ],
      [ 0, 255, 0, 0 ],
      [ 0, 0, 255, 0 ]
    ]
  },
  {
    "autoPlayId": "rgbLight",
    "type": "rgb",
    "data": [
      [ 0, 128, 0 ],
      [ 0, 0, 255 ],
      [ 255, 255, 255 ],
      [ 255, 0, 0 ],
      [ 0, 191, 255 ],
      [ 255, 165, 0 ],
      [ 255, 182, 193 ],
      [ 0, 255, 255 ],
      [ 0, 255, 0 ],
      [ 199, 21, 133 ]
    ]
  }
]
```

## Linux Tips and Tricks

### Setting a statip IP Address

[How to set Raspberry Pi Static IP Address](http://www.modmypi.com/blog/how-to-give-your-raspberry-pi-a-static-ip-address-update)

```text
interface wlan0

static ip_address=10.10.10.220/23
static routers=10.10.10.1
static domain_name_servers=194.161.154.253
```

### View Dynamic Libraries Loaded

[Configure dynamic linker run time bindings](http://man7.org/linux/man-pages/man8/ldconfig.8.html)

```bash
sudo ldconfig -v

sudo ldconfig -v | grep libftd2
```


## Beaglebone Tips and Tricks

Disable the following services

```bash
sudo systemctl disable bonescript-autorun.service
sudo systemctl disable bonescript.socket
sudo systemctl disable apache2.service
sudo systemctl disable apache2.service 
```

Disable Heartbeat

Add following line to /etc/rc.local

```text
sudo echo none > /sys/class/leds/beaglebone:green:usr0/trigger
```
https://www.ccoderun.ca/programming/2016-12-18_BBBW/