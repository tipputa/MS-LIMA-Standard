using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Metabolomics.Core.Parser
{
    public static class MetadataParser
    {
        public static string GetAfterChar(string text, char c)
        {
            return text.Substring(text.Split(c)[0].Length + 1).Trim();
        }
        public static string GetBeforeChar(string text, char c)
        {
            return text.Split(c)[0].Trim();
        }

        public static float ConvertSecToMin(float sec)
        {
            return (float)(Math.Round(sec / 60.0f, 3)); 
        }
        public static float ConvertMinToSec(float min)
        {
            return min * 60.0f;
        }
    }
}
