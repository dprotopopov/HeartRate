using System;
using System.IO;

namespace Cardio.ConsoleTest1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var file = File.OpenRead("TEST.EDF"))
            using (var source = new CardioSource(file))
            using (var heartRateSource = new HeartRateSource(source.GetData(), source.GetFrequency(), 1d))
            {
                var startDateTime = source.GetStartDateTime();
                var current = 0;
                Console.WriteLine(string.Join(";",
                    "dateTime",
                    "heartRate"));
                foreach (var value in heartRateSource.GetData())
                    Console.WriteLine(string.Join(";",
                        $"{startDateTime.AddSeconds(current++ / heartRateSource.GetFrequency()):O}",
                        $"{value}"));
            }
        }
    }
}