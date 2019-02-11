using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CollectionGUI.ActivityUI;
using LowerLimbActivityDriver;

namespace CollectionGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataCollection dataCollection;
        SerialPortSettingsContainer serialPortSettingsContainer;

        private List<string> ListOfActivitiesEnum = DataCollection.ListOfActivitiesEnum;

        private List<ComboBox> listOfComboBoxs = new List<ComboBox>();


        public MainWindow()
        {
            InitializeComponent();

            dataCollection = new DataCollection(new SerialPortSettings());
        }

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            string prefix = "";

            prefix += "#personID\t" + TesteeTextBox.Text + "\n";
            prefix += "#gyroResolution\t" + GyroResolutionTextBox.Text + "\n";
            prefix += "#accResolution\t" + AccelerometerResolution.Text + "\n";
            prefix += "#InLab\t" + IsInlab.Text + "\n";

            if (Comment.Text != "" && Comment.Text != null)
            {
                prefix += "#Comment\t" + Comment.Text + "\n";    
            }

            string tmp = "RawCountinuesData";
            
            StopButton.IsEnabled = true;
            StartButton.IsEnabled = false;
            ActivityTextBlock.Text = ListOfActivitiesEnum[act_num];
            try
            {
                dataCollection.Start(prefix, tmp, int.Parse(TesteeTextBox.Text));
                dataCollection.streamWriter.Write("*" + act_num + "*");
            }
            catch(Exception exp)
            {
                MessageBox.Show(exp.Message);
                StopButton_OnClick(sender, e);
            }

            //Thread tr = new Thread(() =>
            //{
            //    Thread.Sleep(10000);
            //    this.Dispatcher.Invoke(() => StopButton_OnClick(this, e));
            //}
            //);
            //tr.Start();
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (dataCollection.Stop())
            {
                MessageBox.Show(string.Format("Data is file {0} is corrupted", dataCollection.FileName));
            }
            StopButton.IsEnabled = false;
            StartButton.IsEnabled = true;            
        }

        private int act_num = 1;

        private void RadioButtonClick(object sender, RoutedEventArgs e)
        {
            act_num = int.Parse( (sender as RadioButton).Tag.ToString());
            if (StopButton.IsEnabled)
            {
                dataCollection.streamWriter.Write("*" + act_num + "*");
                ActivityTextBlock.Text = ListOfActivitiesEnum[act_num];
            }
        }
    }
}
