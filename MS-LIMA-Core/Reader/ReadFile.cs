using System;
using System.Collections.Generic;
using System.Text;
using Metabolomics.Core;
using Metabolomics.Core.Parser;

namespace Metabolomics.MsLima.Reader
{
    public sealed class ReadFile
    {
        public static List<MspBean> ReadMspFile(string filePath)
        {
            return MspParser.MspFileReader(filePath);
        }
    }
}
