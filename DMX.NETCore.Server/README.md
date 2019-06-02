## Install Entec Driver

export libftd_version=1.4.8

wget https://www.ftdichip.com/Drivers/D2XX/Linux/libftd2xx-arm-v7-hf-$libftd_version.gz

tar xfvz libftd2xx-arm-v7-hf-$libftd_version.gz
cd release/build
sudo cp libftd2xx.* /usr/local/lib
sudo chmod 0755 /usr/local/lib/libftd2xx.so.$libftd_version
sudo rm /usr/local/lib/libftd2xx.so
sudo ln -sf /usr/local/lib/libftd2xx.so.$libftd_version /usr/local/lib/libftd2xx.so

## Install Mosquitto Broker

```bash
sudo apt install mosquitto
```

## Auto Start DMX Server

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
