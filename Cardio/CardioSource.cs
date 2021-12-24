using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using EdfReader;

namespace Cardio
{
    /// <summary>
    ///     Данные сигнала измерений сердечных сокращений, полученные из EDF файла
    /// </summary>
    public class CardioSource : ISource<float>, IDisposable
    {
        private readonly BinaryReader _br;
        private readonly int _cardioSignalNumber;
        private readonly HeaderRecord _header;
        private readonly int _nbOfSamples;

        public CardioSource(Stream stream)
        {
            _br = new BinaryReader(stream);
            _header = _br.ReadHeaderRecord();
            _cardioSignalNumber = _header.CardioSignalNumber(out _nbOfSamples);
        }

        public void Dispose()
        {
            _br.Dispose();
        }

        /// <summary>
        ///     Частота дискретизации выходных данных (Гц)
        /// </summary>
        /// <returns></returns>
        public double GetFrequency()
        {
            return _nbOfSamples / (double) _header.durationOfDataRecords;
        }

        /// <summary>
        ///     Дата и время начала измерений, полученная из заголовка EDF файла
        /// </summary>
        /// <returns></returns>
        public DateTime GetStartDateTime()
        {
            var regex = new Regex(
                @"^(?<day>\d+)\D(?<month>\d+)\D(?<year>\d+)T(?<hour>\d+)\D(?<minute>\d+)\D(?<second>\d+)$");
            var match = regex.Match($"{_header.startdateOfRecording}T{_header.starttimeOfRecording}");
            int.TryParse(match.Groups["day"].Value, out var day);
            int.TryParse(match.Groups["month"].Value, out var month);
            int.TryParse(match.Groups["year"].Value, out var year);
            int.TryParse(match.Groups["hour"].Value, out var hour);
            int.TryParse(match.Groups["minute"].Value, out var minute);
            int.TryParse(match.Groups["second"].Value, out var second);
            return new DateTime(year < 50 ? 2000 + year : 1900 + year, month, day, hour, minute, second);
        }

        public IEnumerable<float> GetData()
        {
            var nr = _header.numberOfDataRecords;
            var numberOfSamples = _header.numberOfSamples;

            for (var i = 0; i < nr; i++)
            {
                var currentRecord = _br.ReadDataRecord(numberOfSamples);
                var signal = currentRecord[_cardioSignalNumber];
                for (var j = 0; j < _nbOfSamples; j++)
                    yield return signal[j];
            }
        }
    }
}