using System.Collections.Generic;

namespace Cardio
{
    public interface ISource<T>
    {
        /// <summary>
        ///     Частота дискретизации выходных данных (Гц)
        /// </summary>
        /// <returns></returns>
        double GetFrequency();

        /// <summary>
        ///     Выходные данные
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetData();
    }
}