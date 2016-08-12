using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using ChristianMoser.WpfInspector.Baml;

namespace ChristianMoser.WpfInspector.Services.Xaml.BamlReader
{
    internal class ResourceReader
    {
        // Fields
        private readonly Resource[] _resources;

        // Methods
        public ResourceReader(Stream stream)
        {
            var reader = new BinaryReader(stream);
            if (reader.ReadUInt32() != 0xbeefcace)
            {
                _resources = new Resource[0];
                return;
                //throw new InvalidOperationException("Invalid resource file signature.");
            }
            if (reader.ReadInt32() > 1)
            {
                reader.BaseStream.Seek((long)reader.ReadInt32(), SeekOrigin.Current);
            }
            else
            {
                reader.ReadInt32();
                string str = reader.ReadString();
                reader.ReadString();
                if (!str.StartsWith("System.Resources.ResourceReader"))
                {
                    _resources = new Resource[0];
                    return;
                    //throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "This resource reader type is not supported: '{0}'.", new object[] { str }));
                }
            }
            int num3 = reader.ReadInt32();
            if ((num3 != 2) && (num3 != 1))
            {
                _resources = new Resource[0];
                return;
                //throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Unknown runtime version '{0}'.", new object[] { num3 }));
            }
            int num4 = reader.ReadInt32();
            string[] resourceTypeNames = new string[reader.ReadInt32()];
            for (int i = 0; i < resourceTypeNames.Length; i++)
            {
                resourceTypeNames[i] = reader.ReadString();
            }
            if ((reader.BaseStream.Position % 8L) != 0L)
            {
                reader.ReadBytes((int)(8L - (reader.BaseStream.Position % 8L)));
            }
            int[] numArray = new int[num4];
            for (int j = 0; j < numArray.Length; j++)
            {
                numArray[j] = reader.ReadInt32();
            }
            int[] numArray2 = new int[num4];
            for (int k = 0; k < numArray2.Length; k++)
            {
                numArray2[k] = reader.ReadInt32();
            }
            long num8 = reader.ReadInt32();
            long position = reader.BaseStream.Position;
            int[] numArray3 = new int[num4];
            string[] strArray2 = new string[num4];
            for (int m = 0; m < strArray2.Length; m++)
            {
                reader.BaseStream.Position = position + numArray2[m];
                int count = this.Read7BitEncodedInt(reader);
                byte[] bytes = reader.ReadBytes(count);
                strArray2[m] = Encoding.Unicode.GetString(bytes, 0, count);
                numArray3[m] = reader.ReadInt32();
            }
            this._resources = new Resource[num4];
            for (int n = 0; n < this._resources.Length; n++)
            {
                reader.BaseStream.Position = num8 + numArray3[n];
                Resource resource = new Resource();
                resource.Name = strArray2[n];
                resource.Value = null;
                if (num3 == 1)
                {
                    this.LoadResourceValue(resource, reader, resourceTypeNames);
                }
                else
                {
                    this.LoadResourceValue(resource, reader);
                }
                this._resources[n] = resource;
            }
        }

        private void Deserialize(Resource resource, Stream stream)
        {
            try
            {
                resource.Value = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Persistence | StreamingContextStates.File)).Deserialize(stream);
            }
            catch (FileNotFoundException exception)
            {
                resource.Exception = exception;
                resource.Value = null;
            }
            catch (SerializationException exception2)
            {
                resource.Exception = exception2;
                resource.Value = null;
            }
            catch (ArgumentNullException exception3)
            {
                resource.Exception = exception3;
                resource.Value = null;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return this._resources.GetEnumerator();
        }

        private void LoadResourceValue(Resource resource, BinaryReader reader)
        {
            switch (this.Read7BitEncodedInt(reader))
            {
                case 0:
                    resource.Value = null;
                    return;

                case 1:
                    resource.Value = reader.ReadString();
                    return;

                case 2:
                    resource.Value = reader.ReadBoolean();
                    return;

                case 3:
                    resource.Value = (char)reader.ReadUInt16();
                    return;

                case 4:
                    resource.Value = reader.ReadByte();
                    return;

                case 5:
                    resource.Value = reader.ReadSByte();
                    return;

                case 6:
                    resource.Value = reader.ReadInt16();
                    return;

                case 7:
                    resource.Value = reader.ReadUInt16();
                    return;

                case 8:
                    resource.Value = reader.ReadInt32();
                    return;

                case 9:
                    resource.Value = reader.ReadUInt32();
                    return;

                case 10:
                    resource.Value = reader.ReadInt64();
                    return;

                case 11:
                    resource.Value = reader.ReadUInt64();
                    return;

                case 12:
                    resource.Value = reader.ReadSingle();
                    return;

                case 13:
                    resource.Value = reader.ReadDouble();
                    return;

                case 14:
                    resource.Value = reader.ReadDecimal();
                    return;

                case 15:
                    resource.Value = new DateTime(reader.ReadInt64());
                    return;

                case 0x10:
                    resource.Value = new TimeSpan(reader.ReadInt64());
                    return;

                case 0x20:
                    resource.Value = reader.ReadBytes(reader.ReadInt32());
                    return;

                case 0x21:
                    resource.Value = new MemoryStream(reader.ReadBytes(reader.ReadInt32()));
                    return;
            }
            this.Deserialize(resource, reader.BaseStream);
        }

        private void LoadResourceValue(Resource resource, BinaryReader reader, string[] resourceTypeNames)
        {
            int index = this.Read7BitEncodedInt(reader);
            if (index != -1)
            {
                Type type = null;
                try
                {
                    type = Type.GetType(resourceTypeNames[index], true);
                }
                catch (FileNotFoundException exception)
                {
                    resource.Exception = exception;
                }
                catch (IndexOutOfRangeException exception2)
                {
                    resource.Exception = exception2;
                }
                if (type == typeof(string))
                {
                    resource.Value = reader.ReadString();
                }
                else if (type == typeof(int))
                {
                    resource.Value = reader.ReadInt32();
                }
                else if (type == typeof(byte))
                {
                    resource.Value = reader.ReadByte();
                }
                else if (type == typeof(sbyte))
                {
                    resource.Value = reader.ReadSByte();
                }
                else if (type == typeof(short))
                {
                    resource.Value = reader.ReadInt16();
                }
                else if (type == typeof(long))
                {
                    resource.Value = reader.ReadInt64();
                }
                else if (type == typeof(ushort))
                {
                    resource.Value = reader.ReadUInt16();
                }
                else if (type == typeof(uint))
                {
                    resource.Value = reader.ReadUInt32();
                }
                else if (type == typeof(ulong))
                {
                    resource.Value = reader.ReadUInt64();
                }
                else if (type == typeof(float))
                {
                    resource.Value = reader.ReadSingle();
                }
                else if (type == typeof(double))
                {
                    resource.Value = reader.ReadDouble();
                }
                else if (type == typeof(decimal))
                {
                    resource.Value = reader.ReadDecimal();
                }
                else if (type == typeof(DateTime))
                {
                    resource.Value = new DateTime(reader.ReadInt64());
                }
                else if (type == typeof(TimeSpan))
                {
                    resource.Value = new TimeSpan(reader.ReadInt64());
                }
                else
                {
                    this.Deserialize(resource, reader.BaseStream);
                }
            }
        }

        private int Read7BitEncodedInt(BinaryReader reader)
        {
            byte num3;
            int num = 0;
            int num2 = 0;
            do
            {
                num3 = reader.ReadByte();
                num |= (num3 & 0x7f) << num2;
                num2 += 7;
            }
            while ((num3 & 0x80) != 0);
            return num;
        }

        // Nested Types
        private enum ResourceTypeCode
        {
            Boolean = 2,
            Byte = 4,
            ByteArray = 0x20,
            Char = 3,
            DateTime = 15,
            Decimal = 14,
            Double = 13,
            Int16 = 6,
            Int32 = 8,
            Int64 = 10,
            Null = 0,
            SByte = 5,
            Single = 12,
            StartOfUserTypes = 0x40,
            Stream = 0x21,
            String = 1,
            TimeSpan = 0x10,
            UInt16 = 7,
            UInt32 = 9,
            UInt64 = 11
        }
    }


}
