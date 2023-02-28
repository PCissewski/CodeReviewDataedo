using System;

namespace ConsoleApp
{

    internal static class Program
    {
        private static void Main(string[] args)
        {
            var reader = new DataReader();
            
            reader.ImportData("data.csv");
            reader.PrintData();
            reader.ClearData();
            reader.AssignChildren();

            Console.ReadLine();
        }
    }
}
