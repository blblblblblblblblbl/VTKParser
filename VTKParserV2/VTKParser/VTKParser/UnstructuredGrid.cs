using System;
using PetroGM.DataIO.VTK;

namespace VTKParser
{
    class UnstructuredGrid : DataSet
    {
        private VtkDataArray _points;
        private VtkDataArray _cells;
        private VtkDataArray _сellTypes;
        public override void Parse(string[] data)
        {
            int linecounter = 0;
            {
                _points = new VtkDataArray();
                _points.Name = "POINTS";
                _points.NumberOfComponents = 3;
                _points.NumberOfTuples = Convert.ToInt32((data[linecounter].Split(' '))[1]);
                _points.DataSize = _points.NumberOfComponents * _points.NumberOfTuples;
                _points.Type = (VtkParser.ValueType)Enum.Parse(typeof(VtkParser.ValueType), (data[linecounter].Split(' '))[2], ignoreCase: true);
                ++linecounter;
                object arr =new double[_points.NumberOfTuples, _points.NumberOfComponents];
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
            }//_points
            {
                _cells = new VtkDataArray();
                _cells.Name = (data[linecounter].Split(' '))[0];
                _cells.NumberOfTuples = Convert.ToInt32((data[linecounter].Split(' '))[1]);
                _cells.DataSize = Convert.ToInt32((data[linecounter].Split(' '))[2]);
                double[][] tempArr = new double[_cells.NumberOfTuples][];
                ++linecounter;
                for (int i = 0; i < _cells.NumberOfTuples; ++i)
                {
                    tempArr[i] = VtkParser.StringToNumbersParse<double>(data[linecounter]);
                    ++linecounter;
                }
                _cells.Data.Add(tempArr);
            }//_cells
            {
                _сellTypes = new VtkDataArray();
                _сellTypes.Name = (data[linecounter].Split(' '))[0];
                _сellTypes.NumberOfComponents = Convert.ToInt32((data[linecounter].Split(' '))[1]);
                int[] cellTypesArr = new int[_сellTypes.NumberOfComponents];
                ++linecounter;
                for (int i = 0; i < _сellTypes.NumberOfComponents; ++i)
                {
                    cellTypesArr[i] = Convert.ToInt32(data[linecounter]);
                    ++linecounter;
                }
                _сellTypes.Data.Add(cellTypesArr);
            }//_сellTypes
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
                string scells = "";
                scells += _cells.Name + " " + $"{_cells.NumberOfTuples}" + " " + $"{_cells.DataSize}" + "\r\n";
                for (int i = 0; i < _cells.NumberOfTuples; ++i)
                {
                    foreach (double el in ((double[][])(_cells.Data[0]))[i])
                    {
                        scells += $"{el} ";
                    }
                    scells=scells.Trim();
                    scells += "\r\n";
                }
                output += scells;
            }
            {
                string scellTypes = "";
                scellTypes += _сellTypes.Name + " " + $"{_сellTypes.NumberOfComponents}" + "\r\n";
                foreach (int el in (int[])_сellTypes.Data[0])
                {
                    scellTypes += $"{el}\r\n";
                }
                output += scellTypes;
            }
            return output;
        }
    }
}
