using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using MathNet.Numerics.IntegralTransforms;

namespace Cardio
{
    public class HeartRateSource : ISource<double>, IDisposable
    {
        private readonly float[] _buffer;
        private readonly double _delay;
        private readonly IEnumerator<float> _enumerator;
        private readonly float[] _filter;
        private readonly double _maxRate;
        private readonly double _minRate;
        private readonly int _n;
        private readonly double _outputFrequency;
        private readonly double _sourceFrequency;

        /// <summary>
        ///     Определение частоты сердечных сокращений методом корреляции сигнала измерений сердечных сокращений с использованием
        ///     быстрых преобразований Фурье.
        /// </summary>
        /// <param name="source">Источник данных</param>
        /// <param name="sourceFrequency">Частота дискретизации в источнике данных (Гц)</param>
        /// <param name="outputFrequency">Частота дискретизации выходных данных (Гц)</param>
        /// <param name="delay">Длительность данных в буффере (сек)</param>
        /// <param name="minRate">Минимальная частота пульса (ударов/мин)</param>
        /// <param name="maxRate">Максимальная частота пульса (ударов/мин)</param>
        /// <param name="filter">Параметры фильтра предварительной обработки данных (по-умолчанию двойное дифференцирование)</param>
        public HeartRateSource(IEnumerable<float> source, double sourceFrequency = 250.0, double outputFrequency = 4.0,
            double delay = 3.0,
            double minRate = 50d,
            double maxRate = 200d,
            float[] filter = null)
        {
            _filter = filter ?? new[] {1f, -2f, 1f};
            _outputFrequency = outputFrequency;
            _sourceFrequency = sourceFrequency;
            _delay = delay;
            _minRate = minRate;
            _maxRate = maxRate;
            _n = (int) Math.Ceiling(delay * sourceFrequency);
            _buffer = new float[_n + _filter.Length - 1];
            _enumerator = source.GetEnumerator();
        }

        public void Dispose()
        {
            _enumerator.Dispose();
        }

        /// <summary>
        ///     Частота дискретизации выходных данных (Гц)
        /// </summary>
        /// <returns></returns>
        public double GetFrequency()
        {
            return _outputFrequency;
        }

        /// <summary>
        ///     Прямое преобразование Фурье
        /// </summary>
        /// <param name="samples">Массив комплексных чисел</param>
        private static void Forward(Complex[] samples)
        {
            Fourier.Forward(samples);
        }

        /// <summary>
        ///     Обратное преобразование Фурье
        /// </summary>
        /// <param name="samples">Массив комплексных чисел</param>
        private static void Inverse(Complex[] samples)
        {
            Fourier.Inverse(samples);
        }

        public IEnumerable<double> GetData()
        {
            var pos = 0L;

            for (var i = 0; i < _buffer.Length - 1; i++)
            {
                if (!_enumerator.MoveNext()) throw new ArgumentException("No data");
                _buffer[pos++] = _enumerator.Current;
            }

            var last = -1;

            while (_enumerator.MoveNext())
            {
                _buffer[pos++ % _buffer.Length] = _enumerator.Current;

                var current = (int) ((pos - _buffer.Length - 1) / _sourceFrequency * _outputFrequency);

                if (current == last) continue;

                var samples = Enumerable.Range(0, 2 * _n)
                    .Select(i =>
                        i < _n
                            ? new Complex(_filter.Select((t, j) => t * _buffer[(pos + i - j) % _buffer.Length]).Sum(),
                                0)
                            : Complex.Zero)
                    .ToArray();
                Forward(samples);
                samples = samples
                    .Select(x => new Complex(Math.Pow(x.Magnitude, 2), 0))
                    .ToArray();
                Inverse(samples);
                var correlation = samples.Select((x, i) => x.Real / (1 + Math.Abs(_buffer.Length - i))).ToArray();
                var max = 0d;
                var index = 0;
                for (var i = (int) Math.Ceiling(60d * _sourceFrequency / _maxRate);
                    i < Math.Min(_n, 60d * _sourceFrequency / _minRate) - 1;
                    i++)
                {
                    if (correlation[i] < max) continue;
                    max = correlation[i];
                    index = i;
                }

                last = current;

                yield return 60d * _sourceFrequency / index;
            }
        }
    }
}