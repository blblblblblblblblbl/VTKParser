using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PetroGM.DataIO;
using PetroGM.DataIO.VTK;
using System.Globalization;

namespace VTKParser
{
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
            FIELD
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
        protected string rawData;
        protected string[] raw_data_for_processing_line_format;
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
        protected  T[] String_To_Numbers_Parse<T> (in string data, in string remove_text)//, out T[] output)
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
            string[] data = this.raw_data_for_processing_line_format;
            int linecounter = 0;
            string dimensions_name = "DIMENSIONS";
            int[] dimensions;
            dimensions = this.String_To_Numbers_Parse<int>(data[linecounter], dimensions_name);//, out dimensions);
            ++linecounter;
            string spacing_name = "SPACING";
            int[] spacing;
            if (data[linecounter].StartsWith("ASPECT_RATIO"))
            {
                spacing = this.String_To_Numbers_Parse<int>(data[linecounter], "ASPECT_RATIO");//, out aspect_ratio);
                spacing_name = "ASPECT_RATIO ";
            }
            else 
            {
                spacing = this.String_To_Numbers_Parse<int>(data[linecounter], "SPACING");
            }
            ++linecounter;
            string origin_name = "ORIGIN";
            int[] origin;
            origin = this.String_To_Numbers_Parse<int>(data[linecounter], origin_name);
            ++linecounter;
            string point_data_name = "POINT_DATA";
            int[] point_data;
            point_data = this.String_To_Numbers_Parse<int>(data[linecounter], point_data_name);
            //все дальше уже атрибуы идут
        }
        protected void Structured_Grid_Initialization() 
        {
            string[] data = this.raw_data_for_processing_line_format;
            int linecounter = 0;
            string dimensions_name = "DIMENSIONS";
            int[] dimensions;
            dimensions = this.String_To_Numbers_Parse<int>(data[linecounter], dimensions_name);//, out dimensions);
            ++linecounter;
            VTKDataArray points = new VTKDataArray();
            points.Name= "POINTS";
            points.DataSize = Convert.ToInt32((data[linecounter].Split(' '))[1]);
            //Console.WriteLine((ValueType)Enum.Parse(typeof(ValueType), (data[linecounter].Split(' '))[2], ignoreCase:true));
            points.Type = (ValueType)Enum.Parse(typeof(ValueType), (data[linecounter].Split(' '))[2], ignoreCase: true);
            //Type T = Type.GetType("Int32");
            //T[,] arr1 = new T[points.DataSize, dimensions.Length];
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
                //            var temp = this.String_To_Numbers_Parse<SByte>(data[linecounter], "");
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
                case ValueType.Double:
                    {
                        arr = new double[points.DataSize, dimensions.Length];
                        for (int i = 0; i < points.DataSize; ++i)
                        {
                            var temp = this.String_To_Numbers_Parse<double>(data[linecounter], "");
                            for (int k = 0; k < dimensions.Length; k++)
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

            //points.Data.Add()
            //points.Data = new (Type)points.Type[points.DataSize, dimensions.Length];

        }
        protected void Untructured_Grid_Initialization()
        {
            string[] data = this.raw_data_for_processing_line_format;
            int linecounter = 0;
            this.DataArray = new List<VTKDataArray>();
            {
                VTKDataArray points = new VTKDataArray();
                points.Name = "POINTS";
                points.DataSize = Convert.ToInt32((data[linecounter].Split(' '))[1]);
                points.Type = (ValueType)Enum.Parse(typeof(ValueType), (data[linecounter].Split(' '))[2], ignoreCase: true);
                ++linecounter;
                object arr;
                int dimensions = 3;
                arr = new double[points.DataSize, dimensions];
                for (int i = 0; i < points.DataSize; ++i)
                {
                    var temp = this.String_To_Numbers_Parse<double>(data[linecounter], "");
                    for (int k = 0; k < dimensions; k++)
                    {
                        ((double[,])arr)[i, k] = temp[k];
                    }
                    ++linecounter;
                }
                points.Data.Add(arr);
                this.DataArray.Add(points);
            }//points
            {
                VTKDataArray cells = new VTKDataArray();
                cells.Name = (data[linecounter].Split(' '))[0];
                cells.NumberOfComponents = Convert.ToInt32((data[linecounter].Split(' '))[1]);
                cells.DataSize = Convert.ToInt32((data[linecounter].Split(' '))[2]);
                double[][] temp_arr = new double[cells.NumberOfComponents][];
                ++linecounter;
                for (int i = 0; i < cells.NumberOfComponents; ++i)
                {
                    temp_arr[i] = this.String_To_Numbers_Parse<double>(data[linecounter], "");
                    ++linecounter;
                }
                cells.Data.Add(temp_arr);
                this.DataArray.Add(cells);
            }//cells
            {
                VTKDataArray cell_types = new VTKDataArray();
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
                this.DataArray.Add(cell_types);
            }//cell_types

        }
        protected void Structured_Polydata_Initialization()
        {
            string[] data = this.raw_data_for_processing_line_format;
            int linecounter = 0;
            VTKDataArray points = new VTKDataArray();
            points.Name = "POINTS";
            points.DataSize = Convert.ToInt32((data[linecounter].Split(' '))[1]);
            points.Type = (ValueType)Enum.Parse(typeof(ValueType), (data[linecounter].Split(' '))[2], ignoreCase: true);
            ++linecounter;
            object arr;
            int dimensions = 3;
            arr = new double[points.DataSize, dimensions];
            for (int i = 0; i < points.DataSize; ++i)
            {
                var temp = this.String_To_Numbers_Parse<double>(data[linecounter], "");
                for (int k = 0; k < dimensions; k++)
                {
                    ((double[,])arr)[i, k] = temp[k];
                }
                ++linecounter;
            }
            points.Data.Add(arr);
            this.DataArray = new List<VTKDataArray>();
            this.DataArray.Add(points);
            while (linecounter<data.Length-1)
            {
                if (data[linecounter].StartsWith("VERTICES") || data[linecounter].StartsWith("LINES") || data[linecounter].StartsWith("POLYGONS") || data[linecounter].StartsWith("TRIANGLE_STRIPS"))
                {
                    VTKDataArray temp = new VTKDataArray();
                    temp.Name = (data[linecounter].Split(' '))[0];
                    //temp.DataSize = Convert.ToInt32((data[linecounter].Split(' '))[1]);
                    temp.NumberOfComponents = Convert.ToInt32((data[linecounter].Split(' '))[1]);
                    temp.DataSize = Convert.ToInt32((data[linecounter].Split(' '))[2]);
                    double[][] temp_arr = new double[temp.NumberOfComponents][];
                    //temp.NumberOfComponents = Convert.ToInt32((data[linecounter].Split(' '))[2]);
                    ++linecounter;
                    for (int i = 0; i < temp.NumberOfComponents; ++i)
                    {
                        temp_arr[i]= this.String_To_Numbers_Parse<double>(data[linecounter], "");
                        ++linecounter;
                    }
                    temp.Data.Add(temp_arr);
                    this.DataArray.Add(temp);
                    --linecounter;
                }
                ++linecounter;
            }

        }
        protected void Rectilinear_Grid_Initialization()
        {
            string[] data = this.raw_data_for_processing_line_format;
            int linecounter = 0;
            string dimensions_name = "DIMENSIONS";
            int[] dimensions;
            dimensions = this.String_To_Numbers_Parse<int>(data[linecounter], dimensions_name);//, out dimensions);
            ++linecounter;
            VTKDataArray xarr = new VTKDataArray();
            VTKDataArray yarr = new VTKDataArray();
            VTKDataArray zarr = new VTKDataArray();
            xarr.Name = "X_COORDINATES";
            yarr.Name = "Y_COORDINATES";
            zarr.Name = "Z_COORDINATES";

            xarr.DataSize = Convert.ToInt32((data[linecounter].Split(' '))[1]);
            ++linecounter;
            //object Data;//= new double[xarr.DataSize];
            //Data = this.String_To_Numbers_Parse<double>(data[linecounter], "");
            xarr.Data.Add(this.String_To_Numbers_Parse<double>(data[linecounter], ""));
            xarr.Type = (ValueType)Enum.Parse(typeof(ValueType), (data[linecounter].Split(' '))[2], ignoreCase: true);

            ++linecounter;
            yarr.DataSize = Convert.ToInt32((data[linecounter].Split(' '))[1]);
            yarr.Type = (ValueType)Enum.Parse(typeof(ValueType), (data[linecounter].Split(' '))[2], ignoreCase: true);
            ++linecounter;
            yarr.Data.Add(this.String_To_Numbers_Parse<double>(data[linecounter], ""));

            ++linecounter;
            zarr.DataSize = Convert.ToInt32((data[linecounter].Split(' '))[1]);
            zarr.Type = (ValueType)Enum.Parse(typeof(ValueType), (data[linecounter].Split(' '))[2], ignoreCase: true);
            ++linecounter;
            zarr.Data.Add(this.String_To_Numbers_Parse<double>(data[linecounter], ""));
            //Console.ReadKey();

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
                    temptuple = this.String_To_Numbers_Parse<double>(data[linecounter], "");
                    for (int j = 0; j<temptuple.Length; ++j)
                    {
                        temparr[k, j] = temptuple[j];
                    }
                    ++linecounter;
                }
                temp.Data.Add(temparr);
            }
        }
        protected void Attribute_Initialization()
        { 
        }
        protected void DataInitialization()//проинициализировать прочитать там заголовок, описание,определить какие там данные 
        {
            string[] raw_data_line_format = this.rawData.Split(new char[] { '\n' });
            {
                string header_info = raw_data_line_format[0];
                this.header = header_info;
            }//set header
            {
                string title_info = raw_data_line_format[1];
                this.title = title_info;
            }//set title
            {
                string data_save_format_info = raw_data_line_format[2];
                this.SaveFormatType = (DataSaveFormat)Enum.Parse(typeof(DataSaveFormat), data_save_format_info);
            }//set SaveFormatType
            {
                string data_set_structure_info = ((raw_data_line_format[3]).Split(new char[] { ' ' }))[1];//в троке два слова dataset и нужное, берем второе
                this.SetStructureType = (DataSetStructure)Enum.Parse(typeof(DataSetStructure), data_set_structure_info);

            }//set SetStructureType
            // теперь надоработать со списком DataArray помещать в него инфу там всякие point cells по листам
            //сначала посчитать ключевые слова в файле, так мы поймем сколько будет ячеек у листа
            this.raw_data_for_processing_line_format = new string[raw_data_line_format.Length - 4];
            Array.Copy(raw_data_line_format, 4, this.raw_data_for_processing_line_format, 0, raw_data_for_processing_line_format.Length);
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
                        this.Structured_Polydata_Initialization();
                        break;
                    }
                case DataSetStructure.RECTILINEAR_GRID:
                    {
                        this.Rectilinear_Grid_Initialization();
                        break;
                    }
                case DataSetStructure.FIELD:
                    {
                        this.DataArray = new List<VTKDataArray>();

                        this.Field_Initialization();
                        break;
                    }
                default:
                    break;
            }
            this.Attribute_Initialization();

        }
        public void RawDataProcess()
        {
            this.DataInitialization();
            this.outputData = this.header + "\n" + this.title + "\n" + this.SaveFormatType + "\n" + this.SetStructureType;
        }
    }
    class Program
    {
        private T[] returnmass<T>(int n)
        {
            T[] arr = new T[n];
            return arr;
        }
        static void Main(string[] args)
        {
            string FilePathR = "C://Users//stitc//Documents//GitHub//VTKParser//VTKParser//examples//field.txt";
            string FilePathW = "C://Users//stitc//Documents//GitHub//VTKParser//VTKParser//examples//test.txt";
            //string FilePathR = "C://Users//stitc//Documents//VTKParser//VTKParser//examples//POLYDATA.txt";
            //string FilePathW = "C://Users//stitc//Documents//VTKParser//VTKParser//examples//test.txt";
            VTKParser parser = new VTKParser();
            parser.Read(FilePathR);
            parser.RawDataProcess();
            parser.Write(FilePathW);
            Console.ReadKey();
        }
    }
}
