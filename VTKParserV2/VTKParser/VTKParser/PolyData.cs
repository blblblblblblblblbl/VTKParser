using System;
using System.Collections.Generic;
using PetroGM.DataIO.VTK;

namespace VTKParser
{
    class PolyData : DataSet
    {
        private VtkDataArray _points;
        private List<VtkDataArray> _dataArray;
        public override void Parse(string[] data)
        {
            int linecounter = 0;
            _points = new VtkDataArray();
            _points.Name = "POINTS";
            _points.NumberOfTuples = Convert.ToInt32((data[linecounter].Split(' '))[1]);
            _points.NumberOfComponents = 3;
            _points.Type = (VtkParser.ValueType)Enum.Parse(typeof(VtkParser.ValueType), (data[linecounter].Split(' '))[2], ignoreCase: true);
            ++linecounter;
            object arr;
            arr = new double[_points.NumberOfTuples, _points.NumberOfComponents];
            for (int i = 0; i < _points.NumberOfTuples; ++i)
            {
                var temp = VtkParser.StringToNumbersParse<double>(data[linecounter]);
                for (int k = 0; k < _points.NumberOfComponents; k++)
                {
                    ((double[,])arr)[i, k] = temp[k];
                }
                ++linecounter;
            }
            _points.Data.Add(arr);
            _dataArray = new List<VtkDataArray>();
            while (linecounter < data.Length - 1)
            {
                if (data[linecounter].StartsWith("VERTICES") || data[linecounter].StartsWith("LINES") || data[linecounter].StartsWith("POLYGONS") || data[linecounter].StartsWith("TRIANGLE_STRIPS"))
                {
                    VtkDataArray temp = new VtkDataArray();
                    temp.Name = (data[linecounter].Split(' '))[0];
                    temp.NumberOfTuples = Convert.ToInt32((data[linecounter].Split(' '))[1]);
                    temp.DataSize = Convert.ToInt32((data[linecounter].Split(' '))[2]);
                    double[][] tempArr = new double[temp.NumberOfTuples][];
                    ++linecounter;
                    for (int i = 0; i < temp.NumberOfTuples; ++i)
                    {
                        tempArr[i] = VtkParser.StringToNumbersParse<double>(data[linecounter]);
                        ++linecounter;
                    }
                    temp.Data.Add(tempArr);
                    _dataArray.Add(temp);
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
                string spoints = _points.Name + " " + $"{_points.NumberOfTuples}" + " " + $"{_points.Type}".ToLower() + "\r\n";
                for (int i = 0; i < _points.NumberOfTuples; ++i)
                {
                    for (int k = 0; k < _points.NumberOfComponents; k++)
                    {
                        spoints += $"{((double[,])(_points.Data[0]))[i, k]} ";
                    }
                    spoints=spoints.Trim();
                    spoints += "\r\n";
                }
                output += spoints;
            }
            {
                string stemp = "";
                int length = _dataArray.Count;
                for (int i = 0; i < length; ++i)
                {
                    stemp += _dataArray[i].Name + " " + _dataArray[i].NumberOfTuples + " " + _dataArray[i].DataSize + "\r\n";
                    for (int k = 0; k < _dataArray[i].NumberOfTuples; ++k)
                    {
                        foreach (double el in ((double[][])(_dataArray[i].Data[0]))[k])
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
