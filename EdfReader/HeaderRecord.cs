namespace EdfReader
{
    public class HeaderRecord
    {
        /// <summary>
        ///     8 ascii : version of this data format (0)
        /// </summary>
        public int version { get; set; }

        /// <summary>
        ///     80 ascii : local patient identification
        /// </summary>
        public string patientIdentification { get; set; }

        /// <summary>
        ///     80 ascii : local recording identification
        /// </summary>
        public string recordingIdentification { get; set; }

        /// <summary>
        ///     8 ascii : startdate of recording (dd.mm.yy)
        /// </summary>
        public string startdateOfRecording { get; set; }

        /// <summary>
        ///     8 ascii : starttime of recording (hh.mm.ss)
        /// </summary>
        public string starttimeOfRecording { get; set; }

        /// <summary>
        ///     8 ascii : number of bytes in header record
        /// </summary>
        public int numberOfBytesInHeaderRecord { get; set; }

        /// <summary>
        ///     8 ascii : number of data records (-1 if unknown, obey item 10 of the additional EDF+ specs)
        /// </summary>
        public int numberOfDataRecords { get; set; }

        /// <summary>
        ///     8 ascii : duration of a data record, in seconds
        /// </summary>
        public decimal durationOfDataRecords { get; set; }

        /// <summary>
        ///     number of signals (ns) in data record
        /// </summary>
        public int numberOfSignals { get; set; }

        /// <summary>
        ///     ns * 16 ascii : ns * label (e.g. EEG Fpz-Cz or Body temp) (mind item 9 of the additional EDF+ specs)
        /// </summary>
        public string[] label { get; set; }

        /// <summary>
        ///     ns * 80 ascii : ns * transducer type (e.g. AgAgCl electrode)
        /// </summary>
        public string[] transducerType { get; set; }

        /// <summary>
        ///     ns * 8 ascii : ns * physical dimension (e.g. uV or degreeC)
        /// </summary>
        public string[] physicalDimension { get; set; }

        /// <summary>
        ///     ns * 8 ascii : ns * physical minimum (e.g. -500 or 34)
        /// </summary>
        public decimal[] physicalMinimum { get; set; }

        /// <summary>
        ///     ns * 8 ascii : ns * physical maximum (e.g. 500 or 40)
        /// </summary>
        public decimal[] physicalMaximum { get; set; }

        /// <summary>
        ///     ns * 8 ascii : ns * digital minimum (e.g. -2048)
        /// </summary>
        public int[] digitalMinimum { get; set; }

        /// <summary>
        ///     ns * 8 ascii : ns * digital maximum (e.g. 2047)
        /// </summary>
        public int[] digitalMaximum { get; set; }

        /// <summary>
        ///     ns * 80 ascii : ns * prefiltering (e.g. HP:0.1Hz LP:75Hz)
        /// </summary>
        public string[] prefiltering { get; set; }

        /// <summary>
        ///     ns * 8 ascii : ns * nr of samples in each data record
        /// </summary>
        public int[] numberOfSamples { get; set; }
    }
}