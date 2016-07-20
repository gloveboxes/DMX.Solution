#!/bin/bash

sudo chmod +x installpackages.sh
sudo chmod +x installenttec.sh

./installenttec.sh
./installpackages.sh


sudo mkdir -m 777 /home/pi/scripts
sudo mkdir -m 777 /home/pi/dmx.server

echo Copying setup and dmx mono executables

cp -r mono/* /home/pi/dmx.server/

echo setting up Samba

sudo cp /etc/samba/smb.conf /etc/samba/smb.conf.old
sudo rm /etc/samba/smb.conf
sudo cp smb.conf /etc/samba

echo setting up DMX Startup Services

cp startdmx.sh /home/pi/scripts
sudo chmod +x /home/pi/scripts/startdmx.sh

sudo cp dmx.service /etc/systemd/system
sudo systemctl enable dmx.service
sudo systemctl start dmx.service
sudo systemctl status dmx.service

sudo smbpasswd -a pi 

sudo reboot