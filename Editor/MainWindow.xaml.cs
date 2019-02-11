using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;
using LowerLimbActivityDriver;

namespace Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> ListOfActivitiesEnum = DataCollection.ListOfActivitiesEnum;
        
        Dictionary<string, string> NewAct = new Dictionary<string, string>();

        Dictionary<string, string> Name2NewNum = new Dictionary<string, string>();
        Dictionary<string, string> OldNum2Name = new Dictionary<string, string>();
        public MainWindow()
        {
            InitializeComponent();
            NewAct.Add("1", "1");
            NewAct.Add("2", "2");
            NewAct.Add("3", "2");

            NewAct.Add("4", "5");
            NewAct.Add("5", "6");
            NewAct.Add("6", "7");

            NewAct.Add("7", "3");
            NewAct.Add("8", "4");

            NewAct.Add("12", "10");
            NewAct.Add("13", "11");
            NewAct.Add("14", "8");
            
            Name2NewNum.Add("walking", "1");
            Name2NewNum.Add("running", "2");
            Name2NewNum.Add("climbingStairs", "3");
            Name2NewNum.Add("descendStairs", "4");

            Name2NewNum.Add("sitting", "5");
            Name2NewNum.Add("sitOnTheChair", "6");
            Name2NewNum.Add("getUpFromChair", "7");
            Name2NewNum.Add("standing", "8");

            Name2NewNum.Add("cycling", "9");
            Name2NewNum.Add("liftingInTheElevator", "10");
            Name2NewNum.Add("descentInTheElevator", "11");

            OldNum2Name.Add("1", "walking");
            OldNum2Name.Add("2", "running");
            OldNum2Name.Add("3", "running");

            OldNum2Name.Add("4", "sitting");
            OldNum2Name.Add("5", "sitOnTheChair");
            OldNum2Name.Add("6", "getUpFromChair");

            OldNum2Name.Add("7", "climbingStairs");
            OldNum2Name.Add("8", "descendStairs");

            OldNum2Name.Add("12", "liftingInTheElevator");
            OldNum2Name.Add("13", "descentInTheElevator");
            OldNum2Name.Add("14", "standing");
        }
       

        private OpenFileDialog dialog;

        string ChangeActNumToNew(string lineOfActs)
        {
            var actsNum = lineOfActs.Split('\t').Last().Split(' ');
            foreach (string act in actsNum)
            {
                var tmp = act.Replace("\r", "").Replace(" ", "");
                if (tmp != "")
                {
                    lineOfActs = lineOfActs.Replace('\t' + tmp + ' ', '\t' + OldNum2Name[tmp] + ' ');
                    lineOfActs = lineOfActs.Replace(' ' + tmp + ' ', ' ' + OldNum2Name[tmp] + ' ');
                    lineOfActs = lineOfActs.Replace('\t' + tmp + '\r', '\t' + OldNum2Name[tmp] + '\r');
                    lineOfActs = lineOfActs.Replace(' ' + tmp + '\r', ' ' + OldNum2Name[tmp] + '\r');
                }
            }

            actsNum = lineOfActs.Split('\t').Last().Split(' ');
            foreach (string act in actsNum)
            {
                var tmp = act.Replace("\r", "").Replace(" ", "");
                if (tmp != "")
                    lineOfActs = lineOfActs.Replace(tmp, Name2NewNum[tmp]);
            }
            
            return lineOfActs;
        }

        string GetNewFileName(string oldFileName, string pathToFile)
        {
            string newFileName = oldFileName.Split('\\').Last();
            string act_name = string.Join("_", new List<string> { newFileName.Split('_')[2], newFileName.Split('_')[3] });
            string[] files = Directory.GetFiles(pathToFile);

            int num = 0;
            foreach (var file in files)
            {
                if (file.Contains(act_name))
                {
                    num++;
                }
            }
            var tmp1 = newFileName.Split('_');

            tmp1[4] = String.Format("{0:00}.txt", num);

            newFileName = string.Join("_", tmp1);
            newFileName = pathToFile + newFileName;

            return newFileName;
        }

        string ReplaceOldActNum2NewInData(string oldText)
        {
            foreach (string oldAct in OldNum2Name.Keys)
            {
                oldText = oldText.Replace("*" + oldAct + "*", "*" + OldNum2Name[oldAct] + "*");
            }
            foreach (string name in Name2NewNum.Keys)
            {
                oldText = oldText.Replace("*" + name + "*", "*" + Name2NewNum[name] + "*");
            }
            return oldText;
        }

        bool IsCorrupet(string Text, int thrsh = 4)
        {
            string pat1 = "0 0 0 0 0 0";
            string pat2 = "0\t0\t0\t0\t0\t0";
            int count = (Text.Length - Text.Replace(pat1, "").Length) / pat1.Length;
            count += (Text.Length - Text.Replace(pat2, "").Length) / pat2.Length;

            if (count >= thrsh)
            {
                return true;
            }
            else
                return false;
        }

        void ChangeOldFormatDataToNew()
        {
            foreach (string fileName in dialog.FileNames)
            {

                string oldText = "";
                string newText = "";
                string newFileName = "";

                using (StreamReader sr = new StreamReader(fileName))
                {
                    // Read the stream to a string, and write the string to the console.
                    oldText = sr.ReadToEnd();
                }
                List<string> array;
                array = new List<string>(oldText.Split('\n'));

                string pathToDir = "";

                if (IsCorrupet(oldText))
                {
                    pathToDir = @"..\..\..\..\Data\Corrupted\";
                }
                else
                    pathToDir = @"..\..\..\..\Data\";

                int i;
                string prefix = "";
                for (i = 0; i < array.Count; i++)
                {

                    if (array[i][0] == '#')
                    {
                        if (array[i].Contains("#ActivityNum"))
                        {
                            array[i] = ChangeActNumToNew(array[i]);
                        }
                        prefix += array[i] + "\n";
                    }
                    else
                    {
                        break;
                    }
                }
                prefix += array[i];
                i++;

                newText = prefix;
                for (; i < array.Count - 1; i++)
                {
                    var parts = array[i].Split('\t');
                    var act = parts.Last().Replace("\r", "");
                    parts[parts.Length - 1] = parts[parts.Length - 1].Replace(act, NewAct[act]);
                    array[i] = string.Join("\t", parts);
                    newText += array[i] + '\n';
                }
                newFileName = GetNewFileName(fileName, pathToDir);
                StreamWriter streamWriter = new StreamWriter(newFileName);
                streamWriter.Write(newText);
                streamWriter.Flush();
                streamWriter.Close();

            }
        }

        void ChangeOldFormatRawDataToNew()
        {
            foreach (string fileName in dialog.FileNames)
            {

                string oldText = "";
                string newText = "";
                string newFileName = "";

                using (StreamReader sr = new StreamReader(fileName))
                {
                    // Read the stream to a string, and write the string to the console.
                    oldText = sr.ReadToEnd();
                }

                List<string> array;
                array = new List<string>(oldText.Split('\n'));

                string pathToDir = "";

                var parts = fileName.Split('\\');
                List<string> newPath = new List<string>();

                for (int j = 0; j < parts.Count() - 3; j++)
                {
                    newPath.Add(parts[j]);
                }
                if (IsCorrupet(oldText))
                {

                    pathToDir = string.Join("\\", newPath.ToArray()) + @"\RawData\Corrupted\";

                }
                else
                    pathToDir = pathToDir = string.Join("\\", newPath.ToArray()) + @"\RawData\";

                int i;
                string prefix = "";
                for (i = 0; i < array.Count; i++)
                {

                    if (array[i][0] == '#')
                    {
                        if (array[i].Contains("#ActivityNum"))
                        {
                            array[i] = ChangeActNumToNew(array[i]);
                        }
                        prefix += array[i] + "\n";
                    }
                    else
                    {
                        break;
                    }
                }
                prefix += array[i];
                i++;

                newText = prefix;
                for (; i < array.Count - 1; i++)
                {
                    if (array[i].Contains("*"))
                    {
                        var splites = array[i].Split('*');
                        string act = splites[1].ToString();
                        array[i] = array[i].Replace("*" + act + "*", "*" + NewAct[act] + "*");
                    }
                    
                    newText += array[i] + '\n';
                }
                newFileName = GetNewFileName(fileName, pathToDir);
                StreamWriter streamWriter = new StreamWriter(newFileName);
                streamWriter.Write(newText);
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        void FindLaying()
        {
            NewFile.Text = "";
            foreach (string fileName in dialog.FileNames)
            {

                string oldText = "";

                using (StreamReader sr = new StreamReader(fileName))
                {
                    // Read the stream to a string, and write the string to the console.
                    oldText = sr.ReadToEnd();
                }

                if (oldText.Contains("lay") || oldText.Contains("Floor"))
                {
                    NewFile.Text += fileName + '\n';
                }
            }
        }

        void FindCorr()
        {
            NewFile.Text = "";
            foreach (string fileName in dialog.FileNames)
            {

                string oldText = "";

                using (StreamReader sr = new StreamReader(fileName))
                {
                    // Read the stream to a string, and write the string to the console.
                    oldText = sr.ReadToEnd();
                }

                if (IsCorrupet(oldText))
                {
                    NewFile.Text += fileName + '\n';
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string fileText = "";

            dialog = new OpenFileDialog()
            {
                Filter = "Text Files(*.txt)|*.txt|All(*.*)|*"
            };

            dialog.Multiselect = true;
            dialog.ShowDialog();

            string text = "";
            foreach (var fileName in dialog.FileNames)
            {
                text += fileName + "\n";
            }
            OldFile.Text = text;


        }


        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            FindCorr();
        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            foreach (string fileName in dialog.FileNames)
            {
                
                string oldText = "";
                string newText = "";
                string newFileName = "";
                
                using (StreamReader sr = new StreamReader(fileName))
                {
                    // Read the stream to a string, and write the string to the console.
                    oldText = sr.ReadToEnd();
                }
                List<string> array;
                array = new List<string>(oldText.Split('\n'));

                string prefix = "";
                int i = 0;
                if (fileName.Contains("RawCountinuesData"))
                {
                    List<int> arrayOfActs = new List<int>();
                    int j;
                    for (j = 0; j < array.Count; j++)
                    {
                        string l = array[j];

                        if (l.Contains("*"))
                        {
                            var splites = l.Split('*');
                            arrayOfActs.Add(int.Parse(splites[1]));
                        }
                    }

                    if (arrayOfActs.Count == 1)
                    {
                        newFileName = fileName.Split('\\').Last();
                        newFileName = newFileName.Replace("RawCountinuesData", ListOfActivitiesEnum[arrayOfActs[0]]);
                    }
                    else
                    {
                        newFileName = fileName.Split('\\').Last().Replace("RawCountinuesData", "Several");                        
                    }

                    for (i = 0; i < array.Count; i++)
                    {
                        if (i == 4)
                        {
                            prefix += "#Activity\t";
                            foreach (int act in arrayOfActs)
                            {
                                prefix += ListOfActivitiesEnum[act] + " ";
                            }
                            prefix += "\n#ActivityNum\t";
                            foreach (int act in arrayOfActs)
                            {
                                prefix += act.ToString() + " ";
                            }
                            prefix += "\n";
	 
                        }
                        if (array[i][0] == '#')
                        {
                            prefix += array[i] + "\n";
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    newFileName = fileName.Split('\\').Last();
                    for (i = 0; i < array.Count; i++)
                    {
                        if (array[i][0] == '#')
                        {
                            prefix += array[i] + "\n";
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                prefix = prefix.Replace("InJym", "InLab");
                prefix += array[i] + "\n";
                i++;

                string act_name = string.Join("_", new List<string> {newFileName.Split('_')[2], newFileName.Split('_')[3]});
                string[] files = Directory.GetFiles(@"..\..\..\..\Data\");
                int num = 0;
                foreach (var file in files)
                {
                    if (file.Contains(act_name))
                    {
                        num++;
                    }
                }
                var tmp1 = newFileName.Split('_');

                tmp1[4] = String.Format("{0:00}.txt", num);

                newFileName = string.Join("_", tmp1);
                newFileName = @"..\..\..\..\Data\" + newFileName;

                NewFile.Text = newFileName + "\n" + prefix;

                int numAct = -1;

                newText = prefix;

                string line = array[i];
                if (line.Contains("*"))
                {
                    var splites = line.Split('*');
                    numAct = int.Parse(splites[1]);
                }
                array.RemoveAt(i);
                array.RemoveAt(array.Count - 1);


                for (; i < array.Count; i++)
                {
                    line = array[i];
                    if (line.Contains("*"))
                    {
                        var splites = line.Split('*');
                        numAct = int.Parse(splites[1]);
                        line = splites[0] + splites[2];
                    }

                    line = line.Replace("|", "");
                    line = line.Replace("   ", " ");
                    line = line.Replace("  ", " ");
                    var tmp = line.Split(' ');
                    if (tmp.Length == 38)
                    {
                        line = line.Replace(' ', '\t');
                        line = line.Replace("\r", "");
                        line += '\t' + numAct.ToString() + '\n';
                        newText += line;
                    }
                }

                StreamWriter streamWriter = new StreamWriter(newFileName);
                streamWriter.Write(newText);
                streamWriter.Flush();
                streamWriter.Close();
                
           }
            
        }
        
        #region For changing old files

        //private void Button_Click_4(object sender, RoutedEventArgs e)
        //{
        //    foreach (string fileName in dialog.FileNames)
        //    {

        //        string oldText = "";
        //        string newText = "";
        //        string newFileName = "";

        //        using (StreamReader sr = new StreamReader(fileName))
        //        {
        //            oldText = sr.ReadToEnd();
        //        }
                
        //        newText = ReplaceOldActNum2NewInData(oldText);

        //        string prefix = "";
        //        int i = 0;
        //        string filePath = @"C:\Projects\kfattila-human-motion-main-code\LowerLimbActivityCollection\CollectionGUI\bin\Debug\RawData\";

        //        newFileName = GetNewFileName(fileName, filePath);
                
        //        List<string> array;
        //        array = new List<string>(oldText.Split('\n'));

        //        for (i = 0; i < array.Count; i++)
        //        {

        //            if (array[i][0] == '#')
        //            {
        //                if (array[i].Contains("#ActivityNum"))
        //                {
        //                    array[i] = ChangeActNumToNew(array[i]);
        //                }
        //                prefix += array[i] + "\n";
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }
        //        newText = prefix;
        //        for (; i < array.Count; i++)
        //        {
        //            newText += array[i] + '\n'; 
        //        }

        //        StreamWriter streamWriter = new StreamWriter(newFileName);
        //        streamWriter.Write(newText);
        //        streamWriter.Flush();
        //        streamWriter.Close();
        //    }

        //}

        //private void Button_Click_3(object sender, RoutedEventArgs e)
        //{
        //    foreach (string fileName in dialog.FileNames)
        //    {

        //        string oldText = "";
        //        string newText = "";
        //        string newFileName = "";

        //        using (StreamReader sr = new StreamReader(fileName))
        //        {
        //            // Read the stream to a string, and write the string to the console.
        //            oldText = sr.ReadToEnd();
        //        }
        //        List<string> array;
        //        array = new List<string>(oldText.Split('\n'));

        //        string prefix = "";
        //        int i = 0;

        //        newFileName = fileName.Split('\\').Last();
        //        for (i = 0; i < array.Count; i++)
        //        {

        //            if (array[i][0] == '#')
        //            {
        //                if (array[i].Contains("#ActivityNum"))
        //                {
        //                    array[i] = ChangeActNumToNew(array[i]);
        //                }
        //                prefix += array[i] + "\n";
        //            }
        //            else
        //            {
        //                break;
        //            }
        //        }

        //        prefix += array[i] + "\n";
        //        i++;

        //        string act_name = string.Join("_", new List<string> { newFileName.Split('_')[2], newFileName.Split('_')[3] });
        //        string[] files = Directory.GetFiles(@"..\..\..\..\Data\Corrupted\");
        //        int num = 0;
        //        foreach (var file in files)
        //        {
        //            if (file.Contains(act_name))
        //            {
        //                num++;
        //            }
        //        }
        //        var tmp1 = newFileName.Split('_');

        //        tmp1[4] = String.Format("{0:00}.txt", num);

        //        newFileName = string.Join("_", tmp1);
        //        newFileName = @"..\..\..\..\Data\Corrupted\" + newFileName;

        //        NewFile.Text = newFileName + "\n" + prefix;

        //        newText = prefix;

        //        for (; i < array.Count - 1; i++)
        //        {
        //            var parts = array[i].Split('\t');
        //            var act = parts.Last().Replace("\r", "");
        //            array[i] = array[i].Replace(act, NewAct[act]);
        //            newText += array[i] + '\n';
        //        }

        //        StreamWriter streamWriter = new StreamWriter(newFileName);
        //        if (newText.Contains("0\t0\t0\t0\t0\t0"))
        //        {
        //            // MessageBox.Show( i.ToString() + ": " + fileName);
        //        }
        //        streamWriter.Write(newText);
        //        streamWriter.Flush();
        //        streamWriter.Close();
        //    }

        //}

        #endregion
    }
}
