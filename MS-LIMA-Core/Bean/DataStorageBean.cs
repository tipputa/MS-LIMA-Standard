using System;
using System.Collections.Generic;
using System.Text;
using Metabolomics.Core;
using Metabolomics.Core.Parser;
using Metabolomics.Core.Handler;

namespace Metabolomics.MsLima.Bean
{
    public class DataStorageBean
    {
        #region Properties
        public List<MspBean> RawMspFile { get; set; }
        public List<MspBean> CopyRawMspFile { get; set; }
        #endregion
    }
}
