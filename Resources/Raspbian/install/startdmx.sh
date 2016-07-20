#!/bin/bash
echo "starting dmx controller"

sleep 1

sudo killall mono

sudo rmmod ftdi_sio
sudo rmmod usbserial

sudo mono /home/pi/dmx.server/DMX.Server.exe&

