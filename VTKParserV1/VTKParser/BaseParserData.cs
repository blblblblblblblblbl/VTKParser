using System.Collections.Generic;

namespace PetroGM.DataIO
{
    internal class BaseParserData
    {
            private List<float> data;

            public BaseParserData()
            {
                data = new List<float>();
            }
            public List<float> Data
            {
                get { return data; }
                set { data = value; }
            }
    }
}
