using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedProject;

namespace Empty_Project
{
    class Program
    {
        static void Main(string[] args)
        {
            string FilePathR = "C://Users//stitc//Documents//GitHub//VTKParser//VTKParser//examples//1.txt";
            string FilePathW = "C://Users//stitc//Documents//GitHub//VTKParser//VTKParser//examples//test.txt";
            //string FilePathR = "C://Users//stitc//Documents//VTKParser//VTKParser//examples//beam-hex.vtk";
            //string FilePathW = "C://Users//stitc//Documents//VTKParser//VTKParser//examples//test.txt";
            VTKParser parser = new VTKParser();
            parser.Read(FilePathR);
            parser.RawDataProcess();
            //parser.Write(FilePathW);
            parser.Output(FilePathW);
            Console.ReadKey();
        }
    }
}
