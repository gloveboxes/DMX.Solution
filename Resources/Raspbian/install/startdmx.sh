#!/bin/bash
echo "starting dmx controller"

sleep 1

sudo killall mono

sudo rmmod ftdi_sio
sudo rmmod usbserial

sudo mono /home/pi/mono/dmx.server/DMX.Server.exe  -c synced -a 90 -i low &

