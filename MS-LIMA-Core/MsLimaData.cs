using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using Metabolomics.MsLima.Bean;

namespace Metabolomics.MsLima
{ 
    public class MsLimaData
    {
        #region Properties

        /// <summary>
        /// Storage of all data for MS-LIMA
        /// </summary>
        public DataStorageBean DataStorage { get; set; } = new DataStorageBean();
        /// <summary>
        /// ParameterBean, all settings for MS-LIMA
        /// </summary>
        public ParameterBean Parameter { get; set; }

        public string AssemblyPath { get { return Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString(); } }
        public string ParameterFilePath { get { return AssemblyPath + "\\" + "MS-LIMA.conf"; } }
        #endregion
        
        public MsLimaData() {
            ReadParameterFile();
        }

        public void ReadParameterFile()
        {
            Parameter = ParameterBean.ReadParameterFile(ParameterFilePath);
        }

        public void WriteParameterFile()
        {
            ParameterBean.WriteParameterFile(Parameter, ParameterFilePath);
        }

    }

}
