using System;
using System.Collections.Generic;
using System.Text;
using PetroGM.DataIO.VTK;

namespace SharedProject
{
    class PolyData : DataSet
    {
        private VTKDataArray points = new VTKDataArray();
        private List<VTKDataArray> DataArray = new List<VTKDataArray>();
        public override void Parse(string[] data)
        {
            int linecounter = 0;
            points.Name = "POINTS";
            points.NumberOfTuples = Convert.ToInt32((data[linecounter].Split(' '))[1]);
            points.NumberOfComponents = 3;
            points.Type = (VTKParser.ValueType)Enum.Parse(typeof(VTKParser.ValueType), (data[linecounter].Split(' '))[2], ignoreCase: true);
            ++linecounter;
            object arr;
            arr = new double[points.NumberOfTuples, points.NumberOfComponents];
            for (int i = 0; i < points.NumberOfTuples; ++i)
            {
                var temp = VTKParser.StringToNumbersParse<double>(data[linecounter], "");
                for (int k = 0; k < points.NumberOfComponents; k++)
                {
                    ((double[,])arr)[i, k] = temp[k];
                }
                ++linecounter;
            }
            points.Data.Add(arr);
            while (linecounter < data.Length - 1)
            {
                if (data[linecounter].StartsWith("VERTICES") || data[linecounter].StartsWith("LINES") || data[linecounter].StartsWith("POLYGONS") || data[linecounter].StartsWith("TRIANGLE_STRIPS"))
                {
                    VTKDataArray temp = new VTKDataArray();
                    temp.Name = (data[linecounter].Split(' '))[0];
                    temp.NumberOfTuples = Convert.ToInt32((data[linecounter].Split(' '))[1]);
                    temp.DataSize = Convert.ToInt32((data[linecounter].Split(' '))[2]);
                    double[][] temp_arr = new double[temp.NumberOfTuples][];
                    ++linecounter;
                    for (int i = 0; i < temp.NumberOfTuples; ++i)
                    {
                        temp_arr[i] = VTKParser.StringToNumbersParse<double>(data[linecounter], "");
                        ++linecounter;
                    }
                    temp.Data.Add(temp_arr);
                    DataArray.Add(temp);
                    --linecounter;
                }
                ++linecounter;
            }
        }
        public override string StringData()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            string output = "";
            {
                string spoints = points.Name + " " + $"{points.NumberOfTuples}" + " " + $"{points.Type}".ToLower() + "\r\n";
                for (int i = 0; i < points.NumberOfTuples; ++i)
                {
                    for (int k = 0; k < points.NumberOfComponents; k++)
                    {
                        spoints += $"{((double[,])(points.Data[0]))[i, k]} ";
                    }
                    spoints=spoints.Trim();
                    spoints += "\r\n";
                }
                output += spoints;
            }
            {
                string stemp = "";
                int length = DataArray.Count;
                for (int i = 0; i < length; ++i)
                {
                    stemp += DataArray[i].Name + " " + DataArray[i].NumberOfTuples + " " + DataArray[i].DataSize + "\r\n";
                    for (int k = 0; k < DataArray[i].NumberOfTuples; ++k)
                    {
                        foreach (double el in ((double[][])(DataArray[i].Data[0]))[k])
                        {
                            stemp += $"{el} ";
                        }
                        stemp=stemp.Trim();
                        stemp += "\r\n";
                    }
                    output += stemp;
                }
            }

            return output;
        }
    }
}
