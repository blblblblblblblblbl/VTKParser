using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PetroGM.DataIO;
using PetroGM.DataIO.VTK;
using System.Globalization;
using System.Text.RegularExpressions;

namespace VTKParser
{
    class Structured_Points_Class
    {
        private int[] dimensions;
        private int[] spacing;
        private int[] origin;
        private string spacing_name;
        public int[] Dimensions
        {
            get 
            {
                return this.dimensions;
            }
            set 
            {
                this.dimensions = value;
            }
        }
        public int[] Spacing
        {
            get
            {
                return this.spacing;
            }
            set
            {
                this.spacing = value;
            }
        }
        public int[] Origin
        {
            get
            {
                return this.origin;
            }
            set
            {
                this.origin = value;
            }
        }
        public string Spacing_name
        {
            get 
            {
                return this.spacing_name;
            }
            set 
            {
                this.spacing_name = value;
            }
        }
        public void Parse(string[] data)
        {
            int linecounter = 0;
            this.Dimensions = VTKParser.String_To_Numbers_Parse<int>(data[linecounter], "DIMENSIONS");//, out dimensions);
            ++linecounter;
            this.Origin = VTKParser.String_To_Numbers_Parse<int>(data[linecounter], "ORIGIN");
            ++linecounter;
            if (data[linecounter].StartsWith("ASPECT_RATIO"))
            {
                this.Spacing = VTKParser.String_To_Numbers_Parse<int>(data[linecounter], "ASPECT_RATIO");//, out aspect_ratio);
                this.Spacing_name = "ASPECT_RATIO ";
            }
            else
            {
                this.Spacing = VTKParser.String_To_Numbers_Parse<int>(data[linecounter], "SPACING");
            }
        }
        public string string_data()
        {
            string output = "";
            {
                string sdimensions = "";
                foreach (int el in dimensions)
                {
                    sdimensions += $"{el} ";
                }
                output += "DIMENSIONS "+sdimensions +"\n";
            }
            {
                string sorigin = "";
                foreach (int el in origin)
                {
                    sorigin += $"{el} ";
                }
                output += "ORIGIN "+sorigin + "\n";
            }
            {
                string sspacing = "";
                foreach (int el in spacing)
                {
                    sspacing += $"{el} ";
                }
                output += $"{this.spacing_name} " + sspacing + "\n";
            }
            return output;
        }
    }
    class Structured_Grid_Class

    {
        private int[] dimensions;
        private VTKDataArray points = new VTKDataArray();
        private int[] Dimensions
        {
            get
            {
                return this.dimensions;
            }
            set
            {
                this.dimensions = value;
            }
        }
        public void Parse(string[] data)
        {
            int linecounter = 0;
            //int[] dimensions;
            Dimensions = VTKParser.String_To_Numbers_Parse<int>(data[linecounter], "DIMENSIONS");//, out dimensions);
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
                //            var temp = String_To_Numbers_Parse<SByte>(data[linecounter], "");
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
                            var temp = VTKParser.String_To_Numbers_Parse<double>(data[linecounter], "");
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
        public string string_data()
        {
            string output="";
            {
                string sdimensions = "";
                foreach (int el in dimensions)
                {
                    sdimensions += $"{el} ";
                }
                output += "DIMENSIONS " + sdimensions + "\n";
            }

            output += points.Name + " " + $"{points.NumberOfTuples}" + " " + $"{points.Type}".ToLower() + "\n";
            for (int i = 0; i < points.NumberOfTuples; ++i)
            {
                for (int k = 0; k < dimensions.Length; k++)
                {
                    output +=$"{((double[,])(points.Data[0]))[i, k]} ";
                }
                output.Trim();
                output += "\n";
            }

            return output;
        }
    }
    class Unstructured_Grid_Class
    {
        private List<VTKDataArray> DataArray = new List<VTKDataArray>();
        private VTKDataArray points = new VTKDataArray();
        private VTKDataArray cells = new VTKDataArray();
        private VTKDataArray cell_types = new VTKDataArray();
        public void Parse(string[] data)
        {
            int linecounter = 0;
            {
                points.Name = "POINTS";
                points.NumberOfComponents = 3;
                points.NumberOfTuples = Convert.ToInt32((data[linecounter].Split(' '))[1]);
                points.DataSize = points.NumberOfComponents * points.NumberOfTuples;
                points.Type = (VTKParser.ValueType)Enum.Parse(typeof(ValueType), (data[linecounter].Split(' '))[2], ignoreCase: true);
                ++linecounter;
                object arr;
                arr = new double[points.NumberOfTuples, points.NumberOfComponents];
                for (int i = 0; i < points.NumberOfTuples; ++i)
                {
                    var temp = VTKParser.String_To_Numbers_Parse<double>(data[linecounter], "");
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
                    temp_arr[i] = VTKParser.String_To_Numbers_Parse<double>(data[linecounter], "");
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
        public string string_data()
        {
            string output = "";
            {
                string spoints = points.Name + " " + $"{points.NumberOfTuples}" + " " + $"{points.Type}".ToLower() + "\n";
                for (int i = 0; i < points.NumberOfTuples; ++i)
                {
                    for (int k = 0; k < points.NumberOfComponents; k++)
                    {
                        spoints += $"{((double[,])(points.Data[0]))[i, k]} ";
                    }
                    spoints.Trim();
                    spoints += "\n";
                }
                output += spoints;
            }
            {
                string scells = "";
                scells += cells.Name + " " + $"{cells.NumberOfTuples}" + " " + $"{cells.DataSize}" + "\n";
                //int number_of_componets = 0;
                for (int i = 0; i < cells.NumberOfTuples; ++i)
                {
                    foreach (double el in ((double[][])(cells.Data[0]))[i])
                    {
                        scells += $"{el} ";
                    }
                    scells.Trim();
                    scells += "\n";
                }
                output += scells;
            }
            {
                string scell_types = "";
                scell_types += cell_types.Name + " " + $"{cell_types.NumberOfComponents}" + "\n";
                foreach (int el in (int[])cell_types.Data[0])
                {
                    scell_types += $"{el}\n";
                }
                output += scell_types;
            }
            return output;
        }
    }
    class Polydata_Class
    {
        private VTKDataArray points = new VTKDataArray();
        private List<VTKDataArray> DataArray = new List<VTKDataArray>();
        public void Parse(string[] data)
        {
            int linecounter = 0;
            points.Name = "POINTS";
            points.NumberOfTuples = Convert.ToInt32((data[linecounter].Split(' '))[1]);
            points.NumberOfComponents = 3;
            points.Type = (VTKParser.ValueType)Enum.Parse(typeof(ValueType), (data[linecounter].Split(' '))[2], ignoreCase: true);
            ++linecounter;
            object arr;
            arr = new double[points.NumberOfTuples, points.NumberOfComponents];
            for (int i = 0; i < points.NumberOfTuples; ++i)
            {
                var temp = VTKParser.String_To_Numbers_Parse<double>(data[linecounter], "");
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
                        temp_arr[i] = VTKParser.String_To_Numbers_Parse<double>(data[linecounter], "");
                        ++linecounter;
                    }
                    temp.Data.Add(temp_arr);
                    DataArray.Add(temp);
                    --linecounter;
                }
                ++linecounter;
            }
        }
        public string string_data()
        {
            string output = "";
            {
                string spoints = points.Name + " " + $"{points.NumberOfTuples}" + " " + $"{points.Type}".ToLower() + "\n";
                for (int i = 0; i < points.NumberOfTuples; ++i)
                {
                    for (int k = 0; k < points.NumberOfComponents; k++)
                    {
                        spoints += $"{((double[,])(points.Data[0]))[i, k]} ";
                    }
                    spoints.Trim();
                    spoints += "\n";
                }
                output += spoints;
            }
            {
                string stemp = "";
                int length = DataArray.Count;
                for (int i = 0; i < length; ++i)
                {
                    stemp += DataArray[i].Name + " " + DataArray[i].NumberOfTuples + " " + DataArray[i].DataSize + "\n";
                    for (int k = 0; k < DataArray[i].NumberOfTuples; ++k)
                    {
                        foreach (double el in ((double[][])(DataArray[i].Data[0]))[k])
                        {
                            stemp += $"{el} ";
                        }
                        stemp.Trim();
                        stemp += "\n";
                    }
                    output += stemp;
                }
            }

            return output;
        }
    }
    class Rectilinear_Grid_Class
    {
        public int[] dimensions;
        public List<VTKDataArray> DataArray = new List<VTKDataArray>();
        public void Parse(string[] data)
        {
            int linecounter = 0;
            dimensions = VTKParser.String_To_Numbers_Parse<int>(data[linecounter], "DIMENSIONS");
            ++linecounter;
            VTKDataArray temp;
            for (int i = 0; i < 3; ++i)
            {
                temp = new VTKDataArray();
                temp.Name = (data[linecounter].Split(' '))[0];
                temp.DataSize = Convert.ToInt32((data[linecounter].Split(' '))[1]);
                temp.Type = (VTKParser.ValueType)Enum.Parse(typeof(VTKParser.ValueType), (data[linecounter].Split(' '))[2], ignoreCase: true);
                ++linecounter;
                temp.Data.Add(VTKParser.String_To_Numbers_Parse<double>(data[linecounter], ""));
                ++linecounter;
                DataArray.Add(temp);
            }
        }
        public string string_data()
        {
            string output = "";
            {
                string sdimensions = "";
                foreach (int el in dimensions)
                {
                    sdimensions += $"{el} ";
                }
                output += "DIMENSIONS " + sdimensions + "\n";
            }
            {
                string sdata = "";
                for (int i = 0; i < DataArray.Count; ++i)
                {
                    sdata += $"{DataArray[i].Name}" + " " + $"{DataArray[i].DataSize}" + " " + $"{DataArray[i].Type}".ToLower()+"\n";
                    foreach (double el in (double[])DataArray[i].Data[0])
                    {
                        sdata += $"{el} ";
                    }
                    sdata.Trim();
                    sdata += "\n";
                }
                output += sdata;
            }
            return output;
        }
    }
    class VTKParser : BaseParser
    {
        #region misunderstand

        #endregion
        #region enums
        public enum EncodingType
        {
            none,
            raw,
            base64
        }
        #endregion



        #region file_info
        protected string header;
        protected string title;

        public enum CompressorType
        {
            None,
            vtkZLibDataCompressor,
            vtkLZ4DataCompressor,
            vtkLZMADataCompressor
        }
        public enum DataSaveFormat
        {
            None,
            appended,
            ASCII,
            binary
        }
        public enum DataSetStructure
        {
            None,
            STRUCTURED_POINTS,
            STRUCTURED_GRID,
            UNSTRUCTURED_GRID,
            POLYDATA,
            RECTILINEAR_GRID,
        }

        public ValueType HeaderType { get; set; } = ValueType.None;
        public CompressorType CompressType { get; set; } = CompressorType.None;
        public DataSaveFormat SaveFormatType { get; set; } = DataSaveFormat.None;
        public DataSetStructure SetStructureType { get; set; } = DataSetStructure.None;
        #endregion

        #region data_info
        public enum DataType
        {
            None,
            FieldData,
            PointData,
            CellData,
            Points,
            Cells
        }
        public enum ValueType
        {
            None,
            Unsigned_char,
            Char,
            Unsigned_short,
            Short,
            Unsigned_int,
            Int,
            Unsigned_long,
            Long,
            Float,
            Double
        }
        public enum AttributeType
        {
            SCALARS = 1,
            COLOR_SCALARS,
            LOOKUP_TABLE,
            VECTORS,
            NORMALS,
            TEXTURE_COORDINATES,
            TENSORS,
            FIELD
        }
        protected string rawData;
        protected string[] raw_data_file_info_line_format;
        protected string[] raw_data_for_processing_line_format;
        protected string[] raw_data_attribute_line_format;
        object dataset;

        protected string outputData;
        public List<VTKDataArray> DataArray { get; set; }

        public EncodingType EncodType { get; set; } = EncodingType.none;
        public long NumberOfPoints { get; set; } = 0;
        public long NumberOfCells { get; set; } = 0;
        public bool IsLittleEndian { get; set; } = true;
        #endregion


        //новые функции
        public override void Read(in string filepath)//read all file and transfer info to rawData
        {
            try
            {
                base.Read(filepath);
                using (FileStream stream = File.OpenRead(filepath))
                {
                    byte[] array = new byte[stream.Length];
                    stream.Read(array, 0, array.Length);
                    this.rawData = System.Text.Encoding.Default.GetString(array);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public override void Write(in string filepath)//write prepared string to file
        {
            try
            {
                //base.CheckOnRead(filepath);
                //base.Read(filepath);
                using (FileStream stream = new FileStream(filepath, FileMode.OpenOrCreate))
                {
                    byte[] array = System.Text.Encoding.Default.GetBytes(this.outputData);
                    stream.Write(array, 0, array.Length);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        protected void data_format()
        {
            this.rawData = String.Join(" ", this.rawData.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            this.rawData = Regex.Replace(this.rawData, @"^\s*\r?\n|\r?\n(?!\s*\S)", "", RegexOptions.Multiline);
        }
        public static T[] String_To_Numbers_Parse<T>(in string data, in string remove_text="")//, out T[] output)
        {
            string data1 = ((data).Remove(0, remove_text.Length)).Trim();
            string[] data1arr = data1.Split(new char[] { ' ' });
            T[] output = new T[data1arr.Length];
            var culture = CultureInfo.GetCultureInfo("en-US");
            for (int i = 0; i < data1arr.Length; ++i)
            {
                //output[i] = (T)Convert.ChangeType(data1arr[i], typeof(T));
                try
                {
                    output[i] = (T)Convert.ChangeType(double.Parse(data1arr[i], culture), typeof(T));
                }
                catch (Exception e)
                {
                    if (culture == CultureInfo.GetCultureInfo("en-US"))
                    {
                        culture = CultureInfo.GetCultureInfo("ru-RU");
                    }
                    else
                    {
                        culture = CultureInfo.GetCultureInfo("en-US");
                    }
                    output[i] = (T)Convert.ChangeType(double.Parse(data1arr[i], culture), typeof(T));
                }
                //output[i] = (T)Convert.ChangeType(double.Parse(data1arr[i], CultureInfo.GetCultureInfo("en-US")), typeof(T));
            }
            return output;
        }
        protected void Structured_Points_Initialization()
        {
            Structured_Points_Class dataset = new Structured_Points_Class();
            dataset.Parse(this.raw_data_for_processing_line_format);
            this.dataset = dataset;
        }
        protected void Structured_Grid_Initialization()
        {
            Structured_Grid_Class dataset = new Structured_Grid_Class();
            dataset.Parse(this.raw_data_for_processing_line_format);
            this.dataset = dataset;
        }
        protected void Untructured_Grid_Initialization()
        {
            Unstructured_Grid_Class dataset = new Unstructured_Grid_Class();
            dataset.Parse(this.raw_data_for_processing_line_format);
            this.dataset = dataset;

        }
        protected void Polydata_Initialization()
        {
            Polydata_Class dataset = new Polydata_Class();
            dataset.Parse(this.raw_data_for_processing_line_format);
            this.dataset = dataset;

        }
        protected void Rectilinear_Grid_Initialization()
        {
            Rectilinear_Grid_Class dataset = new Rectilinear_Grid_Class();
            dataset.Parse(this.raw_data_for_processing_line_format);
            this.dataset = dataset;
        }
        protected void Field_Initialization()
        {
            string[] data = this.raw_data_for_processing_line_format;
            int linecounter = 0;
            string dataName = (data[linecounter].Split(' '))[1];
            int numArrays = Convert.ToInt32((data[linecounter].Split(' '))[2]);
            ++linecounter;
            VTKDataArray temp;
            for (int i = 1; i <= numArrays; ++i)
            {
                temp = new VTKDataArray();
                {
                    string[] fline = (data[linecounter].Split(' '));
                    temp.Name = fline[0];
                    temp.NumberOfComponents = Convert.ToInt32(fline[1]);
                    temp.NumberOfTuples = Convert.ToInt32(fline[2]);
                    temp.Type = (ValueType)Enum.Parse(typeof(ValueType), fline[3], ignoreCase: true);
                }
                ++linecounter;
                double[,] temparr = new double[temp.NumberOfTuples, temp.NumberOfComponents];
                double[] temptuple;
                for (int k = 0; k < temp.NumberOfTuples; ++k)
                {
                    temptuple = String_To_Numbers_Parse<double>(data[linecounter], "");
                    for (int j = 0; j < temptuple.Length; ++j)
                    {
                        temparr[k, j] = temptuple[j];
                    }
                    ++linecounter;
                }
                temp.Data.Add(temparr);
            }
        }
        protected void Attribute_Process()
        {
            string[] data = this.raw_data_attribute_line_format;
            int linecounter = 0;
            VTKDataArray temp;
            while (linecounter < data.Length)
            {
                switch ((AttributeType)Enum.Parse(typeof(AttributeType), data[linecounter].Split(' ')[0]))
                {
                    case AttributeType.SCALARS:
                        {
                            temp = new VTKDataArray();
                            temp.NumberOfTuples = this.DataArray[this.DataArray.Count() - 1].NumberOfTuples;
                            temp.Name = data[linecounter].Split(' ')[1];
                            temp.Type = (ValueType)Enum.Parse(typeof(ValueType), (data[linecounter].Split(' ')[2]), ignoreCase: true);
                            try
                            {
                                temp.NumberOfComponents = Convert.ToInt32(data[linecounter].Split(' ')[3]);
                            }
                            catch (Exception e)
                            {
                                temp.NumberOfComponents = 1;
                            }
                            ++linecounter;
                            double[,] temparr = new double[temp.NumberOfTuples, temp.NumberOfComponents];
                            double[] temptuple;
                            for (int k = 0; k < temp.NumberOfTuples; ++k)
                            {
                                temptuple = String_To_Numbers_Parse<double>(data[linecounter], "");
                                for (int j = 0; j < temptuple.Length; ++j)
                                {
                                    temparr[k, j] = temptuple[j];
                                }
                                ++linecounter;
                            }
                            temp.Data.Add(temparr);
                            this.DataArray.Add(temp);
                            break;
                        }
                    case AttributeType.COLOR_SCALARS:
                        {
                            temp = new VTKDataArray();
                            temp.NumberOfTuples = this.DataArray[this.DataArray.Count() - 1].NumberOfTuples;
                            temp.Name = data[linecounter].Split(' ')[1];
                            temp.NumberOfComponents = Convert.ToInt32(data[linecounter].Split(' ')[2]);
                            ++linecounter;
                            double[,] temparr = new double[temp.NumberOfTuples, temp.NumberOfComponents];
                            double[] temptuple;
                            for (int k = 0; k < temp.NumberOfTuples; ++k)
                            {
                                temptuple = String_To_Numbers_Parse<double>(data[linecounter], "");
                                for (int j = 0; j < temptuple.Length; ++j)
                                {
                                    temparr[k, j] = temptuple[j];
                                }
                                ++linecounter;
                            }
                            temp.Data.Add(temparr);
                            this.DataArray.Add(temp);
                            break;
                        }
                    case AttributeType.LOOKUP_TABLE:
                        {
                            temp = new VTKDataArray();
                            temp.Name = data[linecounter].Split(' ')[1];
                            temp.NumberOfTuples = Convert.ToInt32(data[linecounter].Split(' ')[2]);
                            temp.NumberOfComponents = 4;
                            ++linecounter;
                            double[,] temparr = new double[temp.NumberOfTuples, temp.NumberOfComponents];
                            double[] temptuple;
                            for (int k = 0; k < temp.NumberOfTuples; ++k)
                            {
                                temptuple = String_To_Numbers_Parse<double>(data[linecounter], "");
                                for (int j = 0; j < temptuple.Length; ++j)
                                {
                                    temparr[k, j] = temptuple[j];
                                }
                                ++linecounter;
                            }
                            temp.Data.Add(temparr);
                            this.DataArray.Add(temp);
                            break;
                        }
                    case AttributeType.VECTORS:
                        {
                            temp = new VTKDataArray();
                            temp.Name = data[linecounter].Split(' ')[1];
                            temp.Type = (ValueType)Enum.Parse(typeof(ValueType), (data[linecounter].Split(' ')[2]), ignoreCase: true);
                            temp.NumberOfTuples = this.DataArray[this.DataArray.Count() - 1].NumberOfTuples;
                            temp.NumberOfComponents = 3;
                            ++linecounter;
                            double[,] temparr = new double[temp.NumberOfTuples, temp.NumberOfComponents];
                            double[] temptuple;
                            for (int k = 0; k < temp.NumberOfTuples; ++k)
                            {
                                temptuple = String_To_Numbers_Parse<double>(data[linecounter], "");
                                for (int j = 0; j < temptuple.Length; ++j)
                                {
                                    temparr[k, j] = temptuple[j];
                                }
                                ++linecounter;
                            }
                            temp.Data.Add(temparr);
                            this.DataArray.Add(temp);
                            break;
                        }
                    case AttributeType.NORMALS:
                        {
                            temp = new VTKDataArray();
                            temp.Name = data[linecounter].Split(' ')[1];
                            temp.Type = (ValueType)Enum.Parse(typeof(ValueType), (data[linecounter].Split(' ')[2]), ignoreCase: true);
                            temp.NumberOfTuples = this.DataArray[this.DataArray.Count() - 1].NumberOfTuples;
                            temp.NumberOfComponents = 3;
                            ++linecounter;
                            double[,] temparr = new double[temp.NumberOfTuples, temp.NumberOfComponents];
                            double[] temptuple;
                            for (int k = 0; k < temp.NumberOfTuples; ++k)
                            {
                                temptuple = String_To_Numbers_Parse<double>(data[linecounter], "");
                                for (int j = 0; j < temptuple.Length; ++j)
                                {
                                    temparr[k, j] = temptuple[j];
                                }
                                ++linecounter;
                            }
                            temp.Data.Add(temparr);
                            this.DataArray.Add(temp);
                            break;
                        }
                    case AttributeType.TEXTURE_COORDINATES:
                        {
                            temp = new VTKDataArray();
                            temp.NumberOfTuples = this.DataArray[this.DataArray.Count() - 1].NumberOfTuples;
                            temp.Name = data[linecounter].Split(' ')[1];
                            temp.NumberOfComponents = Convert.ToInt32(data[linecounter].Split(' ')[2]);
                            temp.Type = (ValueType)Enum.Parse(typeof(ValueType), (data[linecounter].Split(' ')[3]), ignoreCase: true);
                            ++linecounter;
                            double[,] temparr = new double[temp.NumberOfTuples, temp.NumberOfComponents];
                            double[] temptuple;
                            for (int k = 0; k < temp.NumberOfTuples; ++k)
                            {
                                temptuple = String_To_Numbers_Parse<double>(data[linecounter], "");
                                for (int j = 0; j < temptuple.Length; ++j)
                                {
                                    temparr[k, j] = temptuple[j];
                                }
                                ++linecounter;
                            }
                            temp.Data.Add(temparr);
                            this.DataArray.Add(temp);
                            break;
                        }
                    case AttributeType.TENSORS:
                        {
                            temp = new VTKDataArray();
                            temp.NumberOfTuples = this.DataArray[this.DataArray.Count() - 1].NumberOfTuples;
                            temp.Name = data[linecounter].Split(' ')[1];
                            temp.Type = (ValueType)Enum.Parse(typeof(ValueType), (data[linecounter].Split(' ')[2]), ignoreCase: true);
                            ++linecounter;
                            temp.DataSize = 3;
                            double[,] temparr;
                            double[] temptuple;
                            for (int k = 0; k < temp.NumberOfTuples; ++k)
                            {
                                temparr = new double[temp.DataSize, temp.DataSize];
                                for (int i = 0; i < temp.DataSize; ++i)
                                {
                                    temptuple = String_To_Numbers_Parse<double>(data[linecounter], "");
                                    for (int j = 0; j < temptuple.Length; ++j)
                                    {
                                        temparr[i, j] = temptuple[j];
                                    }
                                    ++linecounter;
                                }
                                //++linecounter;
                                temp.Data.Add(temparr);
                            }
                            this.DataArray.Add(temp);
                            break;
                        }
                    case AttributeType.FIELD:
                        {
                            break;
                        }
                    default:
                        {
                            ++linecounter;
                            break;
                        }
                }
            }
        }
        protected void File_Info_Process()
        {
            string[] data = this.raw_data_file_info_line_format;
            {
                string header_info = data[0];
                this.header = header_info;
            }//set header
            {
                string title_info = data[1];
                this.title = title_info;
            }//set title
            {
                string data_save_format_info = data[2];
                this.SaveFormatType = (DataSaveFormat)Enum.Parse(typeof(DataSaveFormat), data_save_format_info);
            }//set SaveFormatType
            {
                string data_set_structure_info = ((data[3]).Split(new char[] { ' ' }))[1];//в троке два слова dataset и нужное, берем второе
                this.SetStructureType = (DataSetStructure)Enum.Parse(typeof(DataSetStructure), data_set_structure_info);

            }//set SetStructureType
        }
        protected void Data_Process()
        {
            switch (this.SetStructureType)
            {
                case DataSetStructure.None:
                    break;
                case DataSetStructure.STRUCTURED_POINTS:
                    {
                        this.Structured_Points_Initialization();
                        break;
                    }
                case DataSetStructure.STRUCTURED_GRID:
                    {
                        this.Structured_Grid_Initialization();
                        break;
                    }
                case DataSetStructure.UNSTRUCTURED_GRID:
                    {
                        this.Untructured_Grid_Initialization();
                        break;
                    }
                case DataSetStructure.POLYDATA:
                    {
                        this.Polydata_Initialization();
                        break;
                    }
                case DataSetStructure.RECTILINEAR_GRID:
                    {
                        this.Rectilinear_Grid_Initialization();
                        break;
                    }
                default:
                    break;
            }
        }
        protected void DataInitialization()
        {
            this.data_format();
            const int file_info_lines = 4;
            string[] raw_data_line_format = this.rawData.Split(new char[] { '\n' });
            int attribute_line = raw_data_line_format.Length-1;
            for (int i = 0; i < raw_data_line_format.Length; ++i)
            {
                if (raw_data_line_format[i].StartsWith("POINT_DATA") || raw_data_line_format[i].StartsWith("CELL_DATA"))
                {
                    attribute_line = i;
                    break;
                }
            }

            this.raw_data_file_info_line_format = new string[file_info_lines];
            this.raw_data_attribute_line_format = new string[raw_data_line_format.Length - (attribute_line + 1)];
            this.raw_data_for_processing_line_format = new string[raw_data_line_format.Length - this.raw_data_attribute_line_format.Length-this.raw_data_file_info_line_format.Length];

            Array.Copy(raw_data_line_format, 0, this.raw_data_file_info_line_format, 0, this.raw_data_file_info_line_format.Length);
            Array.Copy(raw_data_line_format, file_info_lines, this.raw_data_for_processing_line_format, 0, this.raw_data_for_processing_line_format.Length);
            Array.Copy(raw_data_line_format,attribute_line,this.raw_data_attribute_line_format,0,this.raw_data_attribute_line_format.Length);

        }
        public void RawDataProcess()
        {
            this.DataInitialization();
            this.File_Info_Process();
            this.Data_Process();
            //this.Attribute_Process();
        }
    }
    class ProgramV2
    {
        static T[] returnmass<T>(int n)
        {
            T[] arr = new T[n];
            return arr;
        }
        static void Main(string[] args)
        {
            //string FilePathR = "C://Users//stitc//Documents//GitHub//VTKParser//VTKParser//examples//field.txt";
            //string FilePathW = "C://Users//stitc//Documents//GitHub//VTKParser//VTKParser//examples//test.txt";
            string FilePathR = "C://Users//stitc//Documents//VTKParser//VTKParser//examples//structured_points.vtk";
            string FilePathW = "C://Users//stitc//Documents//VTKParser//VTKParser//examples//test.txt";
            VTKParser parser = new VTKParser();
            parser.Read(FilePathR);
            parser.RawDataProcess();
            parser.Write(FilePathW);
            Console.ReadKey();
        }
    }
}
