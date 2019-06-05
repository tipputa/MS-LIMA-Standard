using Microsoft.VisualStudio.TestTools.UnitTesting;
using Metabolomics.Core.Parser;
using Metabolomics.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class MessagePackTest
    {
        [TestMethod]
        public void TestNewMessagePack()
        {
            /*
            var file = @"C:\Users\tipputa\Downloads\MSMS-RIKEN-Pos-VS11.msp";
            //var file = @"C:\Users\tipputa\Downloads\LC02MS02 iSTD MSP curated.msp";
            var msp = MspParser.MspFileReader(file);
            */
            var file2 = @"C:\Users\tipputa\Downloads\MSDIAL-LipidDB-VS47.msp";
            var file3 = @"C:\Users\tipputa\Downloads\MSDIAL-LipidDB-VS47.msp2";
            var file4 = @"C:\Users\tipputa\Downloads\TestMessagePackLarge2.msp2";

            var msp = MspParser.MspFileReader(file2);
            SaveNewMessagePack(msp, file3);
            /*var msp = ReadNewMessagePack(file3);

            var largeMsp = new List<MspBean>();
            /*AddList(largeMsp, msp);
            AddList(largeMsp, msp);
            AddList(largeMsp, msp);
            AddList(largeMsp, msp);
            AddList(largeMsp, msp);
            AddList(largeMsp, msp);
            AddList(largeMsp, msp);
            AddList(largeMsp, msp);
            AddList(largeMsp, msp);
            AddList(largeMsp, msp);
            
            AddList(largeMsp, msp);
            AddList(largeMsp, msp);

            Console.WriteLine("num small" + msp.Count);
            Console.WriteLine("num large" + largeMsp.Count);


            SaveNewMessagePack(largeMsp, file4);
            var tmp2 = ReadNewMessagePack(file4);
            Console.WriteLine(tmp2.Count);
            */
        }

        public void SaveNewMessagePack(List<MspBean> msp, string file)
        {
            using (var fs = new FileStream(file, FileMode.Create))
            {
                Metabolomics.Core.Handler.LargeListMessagePack.Serialize<MspBean>(fs, msp, null);
            }
        }

        public List<MspBean> ReadNewMessagePack(string file)
        {
            using (var fs = new FileStream(file, FileMode.Open))
            {
                return Metabolomics.Core.Handler.LargeListMessagePack.Deserialize<MspBean>(fs, null);
            }

        }

        private void AddList<T>(List<T> a, List<T> b){
            foreach(var t in b)
            {
                a.Add(t);
            }
        }
    }
}
