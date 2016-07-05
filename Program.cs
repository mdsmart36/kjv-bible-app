using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildKJVDataTable
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlDataManager sdm = new SqlDataManager();
            string filename = @"c:\Users\Matthew\Downloads\bible.txt\kjv-standard.txt";
            // sdm.standardizeKjvText();
            sdm.buildKjvTable(filename);
        }
    }
}
