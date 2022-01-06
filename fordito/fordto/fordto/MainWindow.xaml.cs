using DocumentFormat.OpenXml.Drawing;
using fordto.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace fordto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TranslatorHelper helper = new TranslatorHelper();
        ImportedExcelData importer = new ImportedExcelData();

        private string _path;
        private bool _solved;

        public bool Solved
        {
            get { return _solved; }
            set { _solved = value; }
        }


        public List<int> CodeList = new List<int>();
        public List<string> MyStack = new List<string>();

       



        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
            MyStack.Add("E");
            MyStack.Add("#");
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void convertBtn_Click(object sender, RoutedEventArgs e)
        {
            string textToConvert = OriginalInputBox.Text;

            if(textToConvert != "" && textToConvert != " " && textToConvert != null)
            {
                string convertedText = helper.ConvertfromNumberToI(textToConvert);

                convertedText = helper.replaceText(convertedText, " ", "");
                convertedText = helper.replaceText(convertedText, "\t", "");
                convertedText += "#";

                ConvertedInputBox.Text = convertedText;
                msgLabel.Text = "Input converted successfully!";
            }
            else
            {
                msgLabel.Text = "The textput field is empty!";
            }
        }

        private void giveToTextBoxBtn_Click(object sender, RoutedEventArgs e)
        {
            solveArea.Text = "asd";
            CodeList.Clear();
            MyStack.Clear();

            string textToConvert = ConvertedInputBox.Text;

            if (textToConvert != "" && textToConvert != " " && textToConvert != null)
            {
                string convertedText = helper.ConvertfromNumberToI(textToConvert);

                convertedText = helper.replaceText(convertedText, " ", "");
                convertedText = helper.replaceText(convertedText, "\t", "");


                if ( !convertedText.EndsWith("#"))
                {
                    convertedText += "#";
                }
                
                if (MyStack.Count() == 0)
                {
                    MyStack.Add("E");
                    MyStack.Add("#");
                }

                string charsToString = String.Join("", MyStack);
                string numsToString = String.Join("", CodeList);

                string itemToSolve = String.Format("({0} ,{1}, {2})",convertedText, charsToString, numsToString);

                solveArea.Text = itemToSolve;
                msgLabel.Text = "Input converted successfully!";
            }
            else
            {
                msgLabel.Text = "The textput field is empty!";
            }

            if(PathInputBox.Text != "" && ConvertedInputBox.Text != "")
            {
                solveBtn.IsEnabled = true;
            }
            
        }
    
        private void openFileDialog_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            
            if (openFileDialog.ShowDialog() == true)
            {
                Path = openFileDialog.FileName;
                PathInputBox.Text = Path;

                dataGrid.ItemsSource = importer.ReadCSV(Path);
                dataGrid.Visibility = Visibility.Visible;
            }
        }

        private void solve_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                importer.readCsvToGetTheRules(_path);

                string inputText = ConvertedInputBox.Text;

                int rowIndex = 0;
                int colIndex = 0;

                string res;

                while (!Solved)
                {
                    var searchedColItem = "";
                    var searchedRowItem = inputText[0];

                    if (MyStack.First().Length > 1)
                    {
                        if (MyStack.First()[1] != '\'') //1
                        {
                            searchedColItem = MyStack.First()[0].ToString();
                        }

                        else
                        {
                            searchedColItem = MyStack.First();
                        }
                    }
                    else
                    {
                        searchedColItem = MyStack.First();
                    }

                    string charsToString;
                    string numsToString;

                    rowIndex = importer.headerRow.FindIndex(s => s.Contains(searchedRowItem));
                    colIndex = importer.firstCol.FindIndex(s => s.Contains(searchedColItem));

                    res = importer.Rules[colIndex][rowIndex];


                    switch (res)
                    {
                        case "accept":
                            solveArea.Text += "\nAccept";
                            msgLabel.Text = "Input solved successfully!";
                            Solved = true;
                            break;

                        case "err":
                            solveArea.Text += "\nErr";
                            msgLabel.Text = "Wrong syntax. Can't solve this input with theese rules.";
                            Solved = true;
                            break;

                        case "pop":
                            inputText = inputText.Substring(1, inputText.Length - 1);

                            if (MyStack[0].Length > 1)
                            {
                                MyStack[0] = MyStack[0].Substring(1);
                            }
                            else
                            {
                                MyStack.RemoveAt(0);
                            }


                            charsToString = String.Join("", MyStack);
                            numsToString = String.Join("", CodeList);

                            string itemToSolve = String.Format("({0} ,{1}, {2})", inputText, charsToString, numsToString);
                            solveArea.Text += "\n" + itemToSolve;

                            break;

                        default:
                            res = res.Substring(1, res.Length - 2);

                            string itemToAppend = res.Split('|')[0];
                            string codeToAppend = res.Split('|')[1];

                            if (itemToAppend == "epsz")
                            {
                                MyStack.RemoveAt(0);
                            }
                            else if (MyStack.First().Length != 1)
                            {
                                string temp = MyStack.First().Substring(1);

                                if (temp == "\'")
                                {
                                    MyStack.RemoveAt(0);
                                    MyStack.Remove(temp);
                                    MyStack.Insert(0, itemToAppend);
                                }

                                else
                                {
                                    MyStack.RemoveAt(0);
                                    MyStack.Insert(0, temp);
                                    MyStack.Insert(0, itemToAppend);
                                }
                            }
                            else
                            {
                                MyStack.RemoveAt(0);
                                MyStack.Insert(0, itemToAppend);
                            }


                            CodeList.Add(int.Parse(codeToAppend));

                            charsToString = String.Join("", MyStack);
                            numsToString = String.Join("", CodeList);

                            itemToSolve = String.Format("({0} ,{1}, {2})", inputText, charsToString, numsToString);
                            solveArea.Text += "\n" + itemToSolve;

                            break;
                    }
                }
                Solved = false;
                solveBtn.IsEnabled = false;
                msgLabel.Text = "Finsihed";
            }
            catch (InvalidOperationException)
            {
                msgLabel.Text = "Something went wrong. Please check the if the input field are not empty.";
            }
            catch (NullReferenceException)
            {
                msgLabel.Text = "please import the csv thwt contains your rules!";
            }
        }
    }
}
