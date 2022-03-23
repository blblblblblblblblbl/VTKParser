using System;
using PetroGM.DataIO.VTK;

namespace VTKParser
{
    class StructuredGrid : DataSet

    {
        private VtkDataArray _points;
        private int[] _dimensions;
        public override void Parse(string[] data)
        {
            int linecounter = 0;
            _dimensions = VtkParser.StringToNumbersParse<int>(data[linecounter], "DIMENSIONS");//, out dimensions);
            ++linecounter;
            _points = new VtkDataArray();
            _points.Name = "POINTS";
            _points.NumberOfTuples = Convert.ToInt32((data[linecounter].Split(' '))[1]);//_points.DataSize;
            _points.DataSize = _points.NumberOfTuples * _dimensions.Length;
            _points.NumberOfComponents = _dimensions.Length;
            _points.Type = (VtkParser.ValueType)Enum.Parse(typeof(VtkParser.ValueType), (data[linecounter].Split(' '))[2], ignoreCase: true);
            ++linecounter;
            object arr;
            switch (_points.Type)
            {
                //case ValueType.None:
                //    {
                //        SByte[,] arr = null;
                //        break;
                //    }
                //case ValueType.Int8:
                //    {
                //        arr = new SByte[_points.DataSize, dimensions.Length];
                //        for (int i = 0; i < _points.DataSize; ++i)
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
                //        Byte[,] arr = new Byte[_points.DataSize, dimensions.Length];
                //        break;
                //    }
                //case ValueType.Int16:
                //    {
                //        Int16[,] arr = new Int16[_points.DataSize, dimensions.Length];
                //        break;
                //    }
                //case ValueType.UInt16:
                //    {
                //        UInt16[,] arr = new UInt16[_points.DataSize, dimensions.Length];
                //        break;
                //    }
                //case ValueType.Int32:
                //    {
                //        Int32[,] arr = new Int32[_points.DataSize, dimensions.Length];
                //        break;
                //    }
                //case ValueType.UInt32:
                //    {
                //        UInt32[,] arr = new UInt32[_points.DataSize, dimensions.Length];
                //        break;
                //    }
                //case ValueType.Int64:
                //    {
                //        Int64[,] arr = new Int64[_points.DataSize, dimensions.Length];
                //        break;
                //    }
                //case ValueType.UInt64:
                //    {
                //        UInt64[,] arr = new UInt64[_points.DataSize, dimensions.Length];
                //        break;
                //    }
                //case ValueType.Float32:
                //    {
                //        float[,] arr = new float[_points.DataSize, dimensions.Length];
                //        break;
                //    }
                case VtkParser.ValueType.Double:
                    {
                        arr = new double[_points.NumberOfTuples, _dimensions.Length];
                        for (int i = 0; i < _points.NumberOfTuples; ++i)
                        {
                            var temp = VtkParser.StringToNumbersParse<double>(data[linecounter]);
                            for (int k = 0; k < _dimensions.Length; k++)
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
            _points.Data.Add(arr);

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

            output += _points.Name + " " + $"{_points.NumberOfTuples}" + " " + $"{_points.Type}".ToLower() + "\r\n";
            for (int i = 0; i < _points.NumberOfTuples; ++i)
            {
                for (int k = 0; k < _dimensions.Length; k++)
                {
                    output += $"{((double[,])(_points.Data[0]))[i, k]} ";
                }
                output=output.Trim();
                output += "\r\n";
            }

            return output;
        }
    }
}
