#!/bin/bash

echo installing required packages

sudo apt-get update
sudo apt-get dist-upgrade -y

sudo apt-get install samba samba-common-bin -y
sudo apt-get install xrdp -y
sudo apt-get install xterm -y
sudo apt-get install mono-complete -y 

sudo wget http://repo.mosquitto.org/debian/mosquitto-repo.gpg.key
sudo apt-key add mosquitto-repo.gpg.key

cd /etc/apt/sources.list.d/

sudo wget http://repo.mosquitto.org/debian/mosquitto-jessie.list
sudo apt-get update
sudo apt-get upgrade -y
sudo apt-get install mosquitto -y

