using _3waysMerge.BL;
using _3waysMerge.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace _3waysMerge
{
    class Program
    {
        static void Main(string[] args)
        {
            string parentPath;
            string leftChildPath;
            string rightChildPath;
            string resultPath;

            Console.WriteLine("Input path to parent file:");
            parentPath = Console.ReadLine();
            CheckIfFileExists(ref parentPath);
            Console.WriteLine();

            Console.WriteLine("Input path to first child file:");
            leftChildPath = Console.ReadLine();
            CheckIfFileExists(ref leftChildPath);
            Console.WriteLine();

            Console.WriteLine("Input path to second child file:");
            rightChildPath = Console.ReadLine();
            CheckIfFileExists(ref rightChildPath);
            Console.WriteLine();

            Console.WriteLine("Input path to result file:");
            resultPath = Console.ReadLine();
            Console.WriteLine();

            Console.WriteLine();

            try
            {
                MergeManager manager = new MergeManager();

                List<ThreeWayMergedDocumentLine> result = manager.GetThreeWayMergeResultAsync(parentPath, leftChildPath, rightChildPath).Result;

                MergePrintManager printManager = new MergePrintManager();

                printManager.PrintThreeWayMergeResult(resultPath, result);

                Console.WriteLine("Merge completed!");
                Console.ReadKey();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }          
        }

        private static void CheckIfFileExists(ref string path)
        {
            if (!File.Exists(path))
            {
                do
                {
                    Console.WriteLine("File does not exist. Please try again:");
                    path = Console.ReadLine();
                } while (!File.Exists(path));
            }
        }
    }
}
