using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace CollectionGUI.ActivityUI
{
    /// <summary>
    /// Interaction logic for WalkingRunningCyclingUserControl.xaml
    /// </summary>
    public partial class SittingFromChairOnChair : UserControl, IGetPrefix
    {
        public SittingFromChairOnChair()
        {
            InitializeComponent();
        }

        public string GetPrefix
        {
            get
            {
                string prefix = ""; 
                for (int i = 0; i < NamePanel.Children.Count; i++)
                {
                    TextBlock name = (TextBlock) NamePanel.Children[i];
                    TextBox value = (TextBox)ValuePanel.Children[i];
                    prefix += (string.IsNullOrEmpty(value.Text) ? "" : "#" + name.Text + " " + value.Text + "\n");
                }
                return prefix;
            }
        }

        public string GetActivity { get; set; }
        
    }
}
