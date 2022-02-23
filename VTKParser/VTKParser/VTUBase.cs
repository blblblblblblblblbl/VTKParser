using System;
using System.Collections.Generic;
using System.Text;

namespace PetroGM.DataIO.VTK
{
    class VTUBase : BaseParser
    {
        public enum CompressorType
        {
            None,
            vtkZLibDataCompressor,
            vtkLZ4DataCompressor,
            vtkLZMADataCompressor
        }
        public enum EncodingType
        {
            none,
            raw,
            base64
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

        public enum DataSaveFormat
        {
            none,
            appended,
            ascii,
            binary
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
        public List<VTKDataArray> DataArray { get; set; }
        //header info
        public ValueType HeaderType { get; set; } = ValueType.None;
        public CompressorType CompressType { get; set; } = CompressorType.None;
        //data info
        public EncodingType EncodType { get; set; } = EncodingType.none;
        protected string rawData;
        public long NumberOfPoints { get; set; } = 0;
        public long NumberOfCells { get; set; } = 0;
        public bool IsLittleEndian { get; set; } = true;
    }
}
