using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LowerLimbActivityDriver;

namespace CollectionGUI
{
    public class SerialPortSettingsContainer : INotifyPropertyChanged
    {
        private SerialPortSettings serialPortSettings;

        public SerialPortSettingsContainer(SerialPortSettings serialPortSettings)
        {
            this.serialPortSettings = serialPortSettings;
        }
        
        #region BaudRate
        /// <summary>
        /// Name of serial port <example>COM3</example>
        /// </summary>
        public int BaudRate
        {
            get { return serialPortSettings.BaudRate; }
            set
            {
                if (!Equals(serialPortSettings.BaudRate, value))
                {
                    serialPortSettings.BaudRate = value;
                    OnPropertyChanged("BaudRate");
                }
            }
        }
        #endregion

        #region Parity
        /// <summary>
        /// Parity of the port
        /// </summary>
        public Parity Parity
        {
            get { return serialPortSettings.Parity; }
            set
            {
                if (!Equals(serialPortSettings.Parity, value))
                {
                    serialPortSettings.Parity = value;
                    OnPropertyChanged("Parity");
                }
            }
        }
        #endregion
        
        #region StopBits
        /// <summary>
        /// Fotmat of the stop bits 
        /// </summary>
        public StopBits StopBits
        {
            get { return serialPortSettings.StopBits; }
            set
            {
                if (!Equals(serialPortSettings.StopBits, value))
                {
                    serialPortSettings.StopBits = value;
                    OnPropertyChanged("StopBits");
                }
            }
        }
        #endregion
        
        #region DataBits
        /// <summary>
        /// Length of the data bits
        /// </summary>
        public int DataBits
        {
            get { return serialPortSettings.DataBits; }
            set
            {
                if (!Equals(serialPortSettings.DataBits, value))
                {
                    serialPortSettings.DataBits = value;
                    OnPropertyChanged("DataBits");
                }
            }
        }
        #endregion
        
        #region Handshake
        /// <summary>
        /// Handshake fromat
        /// </summary>
        public Handshake Handshake
        {
            get { return serialPortSettings.Handshake; }
            set
            {
                if (!Equals(serialPortSettings.Handshake, value))
                {
                    serialPortSettings.Handshake = value;
                    OnPropertyChanged("Handshake");
                }
            }
        }
        #endregion

        #region RtsEnable
        /// <summary>
        /// Is rst enable
        /// </summary>
        public bool RtsEnable
        {
            get { return serialPortSettings.RtsEnable; }
            set
            {
                if (!Equals(serialPortSettings.RtsEnable, value))
                {
                    serialPortSettings.RtsEnable = value;
                    OnPropertyChanged("RtsEnable");
                }
            }
        }
        #endregion

        #region SerialPort
        /// <summary>
        /// Name of serial port <example>COM3</example>
        /// </summary>
        public string SerialPort
        {
            get { return serialPortSettings.SerialPort; }
            set
            {
                if (!Equals(serialPortSettings.SerialPort, value))
                {
                    serialPortSettings.SerialPort = value;
                    OnPropertyChanged("SerialPort");
                }
            }
        }
        #endregion


        private void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
