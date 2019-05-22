using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MessagePack;
using Metabolomics.Core.Handler;

namespace Metabolomics.MsLima.Bean
{
    [MessagePackObject]
    public class ParameterBean
    {
        #region Properties
        /// <summary>
        /// Tolerance of MS2 peaks
        /// </summary>
        [Key(0)]
        public double MS2Tol { get; set; } = 0.01;
        public int GraphHeightInMultipleView { get; set; } = 200;
        public int NumberOfDecimalPlaces { get; set; } = 3;
        public int AutoExportIntervalMillisecond { get; set; } = 1000;
        #endregion
        public ParameterBean() { }

        public static ParameterBean ReadParameterFile(string filePath)
        {
            var parameter = MessagePackDefaultHandler.LoadFromFile<ParameterBean>(filePath);
            if (parameter == null)
            {
                parameter = new ParameterBean();
            }
            return parameter;
        }

        public static void WriteParameterFile(ParameterBean parameter, string filePath)
        {
            if (parameter == null)
            {
                return;
            }
            MessagePackDefaultHandler.SaveToFile<ParameterBean>(parameter, filePath);
        }
    }

}
