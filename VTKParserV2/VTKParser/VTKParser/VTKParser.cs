using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using PetroGM.DataIO;

namespace VTKParser
{
    class VTKParser : BaseParser
    {
       
        #region file_info
        protected string Header;
        protected string Title;

        public enum EncodingType
        {
            none,
            raw,
            base64
        }
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
        protected string RawData;
        protected string[] RawDataFile;
        protected string[] RawDataForProcessing;
        protected List<string[]> RawDataAttributeList = new List<string[]>();
        private DataSet dataset;
        private List<Attributes> attributesList = new List<Attributes>();
        //Attributes attributes;

        protected string OutputData;
        public List<VTKDataArray> DataArray { get; set; }

        public EncodingType EncodType { get; set; } = EncodingType.none;
        public long NumberOfPoints { get; set; } = 0;
        public long NumberOfCells { get; set; } = 0;
        public bool IsLittleEndian { get; set; } = true;
        #endregion

        #region functions
        public override void Read(in string filepath)
        {
            try
            {
                base.Read(filepath);
                using (FileStream stream = File.OpenRead(filepath))
                {
                    byte[] array = new byte[stream.Length];
                    stream.Read(array, 0, array.Length);
                    RawData = System.Text.Encoding.Default.GetString(array);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        //read all file and transfer info to rawData
        public override void Write(in string filepath)
        {
            try
            {
                using (FileStream stream = new FileStream(filepath, FileMode.Truncate/*OpenOrCreate*/))
                {
                    byte[] array = System.Text.Encoding.Default.GetBytes(OutputData);
                    stream.Write(array, 0, array.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        //write prepared string to file
        protected void DataFormat()
        {
            RawData = String.Join(" ", RawData.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            RawData = Regex.Replace(RawData, @"^\s*\r?\n|\r?\n(?!\s*\S)", "", RegexOptions.Multiline);
        }
        //убирает все лишние пробелы и пустые строки
        public static T[] StringToNumbersParse<T>(in string data, in string remove_text = "")
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
        //берет строку убирает из нее часть текста из начала строки и возвращает массив чисел T[]
        protected void StructuredPointsInitialization()
        {
            StructuredPoints dataset = new StructuredPoints();
            dataset.Parse(RawDataForProcessing);
            dataset = dataset;
        }
        protected void StructuredGridInitialization()
        {
            StructuredGrid dataset = new StructuredGrid();
            dataset.Parse(RawDataForProcessing);
            dataset = dataset;
        }
        protected void UnstructuredGridInitialization()
        {
            UnstructuredGrid dataset = new UnstructuredGrid();
            dataset.Parse(RawDataForProcessing);
            dataset = dataset;

        }
        protected void PolydataInitialization()
        {
            PolyData dataset = new PolyData();
            dataset.Parse(RawDataForProcessing);
            dataset = dataset;

        }
        protected void RectilinearGridInitialization()
        {
            RectilinearGrid dataset = new RectilinearGrid();
            dataset.Parse(RawDataForProcessing);
            dataset = dataset;
        }
        protected void AttributeProcess()
        {
            for (int i = 0; i < RawDataAttributeList.Count; ++i)
            {
                Attributes attributes = new Attributes();
                attributes.Parse(RawDataAttributeList[i]);
                attributesList.Add(attributes);
            }

        }
        protected void FileInfoProcess()
        {
            string[] data = RawDataFile;
            {
                string header_info = data[0];
                Header = header_info;
            }//set header
            {
                string title_info = data[1];
                Title = title_info;
            }//set title
            {
                string data_save_format_info = data[2];
                SaveFormatType = (DataSaveFormat)Enum.Parse(typeof(DataSaveFormat), data_save_format_info);
            }//set SaveFormatType
            {
                string data_set_structure_info = ((data[3]).Split(new char[] { ' ' }))[1];//в троке два слова dataset и нужное, берем второе
                SetStructureType = (DataSetStructure)Enum.Parse(typeof(DataSetStructure), data_set_structure_info);

            }//set SetStructureType
        }
        protected void DataProcess()
        {
            switch (SetStructureType)
            {
                case DataSetStructure.None:
                    break;
                case DataSetStructure.STRUCTURED_POINTS:
                    {
                        StructuredPointsInitialization();
                        break;
                    }
                case DataSetStructure.STRUCTURED_GRID:
                    {
                        StructuredGridInitialization();
                        break;
                    }
                case DataSetStructure.UNSTRUCTURED_GRID:
                    {
                        UnstructuredGridInitialization();
                        break;
                    }
                case DataSetStructure.POLYDATA:
                    {
                        PolydataInitialization();
                        break;
                    }
                case DataSetStructure.RECTILINEAR_GRID:
                    {
                        RectilinearGridInitialization();
                        break;
                    }
                default:
                    break;
            }
        }
        protected void DataInitialization()
        {
            DataFormat();
            const int fileInfoLines = 4;
            string[] rawDataLineFormat = RawData.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            List<int> attributeLine = new List<int>();
            for (int i = 0; i < rawDataLineFormat.Length; ++i)
            {
                if (rawDataLineFormat[i].StartsWith("POINT_DATA") || rawDataLineFormat[i].StartsWith("CELL_DATA"))
                {
                    attributeLine.Add(i);
                }
            }

            RawDataFile = new string[fileInfoLines];
            if (attributeLine.Count == 0)
            {
                RawDataForProcessing = new string[rawDataLineFormat.Length - RawDataFile.Length];
            }
            else
            {
                RawDataForProcessing = new string[rawDataLineFormat.Length - RawDataFile.Length - (rawDataLineFormat.Length - attributeLine[0])];
            }

            Array.Copy(rawDataLineFormat, 0, RawDataFile, 0, RawDataFile.Length);
            Array.Copy(rawDataLineFormat, fileInfoLines, RawDataForProcessing, 0, RawDataForProcessing.Length);

            if (attributeLine.Count != 0)
            {
                for (int i = 0; i < attributeLine.Count - 1; ++i)
                {
                    RawDataAttributeList.Add(new string[(attributeLine)[i + 1] - (attributeLine)[i]]);
                    Array.Copy(rawDataLineFormat, attributeLine[i], RawDataAttributeList[i], 0, RawDataAttributeList[i].Length);
                }
                RawDataAttributeList.Add(new string[rawDataLineFormat.Length - (attributeLine)[attributeLine.Count - 1]]);
                Array.Copy(rawDataLineFormat, attributeLine[attributeLine.Count - 1], RawDataAttributeList[attributeLine.Count - 1], 0, RawDataAttributeList[attributeLine.Count - 1].Length);
            }
        }
        //берет сырые данные ввиде строки и сортирует их по строкам с инфой о файле, о данных, о атрибутах
        public void RawDataProcess()
        {
            DataInitialization();
            FileInfoProcess();
            DataProcess();
            AttributeProcess();

        }
        public string FileInfo()
        {
            string output = "";
            output += Header + "\r\n" + Title + "\r\n" + SaveFormatType + "\r\n" + "DATASET " + SetStructureType + "\r\n";
            return output;
        }
        public void Output(string FilePathW)
        {
            OutputData = FileInfo() + dataset.StringData();
            for (int i = 0; i < attributesList.Count; ++i)
            {
                OutputData += attributesList[i].StringData();
            }
            Write(FilePathW);
        }
        #endregion
    }
}
