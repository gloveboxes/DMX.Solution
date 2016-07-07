# DMX.Solution
MQTT Based ENTTEC DMX Server (Windows and Mono on Raspberry Pi), Touch Colour Client


##Architecture 


1. DMX Client - Anything that can publish a json colour message over MQTT
2. DMX Server - Dequeues json colour messages over MQTT and references Universe.json to format data for the ENTTEC DMX Controller 
3. Middleware MQTT Pub/Sub queue for many DMX clients to one DMX Server or many DMX clients to many DMX Servers
    - MQTT Queue support on [Mosquiotto](http://mosquitto.org/) for local or Aure IoT Hub for internet scale queue services
    - Follow the [Mosquitto Debian repository](http://mosquitto.org/2013/01/mosquitto-debian-repository/) for installing Moquiotto on Raspbian.  
    
![Architecture](https://raw.githubusercontent.com/gloveboxes/DMX.Solution/master/Resources/Architecture.jpg)
    
##Tested Platforms (June 2016)

The DMX Server has been tested on the following platforms: -

1. Windows (7,8 and 10)
2. Raspbian Jessie, Kernel 4.4 with [Mono](https://en.wikipedia.org/wiki/Mono_(software))

##DMX Hardware
1. [ENTTEC DMX USB PRO Mk2](http://www.enttec.com/?main_menu=Products&pn=70314). 
2. [ENTTEC DMX Driver](http://www.ftdichip.com/Drivers/D2XX.htm). Very stable on Windows and Raspbian (as at June, 2016). For Raspberry Pi 2 & 3 use the ARM V7 driver, for Raspberry Pi Zero use the ARM V6. The automated setup installs the V7 driver.

##Client JSON Messages

###Declariative mode. 

The client just specifies the colours required. This colours are mapped server side to the correct DMX Channels. See universe.json.

This example would send Red and Green colour information to fixtures 1, 4, and 8. The DMX Server maps the colour data to the correct DMX channel. This makes is simple for a client to send  colour information to a fixture without being concerned about the channel mappings for the fixture.

    {"id":[1,4,8],"Red":200,"Green":150}
    
    additional examples
    
    {"id":[1,4,8],"Red":200,"Green":150,"Blue":20, "White":0}
    
    {"id":[1],"Red"255}
    

###Imperative mode. 

The client sends DMX data values that corrospond to the channels on the fixture. The DMX Server maps this data for fixtures 1, 2, and 3. This allows full control of the fixture capabilites such as rotate and strobe effects.  

This example would send the channel data (base 64 encoded) to fixtures 1,2 and 3 as defined in the DMX Server side fixtures.json definition file.


    {"id":[1,2,3],"data":"/6T1AAAAAA=="}

###Control mode. 

Allows the client to request DMX Server information. For example request runtime statistics.

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



##Acknowledgements 
[FTD2XX](https://github.com/alcexhim/FTD2XX) C# Driver Wrapper for the FTDI chip used by the ENTTEC DMX Controller 


