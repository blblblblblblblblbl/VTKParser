
namespace VTKParser
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
            Dimensions = VtkParser.StringToNumbersParse<int>(data[linecounter], "DIMENSIONS");//, out dimensions);
            ++linecounter;
            Origin = VtkParser.StringToNumbersParse<int>(data[linecounter], "ORIGIN");
            ++linecounter;
            if (data[linecounter].StartsWith("ASPECT_RATIO"))
            {
                Spacing = VtkParser.StringToNumbersParse<int>(data[linecounter], "ASPECT_RATIO");//, out aspect_ratio);
                SpacingName = "ASPECT_RATIO";
            }
            else
            {
                Spacing = VtkParser.StringToNumbersParse<int>(data[linecounter], "SPACING");
            }
        }
        public override string StringData()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            string output = "";
            {
                string sdimensions = "";
                foreach (int el in Dimensions)
                {
                    sdimensions += $"{el} ";
                }

                sdimensions = sdimensions.Trim();
                output += "DIMENSIONS " + sdimensions + "\r\n";
            }
            {
                string sorigin = "";
                foreach (int el in Origin)
                {
                    sorigin += $"{el} ";
                }

                sorigin = sorigin.Trim();
                output += "ORIGIN " + sorigin + "\r\n";
            }
            {
                string sspacing = "";
                foreach (int el in Spacing)
                {
                    sspacing += $"{el} ";
                }

                sspacing = sspacing.Trim();
                output += $"{SpacingName} " + sspacing + "\r\n";
            }
            return output;
        }
    }
}
