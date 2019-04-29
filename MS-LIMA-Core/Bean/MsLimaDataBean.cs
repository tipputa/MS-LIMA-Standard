using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace Metabolomics.MsLima.Bean
{ 
    public class MsLimaDataBean
    {
        #region Properties

        /// <summary>
        /// Storage of all data for MS-LIMA
        /// </summary>
        public DataStorageBean DataStorage { get; set; }
        /// <summary>
        /// ParameterBean, all settings for MS-LIMA
        /// </summary>
        public ParameterBean Parameter { get; set; }

        public string AssemblyPath { get { return Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString(); } }
        public string ParameterFilePath { get { return AssemblyPath + "\\" + "MS-LIMA.conf"; } }
        #endregion
        
        public MsLimaDataBean() {
            WriteParameterFile();
            Console.WriteLine(ParameterFilePath);
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
