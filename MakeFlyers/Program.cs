using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

namespace MakeFlyers
{
    class Program
    {
        private static readonly int CountFlats = 577;
        private static int GeneratedFlyers = 0;
        static void Main(string[] args)
        {
            GenerateFlyers();
            Console.WriteLine("\nСоздание листовок успешно окончено!");
            Console.ReadLine();
        }

        private static void GenerateFlyers()
        {
            try
            {
                StartRound(true);
                StartRound(false);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ReadKey();
                Console.ResetColor();
            }
        }

        private static void StartRound(bool isFirstRound)
        {
            Process[] processes = new Process[3]
            {
                new Process(),
                new Process(),
                new Process()
            };
            using var pipeRead = new AnonymousPipeServerStream(PipeDirection.In,
                HandleInheritability.Inheritable);

            for (int i = 0; i < 3; i++)
            {
                processes[i].StartInfo.FileName = "GenerateFlyers.exe";
                processes[i].StartInfo.Arguments = $"{GetHouse(isFirstRound, i)} {pipeRead.GetClientHandleAsString()}";
                processes[i].Start();
            }
            pipeRead.DisposeLocalCopyOfClientHandle();

            using var sr = new StreamReader(pipeRead);

            while (sr.ReadLine() != null)
            {
                Console.Write($"\rСоздание ... {++GeneratedFlyers} / {CountFlats}");
                if (GeneratedFlyers == CountFlats / 2 + 1) break;
            }
        }

        private static string GetHouse(bool isFirstRound, int number)
        {
            switch (number)
            {
                case 0:
                    if (isFirstRound)
                        return "20_1";
                    else
                        return "26_1";
                case 1:
                    if (isFirstRound)
                        return "24_2";
                    else
                        return "26_2";
                case 2:
                    if (isFirstRound)
                        return "22_2";
                    else
                        return "20_2";
                default: throw new ArgumentException("Некорректный номер дома");
            }
        }
    }
}
