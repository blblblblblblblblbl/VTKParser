using System.Collections.Generic;
using SharedProject;

namespace PetroGM.DataIO.VTK
{
    class VTKDataArray
    {
        //Show reader how to interpret data. If ValueType is None this 
        public VTKParser.ValueType Type { get; set; } = VTKParser.ValueType.None;
        public VTKParser.DataSaveFormat SaveFormat { get; set; } = VTKParser.DataSaveFormat.None;
        public VTKParser.DataType DatType { get; set; } = VTKParser.DataType.None;

        public VTKParser.AttributeType attributeType { get; set; }= VTKParser.AttributeType.None;
        //public int attribute_data_size { get; set; }
        /// <summary>
        /// Size of data to read. It has default value -1 if it's not defined
        /// </summary>
        public long DataSize { get; set; } = -1;
        /// <summary>
        /// Name of data.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Number of tuples. It has default value -1 if it's not defined
        /// </summary>
        public long NumberOfTuples { get; set; } = -1;
        /// <summary>
        /// Number of components.It has default value -1 if it's not defined
        /// </summary>
        public long NumberOfComponents { get; set; } = -1;

        /// <summary>
        /// Contain min value in Data list.
        /// </summary>
        public double RangeMin { get; set; } = double.MinValue;

        /// <summary>
        /// Contain max value in Data list.
        /// </summary>
        public double RangeMax { get; set; } = double.MaxValue;
        public string RawData { get; set; }
        public byte[] Base64Data { get; set; }
        public List<object> Data { get; set; }
        /// <summary>
        /// It has default value -1 if it's not defined
        /// </summary>
        public long Offset { get; set; } = -1;
        public VTKDataArray()
        {
            Data = new List<object>();
        }
    }
}
