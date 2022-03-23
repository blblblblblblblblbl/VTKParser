using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using PetroGM.DataIO;
using PetroGM.DataIO.VTK;

namespace VTKParser
{
    class VtkParser : BaseParser
    {
       
        #region file_info
        private string _header;
        private string _title;

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
        private string _rData;//rawData
        private string[] _rDataF;//rawDataFile
        private string[] _rDataForP;//rawDataForProcessing
        private List<string[]> _rDataAttrL;//rawDataAttributeList
        private DataSet _dataset;
        private List<Attributes> _attrL;//attributesList

        private string _outputData;

        public EncodingType EncodType { get; set; } = EncodingType.none;
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
                    int count =stream.Read(array, 0, array.Length);
                    if (count != stream.Length)
                    {
                        throw new ArgumentException("different count of bytes");
                    }

                    _rData = System.Text.Encoding.Default.GetString(array);
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
                    byte[] array = System.Text.Encoding.Default.GetBytes(_outputData);
                    stream.Write(array, 0, array.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        //write prepared string to file
        private void DataFormat()
        {
            _rData = String.Join(" ", _rData.Split(new char[' '], StringSplitOptions.RemoveEmptyEntries));
            _rData = Regex.Replace(_rData, @"^\s*\r?\n|\r?\n(?!\s*\S)", "", RegexOptions.Multiline);
        }
        //убирает все лишние пробелы и пустые строки
        public static T[] StringToNumbersParse<T>(in string data, in string removeText = "")
        {
            string data1 = ((data).Remove(0, removeText.Length)).Trim();
            string[] data1Arr = data1.Split(new char[] { ' ' });
            T[] output = new T[data1Arr.Length];
            var culture = CultureInfo.GetCultureInfo("en-US");
            for (int i = 0; i < data1Arr.Length; ++i)
            {
                try
                {
                    output[i] = (T)Convert.ChangeType(double.Parse(data1Arr[i], culture), typeof(T));
                }
                catch (Exception)
                {
                    if (CultureInfo.Equals(culture, CultureInfo.GetCultureInfo("en-US")))
                    {
                        culture = CultureInfo.GetCultureInfo("ru-RU");
                    }
                    else
                    {
                        culture = CultureInfo.GetCultureInfo("en-US");
                    }
                    output[i] = (T)Convert.ChangeType(double.Parse(data1Arr[i], culture), typeof(T));
                }
            }
            return output;
        }
        //берет строку убирает из нее часть текста из начала строки и возвращает массив чисел T[]
        private void AttributesParse()
        {
            _attrL = new List<Attributes>();
            for (int i = 0; i < _rDataAttrL.Count; ++i)
            {
                Attributes attributes = new Attributes();
                attributes.Parse(_rDataAttrL[i]);
                _attrL.Add(attributes);
            }

        }
        private void FileInfoParse()
        {
            string[] data = _rDataF;
            {
                string headerInfo = data[0];
                _header = headerInfo;
            }//set header
            {
                string titleInfo = data[1];
                _title = titleInfo;
            }//set title
            {
                string dataSaveFormatInfo = data[2];
                SaveFormatType = (DataSaveFormat)Enum.Parse(typeof(DataSaveFormat), dataSaveFormatInfo);
            }//set SaveFormatType
            {
                string dataSetStructureInfo = ((data[3]).Split(new char[] { ' ' }))[1];//в троке два слова _dataset и нужное, берем второе
                SetStructureType = (DataSetStructure)Enum.Parse(typeof(DataSetStructure), dataSetStructureInfo);

            }//set SetStructureType
        }
        //парсит из строк инфу о файле
        private void DataParse()
        {
            DataSet temp;
            switch (SetStructureType)
            {
                case DataSetStructure.None:
                {
                    temp = null;
                    break;
                }
                case DataSetStructure.STRUCTURED_POINTS:
                    {
                        temp = new StructuredPoints();
                        break;
                    }
                case DataSetStructure.STRUCTURED_GRID:
                    {
                        temp = new StructuredGrid();
                        break;
                    }
                case DataSetStructure.UNSTRUCTURED_GRID:
                    {
                        temp = new UnstructuredGrid();
                        break;
                    }
                case DataSetStructure.POLYDATA:
                    {
                        temp = new PolyData();
                        break;
                    }
                case DataSetStructure.RECTILINEAR_GRID:
                    {
                        temp = new RectilinearGrid();
                        break;
                    }
                default:
                {
                    temp = null;
                    break;
                }
            }

            _dataset = temp;
            if (_dataset != null)
            {
                _dataset.Parse(_rDataForP);
            }
        }
        private void DataInitialization()
        {
            DataFormat();
            const int file = 4;//fileInfoLines
            string[] rDataLF = _rData.Split(new [] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);//rDataLF-rawDataLineFormat
            List<int> attrLine = new List<int>();//attribute lines
            for (int i = 0; i < rDataLF.Length; ++i)
            {
                if (rDataLF[i].StartsWith("POINT_DATA") || rDataLF[i].StartsWith("CELL_DATA"))
                {
                    attrLine.Add(i);
                }
            }

            _rDataF = new string[file];
            if (attrLine.Count == 0)
            {
                _rDataForP = new string[rDataLF.Length - _rDataF.Length];
            }
            else
            {
                _rDataForP = new string[rDataLF.Length - _rDataF.Length - (rDataLF.Length - attrLine[0])];
            }

            Array.Copy(rDataLF, 0, _rDataF, 0, _rDataF.Length);
            Array.Copy(rDataLF, file, _rDataForP, 0, _rDataForP.Length);

            if (attrLine.Count != 0)
            {
                _rDataAttrL = new List<string[]>();
                for (int i = 0; i < attrLine.Count - 1; ++i)
                {
                    _rDataAttrL.Add(new string[(attrLine)[i + 1] - (attrLine)[i]]);
                    Array.Copy(rDataLF, attrLine[i], _rDataAttrL[i], 0, _rDataAttrL[i].Length);
                }
                _rDataAttrL.Add(new string[rDataLF.Length - (attrLine)[attrLine.Count - 1]]);
                Array.Copy(rDataLF, attrLine[attrLine.Count - 1], _rDataAttrL[attrLine.Count - 1], 0, _rDataAttrL[attrLine.Count - 1].Length);
            }
        }
        //берет сырые данные ввиде строки и сортирует их по строкам с инфой о файле, о данных, о атрибутах
        public void Parse()
        {
            DataInitialization();
            FileInfoParse();
            DataParse();
            AttributesParse();
        }
        public string FileInfoToString()
        {
            string output = "";
            output += _header + "\r\n" + _title + "\r\n" + SaveFormatType + "\r\n" + "DATASET " + SetStructureType + "\r\n";
            return output;
        }
        //выводит инфу о файле в виде строки
        public void Output(string filePathW)
        {
            _outputData = FileInfoToString() + _dataset.StringData();
            for (int i = 0; i < _attrL.Count; ++i)
            {
                _outputData += _attrL[i].StringData();
            }
            Write(filePathW);
        }
        #endregion
    }
}
