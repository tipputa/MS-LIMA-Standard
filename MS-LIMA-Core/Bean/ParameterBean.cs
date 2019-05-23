using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MessagePack;
using Metabolomics.Core.Handler;

namespace Metabolomics.MsLima.Bean
{
    public enum CompoundGroupingKey { InChI, InChIKey, ShortInChIKey }
    [MessagePackObject]
    public class WindowParameterBean
    {
        [Key(0)]
        public int AutoExportIntervalMillisecond { get; set; } = 1000;
        [Key(1)]
        public int GraphHeightInMultipleView { get; set; } = 200;
        public WindowParameterBean() { }
    }

    [MessagePackObject]
    public class ParameterBean
    {
        #region Properties
        /// <summary>
        /// Tolerance of MS2 peaks
        /// </summary>
        [Key(0)]
        public WindowParameterBean WinParam { get; set; }
        [Key(1)]
        public CompoundGroupingKey CompoundGroupingKey { get; set; } = CompoundGroupingKey.ShortInChIKey;
        [Key(2)]
        public double MS2Tol { get; set; } = 0.01;
        [Key(3)]
        public int NumberOfDecimalPlaces { get; set; } = 3;
        [Key(4)]
        public int MinimumNumberOfSamplesForConsensus { get; set; } = 1;
        
        #endregion
        public ParameterBean() { }
        public ParameterBean(bool IsWindow = true)
        {
            this.WinParam = new WindowParameterBean();
        }

        public static ParameterBean ReadParameterFile(string filePath, bool IsWindows = true)
        {
            var parameter = MessagePackDefaultHandler.LoadFromFile<ParameterBean>(filePath);
            if (parameter == null)
            {
                parameter = new ParameterBean(IsWindows);
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

        public ParameterBean Copy()
        {
            return new ParameterBean()
            {
                MS2Tol = this.MS2Tol,
                NumberOfDecimalPlaces = this.NumberOfDecimalPlaces,
                MinimumNumberOfSamplesForConsensus = this.MinimumNumberOfSamplesForConsensus,
                WinParam = new WindowParameterBean()
                {
                    GraphHeightInMultipleView = this.WinParam.GraphHeightInMultipleView,
                    AutoExportIntervalMillisecond = this.WinParam.AutoExportIntervalMillisecond
                }
            };
        }
    }

}
