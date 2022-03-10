using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetroGM.DataIO.VTK;

namespace VTKParser
{
    class VTKData
    {
        public enum EncodingType
        {
            none,
            raw,
            base64
        }

        #region file_info

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

        public string Header { get; set; }
        public string Title { get; set; }
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

        public List<VTKDataArray> DataArray { get; set; }

        public EncodingType EncodType { get; set; } = EncodingType.none;
        public long NumberOfPoints { get; set; } = 0;
        public long NumberOfCells { get; set; } = 0;
        public bool IsLittleEndian { get; set; } = true;
        #endregion
    }
}
