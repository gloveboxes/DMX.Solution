#!/bin/bash

sudo chmod +x installpackages.sh
sudo chmod +x installenttec.sh

./installenttec.sh
./installpackages.sh


sudo mkdir -m 777 /home/pi/mono
sudo mkdir -m 777 /home/pi/scripts

echo Copying setup and dmx mono executables

cp -r  mono/*  /home/pi/mono/

echo setting up Samba

sudo cp /etc/samba/smb.conf /etc/samba/smb.conf.old
sudo rm /etc/samba/smb.conf
sudo cp /home/pi/install/smb.conf /etc/samba

echo setting up DMX Startup Services

cp /home/pi/install/startdmx.sh /home/pi/scripts
sudo chmod +x /home/pi/scripts/startdmx.sh
sudo chown pi startdmx.sh


sudo cp /home/pi/install/dmx.service /etc/systemd/system
sudo systemctl enable dmx.service
sudo systemctl start dmx.service
sudo systemctl status dmx.service

sudo smbpasswd -a pi 

sudo reboot
