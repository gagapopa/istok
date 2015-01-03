using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COTES.ISTOK.Tests
{
    static class Program
    {
        [STAThread]
        static void Main(String[] args)
        {
            try
            {
                String filesDirectory = "D:\\tmp";

                var generator = new ValueAggregatorTestDataGenerator();
                Console.WriteLine("Start generating ValueAggregatorTests source test data to directory {0}", filesDirectory);
                generator.GenerateSourceData(filesDirectory);
                Console.WriteLine("Test data are generated successfully");
            }
            catch (Exception exc)
            {
                Console.WriteLine("Test data generation is failed. {0}", exc.Message);
            }
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }
    }
}
