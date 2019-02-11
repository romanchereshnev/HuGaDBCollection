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
            serialPortSettingsContainer = new SerialPortSettingsContainer(dataCollection.SerialPortSettings);

            PortSettingsUserControl.DataContext = serialPortSettingsContainer;

            for (int i = 0; i < 10; i++)
            {
                listOfComboBoxs.Add(new ComboBox());
                listOfComboBoxs[i].ItemsSource = ListOfActivitiesEnum;
                PanelOfActivities.Children.Add(listOfComboBoxs[i]);
                listOfComboBoxs[i].DropDownClosed += MainWindow_DropDownClosed;
                ActivitieesGrid.Children.Add(new Border());
            }

        }

        void MainWindow_DropDownClosed(object sender, EventArgs e)
        {
            var tmp = (ComboBox)sender;
            int index = listOfComboBoxs.IndexOf(tmp);

            if (tmp.SelectedIndex < 0)
            {
                
            }
            else if (tmp.SelectedIndex == 0)
            {
                if (ActivitieesGrid.Children[index] != null)
                    ActivitieesGrid.Children.RemoveAt(index);
                ActivitieesGrid.Children.Insert(index, new Border());
            }
            else if (tmp.SelectedIndex == 1 && tmp.SelectedIndex == 2 && tmp.SelectedIndex == 9)
            {
                if (ActivitieesGrid.Children[index] != null)
                    ActivitieesGrid.Children.RemoveAt(index);
                var activity = new WalkingRunningCyclingUserControl();
                activity.GetActivity = ListOfActivitiesEnum[tmp.SelectedIndex];
                ActivitieesGrid.Children.Insert(index, activity);
            }
            else if (tmp.SelectedIndex < 5)
            {
                if (ActivitieesGrid.Children[index] != null)
                    ActivitieesGrid.Children.RemoveAt(index);
                var activity = new ClimbingDescendUserControl();
                activity.GetActivity = ListOfActivitiesEnum[tmp.SelectedIndex];
                ActivitieesGrid.Children.Insert(index, activity);
            }
            else if (tmp.SelectedIndex < 8)
            {
                if (ActivitieesGrid.Children[index] != null)
                    ActivitieesGrid.Children.RemoveAt(index);
                var activity = new SittingFromChairOnChair();
                activity.GetActivity = ListOfActivitiesEnum[tmp.SelectedIndex];
                ActivitieesGrid.Children.Insert(index, activity);
            }
            else if (tmp.SelectedIndex < 12)
            {
                if (ActivitieesGrid.Children[index] != null)
                    ActivitieesGrid.Children.RemoveAt(index);
                var activity = new LiftingStanding();
                activity.GetActivity = ListOfActivitiesEnum[tmp.SelectedIndex];
                ActivitieesGrid.Children.Insert(index, activity);
            }
        }

        List<string> listOfActivities = new List<string>();
        List<int> listOfActivitiesNums = new List<int>();

        private void StartButton_OnClick(object sender, RoutedEventArgs e)
        {
            listOfActivities.Clear();
            listOfActivitiesNums.Clear();
            act_num = 0;
            string prefix = "";

            prefix += "#personID\t" + TesteeTextBox.Text + "\n";
            prefix += "#gyroResolution\t" + GyroResolutionTextBox.Text + "\n";
            prefix += "#accResolution\t" + AccelerometerResolution.Text + "\n";
            prefix += "#InLab\t" + IsInlab.Text + "\n";

            if (Comment.Text != "" && Comment.Text != null)
            {
                prefix += "#Comment\t" + Comment.Text + "\n";    
            }

            string tmp = "";
            foreach (var child in ActivitieesGrid.Children)
            {

                if (!(child is Border))
                {
                    prefix += (child as IGetPrefix).GetPrefix;
                    listOfActivities.Add((child as IGetPrefix).GetActivity);
                    tmp = (child as IGetPrefix).GetActivity;
             
                }
            }

            prefix += "#Activity\t";
            foreach (string activity in listOfActivities)
            {
                prefix += activity + " ";
            }
            prefix += "\n";

            prefix += "#ActivityNum\t";
            foreach (string activity in listOfActivities)
            {
                prefix += ListOfActivitiesEnum.IndexOf(activity) + " ";
                listOfActivitiesNums.Add(ListOfActivitiesEnum.IndexOf(activity));
            }
            prefix += "\n";

            if (listOfActivities.Count > 1)
            {
                tmp = "Several";
            }
            else
            {
                tmp = listOfActivities[0];
            }
            
            StopButton.IsEnabled = true;
            StartButton.IsEnabled = false;
            ActivityTextBlock.Text = listOfActivities[act_num];
            try
            {
                dataCollection.Start(prefix, tmp, int.Parse(TesteeTextBox.Text));
                dataCollection.streamWriter.Write("*" + listOfActivitiesNums[act_num] + "*");
                act_num++;
            }
            catch (Exception exp)
            {
                MessageBox.Show("There is error:\n" + exp.Message);
                StopButton_OnClick(this, e);
            }
            
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (dataCollection.Stop())
            {
                MessageBox.Show(string.Format("Corrauption in file: {0}", dataCollection.FileName));
            }
            StopButton.IsEnabled = false;
            StartButton.IsEnabled = true;            
        }

        private int act_num = 0;
        private void MainWindow_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            if (act_num < listOfActivitiesNums.Count)
            {
                dataCollection.streamWriter.Write("*" + listOfActivitiesNums[act_num] + "*");
                ActivityTextBlock.Text = listOfActivities[act_num];
                act_num++;
            }
            else
            {
                StopButton_OnClick(this, e);   
            }
        }
    }
}
