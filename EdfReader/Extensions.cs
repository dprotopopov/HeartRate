using System;
using System.IO;
using System.Text;

namespace EdfReader
{
    public static class Extensions
    {
        public static HeaderRecord ReadHeaderRecord(this BinaryReader br)
        {
            var header = new HeaderRecord();
            header.version = int.Parse(br.ReadString(8));
            header.patientIdentification = br.ReadString(80);
            header.recordingIdentification = br.ReadString(80);
            header.startdateOfRecording = br.ReadString(8);
            header.starttimeOfRecording = br.ReadString(8);
            header.numberOfBytesInHeaderRecord = int.Parse(br.ReadString(8));
            br.ReadString(44);
            header.numberOfDataRecords = int.Parse(br.ReadString(8));
            header.durationOfDataRecords = decimal.Parse(br.ReadString(8));
            var ns = header.numberOfSignals = int.Parse(br.ReadString(4));
            header.label = new string[ns];
            header.transducerType = new string[ns];
            header.physicalDimension = new string[ns];
            header.physicalMinimum = new decimal[ns];
            header.physicalMaximum = new decimal[ns];
            header.digitalMinimum = new int[ns];
            header.digitalMaximum = new int[ns];
            header.prefiltering = new string[ns];
            header.numberOfSamples = new int[ns];
            for (var i = 0; i < ns; i++) header.label[i] = br.ReadString(16);
            for (var i = 0; i < ns; i++) header.transducerType[i] = br.ReadString(80);
            for (var i = 0; i < ns; i++) header.physicalDimension[i] = br.ReadString(8);
            for (var i = 0; i < ns; i++) header.physicalMinimum[i] = decimal.Parse(br.ReadString(8));
            for (var i = 0; i < ns; i++) header.physicalMaximum[i] = decimal.Parse(br.ReadString(8));
            for (var i = 0; i < ns; i++) header.digitalMinimum[i] = int.Parse(br.ReadString(8));
            for (var i = 0; i < ns; i++) header.digitalMaximum[i] = int.Parse(br.ReadString(8));
            for (var i = 0; i < ns; i++) header.prefiltering[i] = br.ReadString(80);
            for (var i = 0; i < ns; i++) header.numberOfSamples[i] = int.Parse(br.ReadString(8));
            for (var i = 0; i < ns; i++) br.ReadString(32);
            return header;
        }

        public static short[][] ReadDataRecord(this BinaryReader br, int[] numberOfSamples)
        {
            var ns = numberOfSamples.Length;
            var record = new short[ns][];
            for (var i = 0; i < ns; i++)
            {
                var number = numberOfSamples[i];
                record[i] = new short[number];
                for (var j = 0; j < number; j++) record[i][j] = br.ReadInt16();
            }

            return record;
        }

        private static string ReadString(this BinaryReader br, int len)
        {
            var bytes = new byte[len];
            br.Read(bytes, 0, len);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}