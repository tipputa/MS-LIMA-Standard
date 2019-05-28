using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using MessagePack.Internal;
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
            catch (Exception)
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


    public class LargeListMessagePack
    {
        public static int OffsetCutoff = 1073741824;

        static IFormatterResolver defaultResolver;
        public static IFormatterResolver DefaultResolver {
            get {
                if (defaultResolver == null)
                {
                    return StandardResolver.Instance;
                }
                return defaultResolver;
            }
        }
        public static void Serialize<T>(Stream stream, T obj)
        {
            Serialize(stream, obj, defaultResolver);
        }

        /// <summary>
        /// Serialize to stream with specified resolver.
        /// </summary>
        public static void Serialize<T>(Stream stream, T obj, IFormatterResolver resolver)
        {
            if (resolver == null) resolver = DefaultResolver;
            var formatter = resolver.GetFormatterWithVerify<T>();

            var buffer = new byte[65536];
            var len = formatter.Serialize(ref buffer, 0, obj, resolver);

            // do not need resize.
            stream.Write(buffer, 0, len);
        }

        public static void Serialize<T>(Stream stream, List<T> value, IFormatterResolver resolver)
        {
            if (resolver == null) resolver = DefaultResolver;
            var bytes = new byte[65536];
            var offset = 0;
            if (value == null)
            {
                offset += MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                var formatter = resolver.GetFormatterWithVerify<T>();
                var startOffSet = offset;
                var c = value.Count;
                offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, c);

                for (int i = 0; i < c; i++)
                {
                    offset += formatter.Serialize(ref bytes, offset, value[i], resolver);
                    if (offset > OffsetCutoff)
                    {
                        stream.Write(bytes, startOffSet, offset);
                        startOffSet = offset;
                        offset = 0;
                        bytes = new byte[65536];
                    }
                }
            }
        }
    }

    public class LargeMspFormatFormatter<T> : IMessagePackFormatter<List<T>> {
        public List<T> Deserialize(byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize)
        {
            throw new NotImplementedException();
        }

        public int Serialize(ref byte[] bytes, int offset, List<T> value, IFormatterResolver formatterResolver)
        {
            if (value == null)
            {
                return MessagePackBinary.WriteNil(ref bytes, offset);
            }
            else
            {
                var startOffset = offset;
                var formatter = formatterResolver.GetFormatterWithVerify<T>();

                var c = value.Count;
                offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, c);

                for (int i = 0; i < c; i++)
                {
                    offset += formatter.Serialize(ref bytes, offset, value[i], formatterResolver);
                }                
                return offset - startOffset;
            }
        }
    }

    public class CustomCompositeResolver : IFormatterResolver
    {
        public static IFormatterResolver Instance = new CustomCompositeResolver();

        static readonly IFormatterResolver[] resolvers = new[]
        {
            // resolver custom types first
            SampleCustomResolver.Instance,

            // finaly use standard resolver
            StandardResolver.Instance
        };

        CustomCompositeResolver()
        {
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            static FormatterCache()
            {
                foreach (var item in resolvers)
                {
                    var f = item.GetFormatter<T>();
                    if (f != null)
                    {
                        formatter = f;
                        return;
                    }
                }
            }
        }
    }

    public class SampleCustomResolver : IFormatterResolver
    {
        // Resolver should be singleton.
        public static IFormatterResolver Instance = new SampleCustomResolver();

        SampleCustomResolver()
        {
        }

        // GetFormatter<T>'s get cost should be minimized so use type cache.
        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.formatter;
        }

        static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> formatter;

            // generic's static constructor should be minimized for reduce type generation size!
            // use outer helper method.
            static FormatterCache()
            {
                formatter = (IMessagePackFormatter<T>)SampleCustomResolverGetFormatterHelper.GetFormatter(typeof(T));
            }
        }
    }

    internal static class SampleCustomResolverGetFormatterHelper
    {
        // If type is concrete type, use type-formatter map
        static readonly Dictionary<Type, object> formatterMap = new Dictionary<Type, object>()
    {
        {typeof(List<MspBean>), new LargeMspFormatFormatter<MspBean>()}
        // add more your own custom serializers.
    };

        internal static object GetFormatter(Type t)
        {
            object formatter;
            if (formatterMap.TryGetValue(t, out formatter))
            {
                return formatter;
            }

            // If target type is generics, use MakeGenericType.
            if (t.IsGenericParameter && t.GetGenericTypeDefinition() == typeof(ValueTuple<,>))
            {
                return Activator.CreateInstance(typeof(ValueTupleFormatter<,>).MakeGenericType(t.GenericTypeArguments));
            }

            // If type can not get, must return null for fallback mecanism.
            return null;
        }
    }

    [MessagePackObject]
    public class Test{
        [Key(0)]
        public int Test1 { get; set; } = 0;
        [Key(1)]
        public int Test2 { get; set; } = 0;
    }

}
