using System;
using System.Collections.Generic;
using System.Text;

namespace SharedProject
{
    class StructuredPoints : DataSet
    {
        public int[] Dimensions { get; set; }
        public int[] Spacing { get; set; }
        public int[] Origin { get; set; }
        public string SpacingName { get; set; }
        public override void Parse(string[] data)
        {
            int linecounter = 0;
            this.Dimensions = VTKParser.StringToNumbersParse<int>(data[linecounter], "DIMENSIONS");//, out dimensions);
            ++linecounter;
            this.Origin = VTKParser.StringToNumbersParse<int>(data[linecounter], "ORIGIN");
            ++linecounter;
            if (data[linecounter].StartsWith("ASPECT_RATIO"))
            {
                this.Spacing = VTKParser.StringToNumbersParse<int>(data[linecounter], "ASPECT_RATIO");//, out aspect_ratio);
                this.SpacingName = "ASPECT_RATIO ";
            }
            else
            {
                this.Spacing = VTKParser.StringToNumbersParse<int>(data[linecounter], "SPACING");
            }
        }
        public override string StringData()
        {
            string output = "";
            {
                string sdimensions = "";
                foreach (int el in Dimensions)
                {
                    sdimensions += $"{el} ";
                }
                output += "DIMENSIONS " + sdimensions + "\n";
            }
            {
                string sorigin = "";
                foreach (int el in Origin)
                {
                    sorigin += $"{el} ";
                }
                output += "ORIGIN " + sorigin + "\n";
            }
            {
                string sspacing = "";
                foreach (int el in Spacing)
                {
                    sspacing += $"{el} ";
                }
                output += $"{this.SpacingName} " + sspacing + "\n";
            }
            return output;
        }
    }
}
