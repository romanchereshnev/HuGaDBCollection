using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace LowerLimbActivityDriver
{
    /// <summary>
    /// Get data from serial poert and write it to file
    /// </summary>
    public class DataCollection
    {
        #region Properties

        public static List<string> ListOfActivitiesEnum = new List<string>()
        {
            "none",

            "walking", // 1 
            "running", // 2

            "climbingStairs", // 3
            "descendStairs", // 4

            "sitting", // 5
            "sitOnTheChair", // 6
            "getUpFromChair", // 7

            "standing", // 8

            "cycling", // 9

            "liftingInTheElevator", // 10
            "descentInTheElevator", // 11
        };

        /// <summary> 
        /// Name of directory, were files puts 
        /// </summary> 
        public string DirectoryName { get; set; }

        /// <summary> 
        /// Filename were programm will write data from sensors 
        /// </summary> 
        public string FileName { get; set; }

        /// <summary>
        /// Settings of serial port
        /// </summary>
        public SerialPortSettings SerialPortSettings { get; set; }

        public Action<object, SerialDataReceivedEventArgs> DataReceivedHandler { get; set; }
        #endregion

        #region private fields
        /// <summary> 
        /// Stream of file, were program put data from sensors 
        /// </summary> 
        public StreamWriter streamWriter;

        /// <summary> 
        /// Serial port with refer to Bluetooth concetion 
        /// </summary> 
        private SerialPort mySerialPort;

        #endregion


        public DataCollection(SerialPortSettings settings)
        {
            DirectoryName = "RawData";
            FileName = "";
            SerialPortSettings = settings;
            DataReceivedHandler = dataReceivedHandler;
        }
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             
        #region Start
        /// <summary>
        /// Starting of get information from sensors and write data to file
        /// </summary>
        /// <param name="prefix">Prefix of the file. Without date and axis name.</param>
        /// <param name="activity">Activity in which we collect data<example>Walking or MultyActivity</example></param>
        /// <param name="testeeNumber">ID number of testee</param>
        public void Start(string prefix, string activity, int testeeNumber)
        {
            if (testeeNumber != -1)
            {
                // If there is no directory 
                if (!Directory.Exists(DirectoryName))
                    Directory.CreateDirectory(DirectoryName); // create it     

                FileName = makeFileName(activity, testeeNumber);
                streamWriter = new StreamWriter(FileName);

                DateTime dateTime = DateTime.Now;
                string date = dateTime.Month.ToString("D2") + "-" + dateTime.Day.ToString("D2") + "-" +
                              dateTime.Hour.ToString("D2") + "-" + dateTime.Minute.ToString("D2");
                prefix += "#Date " + date + "\n" +
                          "acc1_x\tacc1_y\tacc1_z\tgyro1_x\tgyro1_y\tgyro1_z\t" +
                          "acc2_x\tacc2_y\tacc2_z\tgyro2_x\tgyro2_y\tgyro2_z\t" +
                          "acc3_x\tacc3_y\tacc3_z\tgyro3_x\tgyro3_y\tgyro3_z\t" +
                          "acc4_x\tacc4_y\tacc4_z\tgyro4_x\tgyro4_y\tgyro4_z\t" +
                          "acc5_x\tacc5_y\tacc5_z\tgyro5_x\tgyro5_y\tgyro5_z\t" +
                          "acc6_x\tacc6_y\tacc6_z\tgyro6_x\tgyro6_y\tgyro6_z\t" +
                          "EMG1\tEMG2\tact\n";
                Console.WriteLine(prefix);
                streamWriter.Write(prefix);
            }

            mySerialPort = initializeSerialPort(SerialPortSettings);
            mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            mySerialPort.Open();
            
        }
        #endregion                                                       
        
        #region Stop
        public bool Stop()
        {
            mySerialPort.DataReceived -= dataReceivedHandler;
            mySerialPort.Close();

            Thread.Sleep(200);
            streamWriter.Close();
            string data = "";
            using (StreamReader sr = new StreamReader(FileName))
            {
                data = sr.ReadToEnd();
            }
            return IsCorrupted(data);
        }
        #endregion

        #region initializeSerialPort
        /// <summary>
        /// Initialize serial port with settings
        /// </summary>
        /// <param name="settings"></param>
        /// <returns>Initialized serial port</returns>
        SerialPort initializeSerialPort(SerialPortSettings settings)
        {
            var tmpSerialPort = new SerialPort(settings.SerialPort);

            tmpSerialPort.BaudRate = settings.BaudRate;
            tmpSerialPort.Parity = settings.Parity;
            tmpSerialPort.StopBits = settings.StopBits;
            tmpSerialPort.DataBits = settings.DataBits;
            tmpSerialPort.Handshake = settings.Handshake;
            tmpSerialPort.RtsEnable = settings.RtsEnable;

            return tmpSerialPort;
        }
        #endregion

        #region dataReceivedHandler
        /// <summary> 
        /// Event hanler. Handle situation, when program read data from port. There can be a lot of data 
        /// </summary> 
        private void dataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender; // Get port 
            string indata = sp.ReadExisting(); // Get data as string 
            streamWriter.Write(indata); // Write data to file                                                     
        }
        #endregion

        #region makeFileName
        /// <summary> 
        /// Create filename. 
        /// It create in these format: directoryName\\LLA_version_activity_testeeID_NumberOfFileWithSameTesteeAndActivity.txt 
        /// <example>Data\\LLA_v1_Walking_1_1.txt</example> 
        /// </summary> 
        private string makeFileName(string activity, int testeeNumber)
        {
            int number = 0;

            string fileName = DirectoryName + "\\LLA_v1" + "_" + activity + "_" + testeeNumber.ToString("D2") + "_" + number.ToString("D2") + ".txt";
            while (File.Exists(fileName))
            {
                number++;
                fileName = DirectoryName + "\\LLA_v1"  + "_" + activity + "_" + testeeNumber.ToString("D2") + "_" + number.ToString("D2") + ".txt";
            }

            Console.WriteLine(fileName);
            return fileName;
        }
        #endregion
       

        private void Parser(string filename, string newFileName)
        {
            try
            { // Open the text file using a stream reader. 
                using (StreamReader sr = new StreamReader(filename))
                {
                    // Read the stream to a string, and write the string to the console. 
                    String line = sr.ReadToEnd();
                    line = line.Replace("|", "");
                    line = line.Replace(" ", " ");
                    line = line.Replace(" ", " ");

                    string[] lines = line.Split('\n');
                    List<string> deletedLines = new
                    List<string>();

                    StringBuilder sb = new StringBuilder();
                    for (int i = 1; i < lines.Length - 1; i++)
                    {
                        sb.AppendLine(lines[i]);
                    }


                    using (StreamWriter newStreamWriter = new StreamWriter("Text.txt"))
                    {
                        newStreamWriter.Write(sb.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }


        public bool IsCorrupted(string file)
        {
            return file.Contains("0 0 0 0 0 0");
        }
    }
}