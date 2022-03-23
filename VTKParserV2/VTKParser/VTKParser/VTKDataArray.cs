using System.Collections.Generic;
namespace PetroGM.DataIO.VTK
{
    class VtkDataArray
    {
        //Show reader how to interpret data. If ValueType is None this 
        public VTKParser.VtkParser.ValueType Type { get; set; } = VTKParser.VtkParser.ValueType.None;
        public VTKParser.VtkParser.DataSaveFormat SaveFormat { get; set; } = VTKParser.VtkParser.DataSaveFormat.None;
        public VTKParser.VtkParser.DataType DatType { get; set; } = VTKParser.VtkParser.DataType.None;

        public VTKParser.VtkParser.AttributeType AttributeType { get; set; }= VTKParser.VtkParser.AttributeType.None;
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
        public VtkDataArray()
        {
            Data = new List<object>();
        }
    }
}
