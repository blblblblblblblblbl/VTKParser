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

namespace SharedProject
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
            None,
            SCALARS,
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
        DataSet dataset;
        Attributes attributes;

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
        protected void DataFormat()
        {
            this.rawData = String.Join(" ", this.rawData.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            this.rawData = Regex.Replace(this.rawData, @"^\s*\r?\n|\r?\n(?!\s*\S)", "", RegexOptions.Multiline);
        }
        public static T[] StringToNumbersParse<T>(in string data, in string remove_text = "")//, out T[] output)
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
        protected void StructuredPointsInitialization()
        {
            StructuredPoints dataset = new StructuredPoints();
            dataset.Parse(this.raw_data_for_processing_line_format);
            this.dataset = dataset;
        }
        protected void StructuredGridInitialization()
        {
            StructuredGrid dataset = new StructuredGrid();
            dataset.Parse(this.raw_data_for_processing_line_format);
            this.dataset = dataset;
        }
        protected void UntructuredGridInitialization()
        {
            UnstructuredGrid dataset = new UnstructuredGrid();
            dataset.Parse(this.raw_data_for_processing_line_format);
            this.dataset = dataset;

        }
        protected void PolydataInitialization()
        {
            PolyData dataset = new PolyData();
            dataset.Parse(this.raw_data_for_processing_line_format);
            this.dataset = dataset;

        }
        protected void RectilinearGridInitialization()
        {
            RectilinearGrid dataset = new RectilinearGrid();
            dataset.Parse(this.raw_data_for_processing_line_format);
            this.dataset = dataset;
        }
        protected void AttributeProcess()
        {
            Attributes attributes = new Attributes();
            attributes.Parse(this.raw_data_attribute_line_format);
            this.attributes = attributes;
        }
        protected void FileInfoProcess()
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
        protected void DataProcess()
        {
            switch (this.SetStructureType)
            {
                case DataSetStructure.None:
                    break;
                case DataSetStructure.STRUCTURED_POINTS:
                    {
                        this.StructuredPointsInitialization();
                        break;
                    }
                case DataSetStructure.STRUCTURED_GRID:
                    {
                        this.StructuredGridInitialization();
                        break;
                    }
                case DataSetStructure.UNSTRUCTURED_GRID:
                    {
                        this.UntructuredGridInitialization();
                        break;
                    }
                case DataSetStructure.POLYDATA:
                    {
                        this.PolydataInitialization();
                        break;
                    }
                case DataSetStructure.RECTILINEAR_GRID:
                    {
                        this.RectilinearGridInitialization();
                        break;
                    }
                default:
                    break;
            }
        }
        protected void DataInitialization()
        {
            this.DataFormat();
            const int file_info_lines = 4;
            string[] raw_data_line_format = this.rawData.Split(new char[] { '\n' });
            int attribute_line = raw_data_line_format.Length - 1;
            for (int i = 0; i < raw_data_line_format.Length; ++i)
            {
                if (raw_data_line_format[i].StartsWith("POINT_DATA") || raw_data_line_format[i].StartsWith("CELL_DATA"))
                {
                    attribute_line = i;
                    break;
                }
            }

            this.raw_data_file_info_line_format = new string[file_info_lines];
            this.raw_data_attribute_line_format = new string[raw_data_line_format.Length - attribute_line];
            this.raw_data_for_processing_line_format = new string[raw_data_line_format.Length - this.raw_data_attribute_line_format.Length - this.raw_data_file_info_line_format.Length];

            Array.Copy(raw_data_line_format, 0, this.raw_data_file_info_line_format, 0, this.raw_data_file_info_line_format.Length);
            Array.Copy(raw_data_line_format, file_info_lines, this.raw_data_for_processing_line_format, 0, this.raw_data_for_processing_line_format.Length);
            Array.Copy(raw_data_line_format, attribute_line, this.raw_data_attribute_line_format, 0, this.raw_data_attribute_line_format.Length);

        }
        public void RawDataProcess()
        {
            this.DataInitialization();
            this.FileInfoProcess();
            this.DataProcess();
            this.AttributeProcess();
            //this.outputData = attributes.string_data();
        }
        public string FileInfo()
        {
            string output = "";
            output += this.header + this.title + this.SaveFormatType + "\n" + this.SetStructureType + "\n";
            return output;
        }
        public void Output(string FilePathW)
        {
            this.outputData = FileInfo() + dataset.StringData() + attributes.StringData();
            Write(FilePathW);
        }
    }
}
