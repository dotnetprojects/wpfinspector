using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Markup;
using System;

namespace ChristianMoser.WpfInspector.Services.Xaml.BamlReader
{
    public static class BamlLoader
    {
        public static object LoadBaml(Stream stream)
        {
            var presentationFrameworkAssembly = Assembly.GetAssembly(typeof(Button));

            if( Environment.Version.Major == 4)
            {
                var xamlAssembly = Assembly.Load("System.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
                var readerType = presentationFrameworkAssembly.GetType("System.Windows.Baml2006.Baml2006Reader");
                var reader = Activator.CreateInstance(readerType, stream);
                var schemaContextProperty = readerType.GetProperty("SchemaContext");
                var schemaContext = schemaContextProperty.GetGetMethod().Invoke(reader, null);
                var writerType = xamlAssembly.GetType("System.Xaml.XamlObjectWriter");
                var writer = Activator.CreateInstance(writerType, schemaContext);
                var readerReadMethod = readerType.GetMethod("Read");
                var writerWriteMethod = writerType.GetMethod("WriteNode");
                while( (bool)readerReadMethod.Invoke(reader, null))
                {
                    writerWriteMethod.Invoke(writer, new[] {reader});
                }
                var writerResultProperty = writerType.GetProperty("Result");
                return writerResultProperty.GetGetMethod().Invoke(writer, null);
            }
            else
            {
                var pc = new ParserContext();
                var readerType = presentationFrameworkAssembly.GetType("System.Windows.Markup.XamlReader");
                var method = readerType.GetMethod("LoadBaml", BindingFlags.NonPublic | BindingFlags.Static);
                return method.Invoke(null, new object[] {stream, pc, null, false});
            }
        }
    }
}
