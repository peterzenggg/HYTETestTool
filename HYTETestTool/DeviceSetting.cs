using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using HidSharp;

namespace HYTETestTool
{
    internal class DeviceSetting:IDisposable
    {

        SerialStream Stream;
        private const int BAUDRATES = 115200;
        private string snNumber;
        public string SnNumber
        {
            get
            {
                return snNumber;
            }
        }
        public DeviceSetting(string PIDVID) 
        {
            SerialDetector Detect = new SerialDetector();
            List<SerialStream> streams = Detect.GetSerialStreams(PIDVID);
            if (streams != null && streams.Count > 0)
            {
                Stream = streams[0]; 
                Stream.BaudRate = BAUDRATES;
            }
            CheckSnNumber();
        }

        private void CheckSnNumber()
        {
            if (Stream != null)
            {
                byte[] data = new byte[] { 0xff, 0xaa, 0xbb, 0x02 };
                writeStream(data);
                byte[] rec = new byte[36];
                Stream.Read(rec);
                if (rec[31] == 255 && rec[30] == 255)
                {
                    snNumber = "Can Write";
                }
                else
                {
                    //string hexString = BitConverter.ToString(rec).Replace("-", " ");
                    byte[] namearr = rec[4..30];
                    snNumber = GetHexString(namearr);
                }
            }
            else
            {
                snNumber = "SerialStream Is Null";
            }
        }

        public bool CheckStreamNotNull()
        {
            return Stream != null;
        }

        private string GetHexString(byte[] data)
        {
            return Encoding.UTF8.GetString(data);
        }

        public void WriteSnNumber(byte[] Bytes)
        {
            writeStream(Bytes);
            CheckSnNumber();
        }

        public void ResetDevice()
        {
            byte[] data = new byte[] { 0xff, 0xaa, 0xbb, 0x03 };
            writeStream(data);
            CheckSnNumber();
        }

        private void writeStream(byte[] Bytes)
        {
            if (Stream != null)
            {
                Stream.Write(Bytes);
                Thread.Sleep(100);
            }
        }

        public void Dispose()
        {
            Stream?.Dispose();
        }
    }
}
