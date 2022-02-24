using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using PetroGM.DataIO;
using PetroGM.DataIO.VTK;

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
            Int8,
            UInt8,
            Int16,
            UInt16,
            Int32,
            UInt32,
            Int64,
            UInt64,
            Float32,
            Float64,
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
            for (int i = 0; i < data1arr.Length; ++i)
            {
                output[i] = (T)Convert.ChangeType(data1arr[i], typeof(T));
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
            Console.WriteLine();
            //{
            //    string dim = ((data[0]).Remove(0, "DIMENSIONS ".Length)).Trim();
            //    string[] dimarr = dim.Split(new char[] { ' ' });
            //    dimensions = new int[dimarr.Length];
            //    for (int i = 0; i < dimarr.Length; ++i)
            //    {
            //        dimensions[i] = Convert.ToInt32(dimarr[i]);
            //    }
            //}// dimensions parse
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
                SaveFormatType = (DataSaveFormat)Enum.Parse(typeof(DataSaveFormat), data_save_format_info);
            }//set SaveFormatType
            {
                string data_set_structure_info = ((raw_data_line_format[3]).Split(new char[] { ' ' }))[1];//в троке два слова dataset и нужное, берем второе
                SetStructureType = (DataSetStructure)Enum.Parse(typeof(DataSetStructure), data_set_structure_info);

            }//set SetStructureType
            // теперь надоработать со списком DataArray помещать в него инфу там всякие point cells по листам
            //сначала посчитать ключевые слова в файле, так мы поймем сколько будет ячеек у листа
            this.raw_data_for_processing_line_format = new string[raw_data_line_format.Length - 4];
            Array.Copy(raw_data_line_format, 4, this.raw_data_for_processing_line_format, 0, raw_data_for_processing_line_format.Length);
            switch (SetStructureType)
            {
                case DataSetStructure.None:
                    break;
                case DataSetStructure.STRUCTURED_POINTS:
                    {
                        this.Structured_Points_Initialization();
                        break;
                    }
                case DataSetStructure.STRUCTURED_GRID:
                    break;
                case DataSetStructure.UNSTRUCTURED_GRID:
                    break;
                case DataSetStructure.POLYDATA:
                    break;
                case DataSetStructure.RECTILINEAR_GRID:
                    break;
                case DataSetStructure.FIELD:
                    break;
                default:
                    break;
            }

        }
        public void RawDataProcess()
        {
            this.DataInitialization();
            this.outputData = this.header + "\n" + this.title + "\n" + this.SaveFormatType + "\n" + this.SetStructureType;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string FilePathR = "C://Users//stitc//Documents//GitHub//VTKParser//VTKParser//examples//structured_points.vtk";
            string FilePathW = "C://Users//stitc//Documents//GitHub//VTKParser//VTKParser//examples//test.vtk";
            VTKParser parser = new VTKParser();
            parser.Read(FilePathR);
            parser.RawDataProcess();
            parser.Write(FilePathW);
            Console.ReadKey();
        }
    }
}
