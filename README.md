# DMX.Solution
MQTT Based ENTTEC DMX Server (Windows and Mono on Raspberry Pi), Touch Colour Client


##Fixture definition

File: fixture.json

    [
    {
        "desc": "LED Flat SlimPar Tri 7 Light",
        "id": 1,
        "numberOfChannels": 7,
        "initialChannelMask": [ 255, 0, 0, 0, 0, 0, 0 ],
        "redChannels": [ 2 ],
        "greenChannels": [ 3 ],
        "blueChannels": [ 4 ],
        "startChannel": 1
    },
    {
        "desc": "DMX Controlled Rotating LED mini light SL3456",
        "id": 5,
        "numberOfChannels": 6,
        "initialChannelMask": [ 0, 0, 0, 0, 0, 0, 0 ],
        "redChannels": [ 2 ],
        "greenChannels": [ 3 ],
        "blueChannels": [ 4 ],
        "startChannel": 32
    }
    ]





