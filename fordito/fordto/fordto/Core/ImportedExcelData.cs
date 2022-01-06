using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace fordto.Core
{
    class ImportedExcelData
    {

        public string[] rows = null;
        #region variables for datagrid
        public string firstRow { get; set; }
        public string secondRow { get; set; }
        public string thirdRow { get; set; }
        public string fourthrow { get; set; }
        public string fifthrow { get; set; }
        public string sixthrow { get; set; }
        public string seventhrow { get; set; }
        #endregion

        public List<string> firstCol = new List<string>();
        public List<string> headerRow = new List<string>();
        private string[][] _rules;

        public string[][] Rules
        {
            get { return _rules; }
            set { _rules = value; }
        }

        public ImportedExcelData() { }
        public ImportedExcelData(string FirstRow, string SecondRow, string ThirdRow, string FourthRow, string FifthRow, string SixthRow, string SeventhRow)
        {
            firstRow = FirstRow;
            secondRow = SecondRow;
            thirdRow = ThirdRow;
            fourthrow = FourthRow;
            fifthrow = FifthRow;
            sixthrow = SixthRow;
            seventhrow = SeventhRow;
        }

        public IEnumerable<ImportedExcelData> ReadCSV(string path)
        {
            try
            {
                string[] rows = File.ReadAllLines(Path.ChangeExtension(path, ".csv"));

                return rows.Select(line =>
                {
                    string[] data = line.Split(';');
                    return new ImportedExcelData(data[0], data[1], data[2], data[3], data[4], data[5], data[6]);
                });
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("You can only use csv files!");
                return null;
            }

        }

        public void readCsvToGetTheRules(string path)
        {
            try
            {
                var rows = File.ReadAllLines(Path.ChangeExtension(path, ".csv"))
                    .Select(l => l.Split(';').ToArray()).ToArray();

                foreach (var item in rows)
                {
                    firstCol.Add(item[0]);
                }

                for (int i = 0; i < rows[0].Length; i++)
                {
                    headerRow.Add(rows[0][i]);
                }

                this.Rules = rows;
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Please impoort the csv wich contains the solver-table!");
                this.Rules = null;
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("There is nothing to solve!");
                this.Rules = null;
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("It seems that your csv is empty.");
                this.Rules = null;
            }

        }
    }
}