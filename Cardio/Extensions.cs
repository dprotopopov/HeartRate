using System;
using EdfReader;

namespace Cardio
{
    public static class Extensions
    {
        public static int CardioSignalNumber(this HeaderRecord header, out int nbOfSamples)
        {
            for (var i = 0; i < header.numberOfSignals; i++)
                if (header.label[i].ToUpper().Contains("EKG"))
                {
                    nbOfSamples = header.numberOfSamples[i];
                    return i;
                }

            throw new ArgumentException("Not found");
        }
    }
}