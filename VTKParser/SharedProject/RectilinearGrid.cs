using System;
using System.Collections.Generic;
using System.Text;
using PetroGM.DataIO.VTK;

namespace SharedProject
{
    class RectilinearGrid : DataSet
    {
        private int[] dimensions;
        private List<VTKDataArray> DataArray = new List<VTKDataArray>();
        public override void Parse(string[] data)
        {
            int linecounter = 0;
            dimensions = VTKParser.StringToNumbersParse<int>(data[linecounter], "DIMENSIONS");
            ++linecounter;
            VTKDataArray temp;
            for (int i = 0; i < 3; ++i)
            {
                temp = new VTKDataArray();
                temp.Name = (data[linecounter].Split(' '))[0];
                temp.DataSize = Convert.ToInt32((data[linecounter].Split(' '))[1]);
                temp.Type = (VTKParser.ValueType)Enum.Parse(typeof(VTKParser.ValueType), (data[linecounter].Split(' '))[2], ignoreCase: true);
                ++linecounter;
                temp.Data.Add(VTKParser.StringToNumbersParse<double>(data[linecounter], ""));
                ++linecounter;
                DataArray.Add(temp);
            }
        }
        public override string StringData()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            string output = "";
            {
                string sdimensions = "";
                foreach (int el in dimensions)
                {
                    sdimensions += $"{el} ";
                }
                output += "DIMENSIONS " + sdimensions + "\r\n";
            }
            {
                string sdata = "";
                for (int i = 0; i < DataArray.Count; ++i)
                {
                    sdata += $"{DataArray[i].Name}" + " " + $"{DataArray[i].DataSize}" + " " + $"{DataArray[i].Type}".ToLower() + "\r\n";
                    foreach (double el in (double[])DataArray[i].Data[0])
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
