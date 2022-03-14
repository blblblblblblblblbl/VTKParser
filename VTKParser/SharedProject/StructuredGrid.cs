﻿using System;
using System.Collections.Generic;
using System.Text;
using PetroGM.DataIO.VTK;

namespace SharedProject
{
    class StructuredGrid : DataSet

    {
        private VTKDataArray points = new VTKDataArray();
        private int[] Dimensions { get; set; }
        public override void Parse(string[] data)
        {
            int linecounter = 0;
            //int[] dimensions;
            Dimensions = VTKParser.StringToNumbersParse<int>(data[linecounter], "DIMENSIONS");//, out dimensions);
            ++linecounter;
            //VTKDataArray points = new VTKDataArray();
            points.Name = "POINTS";
            points.NumberOfTuples = Convert.ToInt32((data[linecounter].Split(' '))[1]);//points.DataSize;
            points.DataSize = points.NumberOfTuples * Dimensions.Length;
            points.NumberOfComponents = Dimensions.Length;
            points.Type = (VTKParser.ValueType)Enum.Parse(typeof(VTKParser.ValueType), (data[linecounter].Split(' '))[2], ignoreCase: true);
            ++linecounter;
            object arr;
            switch (points.Type)
            {
                //case ValueType.None:
                //    {
                //        SByte[,] arr = null;
                //        break;
                //    }
                //case ValueType.Int8:
                //    {
                //        arr = new SByte[points.DataSize, dimensions.Length];
                //        for (int i = 0; i < points.DataSize; ++i)
                //        {
                //            var temp = StringToNumbersParse<SByte>(data[linecounter], "");
                //            for (int k = 0; k < dimensions.Length; k++)
                //            {
                //                ((SByte[,])arr)[i, k] = temp[k];
                //            }
                //            ++linecounter;
                //        }
                //        break;
                //    }
                //case ValueType.UInt8:
                //    {
                //        Byte[,] arr = new Byte[points.DataSize, dimensions.Length];
                //        break;
                //    }
                //case ValueType.Int16:
                //    {
                //        Int16[,] arr = new Int16[points.DataSize, dimensions.Length];
                //        break;
                //    }
                //case ValueType.UInt16:
                //    {
                //        UInt16[,] arr = new UInt16[points.DataSize, dimensions.Length];
                //        break;
                //    }
                //case ValueType.Int32:
                //    {
                //        Int32[,] arr = new Int32[points.DataSize, dimensions.Length];
                //        break;
                //    }
                //case ValueType.UInt32:
                //    {
                //        UInt32[,] arr = new UInt32[points.DataSize, dimensions.Length];
                //        break;
                //    }
                //case ValueType.Int64:
                //    {
                //        Int64[,] arr = new Int64[points.DataSize, dimensions.Length];
                //        break;
                //    }
                //case ValueType.UInt64:
                //    {
                //        UInt64[,] arr = new UInt64[points.DataSize, dimensions.Length];
                //        break;
                //    }
                //case ValueType.Float32:
                //    {
                //        float[,] arr = new float[points.DataSize, dimensions.Length];
                //        break;
                //    }
                case VTKParser.ValueType.Double:
                    {
                        arr = new double[points.NumberOfTuples, Dimensions.Length];
                        for (int i = 0; i < points.NumberOfTuples; ++i)
                        {
                            var temp = VTKParser.StringToNumbersParse<double>(data[linecounter], "");
                            for (int k = 0; k < Dimensions.Length; k++)
                            {
                                ((double[,])arr)[i, k] = temp[k];
                            }
                            ++linecounter;
                        }
                        break;
                    }
                default:
                    {
                        arr = null;
                        break;
                    }
            }
            points.Data.Add(arr);

        }
        public override string StringData()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            string output = "";
            {
                string sdimensions = "";
                foreach (int el in Dimensions)
                {
                    sdimensions += $"{el} ";
                }
                output += "DIMENSIONS " + sdimensions + "\r\n";
            }

            output += points.Name + " " + $"{points.NumberOfTuples}" + " " + $"{points.Type}".ToLower() + "\r\n";
            for (int i = 0; i < points.NumberOfTuples; ++i)
            {
                for (int k = 0; k < Dimensions.Length; k++)
                {
                    output += $"{((double[,])(points.Data[0]))[i, k]} ";
                }
                output=output.Trim();
                output += "\r\n";
            }

            return output;
        }
    }
}
