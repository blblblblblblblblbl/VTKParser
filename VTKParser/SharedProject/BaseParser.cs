using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Numerics;
//using SharpDX;

namespace PetroGM.DataIO
{
    //public class Line
    //{
    //    public List<Vector3> points;
    //    public string name;
    //}

    public abstract class BaseParser
    {
        protected string input_filepath;
        protected bool isLoaded = false;
        protected void CheckOnRead(in string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new Exception("File " + filepath + " doesn't exist!");
            }
        }
        //public static T ConvertValue<T>(string value)
        //{
        //    return (T)Convert.ChangeType(value, typeof(T), CultureInfo.CreateSpecificCulture("en-US"));
        //}
        #region New Converter
        public static bool ConvertValue(string value, out double numb)
        {
            numb = double.NaN;
            bool result = double.TryParse(value, NumberStyles.Number | NumberStyles.AllowExponent,
                CultureInfo.InvariantCulture, out double number);
            if (result)
            {
                numb = number;
            }
            return result;
        }
        public static bool ConvertValue(string value, out float numb)
        {
            numb = float.NaN;
            bool result = float.TryParse(value, NumberStyles.Number | NumberStyles.AllowExponent,
                CultureInfo.InvariantCulture, out float number);
            if (result)
            {
                numb = number;
            }
            return result;
        }
        public static bool ConvertValue(string value, out ulong numb)
        {
            numb = ulong.MinValue;
            bool result = ulong.TryParse(value, NumberStyles.Number | NumberStyles.AllowExponent,
                CultureInfo.InvariantCulture, out ulong number);
            if (result)
            {
                numb = number;
            }
            return result;
        }
        public static bool ConvertValue(string value, out int numb)
        {
            numb = int.MinValue;
            bool result = int.TryParse(value, NumberStyles.Number | NumberStyles.AllowExponent,
                CultureInfo.InvariantCulture, out int number);
            if (result)
            {
                numb = number;
            }
            return result;
        }
        public static bool ConvertValue(string value, out long numb)
        {
            numb = long.MinValue;
            bool result = long.TryParse(value, NumberStyles.Number | NumberStyles.AllowExponent,
                CultureInfo.InvariantCulture, out long number);
            if (result)
            {
                numb = number;
            }
            return result;
        }
        #endregion
        protected virtual void CheckFile(in string filepath)
        {
            FileStream file = null;
            //Open or creating file
            if (File.Exists(filepath))
            {
                using (file = File.Open(filepath, FileMode.Open))
                {
                    /* 
                     * Set the length of filestream to 0 and flush it to the physical file.
                     *
                     * Flushing the stream is important because this ensures that
                     * the changes to the stream trickle down to the physical file.
                     * 
                     */
                    file.SetLength(0);
                }

            }
            else
            {
                using (file = new FileStream(filepath, FileMode.Create)) { }
            }
        }
        public virtual void Read(in string filepath)
        {
            CheckOnRead(filepath);
            input_filepath = filepath;
        }

        public virtual void Write(in string filepath)
        {
            if (!isLoaded)
            {
                throw new Exception("Object is not loaded");
            }
        }

        public string InputFilePath
        {
            get { return input_filepath; }
        }
        public bool IsLoaded
        {
            get { return isLoaded; }
            set { isLoaded = value; }
        }
    }
}
