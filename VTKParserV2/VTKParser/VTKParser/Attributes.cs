using System;
using System.Collections.Generic;
using PetroGM.DataIO.VTK;

namespace VTKParser
{
    class Attributes
    {
        private List<VtkDataArray> _dataArray;
        public string Name { get; set; }
        public int NumberOfTuples { get; set; }

        public void Parse(string[] data)
        {
            _dataArray = new List<VtkDataArray>();
            int linecounter = 1;
            VtkDataArray temp;
            Name = data[0].Split(' ')[0];
            NumberOfTuples = Convert.ToInt32(data[0].Split(' ')[1]);
            while (linecounter < data.Length)
            {
                switch ((VtkParser.AttributeType)Enum.Parse(typeof(VtkParser.AttributeType), data[linecounter].Split(' ')[0]))
                {
                    case VtkParser.AttributeType.SCALARS:
                        {
                            temp = new VtkDataArray();
                            temp.NumberOfTuples = NumberOfTuples;
                            temp.AttributeType = (VtkParser.AttributeType)Enum.Parse(typeof(VtkParser.AttributeType), data[linecounter].Split(' ')[0]);
                            temp.Name = data[linecounter].Split(' ')[1];
                            temp.Type = (VtkParser.ValueType)Enum.Parse(typeof(VtkParser.ValueType), (data[linecounter].Split(' ')[2]), ignoreCase: true);
                            try
                            {
                                temp.NumberOfComponents = Convert.ToInt32(data[linecounter].Split(' ')[3]);
                            }
                            catch (Exception)
                            {
                                temp.NumberOfComponents = 1;
                            }
                            ++linecounter;
                            bool isLookupTable;
                            {
                                string name = data[linecounter].Split(' ')[1];
                                isLookupTable = string.Equals(name, "default");
                            }
                            ++linecounter;
                            double[,] temparr = new double[temp.NumberOfTuples, temp.NumberOfComponents];
                            double[] temptuple;
                            for (int k = 0; k < temp.NumberOfTuples; ++k)
                            {
                                temptuple = VtkParser.StringToNumbersParse<double>(data[linecounter]);
                                for (int j = 0; j < temptuple.Length; ++j)
                                {
                                    temparr[k, j] = temptuple[j];
                                }
                                ++linecounter;
                            }
                            temp.Data.Add(temparr);
                            if (isLookupTable == false)
                            {
                                VtkDataArray lookupTable = new VtkDataArray();
                                lookupTable.Name = data[linecounter].Split(' ')[1];
                                lookupTable.AttributeType = (VtkParser.AttributeType)Enum.Parse(typeof(VtkParser.AttributeType), data[linecounter].Split(' ')[0]);
                                lookupTable.NumberOfTuples = Convert.ToInt32(data[linecounter].Split(' ')[2]);
                                lookupTable.NumberOfComponents = 4;
                                ++linecounter;
                                temparr = new double[lookupTable.NumberOfTuples, lookupTable.NumberOfComponents];
                                for (int k = 0; k < lookupTable.NumberOfTuples; ++k)
                                {
                                    temptuple = VtkParser.StringToNumbersParse<double>(data[linecounter]);
                                    for (int j = 0; j < temptuple.Length; ++j)
                                    {
                                        temparr[k, j] = temptuple[j];
                                    }
                                    ++linecounter;
                                }
                                lookupTable.Data.Add(temparr);
                                temp.Data.Add(lookupTable);
                            }
                            _dataArray.Add(temp);
                            break;
                        }
                    
                    case VtkParser.AttributeType.COLOR_SCALARS:
                        {
                            temp = new VtkDataArray();
                            temp.NumberOfTuples = NumberOfTuples;
                            temp.AttributeType = (VtkParser.AttributeType)Enum.Parse(typeof(VtkParser.AttributeType), data[linecounter].Split(' ')[0]);
                            temp.Name = data[linecounter].Split(' ')[1];
                            temp.NumberOfComponents = Convert.ToInt32(data[linecounter].Split(' ')[2]);
                            ++linecounter;
                            double[,] temparr = new double[temp.NumberOfTuples, temp.NumberOfComponents];
                            double[] temptuple;
                            for (int k = 0; k < temp.NumberOfTuples; ++k)
                            {
                                temptuple = VtkParser.StringToNumbersParse<double>(data[linecounter]);
                                for (int j = 0; j < temptuple.Length; ++j)
                                {
                                    temparr[k, j] = temptuple[j];
                                }
                                ++linecounter;
                            }
                            temp.Data.Add(temparr);
                            _dataArray.Add(temp);
                            break;
                        }
                    case VtkParser.AttributeType.VECTORS:
                        {
                            temp = new VtkDataArray();
                            temp.Name = data[linecounter].Split(' ')[1];
                            temp.AttributeType = (VtkParser.AttributeType)Enum.Parse(typeof(VtkParser.AttributeType), data[linecounter].Split(' ')[0]);
                            temp.Type = (VtkParser.ValueType)Enum.Parse(typeof(VtkParser.ValueType), (data[linecounter].Split(' ')[2]), ignoreCase: true);
                            temp.NumberOfTuples = NumberOfTuples;
                            temp.NumberOfComponents = 3;
                            ++linecounter;
                            double[,] temparr = new double[temp.NumberOfTuples, temp.NumberOfComponents];
                            double[] temptuple;
                            for (int k = 0; k < temp.NumberOfTuples; ++k)
                            {
                                temptuple = VtkParser.StringToNumbersParse<double>(data[linecounter]);
                                for (int j = 0; j < temptuple.Length; ++j)
                                {
                                    temparr[k, j] = temptuple[j];
                                }
                                ++linecounter;
                            }
                            temp.Data.Add(temparr);
                            _dataArray.Add(temp);
                            break;
                        }
                    case VtkParser.AttributeType.NORMALS:
                        {
                            temp = new VtkDataArray();
                            temp.Name = data[linecounter].Split(' ')[1];
                            temp.AttributeType = (VtkParser.AttributeType)Enum.Parse(typeof(VtkParser.AttributeType), data[linecounter].Split(' ')[0]);
                            temp.Type = (VtkParser.ValueType)Enum.Parse(typeof(VtkParser.ValueType), (data[linecounter].Split(' ')[2]), ignoreCase: true);
                            temp.NumberOfTuples = NumberOfTuples;
                            temp.NumberOfComponents = 3;
                            ++linecounter;
                            double[,] temparr = new double[temp.NumberOfTuples, temp.NumberOfComponents];
                            double[] temptuple;
                            for (int k = 0; k < temp.NumberOfTuples; ++k)
                            {
                                temptuple = VtkParser.StringToNumbersParse<double>(data[linecounter]);
                                for (int j = 0; j < temptuple.Length; ++j)
                                {
                                    temparr[k, j] = temptuple[j];
                                }
                                ++linecounter;
                            }
                            temp.Data.Add(temparr);
                            _dataArray.Add(temp);
                            break;
                        }
                    case VtkParser.AttributeType.TEXTURE_COORDINATES:
                        {
                            temp = new VtkDataArray();
                            temp.NumberOfTuples = NumberOfTuples;
                            temp.AttributeType = (VtkParser.AttributeType)Enum.Parse(typeof(VtkParser.AttributeType), data[linecounter].Split(' ')[0]);
                            temp.Name = data[linecounter].Split(' ')[1];
                            temp.NumberOfComponents = Convert.ToInt32(data[linecounter].Split(' ')[2]);
                            temp.Type = (VtkParser.ValueType)Enum.Parse(typeof(VtkParser.ValueType), (data[linecounter].Split(' ')[3]), ignoreCase: true);
                            ++linecounter;
                            double[,] temparr = new double[temp.NumberOfTuples, temp.NumberOfComponents];
                            double[] temptuple;
                            for (int k = 0; k < temp.NumberOfTuples; ++k)
                            {
                                temptuple = VtkParser.StringToNumbersParse<double>(data[linecounter]);
                                for (int j = 0; j < temptuple.Length; ++j)
                                {
                                    temparr[k, j] = temptuple[j];
                                }
                                ++linecounter;
                            }
                            temp.Data.Add(temparr);
                            _dataArray.Add(temp);
                            break;
                        }
                    case VtkParser.AttributeType.TENSORS:
                        {
                            temp = new VtkDataArray();
                            temp.NumberOfTuples = NumberOfTuples;
                            temp.AttributeType = (VtkParser.AttributeType)Enum.Parse(typeof(VtkParser.AttributeType), data[linecounter].Split(' ')[0]);
                            temp.Name = data[linecounter].Split(' ')[1];
                            temp.Type = (VtkParser.ValueType)Enum.Parse(typeof(VtkParser.ValueType), (data[linecounter].Split(' ')[2]), ignoreCase: true);
                            ++linecounter;
                            temp.DataSize = 3;
                            double[,] temparr;
                            double[] temptuple;
                            for (int k = 0; k < temp.NumberOfTuples; ++k)
                            {
                                temparr = new double[temp.DataSize, temp.DataSize];
                                for (int i = 0; i < temp.DataSize; ++i)
                                {
                                    temptuple = VtkParser.StringToNumbersParse<double>(data[linecounter]);
                                    for (int j = 0; j < temptuple.Length; ++j)
                                    {
                                        temparr[i, j] = temptuple[j];
                                    }
                                    ++linecounter;
                                }
                                temp.Data.Add(temparr);
                            }
                            _dataArray.Add(temp);
                            break;
                        }
                    case VtkParser.AttributeType.FIELD:
                        {
                            // здесь я сначала создаю vtkdataarray запоминаю переменные для него потом в него кладу в data еще лист vtkdataarray а потом в data array добаляю temp который уже является field
                            string fieldName = (data[linecounter].Split(' '))[1];
                            VtkParser.AttributeType attributeType = (VtkParser.AttributeType)Enum.Parse(typeof(VtkParser.AttributeType), data[linecounter].Split(' ')[0]);
                            int fieldNumArrays = Convert.ToInt32((data[linecounter].Split(' '))[2]);
                            List<VtkDataArray> fieldList = new List<VtkDataArray>();
                            ++linecounter;
                            for (int i = 1; i <= fieldNumArrays; ++i)
                            {
                                temp = new VtkDataArray();
                                {
                                    string[] fline = (data[linecounter].Split(' '));
                                    temp.Name = fline[0];
                                    temp.NumberOfComponents = Convert.ToInt32(fline[1]);
                                    temp.NumberOfTuples = Convert.ToInt32(fline[2]);
                                    temp.Type = (VtkParser.ValueType)Enum.Parse(typeof(VtkParser.ValueType), fline[3], ignoreCase: true);
                                }
                                ++linecounter;
                                double[,] temparr = new double[temp.NumberOfTuples, temp.NumberOfComponents];
                                double[] temptuple;
                                for (int k = 0; k < temp.NumberOfTuples; ++k)
                                {
                                    temptuple = VtkParser.StringToNumbersParse<double>(data[linecounter]);
                                    for (int j = 0; j < temptuple.Length; ++j)
                                    {
                                        temparr[k, j] = temptuple[j];
                                    }
                                    ++linecounter;
                                }
                                temp.Data.Add(temparr);
                                fieldList.Add(temp);
                            }
                            temp = new VtkDataArray();
                            temp.Data.Add(fieldList);
                            temp.Name = fieldName;
                            temp.DataSize = fieldNumArrays;
                            temp.AttributeType = attributeType;
                            _dataArray.Add(temp);
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
            for (int i = 0; i < _dataArray.Count; ++i)
            {
                temp = "";
                VtkParser.AttributeType attributeType = _dataArray[i].AttributeType;
                switch (attributeType)
                {
                    case VtkParser.AttributeType.SCALARS:
                        {
                            string numberOfComponents = _dataArray[i].NumberOfComponents == 1 ? "" : $" {_dataArray[i].NumberOfComponents}";
                            temp += "SCALARS" + " " + $"{_dataArray[i].Name}" + " " + $"{_dataArray[i].Type}".ToLower() + numberOfComponents + "\r\n";
                            if (_dataArray[i].Data.Count > 1)
                            {
                                temp += "LOOKUP_TABLE" + " " + $"{((VtkDataArray)_dataArray[i].Data[1]).Name}" + "\r\n";
                            }
                            else
                            {
                                temp += "LOOKUP_TABLE" + " " + "default" + "\r\n";
                            }

                            for (int kst = 0; kst < _dataArray[i].NumberOfTuples; ++kst)//kst st-string
                            {
                                for (int kcl = 0; kcl < _dataArray[i].NumberOfComponents; ++kcl)//kcl cl-columm
                                {
                                    temp += $"{((double[,])(_dataArray[i].Data[0]))[kst, kcl]} ";
                                }
                                temp = temp.Trim();
                                temp += "\r\n";
                            }

                            if (_dataArray[i].Data.Count > 1)
                            {
                                VtkDataArray lookupTable = (VtkDataArray)_dataArray[i].Data[1];
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
                    case VtkParser.AttributeType.COLOR_SCALARS:
                        {
                            temp += "SCALARS" + " " + $"{_dataArray[i].Name}" + " " + $"{_dataArray[i].NumberOfComponents}" + "\r\n";
                            for (int kst = 0; kst < _dataArray[i].NumberOfTuples; ++kst)
                            {
                                for (int kcl = 0; kcl < _dataArray[i].NumberOfComponents; ++kcl)
                                {
                                    temp += $"{((double[,])(_dataArray[i].Data[0]))[kst, kcl]} ";
                                }
                                temp = temp.Trim();
                                temp += "\r\n";
                            }
                            break;
                        }
                    case VtkParser.AttributeType.VECTORS:
                        {
                            temp += "VECTORS" + " " + $"{_dataArray[i].Name}" + " " + $"{_dataArray[i].Type}".ToLower() + "\r\n";
                            for (int kst = 0; kst < _dataArray[i].NumberOfTuples; ++kst)
                            {
                                for (int kcl = 0; kcl < _dataArray[i].NumberOfComponents; ++kcl)
                                {
                                    temp += $"{((double[,])(_dataArray[i].Data[0]))[kst, kcl]} ";
                                }
                                temp = temp.Trim();
                                temp += "\r\n";
                            }
                            break;
                        }
                    case VtkParser.AttributeType.NORMALS:
                        {
                            temp += "NORMALS" + " " + $"{_dataArray[i].Name}" + " " + $"{_dataArray[i].Type}".ToLower() + "\r\n";
                            for (int kst = 0; kst < _dataArray[i].NumberOfTuples; ++kst)
                            {
                                for (int kcl = 0; kcl < _dataArray[i].NumberOfComponents; ++kcl)
                                {
                                    temp += $"{((double[,])(_dataArray[i].Data[0]))[kst, kcl]} ";
                                }
                                temp = temp.Trim();
                                temp += "\r\n";
                            }
                            break;
                        }
                    case VtkParser.AttributeType.TEXTURE_COORDINATES:
                        {
                            temp += "TEXTURE_COORDINATES" + " " + $"{_dataArray[i].Name}" + " " + $"{_dataArray[i].NumberOfComponents}" + " " + $"{_dataArray[i].Type}".ToLower() + "\r\n";
                            for (int kst = 0; kst < _dataArray[i].NumberOfTuples; ++kst)
                            {
                                for (int kcl = 0; kcl < _dataArray[i].NumberOfComponents; ++kcl)
                                {
                                    temp += $"{((double[,])(_dataArray[i].Data[0]))[kst, kcl]} ";
                                }
                                temp = temp.Trim();
                                temp += "\r\n";
                            }
                            break;
                        }
                    case VtkParser.AttributeType.TENSORS:
                        {
                            temp += "TENSORS" + " " + $"{_dataArray[i].Name}" + " " + $"{_dataArray[i].Type}".ToLower() + "\r\n";
                            for (int k = 0; k < _dataArray[i].Data.Count; ++k)
                            {
                                for (int kst = 0; kst < _dataArray[i].DataSize; ++kst)
                                {
                                    for (int kcl = 0; kcl < _dataArray[i].DataSize; ++kcl)
                                    {
                                        temp += $"{((double[,])(_dataArray[i].Data[k]))[kst, kcl]} ";
                                    }
                                    temp = temp.Trim();
                                    temp += "\r\n";
                                }
                            }
                            break;
                        }
                    case VtkParser.AttributeType.FIELD:
                        {
                            temp += $"{attributeType} " + $"{_dataArray[i].Name} " + $"{_dataArray[i].DataSize}\r\n";
                            List<VtkDataArray> fieldList = (List<VtkDataArray>)(_dataArray[i].Data[0]);
                            for (int j = 0; j < _dataArray[i].DataSize; ++j)
                            {
                                temp += $"{fieldList[j].Name} " + $"{fieldList[j].NumberOfComponents} " + $"{fieldList[j].NumberOfTuples} " + $"{fieldList[j].Type}\r\n".ToLower();
                                for (int kst = 0; kst < fieldList[j].NumberOfTuples; ++kst)
                                {
                                    for (int kcl = 0; kcl < fieldList[j].NumberOfComponents; ++kcl)
                                    {
                                        temp += $"{((double[,])(fieldList[j].Data[0]))[kst, kcl]} ";
                                    }
                                    temp = temp.Trim();
                                    temp += "\r\n";
                                }
                            }
                            break;
                        }

                }
                output += temp;
            }
            return output;
        }

    }
}
