using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
using System.IO;

namespace Metabolomics.Core.Handler
{
    public class MessagePackDefaultHandler
    {
        public static T LoadFromFile<T>(string path)
        {
            try
            {
                T res;
                using (var fs = new FileStream(path, FileMode.Open))
                {
                    res = LZ4MessagePackSerializer.Deserialize<T>(fs);
                }
                return res;
            }
            catch(Exception)
            {                
                return default(T);
            }
        }

        public static void SaveToFile<T>(T obj, string path)
        {
            try
            {
                using (var fs = new FileStream(path, FileMode.Create))
                {
                    LZ4MessagePackSerializer.Serialize<T>(fs, obj);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Erorr!! Cannot Save file");
            }
        }
    }
}
