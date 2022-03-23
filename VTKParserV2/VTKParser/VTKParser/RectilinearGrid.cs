using System;
using System.Collections.Generic;
using PetroGM.DataIO.VTK;

namespace VTKParser
{
    class RectilinearGrid : DataSet
    {
        private int[] _dimensions;
        private List<VtkDataArray> _dataArray;
        public override void Parse(string[] data)
        {
            _dataArray = new List<VtkDataArray>();
            int linecounter = 0;
            _dimensions = VtkParser.StringToNumbersParse<int>(data[linecounter], "DIMENSIONS");
            ++linecounter;
            VtkDataArray temp;
            for (int i = 0; i < 3; ++i)
            {
                temp = new VtkDataArray();
                temp.Name = (data[linecounter].Split(' '))[0];
                temp.DataSize = Convert.ToInt32((data[linecounter].Split(' '))[1]);
                temp.Type = (VtkParser.ValueType)Enum.Parse(typeof(VtkParser.ValueType), (data[linecounter].Split(' '))[2], ignoreCase: true);
                ++linecounter;
                temp.Data.Add(VtkParser.StringToNumbersParse<double>(data[linecounter]));
                ++linecounter;
                _dataArray.Add(temp);
            }
        }
        public override string StringData()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            string output = "";
            {
                string sdimensions = "";
                foreach (int el in _dimensions)
                {
                    sdimensions += $"{el} ";
                }
                output += "DIMENSIONS " + sdimensions + "\r\n";
            }
            {
                string sdata = "";
                for (int i = 0; i < _dataArray.Count; ++i)
                {
                    sdata += $"{_dataArray[i].Name}" + " " + $"{_dataArray[i].DataSize}" + " " + $"{_dataArray[i].Type}".ToLower() + "\r\n";
                    foreach (double el in (double[])_dataArray[i].Data[0])
                    {
                        sdata += $"{el} ";
                    }
                    sdata=sdata.Trim();
                    sdata += "\r\n";
                }
                output += sdata;
            }
            return output;
        }
    }
}
