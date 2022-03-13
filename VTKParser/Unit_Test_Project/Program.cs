using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedProject;

namespace Unit_Test_Project
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 1; i < 17; ++i)
            {
                string FilePathR = $"C://Users//stitc//Documents//GitHub//VTKParser//VTKParser//Unit_Test_Project//TestFiles//test ({i}).vtk";
                string FilePathW = $"C://Users//stitc//Documents//GitHub//VTKParser//VTKParser//Unit_Test_Project//TestFiles//output{i}.txt";
                //string FilePathR = "C://Users//stitc//Documents//VTKParser//VTKParser//examples//beam-hex.vtk";
                //string FilePathW = "C://Users//stitc//Documents//VTKParser//VTKParser//examples//test.txt";
                VTKParser parser = new VTKParser();
                parser.Read(FilePathR);
                parser.RawDataProcess();
                //parser.Write(FilePathW);
                parser.Output(FilePathW);
            }
            string indata;
            string outdata;
            string infilepath;
            string outfilepath;
            for (int i = 1; i < 17; ++i)
            {

                infilepath = $"C://Users//stitc//Documents//GitHub//VTKParser//VTKParser//Unit_Test_Project//TestFiles//test ({i}).vtk"; 
                outfilepath = $"C://Users//stitc//Documents//GitHub//VTKParser//VTKParser//Unit_Test_Project//TestFiles//output{i}.txt"; 
                using (FileStream stream = File.OpenRead(infilepath))
                {
                    byte[] array = new byte[stream.Length];
                    stream.Read(array, 0, array.Length);
                    indata = System.Text.Encoding.Default.GetString(array);
                }
                using (FileStream stream = File.OpenRead(outfilepath))
                {
                    byte[] array = new byte[stream.Length];
                    stream.Read(array, 0, array.Length);
                    outdata = System.Text.Encoding.Default.GetString(array);
                }

                Console.WriteLine(String.Compare(indata,outdata));
            } 
            Console.ReadKey();
        }
    }
}
