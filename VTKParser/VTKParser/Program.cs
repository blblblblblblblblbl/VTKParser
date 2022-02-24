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
        public enum DataType
        {
            None,
            FieldData,
            PointData,
            CellData,
            Points,
            Cells
        }
        #endregion


        public List<VTKDataArray> DataArray { get; set; }

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
        protected string rawData;
        protected string outputData;

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
                //if (String.Compare(data_save_format_info, "ASCII") == 0)
                //{
                //    SaveFormatType = DataSaveFormat.ASCII;
                //}
                //else
                //{
                //    SaveFormatType = DataSaveFormat.binary;
                //}
            }//set SaveFormatType
            {
                string data_set_structure_info = ((raw_data_line_format[3]).Split(new char[] { ' ' }))[1];//в троке два слова dataset и нужное, берем второе
                SetStructureType = (DataSetStructure)Enum.Parse(typeof(DataSetStructure), data_set_structure_info);

            }//set SetStructureType
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
            string FilePathR = "C://Users//stitc//Documents//GitHub//VTKParser//VTKParser//examples//beam-quad.vtk";
            string FilePathW = "C://Users//stitc//Documents//GitHub//VTKParser//VTKParser//examples//test.vtk";
            VTKParser parser = new VTKParser();
            parser.Read(FilePathR);
            parser.RawDataProcess();
            parser.Write(FilePathW);
            Console.ReadKey();
        }
    }
}
