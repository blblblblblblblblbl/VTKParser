using System;
using System.Collections.Generic;
using System.Text;
using PetroGM.DataIO.VTK;

namespace SharedProject
{
    class UnstructuredGrid : DataSet
    {
        private List<VTKDataArray> DataArray = new List<VTKDataArray>();
        private VTKDataArray points = new VTKDataArray();
        private VTKDataArray cells = new VTKDataArray();
        private VTKDataArray cell_types = new VTKDataArray();
        public override void Parse(string[] data)
        {
            int linecounter = 0;
            {
                points.Name = "POINTS";
                points.NumberOfComponents = 3;
                points.NumberOfTuples = Convert.ToInt32((data[linecounter].Split(' '))[1]);
                points.DataSize = points.NumberOfComponents * points.NumberOfTuples;
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
                DataArray.Add(points);
            }//points
            {
                cells.Name = (data[linecounter].Split(' '))[0];
                cells.NumberOfTuples = Convert.ToInt32((data[linecounter].Split(' '))[1]);
                cells.DataSize = Convert.ToInt32((data[linecounter].Split(' '))[2]);
                double[][] temp_arr = new double[cells.NumberOfTuples][];
                ++linecounter;
                for (int i = 0; i < cells.NumberOfTuples; ++i)
                {
                    temp_arr[i] = VTKParser.StringToNumbersParse<double>(data[linecounter], "");
                    ++linecounter;
                }
                cells.Data.Add(temp_arr);
                DataArray.Add(cells);
            }//cells
            {
                cell_types.Name = (data[linecounter].Split(' '))[0];
                cell_types.NumberOfComponents = Convert.ToInt32((data[linecounter].Split(' '))[1]);
                int[] cell_types_arr = new int[cell_types.NumberOfComponents];
                ++linecounter;
                for (int i = 0; i < cell_types.NumberOfComponents; ++i)
                {
                    cell_types_arr[i] = Convert.ToInt32(data[linecounter]);
                    ++linecounter;
                }
                cell_types.Data.Add(cell_types_arr);
                DataArray.Add(cell_types);
            }//cell_types
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
                string scells = "";
                scells += cells.Name + " " + $"{cells.NumberOfTuples}" + " " + $"{cells.DataSize}" + "\r\n";
                //int number_of_componets = 0;
                for (int i = 0; i < cells.NumberOfTuples; ++i)
                {
                    foreach (double el in ((double[][])(cells.Data[0]))[i])
                    {
                        scells += $"{el} ";
                    }
                    scells=scells.Trim();
                    scells += "\r\n";
                }
                output += scells;
            }
            {
                string scell_types = "";
                scell_types += cell_types.Name + " " + $"{cell_types.NumberOfComponents}" + "\r\n";
                foreach (int el in (int[])cell_types.Data[0])
                {
                    scell_types += $"{el}\r\n";
                }
                output += scell_types;
            }
            return output;
        }
    }
}
