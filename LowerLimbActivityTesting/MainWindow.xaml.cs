using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms.DataVisualization.Charting;
using LowerLimbActivityDriver;

namespace LowerLimbActivityTesting
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LowerLimbActivityDriver.DataCollection dataCollection;
        private StreamWriter newStreamWriter;
                    
                    
                    

        public MainWindow()
        {
            InitializeComponent();
            dataCollection = new DataCollection(new SerialPortSettings());
            dataCollection.DataReceivedHandler = DataRecive;
            newStreamWriter = new StreamWriter("Text.txt");
        }

        private double shift = 0;
        private String[] Letters = {"X", "Y", "Z"};
        private System.Drawing.Color[] Colorss = { System.Drawing.Color.Red, System.Drawing.Color.Green, System.Drawing.Color.Blue };

        List<double>[] DataFromSensors = new List<double>[38];
        Chart[] Charts = new Chart[14];

        private StreamReader streamReader;

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Charts[0] = ChartLAcc1;
            Charts[1] = ChartLGyr1;
            Charts[2] = ChartLAcc2;
            Charts[3] = ChartLGyr2;
            Charts[4] = ChartLAcc3;
            Charts[5] = ChartLGyr3;
            
            Charts[6] = ChartRAcc1;
            Charts[7] = ChartRGyr1;
            Charts[8] = ChartRAcc2;
            Charts[9] = ChartRGyr2;
            Charts[10] = ChartRAcc3;
            Charts[11] = ChartRGyr3;
            Charts[12] = ChartLEMG;
            Charts[13] = ChartREMG;

            for (int i = 0; i < Charts.Length; i++)
            {
                Charts[i].ChartAreas.Add(new ChartArea("Default"));
                
            }
            
            for (int a = 0; a < 12; a++)
            {
                for (int i = 0; i < Letters.Length; i++)
                {
                    Charts[a].Series.Add(new Series(Letters[i]));
                    Charts[a].Series[Letters[i]].ChartArea = "Default";
                    Charts[a].Series[Letters[i]].ChartType = SeriesChartType.Line;
                    Charts[a].Series[Letters[i]].Color = Colorss[i];    
                }
            }

            Charts[12].Series.Add(new Series("EMG1"));
            Charts[12].Series["EMG1"].ChartArea = "Default";
            Charts[12].Series["EMG1"].ChartType = SeriesChartType.Line;
            Charts[12].Series["EMG1"].Color = System.Drawing.Color.Red;
            Charts[12].ChartAreas["Default"].AxisY.Maximum = 256;


            Charts[13].Series.Add(new Series("EMG2"));
            Charts[13].Series["EMG2"].ChartArea = "Default";
            Charts[13].Series["EMG2"].ChartType = SeriesChartType.Line;
            Charts[13].Series["EMG2"].Color = System.Drawing.Color.Green;
            Charts[13].ChartAreas["Default"].AxisY.Maximum = 256;

            for (int i = 0; i < DataFromSensors.Length; i++)
            {
                DataFromSensors[i] = new List<double>();
            }

            
            
            #region Example
            //double[] axisYData = new double[] { 0.1, 1.5, 1.9 };

            //List<double> data = new List<double>();
            //double dy = 2 * Math.PI / 100;
            //for (int i = 0; i < 1000; i++)
            //{
            //    data.Add(Math.Cos(0.7 * dy * i + shift) + Math.Sin(3.5 * dy * i + shift / 3));
            //}
            //ChartAcc1.Series["X"].Points.DataBindY(data);
            //data.Clear();

            //shift = 10;
            //for (int i = 0; i < 1000; i++)
            //{
            //    data.Add(Math.Cos(0.7 * dy * i + shift) + Math.Cos(3.5 * dy * i + shift / 3));
            //}
            //ChartAcc1.Series["Y"].Points.DataBindY(data);
            //data.Clear();

            //for (int i = 0; i < 1000; i++)
            //{
            //    data.Add(Math.Cos(0.7*dy*i + shift) + Math.Exp(i*0.001));
            //}
            //ChartAcc1.Series["Z"].Points.DataBindY(data);
            //data.Clear();


            //CreateGraph();
            //Thread t = new Thread(Callback);
            //t.Start();
            //t.IsBackground = true;
            #endregion

            #region For test
            /*string tmp = "";
            var MyFile = new FileInfo(@"C:\Projects\kfattila-human-motion-main-code\Data\1_Walking_1_2-14-13-3_3.txt");
            streamReader = MyFile.OpenText();

            Thread t = new Thread(Callback);
            t.Start();
            t.IsBackground = true; */

            #endregion

            //new Thread(() =>
            //{
            //    Thread.Sleep(60000);
            //    newStreamWriter.Write(data);
            //    Dispatcher.Invoke( () => this.Close());
            //}).Start();

            dataCollection.Start("", "", -1);
        }

        //private void Callback()
        //{
        //    Random r = new Random();
        //    while (true)
        //    {
        //        Thread.Sleep(r.Next(50, 500));
        //        string tmp = "";
        //        {
        //            for (int i = 0; i < 5000; i++)
        //            {
        //                tmp += (char)streamReader.Read() ;
        //            }
        //        }
        //        DataHeahler(tmp);
        //    }
        //}

        private void CreateGraph()
        {

            int dataNum = 0;
            for (int i = 0; i < Charts.Length-2; i++)
            {
                for (int j = 0; j < Letters.Length; j++)
                {
                    Charts[i].Series[Letters[j]].Points.DataBindY(DataFromSensors[dataNum]);
                    dataNum++;
                }              
            }

            Charts[12].Series["EMG1"].Points.DataBindY(DataFromSensors[dataNum]);
            dataNum++;
            Charts[13].Series["EMG2"].Points.DataBindY(DataFromSensors[dataNum]);
        }

        private string data = "";

        private void DataRecive(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender; // Get port 
            DataHeahler(sp.ReadExisting());         
        }

        private int count = 0;
        private string dataToWrite = "";
        private void DataHeahler(string recievedString)
        {
            dataToWrite += recievedString;

            count += recievedString.ToCharArray().Where(s => s == '\n').Count();
            
            
            if (count >= 200)
            {
                count = 0;
                
                string data = dataToWrite.Replace("|", "");
                data = data.Replace("   ", " ");
                data = data.Replace("  ", " ");

                List<string> array;
                array = new List<string>(data.Split('\n'));

                array.RemoveAt(0);
                array.RemoveAt(array.Count - 1);
                string[] line;
                for (int i = 0; i < array.Count; i++)
                {                           
                    line = array[i].Split(' ');
                    for (int j = 0; j < line.Length; j++)
                    {                                                              
                        DataFromSensors[j].Add(double.Parse(line[j]));
                    }
                }
                newStreamWriter.Write(dataToWrite);
                dataToWrite = "";

                Dispatcher.Invoke( () => CreateGraph());
                foreach (List<double> dataFromSensor in DataFromSensors)
                {
                    dataFromSensor.Clear();
                }

            }
        }

    }
}
