using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace fordto.Core
{
    public class TranslatorHelper
    {
        public string ConvertfromNumberToI(string str)
        {
            return Regex.Replace(str, "([0-9])+", "i");
        }

        public string replaceText(string input, string search, string replace)
        {
            while (input.Contains(search))
            {
                input = input.Replace(search, replace);
            }
            return input;
        }

        
    }
}
