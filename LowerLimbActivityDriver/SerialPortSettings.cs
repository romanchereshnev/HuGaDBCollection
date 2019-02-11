using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LowerLimbActivityDriver
{
    public class SerialPortSettings
    {
        public int BaudRate = 115200;
        public Parity Parity = Parity.None;
        public StopBits StopBits = StopBits.One;
        public int DataBits = 8;
        public Handshake Handshake = Handshake.None;
        public bool RtsEnable = true;
        public string SerialPort = "COM3";
    }
}
