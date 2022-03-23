using System;
using System.Collections.Generic;
using System.Text;
using PetroGM.DataIO.VTK;

namespace SharedProject
{
    class Attributes
    {
        //string field_name;
        //int field_num_arrays;
        private List<VTKDataArray> DataArray = new List<VTKDataArray>();
        public string Name { get; set; }
        public int NumberOfTuples { get; set; }

        public void Parse(string[] data)
        {
            int linecounter = 1;
            VTKDataArray temp;
            Name = data[0].Split(' ')[0];
            NumberOfTuples = Convert.ToInt32(data[0].Split(' ')[1]);
            while (linecounter < data.Length)
            {
                switch ((VTKParser.AttributeType)Enum.Parse(typeof(VTKParser.AttributeType), data[linecounter].Split(' ')[0]))
                {
                    case VTKParser.AttributeType.SCALARS:
                        {
                            temp = new VTKDataArray();
                            temp.NumberOfTuples = NumberOfTuples;
                            temp.attributeType = (VTKParser.AttributeType)Enum.Parse(typeof(VTKParser.AttributeType), data[linecounter].Split(' ')[0]);
                            temp.Name = data[linecounter].Split(' ')[1];
                            temp.Type = (VTKParser.ValueType)Enum.Parse(typeof(VTKParser.ValueType), (data[linecounter].Split(' ')[2]), ignoreCase: true);
                            try
                            {
                                temp.NumberOfComponents = Convert.ToInt32(data[linecounter].Split(' ')[3]);
                            }
                            catch (Exception e)
                            {
                                temp.NumberOfComponents = 1;
                            }
                            ++linecounter;
                            bool isLookupTable;//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! опасно если кто-то назовет defaultName
                            {
                                string name = data[linecounter].Split(' ')[1];
                                isLookupTable = name.StartsWith("default");
                            }
                            ++linecounter;//еще строка из-за lookup table
                            double[,] temparr = new double[temp.NumberOfTuples, temp.NumberOfComponents];
                            double[] temptuple;
                            for (int k = 0; k < temp.NumberOfTuples; ++k)
                            {
                                temptuple = VTKParser.StringToNumbersParse<double>(data[linecounter], "");
                                for (int j = 0; j < temptuple.Length; ++j)
                                {
                                    temparr[k, j] = temptuple[j];
                                }
                                ++linecounter;
                            }
                            temp.Data.Add(temparr);
                            if (isLookupTable == false)
                            {
                                VTKDataArray lookupTable = new VTKDataArray();
                                lookupTable.Name = data[linecounter].Split(' ')[1];
                                lookupTable.attributeType = (VTKParser.AttributeType)Enum.Parse(typeof(VTKParser.AttributeType), data[linecounter].Split(' ')[0]);
                                lookupTable.NumberOfTuples = Convert.ToInt32(data[linecounter].Split(' ')[2]);
                                lookupTable.NumberOfComponents = 4;
                                ++linecounter;
                                temparr = new double[lookupTable.NumberOfTuples, lookupTable.NumberOfComponents];
                                for (int k = 0; k < lookupTable.NumberOfTuples; ++k)
                                {
                                    temptuple = VTKParser.StringToNumbersParse<double>(data[linecounter], "");
                                    for (int j = 0; j < temptuple.Length; ++j)
                                    {
                                        temparr[k, j] = temptuple[j];
                                    }
                                    ++linecounter;
                                }
                                lookupTable.Data.Add(temparr);
                                temp.Data.Add(lookupTable);
                            }
                            this.DataArray.Add(temp);
                            break;
                        }
                    //case VTKParser.AttributeType.LOOKUP_TABLE:
                    //    {
                    //        temp = new VTKDataArray();
                    //        temp.Name = data[linecounter].Split(' ')[1];
                    //        temp.attributeType = (VTKParser.AttributeType)Enum.Parse(typeof(VTKParser.AttributeType), data[linecounter].Split(' ')[0]);
                    //        temp.NumberOfTuples = Convert.ToInt32(data[linecounter].Split(' ')[2]);
                    //        temp.NumberOfComponents = 4;
                    //        ++linecounter;
                    //        double[,] temparr = new double[temp.NumberOfTuples, temp.NumberOfComponents];
                    //        double[] temptuple;
                    //        for (int k = 0; k < temp.NumberOfTuples; ++k)
                    //        {
                    //            temptuple = VTKParser.StringToNumbersParse<double>(data[linecounter], "");
                    //            for (int j = 0; j < temptuple.Length; ++j)
                    //            {
                    //                temparr[k, j] = temptuple[j];
                    //            }
                    //            ++linecounter;
                    //        }
                    //        temp.Data.Add(temparr);
                    //        this.DataArray.Add(temp);
                    //        break;
                    //    }
                    case VTKParser.AttributeType.COLOR_SCALARS:
                        {
                            temp = new VTKDataArray();
                            temp.NumberOfTuples = NumberOfTuples;
                            temp.attributeType = (VTKParser.AttributeType)Enum.Parse(typeof(VTKParser.AttributeType), data[linecounter].Split(' ')[0]);
                            temp.Name = data[linecounter].Split(' ')[1];
                            temp.NumberOfComponents = Convert.ToInt32(data[linecounter].Split(' ')[2]);
                            ++linecounter;
                            double[,] temparr = new double[temp.NumberOfTuples, temp.NumberOfComponents];
                            double[] temptuple;
                            for (int k = 0; k < temp.NumberOfTuples; ++k)
                            {
                                temptuple = VTKParser.StringToNumbersParse<double>(data[linecounter], "");
                                for (int j = 0; j < temptuple.Length; ++j)
                                {
                                    temparr[k, j] = temptuple[j];
                                }
                                ++linecounter;
                            }
                            temp.Data.Add(temparr);
                            this.DataArray.Add(temp);
                            break;
                        }
                    case VTKParser.AttributeType.VECTORS:
                        {
                            temp = new VTKDataArray();
                            temp.Name = data[linecounter].Split(' ')[1];
                            temp.attributeType = (VTKParser.AttributeType)Enum.Parse(typeof(VTKParser.AttributeType), data[linecounter].Split(' ')[0]);
                            temp.Type = (VTKParser.ValueType)Enum.Parse(typeof(VTKParser.ValueType), (data[linecounter].Split(' ')[2]), ignoreCase: true);
                            temp.NumberOfTuples = NumberOfTuples;
                            temp.NumberOfComponents = 3;
                            ++linecounter;
                            double[,] temparr = new double[temp.NumberOfTuples, temp.NumberOfComponents];
                            double[] temptuple;
                            for (int k = 0; k < temp.NumberOfTuples; ++k)
                            {
                                temptuple = VTKParser.StringToNumbersParse<double>(data[linecounter], "");
                                for (int j = 0; j < temptuple.Length; ++j)
                                {
                                    temparr[k, j] = temptuple[j];
                                }
                                ++linecounter;
                            }
                            temp.Data.Add(temparr);
                            this.DataArray.Add(temp);
                            break;
                        }
                    case VTKParser.AttributeType.NORMALS:
                        {
                            temp = new VTKDataArray();
                            temp.Name = data[linecounter].Split(' ')[1];
                            temp.attributeType = (VTKParser.AttributeType)Enum.Parse(typeof(VTKParser.AttributeType), data[linecounter].Split(' ')[0]);
                            temp.Type = (VTKParser.ValueType)Enum.Parse(typeof(VTKParser.ValueType), (data[linecounter].Split(' ')[2]), ignoreCase: true);
                            temp.NumberOfTuples = NumberOfTuples;
                            temp.NumberOfComponents = 3;
                            ++linecounter;
                            double[,] temparr = new double[temp.NumberOfTuples, temp.NumberOfComponents];
                            double[] temptuple;
                            for (int k = 0; k < temp.NumberOfTuples; ++k)
                            {
                                temptuple = VTKParser.StringToNumbersParse<double>(data[linecounter], "");
                                for (int j = 0; j < temptuple.Length; ++j)
                                {
                                    temparr[k, j] = temptuple[j];
                                }
                                ++linecounter;
                            }
                            temp.Data.Add(temparr);
                            this.DataArray.Add(temp);
                            break;
                        }
                    case VTKParser.AttributeType.TEXTURE_COORDINATES:
                        {
                            temp = new VTKDataArray();
                            temp.NumberOfTuples = NumberOfTuples;
                            temp.attributeType = (VTKParser.AttributeType)Enum.Parse(typeof(VTKParser.AttributeType), data[linecounter].Split(' ')[0]);
                            temp.Name = data[linecounter].Split(' ')[1];
                            temp.NumberOfComponents = Convert.ToInt32(data[linecounter].Split(' ')[2]);
                            temp.Type = (VTKParser.ValueType)Enum.Parse(typeof(VTKParser.ValueType), (data[linecounter].Split(' ')[3]), ignoreCase: true);
                            ++linecounter;
                            double[,] temparr = new double[temp.NumberOfTuples, temp.NumberOfComponents];
                            double[] temptuple;
                            for (int k = 0; k < temp.NumberOfTuples; ++k)
                            {
                                temptuple = VTKParser.StringToNumbersParse<double>(data[linecounter], "");
                                for (int j = 0; j < temptuple.Length; ++j)
                                {
                                    temparr[k, j] = temptuple[j];
                                }
                                ++linecounter;
                            }
                            temp.Data.Add(temparr);
                            this.DataArray.Add(temp);
                            break;
                        }
                    case VTKParser.AttributeType.TENSORS:
                        {
                            temp = new VTKDataArray();
                            temp.NumberOfTuples = NumberOfTuples;
                            temp.attributeType = (VTKParser.AttributeType)Enum.Parse(typeof(VTKParser.AttributeType), data[linecounter].Split(' ')[0]);
                            temp.Name = data[linecounter].Split(' ')[1];
                            temp.Type = (VTKParser.ValueType)Enum.Parse(typeof(VTKParser.ValueType), (data[linecounter].Split(' ')[2]), ignoreCase: true);
                            ++linecounter;
                            temp.DataSize = 3;
                            double[,] temparr;
                            double[] temptuple;
                            for (int k = 0; k < temp.NumberOfTuples; ++k)
                            {
                                temparr = new double[temp.DataSize, temp.DataSize];
                                for (int i = 0; i < temp.DataSize; ++i)
                                {
                                    temptuple = VTKParser.StringToNumbersParse<double>(data[linecounter], "");
                                    for (int j = 0; j < temptuple.Length; ++j)
                                    {
                                        temparr[i, j] = temptuple[j];
                                    }
                                    ++linecounter;
                                }
                                //++linecounter;
                                temp.Data.Add(temparr);
                            }
                            this.DataArray.Add(temp);
                            break;
                        }
                    case VTKParser.AttributeType.FIELD:
                        {
                            // здесь я сначала создаю vtkdataarray запоминаю переменные для него потом в него кладу в data еще лист vtkdataarray а потом в data array добаляю temp который уже является field
                            string field_name = (data[linecounter].Split(' '))[1];
                            VTKParser.AttributeType attributeType = (VTKParser.AttributeType)Enum.Parse(typeof(VTKParser.AttributeType), data[linecounter].Split(' ')[0]);
                            int field_num_arrays = Convert.ToInt32((data[linecounter].Split(' '))[2]);
                            List<VTKDataArray> field_list = new List<VTKDataArray>();
                            ++linecounter;
                            for (int i = 1; i <= field_num_arrays; ++i)
                            {
                                temp = new VTKDataArray();
                                {
                                    string[] fline = (data[linecounter].Split(' '));
                                    temp.Name = fline[0];
                                    temp.NumberOfComponents = Convert.ToInt32(fline[1]);
                                    temp.NumberOfTuples = Convert.ToInt32(fline[2]);
                                    temp.Type = (VTKParser.ValueType)Enum.Parse(typeof(VTKParser.ValueType), fline[3], ignoreCase: true);
                                }
                                ++linecounter;
                                double[,] temparr = new double[temp.NumberOfTuples, temp.NumberOfComponents];
                                double[] temptuple;
                                for (int k = 0; k < temp.NumberOfTuples; ++k)
                                {
                                    temptuple = VTKParser.StringToNumbersParse<double>(data[linecounter], "");
                                    for (int j = 0; j < temptuple.Length; ++j)
                                    {
                                        temparr[k, j] = temptuple[j];
                                    }
                                    ++linecounter;
                                }
                                temp.Data.Add(temparr);
                                field_list.Add(temp);
                            }
                            temp = new VTKDataArray();
                            temp.Data.Add(field_list);
                            temp.Name = field_name;
                            temp.DataSize = field_num_arrays;
                            temp.attributeType = attributeType;
                            DataArray.Add(temp);
                            break;
                        }
                    default:
                        {
                            ++linecounter;
                            break;
                        }
                }
            }
        }
        public string StringData()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            string output = "";
            string temp;
            output += $"{Name} {NumberOfTuples}\r\n";
            for (int i = 0; i < DataArray.Count; ++i)
            {
                temp = "";
                VTKParser.AttributeType attributeType = DataArray[i].attributeType;
                switch (attributeType)
                {
                    case VTKParser.AttributeType.SCALARS:
                        {
                            string NumberOfComponents = DataArray[i].NumberOfComponents == 1 ? "" : $" {DataArray[i].NumberOfComponents}";
                            temp += "SCALARS" + " " + $"{DataArray[i].Name}" + " " + $"{DataArray[i].Type}".ToLower() + NumberOfComponents + "\r\n";
                            if (DataArray[i].Data.Count > 1)
                            {
                                temp += "LOOKUP_TABLE" + " " + $"{((VTKDataArray)DataArray[i].Data[1]).Name}" + "\r\n";
                            }
                            else
                            {
                                temp += "LOOKUP_TABLE" + " " + "default" + "\r\n";
                            }

                            for (int kst = 0; kst < DataArray[i].NumberOfTuples; ++kst)//kst st-string
                            {
                                for (int kcl = 0; kcl < DataArray[i].NumberOfComponents; ++kcl)//kcl cl-columm
                                {
                                    temp += $"{((double[,])(DataArray[i].Data[0]))[kst, kcl]} ";
                                }
                                temp = temp.Trim();
                                temp += "\r\n";
                            }

                            if (DataArray[i].Data.Count > 1)
                            {
                                VTKDataArray lookupTable = (VTKDataArray)DataArray[i].Data[1];
                                temp += "LOOKUP_TABLE" + " " + $"{lookupTable.Name}" + " " + $"{lookupTable.NumberOfTuples}" + "\r\n";
                                for (int kst = 0; kst < lookupTable.NumberOfTuples; ++kst)
                                {
                                    for (int kcl = 0; kcl < lookupTable.NumberOfComponents; ++kcl)
                                    {
                                        temp += $"{((double[,])(lookupTable.Data[0]))[kst, kcl]} ";
                                    }
                                    temp = temp.Trim();
                                    temp += "\r\n";
                                }
                            }

                            break;
                        }
                    //case VTKParser.AttributeType.LOOKUP_TABLE:
                    //    {
                    //        temp += "LOOKUP_TABLE" + " " + $"{DataArray[i].Name}" + " " + $"{DataArray[i].NumberOfTuples}" + "\r\n";
                    //        for (int kst = 0; kst < DataArray[i].NumberOfTuples; ++kst)
                    //        {
                    //            for (int kcl = 0; kcl < DataArray[i].NumberOfComponents; ++kcl)
                    //            {
                    //                temp += $"{((double[,])(DataArray[i].Data[0]))[kst, kcl]} ";
                    //            }
                    //            temp = temp.Trim();
                    //            temp += "\r\n";
                    //        }
                    //        break;
                    //    }
                    case VTKParser.AttributeType.COLOR_SCALARS:
                        {
                            temp += "SCALARS" + " " + $"{DataArray[i].Name}" + " " + $"{DataArray[i].NumberOfComponents}" + "\r\n";
                            for (int kst = 0; kst < DataArray[i].NumberOfTuples; ++kst)
                            {
                                for (int kcl = 0; kcl < DataArray[i].NumberOfComponents; ++kcl)
                                {
                                    temp += $"{((double[,])(DataArray[i].Data[0]))[kst, kcl]} ";
                                }
                                temp = temp.Trim();
                                temp += "\r\n";
                            }
                            break;
                        }
                    case VTKParser.AttributeType.VECTORS:
                        {
                            temp += "VECTORS" + " " + $"{DataArray[i].Name}" + " " + $"{DataArray[i].Type}".ToLower() + "\r\n";
                            for (int kst = 0; kst < DataArray[i].NumberOfTuples; ++kst)
                            {
                                for (int kcl = 0; kcl < DataArray[i].NumberOfComponents; ++kcl)
                                {
                                    temp += $"{((double[,])(DataArray[i].Data[0]))[kst, kcl]} ";
                                }
                                temp = temp.Trim();
                                temp += "\r\n";
                            }
                            break;
                        }
                    case VTKParser.AttributeType.NORMALS:
                        {
                            temp += "NORMALS" + " " + $"{DataArray[i].Name}" + " " + $"{DataArray[i].Type}".ToLower() + "\r\n";
                            for (int kst = 0; kst < DataArray[i].NumberOfTuples; ++kst)
                            {
                                for (int kcl = 0; kcl < DataArray[i].NumberOfComponents; ++kcl)
                                {
                                    temp += $"{((double[,])(DataArray[i].Data[0]))[kst, kcl]} ";
                                }
                                temp = temp.Trim();
                                temp += "\r\n";
                            }
                            break;
                        }
                    case VTKParser.AttributeType.TEXTURE_COORDINATES:
                        {
                            temp += "TEXTURE_COORDINATES" + " " + $"{DataArray[i].Name}" + " " + $"{DataArray[i].NumberOfComponents}" + " " + $"{DataArray[i].Type}".ToLower() + "\r\n";
                            for (int kst = 0; kst < DataArray[i].NumberOfTuples; ++kst)
                            {
                                for (int kcl = 0; kcl < DataArray[i].NumberOfComponents; ++kcl)
                                {
                                    temp += $"{((double[,])(DataArray[i].Data[0]))[kst, kcl]} ";
                                }
                                temp = temp.Trim();
                                temp += "\r\n";
                            }
                            break;
                        }
                    case VTKParser.AttributeType.TENSORS:
                        {
                            temp += "TENSORS" + " " + $"{DataArray[i].Name}" + " " + $"{DataArray[i].Type}".ToLower() + "\r\n";
                            for (int k = 0; k < DataArray[i].Data.Count; ++k)
                            {
                                for (int kst = 0; kst < DataArray[i].DataSize; ++kst)
                                {
                                    for (int kcl = 0; kcl < DataArray[i].DataSize; ++kcl)
                                    {
                                        temp += $"{((double[,])(DataArray[i].Data[k]))[kst, kcl]} ";
                                    }
                                    temp = temp.Trim();
                                    temp += "\r\n";
                                }
                                //temp += "\r\n";
                            }
                            break;
                        }
                    case VTKParser.AttributeType.FIELD:
                        {
                            temp += $"{attributeType} " + $"{DataArray[i].Name} " + $"{DataArray[i].DataSize}\r\n";
                            List<VTKDataArray> field_list = (List<VTKDataArray>)(DataArray[i].Data[0]);
                            for (int j = 0; j < DataArray[i].DataSize; ++j)
                            {
                                temp += $"{field_list[j].Name} " + $"{field_list[j].NumberOfComponents} " + $"{field_list[j].NumberOfTuples} " + $"{field_list[j].Type}\r\n".ToLower();
                                for (int kst = 0; kst < field_list[j].NumberOfTuples; ++kst)
                                {
                                    for (int kcl = 0; kcl < field_list[j].NumberOfComponents; ++kcl)
                                    {
                                        temp += $"{((double[,])(field_list[j].Data[0]))[kst, kcl]} ";
                                    }
                                    temp = temp.Trim();
                                    temp += "\r\n";
                                }
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                output += temp;
            }
            return output;
        }

    }
}
