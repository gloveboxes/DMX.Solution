﻿using FTD2XX;
using System;

namespace DMX.Server
{
    public class DmxController : Device
    {

        const byte DMX_START_CODE = 0x7E;
        const byte DMX_END_CODE = 0xE7;
        const byte OFFSET = 0xFF;
        const byte BYTE_LENGTH = 8;
        byte[] APIKEY = new byte[] { 0xAD, 0x88, 0xD0, 0xC8 };
        const byte Label = 6;

        bool mvarIsOpenDMXInitialized = false;

        const byte ENTTEC_PRO_ENABLE_API2 = 0x0D;
        const byte SET_PORT_ASSIGNMENT_LABEL = 0xCB;

        byte[] channelBuffer;
        byte[] dataPacket;

        int channels;

        public DmxController(uint port) : base(port) { }

        private void InitialiseDataStructures(int channels)
        {
            this.channels = channels;

            if (channels > 512)
            {
                throw new ArgumentException("fixtures * channels per fixture must be less than or equal to 512");
            }

            channelBuffer = new byte[channels + 1];  // add for for an initial control byte that must be zero

            byte[] dataPacket = new byte[4 + channelBuffer.Length + 1];
        }

        public void InitializeDMX(int channels)
        {
            if (mvarIsOpenDMXInitialized) return;

            InitialiseDataStructures(channels);

            Reset();
            SetBaudRate(12);
            SetDataCharacteristics(BitsPerWord.Eight, StopBits.Two, Parity.None);
            SetFlowControl(FlowControl.None, 0, 0);
            ClearRTS();
            Purge();

            WriteCtrlPacket(ENTTEC_PRO_ENABLE_API2, APIKEY);
            WriteCtrlPacket(SET_PORT_ASSIGNMENT_LABEL, new byte[] { 1, 1 });

            mvarIsOpenDMXInitialized = true;

            return;
        }

        public void UpdateChannel(uint dmxChannel, int length, byte[] data)
        {
            if (dmxChannel < 1 || (dmxChannel + length) > channels) { return; }
            Array.Copy(data, 0, channelBuffer, dmxChannel, Math.Min(data.Length, length));
        }

        public void UpdateChannel(int dmxChannel, byte value)
        {
            if (dmxChannel < 1 || dmxChannel > channels) { return; }
            channelBuffer[dmxChannel] = value;  // rbg offset start at 1, byte zero is a control byte
        }

        public void DmxUpdate()
        {
            WriteDataPacket();
        }

        void WriteDataPacket()
        {
            if (dataPacket == null)
            {
                dataPacket = new byte[channelBuffer.Length + 5];

                dataPacket[0] = DMX_START_CODE;
                dataPacket[1] = Label;
                dataPacket[2] = (byte)(channelBuffer.Length & OFFSET);
                dataPacket[3] = (byte)((channelBuffer.Length >> BYTE_LENGTH) & OFFSET);
                dataPacket[dataPacket.Length - 1] = DMX_END_CODE;
            }

            Array.Copy(channelBuffer, 0, dataPacket, 4, channelBuffer.Length);

            WritePacket(dataPacket);
        }

        void WriteCtrlPacket(byte label, byte[] data)
        {
            int length = data.Length;

            byte[] packet = new byte[4 + data.Length + 1];

            packet[0] = DMX_START_CODE;
            packet[1] = label;
            packet[2] = (byte)(length & OFFSET);
            packet[3] = (byte)((length >> BYTE_LENGTH) & OFFSET);

            Array.Copy(data, 0, packet, 4, data.Length);
            packet[packet.Length - 1] = DMX_END_CODE;

            WritePacket(packet);
        }

        void WritePacket(byte[] data)
        {
            int bytesWritten = 0;
            bytesWritten = Write(data);
        }
    }
}
