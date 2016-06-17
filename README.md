# DMX.Solution
MQTT Based ENTTEC DMX Server (Windows and Mono on Raspberry Pi), Touch Colour Client


##Architecture 

1. DMX Server
2. DMX Client
3. Middleware MQTT Pub/Sub queue for many DMX clients to one DMX Server or many DMX clients to many DMX Servers
    - MQTT Queue support on [Mosquiotto](http://mosquitto.org/) for local or Aure IoT Hub for internet scale queue services
    - Follow the [Mosquitto Debian repository](http://mosquitto.org/2013/01/mosquitto-debian-repository/) for installing Moquiotto on Raspbian.  
    
##Tested Platforms
1. Windows
2. Raspbian running Mono

##DMX Hardware
1. [ENTTEC DMX USB PRO Mk2](http://www.enttec.com/?main_menu=Products&pn=70314). 
2. [ENTTEC DMX Driver](http://www.ftdichip.com/Drivers/D2XX.htm). Very stable on Windows and Raspbian (as at June, 2016)

##Client JSON Messages

###Declariative mode. 

The client just specifies the colours required. This colours are mapped server side to the correct DMX Channels. See fixtures.json.

This example would send Red and Green colour information to the DMX Controller that would map the colour data to the right DMX channel on the fixture.

    {"id":[1,4,8],"Red":200,"Green":150}
    
    additional examples
    
    {"id":[1,4,8],"Red":200,"Green":150,"Blue":20, "White":0}
    
    {"id":[1],"Red"255}
    

###Imperative mode. The client sends DMX data that is positional (ie maps to a DMX Channel) plus the DMX channel value.

This example would send the channel data (base 64 encoded) to fixtures 1,2 and 3 as defined in the DMX Server side fixtures.json definition file.


    {"id":[1,2,3], data:[ABNHK/K]}  

###Control mode. Allows the client to send a command to the DMX Server. For example request runtime statistics.

Example

    {"command":"status"}


##Fixture definition

File: fixture.json

    [
    {
        "desc": "LED Flat SlimPar Tri 7 Light",
        "id": 1,
        "startChannel": 1
        "numberOfChannels": 7,
        "initialChannelMask": [ 255, 0, 0, 0, 0, 0, 0 ],
        "redChannels": [ 2 ],
        "greenChannels": [ 3 ],
        "blueChannels": [ 4 ],
        
    },
    {
        "desc": "DMX Controlled Rotating LED mini light SL3456",
        "id": 5,
        "startChannel": 32
        "numberOfChannels": 6,
        "initialChannelMask": [ 0, 0, 0, 0, 0, 0, 0 ],
        "redChannels": [ 2 ],
        "greenChannels": [ 3 ],
        "blueChannels": [ 4 ],
       
    }
    ]



##Aknowledgements
[FTD2XX](https://github.com/alcexhim/FTD2XX) C# Driver Wrapper for the FTDI chip used by the ENTTEC DMX Controller 


