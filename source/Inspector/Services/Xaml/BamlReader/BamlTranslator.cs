using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.IO;

namespace ChristianMoser.WpfInspector.Baml
{
    internal class BamlTranslator
    {
        // Fields
        private IDictionary assemblyTable;
        private IList constructorParameterTable;
        private IDictionary dictionaryKeyPositionTable;
        private IDictionary dictionaryKeyStartTable;
        private Stack elementStack;
        private PropertyDeclaration[] knownPropertyTable;
        private Hashtable knownResourceTable;
        private TypeDeclaration[] knownTypeTable;
        private int lineNumber;
        private int linePosition;
        private NamespaceManager namespaceManager;
        private IDictionary propertyTable;
        private Element rootElement;
        private IList staticResourceTable;
        private IDictionary stringTable;
        private IDictionary typeTable;

        // Methods
        public BamlTranslator(Stream stream)
        {
            BamlRecordType type;
            long position;
            int num6;
            this.assemblyTable = new Hashtable();
            this.stringTable = new Hashtable();
            this.typeTable = new Hashtable();
            this.propertyTable = new Hashtable();
            this.staticResourceTable = new ArrayList();
            this.namespaceManager = new NamespaceManager();
            this.rootElement = null;
            this.elementStack = new Stack();
            this.constructorParameterTable = new ArrayList();
            knownTypeTable = null;
            knownPropertyTable = null;
            this.knownResourceTable = new Hashtable();
            this.dictionaryKeyStartTable = new Hashtable();
            this.dictionaryKeyPositionTable = new Hashtable();
            this.lineNumber = 0;
            this.linePosition = 0;
            BamlBinaryReader reader = new BamlBinaryReader(stream);
            int num = reader.ReadInt32();
            string str = new string(new BinaryReader(stream, Encoding.Unicode).ReadChars(num >> 1));
            if (str != "MSBAML")
            {
                throw new NotSupportedException();
            }
            int num2 = reader.ReadInt32();
            int num3 = reader.ReadInt32();
            int num4 = reader.ReadInt32();
            if (((num2 == 0x600000) && (num3 == 0x600000)) && (num4 == 0x600000))
            {
                goto Label_04F2;
            }
            throw new NotSupportedException();
        Label_01CB:
            switch (type)
            {
                case BamlRecordType.DocumentStart:
                    reader.ReadBoolean();
                    reader.ReadInt32();
                    reader.ReadBoolean();
                    break;

                case BamlRecordType.DocumentEnd:
                    break;

                case BamlRecordType.ElementStart:
                    this.namespaceManager.OnElementStart();
                    this.ReadElementStart(reader);
                    break;

                case BamlRecordType.ElementEnd:
                    this.ReadElementEnd();
                    this.namespaceManager.OnElementEnd();
                    break;

                case BamlRecordType.Property:
                    this.ReadPropertyRecord(reader);
                    break;

                case BamlRecordType.PropertyCustom:
                    this.ReadPropertyCustom(reader);
                    break;

                case BamlRecordType.PropertyComplexStart:
                    this.ReadPropertyComplexStart(reader);
                    break;

                case BamlRecordType.PropertyComplexEnd:
                    this.ReadPropertyComplexEnd();
                    break;

                case BamlRecordType.PropertyListStart:
                    this.ReadPropertyListStart(reader);
                    break;

                case BamlRecordType.PropertyListEnd:
                    this.ReadPropertyListEnd();
                    break;

                case BamlRecordType.PropertyDictionaryStart:
                    this.ReadPropertyDictionaryStart(reader);
                    break;

                case BamlRecordType.PropertyDictionaryEnd:
                    this.ReadPropertyDictionaryEnd();
                    break;

                case BamlRecordType.Text:
                    this.ReadText(reader);
                    break;

                case BamlRecordType.TextWithConverter:
                    this.ReadTextWithConverter(reader);
                    break;

                case BamlRecordType.XmlnsProperty:
                    this.ReadXmlnsProperty(reader);
                    break;

                case BamlRecordType.DefAttribute:
                    this.ReadDefAttribute(reader);
                    break;

                case BamlRecordType.PIMapping:
                    this.ReadNamespaceMapping(reader);
                    break;

                case BamlRecordType.AssemblyInfo:
                    this.ReadAssemblyInfo(reader);
                    break;

                case BamlRecordType.TypeInfo:
                    this.ReadTypeInfo(reader);
                    break;

                case BamlRecordType.AttributeInfo:
                    this.ReadAttributeInfo(reader);
                    break;

                case BamlRecordType.StringInfo:
                    this.ReadStringInfo(reader);
                    break;

                case BamlRecordType.PropertyTypeReference:
                    this.ReadPropertyTypeReference(reader);
                    break;

                case BamlRecordType.PropertyWithExtension:
                    this.ReadPropertyWithExtension(reader);
                    break;

                case BamlRecordType.PropertyWithConverter:
                    this.ReadPropertyWithConverter(reader);
                    break;

                case BamlRecordType.DeferableContentStart:
                    reader.ReadInt32();
                    break;

                case BamlRecordType.DefAttributeKeyString:
                    this.ReadDefAttributeKeyString(reader);
                    break;

                case BamlRecordType.DefAttributeKeyType:
                    this.ReadDefAttributeKeyType(reader);
                    break;

                case BamlRecordType.KeyElementStart:
                    this.ReadKeyElementStart(reader);
                    break;

                case BamlRecordType.KeyElementEnd:
                    this.ReadKeyElementEnd();
                    break;

                case BamlRecordType.ConstructorParametersStart:
                    this.ReadConstructorParametersStart();
                    break;

                case BamlRecordType.ConstructorParametersEnd:
                    this.ReadConstructorParametersEnd();
                    break;

                case BamlRecordType.ConstructorParameterType:
                    this.ReadConstructorParameterType(reader);
                    break;

                case BamlRecordType.ConnectionId:
                    reader.ReadInt32();
                    break;

                case BamlRecordType.ContentProperty:
                    this.ReadContentProperty(reader);
                    break;

                case BamlRecordType.StaticResourceStart:
                    this.ReadStaticResourceStart(reader);
                    break;

                case BamlRecordType.StaticResourceEnd:
                    this.ReadStaticResourceEnd(reader);
                    break;

                case BamlRecordType.StaticResourceId:
                    this.ReadStaticResourceIdentifier(reader);
                    break;

                case BamlRecordType.TextWithId:
                    this.ReadTextWithId(reader);
                    break;

                case BamlRecordType.PresentationOptionsAttribute:
                    this.ReadPresentationOptionsAttribute(reader);
                    break;

                case BamlRecordType.LineNumberAndPosition:
                    this.lineNumber = reader.ReadInt32();
                    this.linePosition = reader.ReadInt32();
                    break;

                case BamlRecordType.LinePosition:
                    this.linePosition = reader.ReadInt32();
                    break;

                case BamlRecordType.OptimizedStaticResource:
                    this.ReadOptimizedStaticResource(reader);
                    break;

                case BamlRecordType.PropertyWithStaticResourceId:
                    this.ReadPropertyWithStaticResourceIdentifier(reader);
                    break;

                default:
                    throw new NotSupportedException(type.ToString());
            }
            if (num6 > 0)
            {
                reader.BaseStream.Position = position + num6;
            }
        Label_04F2:
            if (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                type = (BamlRecordType)reader.ReadByte();
                position = reader.BaseStream.Position;
                num6 = 0;
                switch (type)
                {
                    case BamlRecordType.Property:
                    case BamlRecordType.PropertyCustom:
                    case BamlRecordType.Text:
                    case BamlRecordType.TextWithConverter:
                    case BamlRecordType.XmlnsProperty:
                    case BamlRecordType.DefAttribute:
                    case BamlRecordType.PIMapping:
                    case BamlRecordType.AssemblyInfo:
                    case BamlRecordType.TypeInfo:
                    case BamlRecordType.AttributeInfo:
                    case BamlRecordType.StringInfo:
                    case BamlRecordType.PropertyWithConverter:
                    case BamlRecordType.DefAttributeKeyString:
                    case BamlRecordType.TextWithId:
                    case BamlRecordType.PresentationOptionsAttribute:
                        num6 = reader.ReadCompressedInt32();
                        goto Label_01CB;

                    case BamlRecordType.RoutedEvent:
                    case BamlRecordType.ClrEvent:
                    case BamlRecordType.XmlAttribute:
                    case BamlRecordType.ProcessingInstruction:
                    case BamlRecordType.Comment:
                    case BamlRecordType.DefTag:
                    case BamlRecordType.EndAttributes:
                    case BamlRecordType.TypeSerializerInfo:
                    case BamlRecordType.DeferableContentStart:
                        goto Label_01CB;
                }
                goto Label_01CB;
            }
        }

        private void AddContent(Element parent, object content)
        {
            if (content == null)
            {
                throw new ArgumentNullException();
            }
            Property contentProperty = this.GetContentProperty(parent);
            if (contentProperty == null)
            {
                contentProperty = new Property(PropertyType.Content);
                parent.Properties.Add(contentProperty);
            }
            if (contentProperty.Value != null)
            {
                if (contentProperty.Value is string)
                {
                    IList list = new ArrayList();
                    list.Add(contentProperty.Value);
                    contentProperty.Value = list;
                }
                if (!(contentProperty.Value is IList))
                {
                    throw new NotSupportedException();
                }
                ((IList)contentProperty.Value).Add(content);
            }
            else if (content is string)
            {
                contentProperty.Value = content;
            }
            else
            {
                IList list3 = new ArrayList();
                list3.Add(content);
                contentProperty.Value = list3;
            }
        }

        private void AddDictionaryEntry(Element dictionary, int position, Property keyProperty)
        {
            IDictionary dictionary2 = (IDictionary)this.dictionaryKeyPositionTable[dictionary];
            if (dictionary2 == null)
            {
                dictionary2 = new Hashtable();
                this.dictionaryKeyPositionTable.Add(dictionary, dictionary2);
            }
            IList list = (IList)dictionary2[position];
            if (list == null)
            {
                list = new ArrayList();
                dictionary2.Add(position, list);
            }
            list.Add(keyProperty);
        }

        private void AddElementToTree(Element element, BamlBinaryReader reader)
        {
            if (this.rootElement == null)
            {
                this.rootElement = element;
            }
            else
            {
                Property property = this.elementStack.Peek() as Property;
                if (property != null)
                {
                    switch (property.PropertyType)
                    {
                        case PropertyType.List:
                        case PropertyType.Dictionary:
                            ((IList)property.Value).Add(element);
                            return;

                        case PropertyType.Complex:
                            property.Value = element;
                            return;
                    }
                    throw new NotSupportedException();
                }
                Element key = this.elementStack.Peek() as Element;
                if (this.dictionaryKeyPositionTable.Contains(key))
                {
                    int num = (int)(reader.BaseStream.Position - 1L);
                    if (!this.dictionaryKeyStartTable.Contains(key))
                    {
                        this.dictionaryKeyStartTable.Add(key, num);
                    }
                    int num2 = (int)this.dictionaryKeyStartTable[key];
                    int num3 = num - num2;
                    IDictionary dictionary = (IDictionary)this.dictionaryKeyPositionTable[key];
                    if ((dictionary != null) && dictionary.Contains(num3))
                    {
                        IList list2 = (IList)dictionary[num3];
                        foreach (Property property2 in list2)
                        {
                            element.Properties.Add(property2);
                        }
                    }
                }
                if (key == null)
                {
                    throw new NotSupportedException();
                }
                if ((this.constructorParameterTable.Count > 0) && (this.constructorParameterTable[this.constructorParameterTable.Count - 1] == key))
                {
                    key.Arguments.Add(element);
                }
                else
                {
                    this.AddContent(key, element);
                }
            }
        }

        private Element CreateTypeExtension(short typeIdentifier)
        {
            return this.CreateTypeExtension(typeIdentifier, true);
        }

        private Element CreateTypeExtension(short typeIdentifier, bool wrapInType)
        {
            Element element = new Element();
            element.TypeDeclaration = new TypeDeclaration("x:Type");
            TypeDeclaration typeDeclaration = this.GetTypeDeclaration(typeIdentifier);
            if (typeDeclaration == null)
            {
                throw new NotSupportedException();
            }
            if (!wrapInType)
            {
                element.TypeDeclaration = typeDeclaration;
                return element;
            }
            element.Arguments.Add(typeDeclaration);
            return element;
        }

        private string GetAssembly(string assemblyName)
        {
            return assemblyName;
        }

        private Property GetContentProperty(Element parent)
        {
            foreach (Property property in parent.Properties)
            {
                if (property.PropertyType == PropertyType.Content)
                {
                    return property;
                }
            }
            return null;
        }

        private PropertyDeclaration GetPropertyDeclaration(short identifier)
        {
            PropertyDeclaration declaration = null;
            if (identifier >= 0)
            {
                declaration = (PropertyDeclaration)this.propertyTable[identifier];
            }
            else
            {
                if (knownPropertyTable == null)
                {
                    this.Initialize();
                }
                declaration = knownPropertyTable[-identifier];
            }
            if (declaration == null)
            {
                throw new NotSupportedException();
            }
            return declaration;
        }

        private object GetResourceName(short identifier)
        {
            if (identifier >= 0)
            {
                PropertyDeclaration declaration = (PropertyDeclaration)this.propertyTable[identifier];
                return new ResourceName(declaration.Name);
            }
            if (this.knownResourceTable.Count == 0)
            {
                this.Initialize();
            }
            identifier = (short)-identifier;
            if (identifier > 0xe8)
            {
                identifier = (short)(identifier - 0xe8);
            }
            return (ResourceName)this.knownResourceTable[(int)identifier];
        }

        private object GetStaticResource(short identifier)
        {
            return this.staticResourceTable[identifier];
        }

        private TypeDeclaration GetTypeDeclaration(short identifier)
        {
            TypeDeclaration type = null;
            if (identifier >= 0)
            {
                type = (TypeDeclaration)this.typeTable[identifier];
            }
            else
            {
                if (knownTypeTable == null)
                {
                    this.Initialize();
                }
                type = knownTypeTable[-identifier];
            }
            string xmlNamespace = this.namespaceManager.GetXmlNamespace(type);
            if (xmlNamespace != null)
            {
                string prefix = this.namespaceManager.GetPrefix(xmlNamespace);
                if (prefix != null)
                {
                    type = type.Copy(prefix);
                }
            }
            if (type == null)
            {
                throw new NotSupportedException();
            }
            return type;
        }

        private void Initialize()
        {
            knownTypeTable = new TypeDeclaration[760];
            knownTypeTable[0] = new TypeDeclaration(string.Empty, string.Empty, string.Empty);
            knownTypeTable[1] = new TypeDeclaration("AccessText", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[2] = new TypeDeclaration("AdornedElementPlaceholder", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[3] = new TypeDeclaration("Adorner", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[4] = new TypeDeclaration("AdornerDecorator", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[5] = new TypeDeclaration("AdornerLayer", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[6] = new TypeDeclaration("AffineTransform3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[7] = new TypeDeclaration("AmbientLight", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[8] = new TypeDeclaration("AnchoredBlock", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[9] = new TypeDeclaration("Animatable", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[10] = new TypeDeclaration("AnimationClock", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[11] = new TypeDeclaration("AnimationTimeline", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[12] = new TypeDeclaration("Application", "System.Net.Mime", this.GetAssembly("System"));
            knownTypeTable[13] = new TypeDeclaration("ArcSegment", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[14] = new TypeDeclaration("ArrayExtension", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
            knownTypeTable[15] = new TypeDeclaration("AxisAngleRotation3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x10] = new TypeDeclaration("BaseIListConverter", "System.Windows.Media.Converters", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x11] = new TypeDeclaration("BeginStoryboard", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x12] = new TypeDeclaration("BevelBitmapEffect", "System.Windows.Media.Effects", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x13] = new TypeDeclaration("BezierSegment", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[20] = new TypeDeclaration("Binding", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x15] = new TypeDeclaration("BindingBase", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x16] = new TypeDeclaration("BindingExpression", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x17] = new TypeDeclaration("BindingExpressionBase", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x18] = new TypeDeclaration("BindingListCollectionView", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x19] = new TypeDeclaration("BitmapDecoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1a] = new TypeDeclaration("BitmapEffect", "System.Windows.Media.Effects", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1b] = new TypeDeclaration("BitmapEffectCollection", "System.Windows.Media.Effects", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1c] = new TypeDeclaration("BitmapEffectGroup", "System.Windows.Media.Effects", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1d] = new TypeDeclaration("BitmapEffectInput", "System.Windows.Media.Effects", this.GetAssembly("PresentationCore"));
            knownTypeTable[30] = new TypeDeclaration("BitmapEncoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1f] = new TypeDeclaration("BitmapFrame", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x20] = new TypeDeclaration("BitmapImage", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x21] = new TypeDeclaration("BitmapMetadata", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x22] = new TypeDeclaration("BitmapPalette", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x23] = new TypeDeclaration("BitmapSource", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x24] = new TypeDeclaration("Block", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x25] = new TypeDeclaration("BlockUIContainer", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x26] = new TypeDeclaration("BlurBitmapEffect", "System.Windows.Media.Effects", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x27] = new TypeDeclaration("BmpBitmapDecoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[40] = new TypeDeclaration("BmpBitmapEncoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x29] = new TypeDeclaration("Bold", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2b] = new TypeDeclaration("Boolean", "System", this.GetAssembly("mscorlib"));
            knownTypeTable[0x2c] = new TypeDeclaration("BooleanAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2d] = new TypeDeclaration("BooleanAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2e] = new TypeDeclaration("BooleanConverter", "System.ComponentModel", this.GetAssembly("System"));
            knownTypeTable[0x2f] = new TypeDeclaration("BooleanKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x30] = new TypeDeclaration("BooleanKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x31] = new TypeDeclaration("BooleanToVisibilityConverter", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2a] = new TypeDeclaration("BoolIListConverter", "System.Windows.Media.Converters", this.GetAssembly("PresentationCore"));
            knownTypeTable[50] = new TypeDeclaration("Border", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x33] = new TypeDeclaration("BorderGapMaskConverter", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x34] = new TypeDeclaration("Brush", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x35] = new TypeDeclaration("BrushConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x36] = new TypeDeclaration("BulletDecorator", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x37] = new TypeDeclaration("Button", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x38] = new TypeDeclaration("ButtonBase", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x39] = new TypeDeclaration("Byte", "System", this.GetAssembly("mscorlib"));
            knownTypeTable[0x3a] = new TypeDeclaration("ByteAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x3b] = new TypeDeclaration("ByteAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[60] = new TypeDeclaration("ByteAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x3d] = new TypeDeclaration("ByteConverter", "System.ComponentModel", this.GetAssembly("System"));
            knownTypeTable[0x3e] = new TypeDeclaration("ByteKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x3f] = new TypeDeclaration("ByteKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x40] = new TypeDeclaration("CachedBitmap", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x41] = new TypeDeclaration("Camera", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x42] = new TypeDeclaration("Canvas", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x43] = new TypeDeclaration("Char", "System", this.GetAssembly("mscorlib"));
            knownTypeTable[0x44] = new TypeDeclaration("CharAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x45] = new TypeDeclaration("CharAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[70] = new TypeDeclaration("CharConverter", "System.ComponentModel", this.GetAssembly("System"));
            knownTypeTable[0x47] = new TypeDeclaration("CharIListConverter", "System.Windows.Media.Converters", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x48] = new TypeDeclaration("CharKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x49] = new TypeDeclaration("CharKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x4a] = new TypeDeclaration("CheckBox", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x4b] = new TypeDeclaration("Clock", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x4c] = new TypeDeclaration("ClockController", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x4d] = new TypeDeclaration("ClockGroup", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x4e] = new TypeDeclaration("CollectionContainer", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x4f] = new TypeDeclaration("CollectionView", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
            knownTypeTable[80] = new TypeDeclaration("CollectionViewSource", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x51] = new TypeDeclaration("Color", "Microsoft.Win32", this.GetAssembly("mscorlib"));
            knownTypeTable[0x52] = new TypeDeclaration("ColorAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x53] = new TypeDeclaration("ColorAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x54] = new TypeDeclaration("ColorAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x55] = new TypeDeclaration("ColorConvertedBitmap", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x56] = new TypeDeclaration("ColorConvertedBitmapExtension", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x57] = new TypeDeclaration("ColorConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x58] = new TypeDeclaration("ColorKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x59] = new TypeDeclaration("ColorKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[90] = new TypeDeclaration("ColumnDefinition", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x5b] = new TypeDeclaration("CombinedGeometry", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x5c] = new TypeDeclaration("ComboBox", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x5d] = new TypeDeclaration("ComboBoxItem", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x5e] = new TypeDeclaration("CommandConverter", "System.Windows.Input", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x5f] = new TypeDeclaration("ComponentResourceKey", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x60] = new TypeDeclaration("ComponentResourceKeyConverter", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x61] = new TypeDeclaration("CompositionTarget", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x62] = new TypeDeclaration("Condition", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x63] = new TypeDeclaration("ContainerVisual", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[100] = new TypeDeclaration("ContentControl", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x65] = new TypeDeclaration("ContentElement", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x66] = new TypeDeclaration("ContentPresenter", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x67] = new TypeDeclaration("ContentPropertyAttribute", "System.Windows.Markup", this.GetAssembly("WindowsBase"));
            knownTypeTable[0x68] = new TypeDeclaration("ContentWrapperAttribute", "System.Windows.Markup", this.GetAssembly("WindowsBase"));
            knownTypeTable[0x69] = new TypeDeclaration("ContextMenu", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x6a] = new TypeDeclaration("ContextMenuService", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x6b] = new TypeDeclaration("Control", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x6d] = new TypeDeclaration("ControllableStoryboardAction", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x6c] = new TypeDeclaration("ControlTemplate", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[110] = new TypeDeclaration("CornerRadius", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x6f] = new TypeDeclaration("CornerRadiusConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x70] = new TypeDeclaration("CroppedBitmap", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x71] = new TypeDeclaration("CultureInfo", "System.Globalization", this.GetAssembly("mscorlib"));
            knownTypeTable[0x72] = new TypeDeclaration("CultureInfoConverter", "System.ComponentModel", this.GetAssembly("System"));
            knownTypeTable[0x73] = new TypeDeclaration("CultureInfoIetfLanguageTagConverter", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x74] = new TypeDeclaration("Cursor", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x75] = new TypeDeclaration("CursorConverter", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x76] = new TypeDeclaration("DashStyle", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x77] = new TypeDeclaration("DataChangedEventManager", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
            knownTypeTable[120] = new TypeDeclaration("DataTemplate", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x79] = new TypeDeclaration("DataTemplateKey", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x7a] = new TypeDeclaration("DataTrigger", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x7b] = new TypeDeclaration("DateTime", "System", this.GetAssembly("mscorlib"));
            knownTypeTable[0x7c] = new TypeDeclaration("DateTimeConverter", "System.ComponentModel", this.GetAssembly("System"));
            knownTypeTable[0x7d] = new TypeDeclaration("DateTimeConverter2", "System.Windows.Markup", this.GetAssembly("WindowsBase"));
            knownTypeTable[0x7e] = new TypeDeclaration("Decimal", "System", this.GetAssembly("mscorlib"));
            knownTypeTable[0x7f] = new TypeDeclaration("DecimalAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x80] = new TypeDeclaration("DecimalAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x81] = new TypeDeclaration("DecimalAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[130] = new TypeDeclaration("DecimalConverter", "System.ComponentModel", this.GetAssembly("System"));
            knownTypeTable[0x83] = new TypeDeclaration("DecimalKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x84] = new TypeDeclaration("DecimalKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x85] = new TypeDeclaration("Decorator", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x86] = new TypeDeclaration("DefinitionBase", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x87] = new TypeDeclaration("DependencyObject", "System.Windows", this.GetAssembly("WindowsBase"));
            knownTypeTable[0x88] = new TypeDeclaration("DependencyProperty", "System.Windows", this.GetAssembly("WindowsBase"));
            knownTypeTable[0x89] = new TypeDeclaration("DependencyPropertyConverter", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x8a] = new TypeDeclaration("DialogResultConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x8b] = new TypeDeclaration("DiffuseMaterial", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[140] = new TypeDeclaration("DirectionalLight", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x8d] = new TypeDeclaration("DiscreteBooleanKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x8e] = new TypeDeclaration("DiscreteByteKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x8f] = new TypeDeclaration("DiscreteCharKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x90] = new TypeDeclaration("DiscreteColorKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x91] = new TypeDeclaration("DiscreteDecimalKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x92] = new TypeDeclaration("DiscreteDoubleKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x93] = new TypeDeclaration("DiscreteInt16KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x94] = new TypeDeclaration("DiscreteInt32KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x95] = new TypeDeclaration("DiscreteInt64KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[150] = new TypeDeclaration("DiscreteMatrixKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x97] = new TypeDeclaration("DiscreteObjectKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x98] = new TypeDeclaration("DiscretePoint3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x99] = new TypeDeclaration("DiscretePointKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x9a] = new TypeDeclaration("DiscreteQuaternionKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x9b] = new TypeDeclaration("DiscreteRectKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x9c] = new TypeDeclaration("DiscreteRotation3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x9d] = new TypeDeclaration("DiscreteSingleKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x9e] = new TypeDeclaration("DiscreteSizeKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x9f] = new TypeDeclaration("DiscreteStringKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[160] = new TypeDeclaration("DiscreteThicknessKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xa1] = new TypeDeclaration("DiscreteVector3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xa2] = new TypeDeclaration("DiscreteVectorKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xa3] = new TypeDeclaration("DockPanel", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xa4] = new TypeDeclaration("DocumentPageView", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xa5] = new TypeDeclaration("DocumentReference", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xa6] = new TypeDeclaration("DocumentViewer", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xa7] = new TypeDeclaration("DocumentViewerBase", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xa8] = new TypeDeclaration("Double", "System", this.GetAssembly("mscorlib"));
            knownTypeTable[0xa9] = new TypeDeclaration("DoubleAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[170] = new TypeDeclaration("DoubleAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xab] = new TypeDeclaration("DoubleAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xac] = new TypeDeclaration("DoubleAnimationUsingPath", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xad] = new TypeDeclaration("DoubleCollection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xae] = new TypeDeclaration("DoubleCollectionConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xaf] = new TypeDeclaration("DoubleConverter", "System.ComponentModel", this.GetAssembly("System"));
            knownTypeTable[0xb0] = new TypeDeclaration("DoubleIListConverter", "System.Windows.Media.Converters", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xb1] = new TypeDeclaration("DoubleKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xb2] = new TypeDeclaration("DoubleKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xb3] = new TypeDeclaration("Drawing", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[180] = new TypeDeclaration("DrawingBrush", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xb5] = new TypeDeclaration("DrawingCollection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xb6] = new TypeDeclaration("DrawingContext", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xb7] = new TypeDeclaration("DrawingGroup", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xb8] = new TypeDeclaration("DrawingImage", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xb9] = new TypeDeclaration("DrawingVisual", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xba] = new TypeDeclaration("DropShadowBitmapEffect", "System.Windows.Media.Effects", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xbb] = new TypeDeclaration("Duration", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xbc] = new TypeDeclaration("DurationConverter", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xbd] = new TypeDeclaration("DynamicResourceExtension", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[190] = new TypeDeclaration("DynamicResourceExtensionConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xbf] = new TypeDeclaration("Ellipse", "System.Windows.Shapes", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xc0] = new TypeDeclaration("EllipseGeometry", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xc1] = new TypeDeclaration("EmbossBitmapEffect", "System.Windows.Media.Effects", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xc2] = new TypeDeclaration("EmissiveMaterial", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xc3] = new TypeDeclaration("EnumConverter", "System.ComponentModel", this.GetAssembly("System"));
            knownTypeTable[0xc4] = new TypeDeclaration("EventManager", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xc5] = new TypeDeclaration("EventSetter", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xc6] = new TypeDeclaration("EventTrigger", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xc7] = new TypeDeclaration("Expander", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[200] = new TypeDeclaration("Expression", "System.Windows", this.GetAssembly("WindowsBase"));
            knownTypeTable[0xc9] = new TypeDeclaration("ExpressionConverter", "System.Windows", this.GetAssembly("WindowsBase"));
            knownTypeTable[0xca] = new TypeDeclaration("Figure", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xcb] = new TypeDeclaration("FigureLength", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xcc] = new TypeDeclaration("FigureLengthConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xcd] = new TypeDeclaration("FixedDocument", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xce] = new TypeDeclaration("FixedDocumentSequence", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xcf] = new TypeDeclaration("FixedPage", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xd0] = new TypeDeclaration("Floater", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xd1] = new TypeDeclaration("FlowDocument", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[210] = new TypeDeclaration("FlowDocumentPageViewer", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xd3] = new TypeDeclaration("FlowDocumentReader", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xd4] = new TypeDeclaration("FlowDocumentScrollViewer", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xd5] = new TypeDeclaration("FocusManager", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xd6] = new TypeDeclaration("FontFamily", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xd7] = new TypeDeclaration("FontFamilyConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xd8] = new TypeDeclaration("FontSizeConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xd9] = new TypeDeclaration("FontStretch", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xda] = new TypeDeclaration("FontStretchConverter", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xdb] = new TypeDeclaration("FontStyle", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[220] = new TypeDeclaration("FontStyleConverter", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xdd] = new TypeDeclaration("FontWeight", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xde] = new TypeDeclaration("FontWeightConverter", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xdf] = new TypeDeclaration("FormatConvertedBitmap", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xe0] = new TypeDeclaration("Frame", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xe1] = new TypeDeclaration("FrameworkContentElement", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xe2] = new TypeDeclaration("FrameworkElement", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xe3] = new TypeDeclaration("FrameworkElementFactory", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xe4] = new TypeDeclaration("FrameworkPropertyMetadata", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xe5] = new TypeDeclaration("FrameworkPropertyMetadataOptions", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[230] = new TypeDeclaration("FrameworkRichTextComposition", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xe7] = new TypeDeclaration("FrameworkTemplate", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xe8] = new TypeDeclaration("FrameworkTextComposition", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xe9] = new TypeDeclaration("Freezable", "System.Windows", this.GetAssembly("WindowsBase"));
            knownTypeTable[0xea] = new TypeDeclaration("GeneralTransform", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xeb] = new TypeDeclaration("GeneralTransformCollection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xec] = new TypeDeclaration("GeneralTransformGroup", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xed] = new TypeDeclaration("Geometry", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xee] = new TypeDeclaration("Geometry3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xef] = new TypeDeclaration("GeometryCollection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[240] = new TypeDeclaration("GeometryConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xf1] = new TypeDeclaration("GeometryDrawing", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xf2] = new TypeDeclaration("GeometryGroup", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xf3] = new TypeDeclaration("GeometryModel3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xf4] = new TypeDeclaration("GestureRecognizer", "System.Windows.Ink", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xf5] = new TypeDeclaration("GifBitmapDecoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xf6] = new TypeDeclaration("GifBitmapEncoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xf7] = new TypeDeclaration("GlyphRun", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xf8] = new TypeDeclaration("GlyphRunDrawing", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[250] = new TypeDeclaration("Glyphs", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xf9] = new TypeDeclaration("GlyphTypeface", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xfb] = new TypeDeclaration("GradientBrush", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xfc] = new TypeDeclaration("GradientStop", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xfd] = new TypeDeclaration("GradientStopCollection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0xfe] = new TypeDeclaration("Grid", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0xff] = new TypeDeclaration("GridLength", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x100] = new TypeDeclaration("GridLengthConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x101] = new TypeDeclaration("GridSplitter", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x102] = new TypeDeclaration("GridView", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x103] = new TypeDeclaration("GridViewColumn", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[260] = new TypeDeclaration("GridViewColumnHeader", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x105] = new TypeDeclaration("GridViewHeaderRowPresenter", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x106] = new TypeDeclaration("GridViewRowPresenter", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x107] = new TypeDeclaration("GridViewRowPresenterBase", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x108] = new TypeDeclaration("GroupBox", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x109] = new TypeDeclaration("GroupItem", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x10a] = new TypeDeclaration("Guid", "System", this.GetAssembly("mscorlib"));
            knownTypeTable[0x10b] = new TypeDeclaration("GuidConverter", "System.ComponentModel", this.GetAssembly("System"));
            knownTypeTable[0x10c] = new TypeDeclaration("GuidelineSet", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x10d] = new TypeDeclaration("HeaderedContentControl", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[270] = new TypeDeclaration("HeaderedItemsControl", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x10f] = new TypeDeclaration("HierarchicalDataTemplate", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x110] = new TypeDeclaration("HostVisual", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x111] = new TypeDeclaration("Hyperlink", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x112] = new TypeDeclaration("IAddChild", "System.Windows.Markup", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x113] = new TypeDeclaration("IAddChildInternal", "System.Windows.Markup", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x114] = new TypeDeclaration("ICommand", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x115] = new TypeDeclaration("IComponentConnector", "System.Windows.Markup", this.GetAssembly("WindowsBase"));
            knownTypeTable[280] = new TypeDeclaration("IconBitmapDecoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x119] = new TypeDeclaration("Image", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x11a] = new TypeDeclaration("ImageBrush", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x11b] = new TypeDeclaration("ImageDrawing", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x11c] = new TypeDeclaration("ImageMetadata", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x11d] = new TypeDeclaration("ImageSource", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x11e] = new TypeDeclaration("ImageSourceConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x116] = new TypeDeclaration("INameScope", "System.Windows.Markup", this.GetAssembly("WindowsBase"));
            knownTypeTable[0x120] = new TypeDeclaration("InkCanvas", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x121] = new TypeDeclaration("InkPresenter", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[290] = new TypeDeclaration("Inline", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x123] = new TypeDeclaration("InlineCollection", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x124] = new TypeDeclaration("InlineUIContainer", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x11f] = new TypeDeclaration("InPlaceBitmapMetadataWriter", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x125] = new TypeDeclaration("InputBinding", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x126] = new TypeDeclaration("InputDevice", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x127] = new TypeDeclaration("InputLanguageManager", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x128] = new TypeDeclaration("InputManager", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x129] = new TypeDeclaration("InputMethod", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x12a] = new TypeDeclaration("InputScope", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x12b] = new TypeDeclaration("InputScopeConverter", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[300] = new TypeDeclaration("InputScopeName", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x12d] = new TypeDeclaration("InputScopeNameConverter", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x12e] = new TypeDeclaration("Int16", "System", this.GetAssembly("mscorlib"));
            knownTypeTable[0x12f] = new TypeDeclaration("Int16Animation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x130] = new TypeDeclaration("Int16AnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x131] = new TypeDeclaration("Int16AnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x132] = new TypeDeclaration("Int16Converter", "System.ComponentModel", this.GetAssembly("System"));
            knownTypeTable[0x133] = new TypeDeclaration("Int16KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x134] = new TypeDeclaration("Int16KeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x135] = new TypeDeclaration("Int32", "System", this.GetAssembly("mscorlib"));
            knownTypeTable[310] = new TypeDeclaration("Int32Animation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x137] = new TypeDeclaration("Int32AnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x138] = new TypeDeclaration("Int32AnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x139] = new TypeDeclaration("Int32Collection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x13a] = new TypeDeclaration("Int32CollectionConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x13b] = new TypeDeclaration("Int32Converter", "System.ComponentModel", this.GetAssembly("System"));
            knownTypeTable[0x13c] = new TypeDeclaration("Int32KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x13d] = new TypeDeclaration("Int32KeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x13e] = new TypeDeclaration("Int32Rect", "System.Windows", this.GetAssembly("WindowsBase"));
            knownTypeTable[0x13f] = new TypeDeclaration("Int32RectConverter", "System.Windows", this.GetAssembly("WindowsBase"));
            knownTypeTable[320] = new TypeDeclaration("Int64", "System", this.GetAssembly("mscorlib"));
            knownTypeTable[0x141] = new TypeDeclaration("Int64Animation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x142] = new TypeDeclaration("Int64AnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x143] = new TypeDeclaration("Int64AnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x144] = new TypeDeclaration("Int64Converter", "System.ComponentModel", this.GetAssembly("System"));
            knownTypeTable[0x145] = new TypeDeclaration("Int64KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x146] = new TypeDeclaration("Int64KeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x117] = new TypeDeclaration("IStyleConnector", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x147] = new TypeDeclaration("Italic", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x148] = new TypeDeclaration("ItemCollection", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x149] = new TypeDeclaration("ItemsControl", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[330] = new TypeDeclaration("ItemsPanelTemplate", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x14b] = new TypeDeclaration("ItemsPresenter", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x14c] = new TypeDeclaration("JournalEntry", "System.Windows.Navigation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x14d] = new TypeDeclaration("JournalEntryListConverter", "System.Windows.Navigation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x14e] = new TypeDeclaration("JournalEntryUnifiedViewConverter", "System.Windows.Navigation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x14f] = new TypeDeclaration("JpegBitmapDecoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x150] = new TypeDeclaration("JpegBitmapEncoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x151] = new TypeDeclaration("KeyBinding", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x159] = new TypeDeclaration("KeyboardDevice", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x152] = new TypeDeclaration("KeyConverter", "System.Windows.Input", this.GetAssembly("WindowsBase"));
            knownTypeTable[0x153] = new TypeDeclaration("KeyGesture", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[340] = new TypeDeclaration("KeyGestureConverter", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x155] = new TypeDeclaration("KeySpline", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x156] = new TypeDeclaration("KeySplineConverter", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x157] = new TypeDeclaration("KeyTime", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x158] = new TypeDeclaration("KeyTimeConverter", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x15a] = new TypeDeclaration("Label", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x15b] = new TypeDeclaration("LateBoundBitmapDecoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x15c] = new TypeDeclaration("LengthConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x15d] = new TypeDeclaration("Light", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[350] = new TypeDeclaration("Line", "System.Windows.Shapes", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x162] = new TypeDeclaration("LinearByteKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x163] = new TypeDeclaration("LinearColorKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x164] = new TypeDeclaration("LinearDecimalKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x165] = new TypeDeclaration("LinearDoubleKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x166] = new TypeDeclaration("LinearGradientBrush", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x167] = new TypeDeclaration("LinearInt16KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[360] = new TypeDeclaration("LinearInt32KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x169] = new TypeDeclaration("LinearInt64KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x16a] = new TypeDeclaration("LinearPoint3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x16b] = new TypeDeclaration("LinearPointKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x16c] = new TypeDeclaration("LinearQuaternionKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x16d] = new TypeDeclaration("LinearRectKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x16e] = new TypeDeclaration("LinearRotation3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x16f] = new TypeDeclaration("LinearSingleKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x170] = new TypeDeclaration("LinearSizeKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x171] = new TypeDeclaration("LinearThicknessKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[370] = new TypeDeclaration("LinearVector3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x173] = new TypeDeclaration("LinearVectorKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x15f] = new TypeDeclaration("LineBreak", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x160] = new TypeDeclaration("LineGeometry", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x161] = new TypeDeclaration("LineSegment", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x174] = new TypeDeclaration("List", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x175] = new TypeDeclaration("ListBox", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x176] = new TypeDeclaration("ListBoxItem", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x177] = new TypeDeclaration("ListCollectionView", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x178] = new TypeDeclaration("ListItem", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x179] = new TypeDeclaration("ListView", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x17a] = new TypeDeclaration("ListViewItem", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x17b] = new TypeDeclaration("Localization", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[380] = new TypeDeclaration("LostFocusEventManager", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x17d] = new TypeDeclaration("MarkupExtension", "System.Windows.Markup", this.GetAssembly("WindowsBase"));
            knownTypeTable[0x17e] = new TypeDeclaration("Material", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x17f] = new TypeDeclaration("MaterialCollection", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x180] = new TypeDeclaration("MaterialGroup", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x181] = new TypeDeclaration("Matrix", "System.Windows.Media", this.GetAssembly("WindowsBase"));
            knownTypeTable[0x182] = new TypeDeclaration("Matrix3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x183] = new TypeDeclaration("Matrix3DConverter", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x184] = new TypeDeclaration("MatrixAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x185] = new TypeDeclaration("MatrixAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[390] = new TypeDeclaration("MatrixAnimationUsingPath", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x187] = new TypeDeclaration("MatrixCamera", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x188] = new TypeDeclaration("MatrixConverter", "System.Windows.Media", this.GetAssembly("WindowsBase"));
            knownTypeTable[0x189] = new TypeDeclaration("MatrixKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x18a] = new TypeDeclaration("MatrixKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x18b] = new TypeDeclaration("MatrixTransform", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x18c] = new TypeDeclaration("MatrixTransform3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x18d] = new TypeDeclaration("MediaClock", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x18e] = new TypeDeclaration("MediaElement", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x18f] = new TypeDeclaration("MediaPlayer", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[400] = new TypeDeclaration("MediaTimeline", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x191] = new TypeDeclaration("Menu", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x192] = new TypeDeclaration("MenuBase", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x193] = new TypeDeclaration("MenuItem", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x194] = new TypeDeclaration("MenuScrollingVisibilityConverter", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x195] = new TypeDeclaration("MeshGeometry3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x196] = new TypeDeclaration("Model3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x197] = new TypeDeclaration("Model3DCollection", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x198] = new TypeDeclaration("Model3DGroup", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x199] = new TypeDeclaration("ModelVisual3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[410] = new TypeDeclaration("ModifierKeysConverter", "System.Windows.Input", this.GetAssembly("WindowsBase"));
            knownTypeTable[0x19b] = new TypeDeclaration("MouseActionConverter", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x19c] = new TypeDeclaration("MouseBinding", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x19d] = new TypeDeclaration("MouseDevice", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x19e] = new TypeDeclaration("MouseGesture", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x19f] = new TypeDeclaration("MouseGestureConverter", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1a0] = new TypeDeclaration("MultiBinding", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1a1] = new TypeDeclaration("MultiBindingExpression", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1a2] = new TypeDeclaration("MultiDataTrigger", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1a3] = new TypeDeclaration("MultiTrigger", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[420] = new TypeDeclaration("NameScope", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1a5] = new TypeDeclaration("NavigationWindow", "System.Windows.Navigation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1a7] = new TypeDeclaration("NullableBoolConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1a8] = new TypeDeclaration("NullableConverter", "System.ComponentModel", this.GetAssembly("System"));
            knownTypeTable[0x1a6] = new TypeDeclaration("NullExtension", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1a9] = new TypeDeclaration("NumberSubstitution", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1aa] = new TypeDeclaration("Object", "System", this.GetAssembly("mscorlib"));
            knownTypeTable[0x1ab] = new TypeDeclaration("ObjectAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1ac] = new TypeDeclaration("ObjectAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1ad] = new TypeDeclaration("ObjectDataProvider", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
            knownTypeTable[430] = new TypeDeclaration("ObjectKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1af] = new TypeDeclaration("ObjectKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1b0] = new TypeDeclaration("OrthographicCamera", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1b1] = new TypeDeclaration("OuterGlowBitmapEffect", "System.Windows.Media.Effects", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1b2] = new TypeDeclaration("Page", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1b3] = new TypeDeclaration("PageContent", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1b4] = new TypeDeclaration("PageFunctionBase", "System.Windows.Navigation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1b5] = new TypeDeclaration("Panel", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1b6] = new TypeDeclaration("Paragraph", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1b7] = new TypeDeclaration("ParallelTimeline", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[440] = new TypeDeclaration("ParserContext", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1b9] = new TypeDeclaration("PasswordBox", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1ba] = new TypeDeclaration("Path", "System.Windows.Shapes", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1bb] = new TypeDeclaration("PathFigure", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1bc] = new TypeDeclaration("PathFigureCollection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1bd] = new TypeDeclaration("PathFigureCollectionConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1be] = new TypeDeclaration("PathGeometry", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1bf] = new TypeDeclaration("PathSegment", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1c0] = new TypeDeclaration("PathSegmentCollection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1c1] = new TypeDeclaration("PauseStoryboard", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[450] = new TypeDeclaration("Pen", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1c3] = new TypeDeclaration("PerspectiveCamera", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1c4] = new TypeDeclaration("PixelFormat", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1c5] = new TypeDeclaration("PixelFormatConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1c6] = new TypeDeclaration("PngBitmapDecoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1c7] = new TypeDeclaration("PngBitmapEncoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1c8] = new TypeDeclaration("Point", "System.Windows", this.GetAssembly("WindowsBase"));
            knownTypeTable[0x1c9] = new TypeDeclaration("Point3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1ca] = new TypeDeclaration("Point3DAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1cb] = new TypeDeclaration("Point3DAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[460] = new TypeDeclaration("Point3DAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1cd] = new TypeDeclaration("Point3DCollection", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1ce] = new TypeDeclaration("Point3DCollectionConverter", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1cf] = new TypeDeclaration("Point3DConverter", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1d0] = new TypeDeclaration("Point3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1d1] = new TypeDeclaration("Point3DKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1d2] = new TypeDeclaration("Point4D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1d3] = new TypeDeclaration("Point4DConverter", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1d4] = new TypeDeclaration("PointAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1d5] = new TypeDeclaration("PointAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[470] = new TypeDeclaration("PointAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1d7] = new TypeDeclaration("PointAnimationUsingPath", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1d8] = new TypeDeclaration("PointCollection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1d9] = new TypeDeclaration("PointCollectionConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1da] = new TypeDeclaration("PointConverter", "System.Windows", this.GetAssembly("WindowsBase"));
            knownTypeTable[0x1db] = new TypeDeclaration("PointIListConverter", "System.Windows.Media.Converters", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1dc] = new TypeDeclaration("PointKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1dd] = new TypeDeclaration("PointKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1de] = new TypeDeclaration("PointLight", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1df] = new TypeDeclaration("PointLightBase", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[480] = new TypeDeclaration("PolyBezierSegment", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1e3] = new TypeDeclaration("Polygon", "System.Windows.Shapes", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1e4] = new TypeDeclaration("Polyline", "System.Windows.Shapes", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1e1] = new TypeDeclaration("PolyLineSegment", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1e2] = new TypeDeclaration("PolyQuadraticBezierSegment", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1e5] = new TypeDeclaration("Popup", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1e6] = new TypeDeclaration("PresentationSource", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1e7] = new TypeDeclaration("PriorityBinding", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1e8] = new TypeDeclaration("PriorityBindingExpression", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1e9] = new TypeDeclaration("ProgressBar", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[490] = new TypeDeclaration("ProjectionCamera", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1eb] = new TypeDeclaration("PropertyPath", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1ec] = new TypeDeclaration("PropertyPathConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1ed] = new TypeDeclaration("QuadraticBezierSegment", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1ee] = new TypeDeclaration("Quaternion", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1ef] = new TypeDeclaration("QuaternionAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1f0] = new TypeDeclaration("QuaternionAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1f1] = new TypeDeclaration("QuaternionAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1f2] = new TypeDeclaration("QuaternionConverter", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1f3] = new TypeDeclaration("QuaternionKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[500] = new TypeDeclaration("QuaternionKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1f5] = new TypeDeclaration("QuaternionRotation3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1f6] = new TypeDeclaration("RadialGradientBrush", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1f7] = new TypeDeclaration("RadioButton", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1f8] = new TypeDeclaration("RangeBase", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x1f9] = new TypeDeclaration("Rect", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1fa] = new TypeDeclaration("Rect3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1fb] = new TypeDeclaration("Rect3DConverter", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x202] = new TypeDeclaration("Rectangle", "System.Windows.Shapes", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x203] = new TypeDeclaration("RectangleGeometry", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1fc] = new TypeDeclaration("RectAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1fd] = new TypeDeclaration("RectAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[510] = new TypeDeclaration("RectAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x1ff] = new TypeDeclaration("RectConverter", "System.Windows", this.GetAssembly("WindowsBase"));
            knownTypeTable[0x200] = new TypeDeclaration("RectKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x201] = new TypeDeclaration("RectKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x204] = new TypeDeclaration("RelativeSource", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x205] = new TypeDeclaration("RemoveStoryboard", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x206] = new TypeDeclaration("RenderOptions", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x207] = new TypeDeclaration("RenderTargetBitmap", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[520] = new TypeDeclaration("RepeatBehavior", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x209] = new TypeDeclaration("RepeatBehaviorConverter", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x20a] = new TypeDeclaration("RepeatButton", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x20b] = new TypeDeclaration("ResizeGrip", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x20c] = new TypeDeclaration("ResourceDictionary", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x20d] = new TypeDeclaration("ResourceKey", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x20e] = new TypeDeclaration("ResumeStoryboard", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x20f] = new TypeDeclaration("RichTextBox", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x210] = new TypeDeclaration("RotateTransform", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x211] = new TypeDeclaration("RotateTransform3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[530] = new TypeDeclaration("Rotation3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x213] = new TypeDeclaration("Rotation3DAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x214] = new TypeDeclaration("Rotation3DAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x215] = new TypeDeclaration("Rotation3DAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x216] = new TypeDeclaration("Rotation3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x217] = new TypeDeclaration("Rotation3DKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x218] = new TypeDeclaration("RoutedCommand", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x219] = new TypeDeclaration("RoutedEvent", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x21a] = new TypeDeclaration("RoutedEventConverter", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x21b] = new TypeDeclaration("RoutedUICommand", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[540] = new TypeDeclaration("RoutingStrategy", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x21d] = new TypeDeclaration("RowDefinition", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x21e] = new TypeDeclaration("Run", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x21f] = new TypeDeclaration("RuntimeNamePropertyAttribute", "System.Windows.Markup", this.GetAssembly("WindowsBase"));
            knownTypeTable[0x220] = new TypeDeclaration("SByte", "System", this.GetAssembly("mscorlib"));
            knownTypeTable[0x221] = new TypeDeclaration("SByteConverter", "System.ComponentModel", this.GetAssembly("System"));
            knownTypeTable[0x222] = new TypeDeclaration("ScaleTransform", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x223] = new TypeDeclaration("ScaleTransform3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x224] = new TypeDeclaration("ScrollBar", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x225] = new TypeDeclaration("ScrollContentPresenter", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[550] = new TypeDeclaration("ScrollViewer", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x227] = new TypeDeclaration("Section", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x228] = new TypeDeclaration("SeekStoryboard", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x229] = new TypeDeclaration("Selector", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x22a] = new TypeDeclaration("Separator", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x22b] = new TypeDeclaration("SetStoryboardSpeedRatio", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x22c] = new TypeDeclaration("Setter", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x22d] = new TypeDeclaration("SetterBase", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x22e] = new TypeDeclaration("Shape", "System.Windows.Shapes", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x22f] = new TypeDeclaration("Single", "System", this.GetAssembly("mscorlib"));
            knownTypeTable[560] = new TypeDeclaration("SingleAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x231] = new TypeDeclaration("SingleAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x232] = new TypeDeclaration("SingleAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x233] = new TypeDeclaration("SingleConverter", "System.ComponentModel", this.GetAssembly("System"));
            knownTypeTable[0x234] = new TypeDeclaration("SingleKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x235] = new TypeDeclaration("SingleKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x236] = new TypeDeclaration("Size", "System.Windows", this.GetAssembly("WindowsBase"));
            knownTypeTable[0x237] = new TypeDeclaration("Size3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x238] = new TypeDeclaration("Size3DConverter", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x239] = new TypeDeclaration("SizeAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[570] = new TypeDeclaration("SizeAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x23b] = new TypeDeclaration("SizeAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x23c] = new TypeDeclaration("SizeConverter", "System.Windows", this.GetAssembly("WindowsBase"));
            knownTypeTable[0x23d] = new TypeDeclaration("SizeKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x23e] = new TypeDeclaration("SizeKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x23f] = new TypeDeclaration("SkewTransform", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x240] = new TypeDeclaration("SkipStoryboardToFill", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x241] = new TypeDeclaration("Slider", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x242] = new TypeDeclaration("SolidColorBrush", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x243] = new TypeDeclaration("SoundPlayerAction", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[580] = new TypeDeclaration("Span", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x245] = new TypeDeclaration("SpecularMaterial", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x246] = new TypeDeclaration("SpellCheck", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x247] = new TypeDeclaration("SplineByteKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x248] = new TypeDeclaration("SplineColorKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x249] = new TypeDeclaration("SplineDecimalKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x24a] = new TypeDeclaration("SplineDoubleKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x24b] = new TypeDeclaration("SplineInt16KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x24c] = new TypeDeclaration("SplineInt32KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x24d] = new TypeDeclaration("SplineInt64KeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[590] = new TypeDeclaration("SplinePoint3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x24f] = new TypeDeclaration("SplinePointKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x250] = new TypeDeclaration("SplineQuaternionKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x251] = new TypeDeclaration("SplineRectKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x252] = new TypeDeclaration("SplineRotation3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x253] = new TypeDeclaration("SplineSingleKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x254] = new TypeDeclaration("SplineSizeKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x255] = new TypeDeclaration("SplineThicknessKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x256] = new TypeDeclaration("SplineVector3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x257] = new TypeDeclaration("SplineVectorKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[600] = new TypeDeclaration("SpotLight", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x259] = new TypeDeclaration("StackPanel", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x25a] = new TypeDeclaration("StaticExtension", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x25b] = new TypeDeclaration("StaticResourceExtension", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x25c] = new TypeDeclaration("StatusBar", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x25d] = new TypeDeclaration("StatusBarItem", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x25e] = new TypeDeclaration("StickyNoteControl", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x25f] = new TypeDeclaration("StopStoryboard", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x260] = new TypeDeclaration("Storyboard", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x261] = new TypeDeclaration("StreamGeometry", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[610] = new TypeDeclaration("StreamGeometryContext", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x263] = new TypeDeclaration("StreamResourceInfo", "System.Windows.Resources", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x264] = new TypeDeclaration("String", "System", this.GetAssembly("mscorlib"));
            knownTypeTable[0x265] = new TypeDeclaration("StringAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x266] = new TypeDeclaration("StringAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x267] = new TypeDeclaration("StringConverter", "System.ComponentModel", this.GetAssembly("System"));
            knownTypeTable[0x268] = new TypeDeclaration("StringKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x269] = new TypeDeclaration("StringKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x26a] = new TypeDeclaration("StrokeCollection", "System.Windows.Ink", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x26b] = new TypeDeclaration("StrokeCollectionConverter", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[620] = new TypeDeclaration("Style", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x26d] = new TypeDeclaration("Stylus", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x26e] = new TypeDeclaration("StylusDevice", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x26f] = new TypeDeclaration("TabControl", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x270] = new TypeDeclaration("TabItem", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x272] = new TypeDeclaration("Table", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x273] = new TypeDeclaration("TableCell", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x274] = new TypeDeclaration("TableColumn", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x275] = new TypeDeclaration("TableRow", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[630] = new TypeDeclaration("TableRowGroup", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x277] = new TypeDeclaration("TabletDevice", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x271] = new TypeDeclaration("TabPanel", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x278] = new TypeDeclaration("TemplateBindingExpression", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x279] = new TypeDeclaration("TemplateBindingExpressionConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x27a] = new TypeDeclaration("TemplateBindingExtension", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x27b] = new TypeDeclaration("TemplateBindingExtensionConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x27c] = new TypeDeclaration("TemplateKey", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x27d] = new TypeDeclaration("TemplateKeyConverter", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x27e] = new TypeDeclaration("TextBlock", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x27f] = new TypeDeclaration("TextBox", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[640] = new TypeDeclaration("TextBoxBase", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x281] = new TypeDeclaration("TextComposition", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x282] = new TypeDeclaration("TextCompositionManager", "System.Windows.Input", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x283] = new TypeDeclaration("TextDecoration", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x284] = new TypeDeclaration("TextDecorationCollection", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x285] = new TypeDeclaration("TextDecorationCollectionConverter", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x286] = new TypeDeclaration("TextEffect", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x287] = new TypeDeclaration("TextEffectCollection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x288] = new TypeDeclaration("TextElement", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x289] = new TypeDeclaration("TextSearch", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[650] = new TypeDeclaration("ThemeDictionaryExtension", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x28b] = new TypeDeclaration("Thickness", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x28c] = new TypeDeclaration("ThicknessAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x28d] = new TypeDeclaration("ThicknessAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x28e] = new TypeDeclaration("ThicknessAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x28f] = new TypeDeclaration("ThicknessConverter", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x290] = new TypeDeclaration("ThicknessKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x291] = new TypeDeclaration("ThicknessKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x292] = new TypeDeclaration("Thumb", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x293] = new TypeDeclaration("TickBar", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[660] = new TypeDeclaration("TiffBitmapDecoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x295] = new TypeDeclaration("TiffBitmapEncoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x296] = new TypeDeclaration("TileBrush", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x299] = new TypeDeclaration("Timeline", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x29a] = new TypeDeclaration("TimelineCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x29b] = new TypeDeclaration("TimelineGroup", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x297] = new TypeDeclaration("TimeSpan", "System", this.GetAssembly("mscorlib"));
            knownTypeTable[0x298] = new TypeDeclaration("TimeSpanConverter", "System.ComponentModel", this.GetAssembly("System"));
            knownTypeTable[0x29c] = new TypeDeclaration("ToggleButton", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x29d] = new TypeDeclaration("ToolBar", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[670] = new TypeDeclaration("ToolBarOverflowPanel", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x29f] = new TypeDeclaration("ToolBarPanel", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2a0] = new TypeDeclaration("ToolBarTray", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2a1] = new TypeDeclaration("ToolTip", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2a2] = new TypeDeclaration("ToolTipService", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2a3] = new TypeDeclaration("Track", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2a4] = new TypeDeclaration("Transform", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2a5] = new TypeDeclaration("Transform3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2a6] = new TypeDeclaration("Transform3DCollection", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2a7] = new TypeDeclaration("Transform3DGroup", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[680] = new TypeDeclaration("TransformCollection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2a9] = new TypeDeclaration("TransformConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2ab] = new TypeDeclaration("TransformedBitmap", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2aa] = new TypeDeclaration("TransformGroup", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2ac] = new TypeDeclaration("TranslateTransform", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2ad] = new TypeDeclaration("TranslateTransform3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2ae] = new TypeDeclaration("TreeView", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2af] = new TypeDeclaration("TreeViewItem", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2b0] = new TypeDeclaration("Trigger", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2b1] = new TypeDeclaration("TriggerAction", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[690] = new TypeDeclaration("TriggerBase", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2b3] = new TypeDeclaration("TypeExtension", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2b4] = new TypeDeclaration("TypeTypeConverter", "System.Net.Configuration", this.GetAssembly("System"));
            knownTypeTable[0x2b5] = new TypeDeclaration("Typography", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2b6] = new TypeDeclaration("UIElement", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2b7] = new TypeDeclaration("UInt16", "System", this.GetAssembly("mscorlib"));
            knownTypeTable[0x2b8] = new TypeDeclaration("UInt16Converter", "System.ComponentModel", this.GetAssembly("System"));
            knownTypeTable[0x2b9] = new TypeDeclaration("UInt32", "System", this.GetAssembly("mscorlib"));
            knownTypeTable[0x2ba] = new TypeDeclaration("UInt32Converter", "System.ComponentModel", this.GetAssembly("System"));
            knownTypeTable[0x2bb] = new TypeDeclaration("UInt64", "System", this.GetAssembly("mscorlib"));
            knownTypeTable[700] = new TypeDeclaration("UInt64Converter", "System.ComponentModel", this.GetAssembly("System"));
            knownTypeTable[0x2be] = new TypeDeclaration("Underline", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2bf] = new TypeDeclaration("UniformGrid", "System.Windows.Controls.Primitives", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2c0] = new TypeDeclaration("Uri", "System", this.GetAssembly("System"));
            knownTypeTable[0x2c1] = new TypeDeclaration("UriTypeConverter", "System", this.GetAssembly("System"));
            knownTypeTable[0x2c2] = new TypeDeclaration("UserControl", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2bd] = new TypeDeclaration("UShortIListConverter", "System.Windows.Media.Converters", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2c3] = new TypeDeclaration("Validation", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2c4] = new TypeDeclaration("Vector", "System.Windows", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2c5] = new TypeDeclaration("Vector3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[710] = new TypeDeclaration("Vector3DAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2c7] = new TypeDeclaration("Vector3DAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2c8] = new TypeDeclaration("Vector3DAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2c9] = new TypeDeclaration("Vector3DCollection", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2ca] = new TypeDeclaration("Vector3DCollectionConverter", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2cb] = new TypeDeclaration("Vector3DConverter", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2cc] = new TypeDeclaration("Vector3DKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2cd] = new TypeDeclaration("Vector3DKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2ce] = new TypeDeclaration("VectorAnimation", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2cf] = new TypeDeclaration("VectorAnimationBase", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[720] = new TypeDeclaration("VectorAnimationUsingKeyFrames", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2d1] = new TypeDeclaration("VectorCollection", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2d2] = new TypeDeclaration("VectorCollectionConverter", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2d3] = new TypeDeclaration("VectorConverter", "System.Windows", this.GetAssembly("WindowsBase"));
            knownTypeTable[0x2d4] = new TypeDeclaration("VectorKeyFrame", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2d5] = new TypeDeclaration("VectorKeyFrameCollection", "System.Windows.Media.Animation", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2d6] = new TypeDeclaration("VideoDrawing", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2d7] = new TypeDeclaration("ViewBase", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2d8] = new TypeDeclaration("Viewbox", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2d9] = new TypeDeclaration("Viewport3D", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[730] = new TypeDeclaration("Viewport3DVisual", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2db] = new TypeDeclaration("VirtualizingPanel", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2dc] = new TypeDeclaration("VirtualizingStackPanel", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2dd] = new TypeDeclaration("Visual", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2de] = new TypeDeclaration("Visual3D", "System.Windows.Media.Media3D", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2df] = new TypeDeclaration("VisualBrush", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2e0] = new TypeDeclaration("VisualTarget", "System.Windows.Media", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2e1] = new TypeDeclaration("WeakEventManager", "System.Windows", this.GetAssembly("WindowsBase"));
            knownTypeTable[0x2e2] = new TypeDeclaration("WhitespaceSignificantCollectionAttribute", "System.Windows.Markup", this.GetAssembly("WindowsBase"));
            knownTypeTable[0x2e3] = new TypeDeclaration("Window", "System.Windows", this.GetAssembly("PresentationFramework"));
            knownTypeTable[740] = new TypeDeclaration("WmpBitmapDecoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2e5] = new TypeDeclaration("WmpBitmapEncoder", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2e6] = new TypeDeclaration("WrapPanel", "System.Windows.Controls", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2e7] = new TypeDeclaration("WriteableBitmap", "System.Windows.Media.Imaging", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2e8] = new TypeDeclaration("XamlBrushSerializer", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2e9] = new TypeDeclaration("XamlInt32CollectionSerializer", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2ea] = new TypeDeclaration("XamlPathDataSerializer", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2eb] = new TypeDeclaration("XamlPoint3DCollectionSerializer", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2ec] = new TypeDeclaration("XamlPointCollectionSerializer", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2ed] = new TypeDeclaration("XamlReader", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
            knownTypeTable[750] = new TypeDeclaration("XamlStyleSerializer", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2ef] = new TypeDeclaration("XamlTemplateSerializer", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2f0] = new TypeDeclaration("XamlVector3DCollectionSerializer", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2f1] = new TypeDeclaration("XamlWriter", "System.Windows.Markup", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2f2] = new TypeDeclaration("XmlDataProvider", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2f3] = new TypeDeclaration("XmlLangPropertyAttribute", "System.Windows.Markup", this.GetAssembly("WindowsBase"));
            knownTypeTable[0x2f4] = new TypeDeclaration("XmlLanguage", "System.Windows.Markup", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2f5] = new TypeDeclaration("XmlLanguageConverter", "System.Windows.Markup", this.GetAssembly("PresentationCore"));
            knownTypeTable[0x2f6] = new TypeDeclaration("XmlNamespaceMapping", "System.Windows.Data", this.GetAssembly("PresentationFramework"));
            knownTypeTable[0x2f7] = new TypeDeclaration("ZoomPercentageConverter", "System.Windows.Documents", this.GetAssembly("PresentationFramework"));
            knownPropertyTable = new PropertyDeclaration[0x10d];
            knownPropertyTable[1] = new PropertyDeclaration("Text", knownTypeTable[1]);
            knownPropertyTable[2] = new PropertyDeclaration("Storyboard", knownTypeTable[0x11]);
            knownPropertyTable[3] = new PropertyDeclaration("Children", knownTypeTable[0x1c]);
            knownPropertyTable[4] = new PropertyDeclaration("Background", knownTypeTable[50]);
            knownPropertyTable[5] = new PropertyDeclaration("BorderBrush", knownTypeTable[50]);
            knownPropertyTable[6] = new PropertyDeclaration("BorderThickness", knownTypeTable[50]);
            knownPropertyTable[7] = new PropertyDeclaration("Command", knownTypeTable[0x38]);
            knownPropertyTable[8] = new PropertyDeclaration("CommandParameter", knownTypeTable[0x38]);
            knownPropertyTable[9] = new PropertyDeclaration("CommandTarget", knownTypeTable[0x38]);
            knownPropertyTable[10] = new PropertyDeclaration("IsPressed", knownTypeTable[0x38]);
            knownPropertyTable[11] = new PropertyDeclaration("MaxWidth", knownTypeTable[90]);
            knownPropertyTable[12] = new PropertyDeclaration("MinWidth", knownTypeTable[90]);
            knownPropertyTable[13] = new PropertyDeclaration("Width", knownTypeTable[90]);
            knownPropertyTable[14] = new PropertyDeclaration("Content", knownTypeTable[100]);
            knownPropertyTable[15] = new PropertyDeclaration("ContentTemplate", knownTypeTable[100]);
            knownPropertyTable[0x10] = new PropertyDeclaration("ContentTemplateSelector", knownTypeTable[100]);
            knownPropertyTable[0x11] = new PropertyDeclaration("HasContent", knownTypeTable[100]);
            knownPropertyTable[0x12] = new PropertyDeclaration("Focusable", knownTypeTable[0x65]);
            knownPropertyTable[0x13] = new PropertyDeclaration("Content", knownTypeTable[0x66]);
            knownPropertyTable[20] = new PropertyDeclaration("ContentSource", knownTypeTable[0x66]);
            knownPropertyTable[0x15] = new PropertyDeclaration("ContentTemplate", knownTypeTable[0x66]);
            knownPropertyTable[0x16] = new PropertyDeclaration("ContentTemplateSelector", knownTypeTable[0x66]);
            knownPropertyTable[0x17] = new PropertyDeclaration("RecognizesAccessKey", knownTypeTable[0x66]);
            knownPropertyTable[0x18] = new PropertyDeclaration("Background", knownTypeTable[0x6b]);
            knownPropertyTable[0x19] = new PropertyDeclaration("BorderBrush", knownTypeTable[0x6b]);
            knownPropertyTable[0x1a] = new PropertyDeclaration("BorderThickness", knownTypeTable[0x6b]);
            knownPropertyTable[0x1b] = new PropertyDeclaration("FontFamily", knownTypeTable[0x6b]);
            knownPropertyTable[0x1c] = new PropertyDeclaration("FontSize", knownTypeTable[0x6b]);
            knownPropertyTable[0x1d] = new PropertyDeclaration("FontStretch", knownTypeTable[0x6b]);
            knownPropertyTable[30] = new PropertyDeclaration("FontStyle", knownTypeTable[0x6b]);
            knownPropertyTable[0x1f] = new PropertyDeclaration("FontWeight", knownTypeTable[0x6b]);
            knownPropertyTable[0x20] = new PropertyDeclaration("Foreground", knownTypeTable[0x6b]);
            knownPropertyTable[0x21] = new PropertyDeclaration("HorizontalContentAlignment", knownTypeTable[0x6b]);
            knownPropertyTable[0x22] = new PropertyDeclaration("IsTabStop", knownTypeTable[0x6b]);
            knownPropertyTable[0x23] = new PropertyDeclaration("Padding", knownTypeTable[0x6b]);
            knownPropertyTable[0x24] = new PropertyDeclaration("TabIndex", knownTypeTable[0x6b]);
            knownPropertyTable[0x25] = new PropertyDeclaration("Template", knownTypeTable[0x6b]);
            knownPropertyTable[0x26] = new PropertyDeclaration("VerticalContentAlignment", knownTypeTable[0x6b]);
            knownPropertyTable[0x27] = new PropertyDeclaration("Dock", knownTypeTable[0xa3]);
            knownPropertyTable[40] = new PropertyDeclaration("LastChildFill", knownTypeTable[0xa3]);
            knownPropertyTable[0x29] = new PropertyDeclaration("Document", knownTypeTable[0xa7]);
            knownPropertyTable[0x2a] = new PropertyDeclaration("Children", knownTypeTable[0xb7]);
            knownPropertyTable[0x2b] = new PropertyDeclaration("Document", knownTypeTable[0xd3]);
            knownPropertyTable[0x2c] = new PropertyDeclaration("Document", knownTypeTable[0xd4]);
            knownPropertyTable[0x2d] = new PropertyDeclaration("Style", knownTypeTable[0xe1]);
            knownPropertyTable[0x2e] = new PropertyDeclaration("FlowDirection", knownTypeTable[0xe2]);
            knownPropertyTable[0x2f] = new PropertyDeclaration("Height", knownTypeTable[0xe2]);
            knownPropertyTable[0x30] = new PropertyDeclaration("HorizontalAlignment", knownTypeTable[0xe2]);
            knownPropertyTable[0x31] = new PropertyDeclaration("Margin", knownTypeTable[0xe2]);
            knownPropertyTable[50] = new PropertyDeclaration("MaxHeight", knownTypeTable[0xe2]);
            knownPropertyTable[0x33] = new PropertyDeclaration("MaxWidth", knownTypeTable[0xe2]);
            knownPropertyTable[0x34] = new PropertyDeclaration("MinHeight", knownTypeTable[0xe2]);
            knownPropertyTable[0x35] = new PropertyDeclaration("MinWidth", knownTypeTable[0xe2]);
            knownPropertyTable[0x36] = new PropertyDeclaration("Name", knownTypeTable[0xe2]);
            knownPropertyTable[0x37] = new PropertyDeclaration("Style", knownTypeTable[0xe2]);
            knownPropertyTable[0x38] = new PropertyDeclaration("VerticalAlignment", knownTypeTable[0xe2]);
            knownPropertyTable[0x39] = new PropertyDeclaration("Width", knownTypeTable[0xe2]);
            knownPropertyTable[0x3a] = new PropertyDeclaration("Children", knownTypeTable[0xec]);
            knownPropertyTable[0x3b] = new PropertyDeclaration("Children", knownTypeTable[0xf2]);
            knownPropertyTable[60] = new PropertyDeclaration("GradientStops", knownTypeTable[0xfb]);
            knownPropertyTable[0x3d] = new PropertyDeclaration("Column", knownTypeTable[0xfe]);
            knownPropertyTable[0x3e] = new PropertyDeclaration("ColumnSpan", knownTypeTable[0xfe]);
            knownPropertyTable[0x3f] = new PropertyDeclaration("Row", knownTypeTable[0xfe]);
            knownPropertyTable[0x40] = new PropertyDeclaration("RowSpan", knownTypeTable[0xfe]);
            knownPropertyTable[0x41] = new PropertyDeclaration("Header", knownTypeTable[0x103]);
            knownPropertyTable[0x42] = new PropertyDeclaration("HasHeader", knownTypeTable[0x10d]);
            knownPropertyTable[0x43] = new PropertyDeclaration("Header", knownTypeTable[0x10d]);
            knownPropertyTable[0x44] = new PropertyDeclaration("HeaderTemplate", knownTypeTable[0x10d]);
            knownPropertyTable[0x45] = new PropertyDeclaration("HeaderTemplateSelector", knownTypeTable[0x10d]);
            knownPropertyTable[70] = new PropertyDeclaration("HasHeader", knownTypeTable[270]);
            knownPropertyTable[0x47] = new PropertyDeclaration("Header", knownTypeTable[270]);
            knownPropertyTable[0x48] = new PropertyDeclaration("HeaderTemplate", knownTypeTable[270]);
            knownPropertyTable[0x49] = new PropertyDeclaration("HeaderTemplateSelector", knownTypeTable[270]);
            knownPropertyTable[0x4a] = new PropertyDeclaration("NavigateUri", knownTypeTable[0x111]);
            knownPropertyTable[0x4b] = new PropertyDeclaration("Source", knownTypeTable[0x119]);
            knownPropertyTable[0x4c] = new PropertyDeclaration("Stretch", knownTypeTable[0x119]);
            knownPropertyTable[0x4d] = new PropertyDeclaration("ItemContainerStyle", knownTypeTable[0x149]);
            knownPropertyTable[0x4e] = new PropertyDeclaration("ItemContainerStyleSelector", knownTypeTable[0x149]);
            knownPropertyTable[0x4f] = new PropertyDeclaration("ItemTemplate", knownTypeTable[0x149]);
            knownPropertyTable[80] = new PropertyDeclaration("ItemTemplateSelector", knownTypeTable[0x149]);
            knownPropertyTable[0x51] = new PropertyDeclaration("ItemsPanel", knownTypeTable[0x149]);
            knownPropertyTable[0x52] = new PropertyDeclaration("ItemsSource", knownTypeTable[0x149]);
            knownPropertyTable[0x53] = new PropertyDeclaration("Children", knownTypeTable[0x180]);
            knownPropertyTable[0x54] = new PropertyDeclaration("Children", knownTypeTable[0x198]);
            knownPropertyTable[0x55] = new PropertyDeclaration("Content", knownTypeTable[0x1b2]);
            knownPropertyTable[0x56] = new PropertyDeclaration("Background", knownTypeTable[0x1b5]);
            knownPropertyTable[0x57] = new PropertyDeclaration("Data", knownTypeTable[0x1ba]);
            knownPropertyTable[0x58] = new PropertyDeclaration("Segments", knownTypeTable[0x1bb]);
            knownPropertyTable[0x59] = new PropertyDeclaration("Figures", knownTypeTable[0x1be]);
            knownPropertyTable[90] = new PropertyDeclaration("Child", knownTypeTable[0x1e5]);
            knownPropertyTable[0x5b] = new PropertyDeclaration("IsOpen", knownTypeTable[0x1e5]);
            knownPropertyTable[0x5c] = new PropertyDeclaration("Placement", knownTypeTable[0x1e5]);
            knownPropertyTable[0x5d] = new PropertyDeclaration("PopupAnimation", knownTypeTable[0x1e5]);
            knownPropertyTable[0x5e] = new PropertyDeclaration("Height", knownTypeTable[0x21d]);
            knownPropertyTable[0x5f] = new PropertyDeclaration("MaxHeight", knownTypeTable[0x21d]);
            knownPropertyTable[0x60] = new PropertyDeclaration("MinHeight", knownTypeTable[0x21d]);
            knownPropertyTable[0x61] = new PropertyDeclaration("CanContentScroll", knownTypeTable[550]);
            knownPropertyTable[0x62] = new PropertyDeclaration("HorizontalScrollBarVisibility", knownTypeTable[550]);
            knownPropertyTable[0x63] = new PropertyDeclaration("VerticalScrollBarVisibility", knownTypeTable[550]);
            knownPropertyTable[100] = new PropertyDeclaration("Fill", knownTypeTable[0x22e]);
            knownPropertyTable[0x65] = new PropertyDeclaration("Stroke", knownTypeTable[0x22e]);
            knownPropertyTable[0x66] = new PropertyDeclaration("StrokeThickness", knownTypeTable[0x22e]);
            knownPropertyTable[0x67] = new PropertyDeclaration("Background", knownTypeTable[0x27e]);
            knownPropertyTable[0x68] = new PropertyDeclaration("FontFamily", knownTypeTable[0x27e]);
            knownPropertyTable[0x69] = new PropertyDeclaration("FontSize", knownTypeTable[0x27e]);
            knownPropertyTable[0x6a] = new PropertyDeclaration("FontStretch", knownTypeTable[0x27e]);
            knownPropertyTable[0x6b] = new PropertyDeclaration("FontStyle", knownTypeTable[0x27e]);
            knownPropertyTable[0x6c] = new PropertyDeclaration("FontWeight", knownTypeTable[0x27e]);
            knownPropertyTable[0x6d] = new PropertyDeclaration("Foreground", knownTypeTable[0x27e]);
            knownPropertyTable[110] = new PropertyDeclaration("Text", knownTypeTable[0x27e]);
            knownPropertyTable[0x6f] = new PropertyDeclaration("TextDecorations", knownTypeTable[0x27e]);
            knownPropertyTable[0x70] = new PropertyDeclaration("TextTrimming", knownTypeTable[0x27e]);
            knownPropertyTable[0x71] = new PropertyDeclaration("TextWrapping", knownTypeTable[0x27e]);
            knownPropertyTable[0x72] = new PropertyDeclaration("Text", knownTypeTable[0x27f]);
            knownPropertyTable[0x73] = new PropertyDeclaration("Background", knownTypeTable[0x288]);
            knownPropertyTable[0x74] = new PropertyDeclaration("FontFamily", knownTypeTable[0x288]);
            knownPropertyTable[0x75] = new PropertyDeclaration("FontSize", knownTypeTable[0x288]);
            knownPropertyTable[0x76] = new PropertyDeclaration("FontStretch", knownTypeTable[0x288]);
            knownPropertyTable[0x77] = new PropertyDeclaration("FontStyle", knownTypeTable[0x288]);
            knownPropertyTable[120] = new PropertyDeclaration("FontWeight", knownTypeTable[0x288]);
            knownPropertyTable[0x79] = new PropertyDeclaration("Foreground", knownTypeTable[0x288]);
            knownPropertyTable[0x7a] = new PropertyDeclaration("Children", knownTypeTable[0x29b]);
            knownPropertyTable[0x7b] = new PropertyDeclaration("IsDirectionReversed", knownTypeTable[0x2a3]);
            knownPropertyTable[0x7c] = new PropertyDeclaration("Maximum", knownTypeTable[0x2a3]);
            knownPropertyTable[0x7d] = new PropertyDeclaration("Minimum", knownTypeTable[0x2a3]);
            knownPropertyTable[0x7e] = new PropertyDeclaration("Orientation", knownTypeTable[0x2a3]);
            knownPropertyTable[0x7f] = new PropertyDeclaration("Value", knownTypeTable[0x2a3]);
            knownPropertyTable[0x80] = new PropertyDeclaration("ViewportSize", knownTypeTable[0x2a3]);
            knownPropertyTable[0x81] = new PropertyDeclaration("Children", knownTypeTable[0x2a7]);
            knownPropertyTable[130] = new PropertyDeclaration("Children", knownTypeTable[0x2aa]);
            knownPropertyTable[0x83] = new PropertyDeclaration("ClipToBounds", knownTypeTable[0x2b6]);
            knownPropertyTable[0x84] = new PropertyDeclaration("Focusable", knownTypeTable[0x2b6]);
            knownPropertyTable[0x85] = new PropertyDeclaration("IsEnabled", knownTypeTable[0x2b6]);
            knownPropertyTable[0x86] = new PropertyDeclaration("RenderTransform", knownTypeTable[0x2b6]);
            knownPropertyTable[0x87] = new PropertyDeclaration("Visibility", knownTypeTable[0x2b6]);
            knownPropertyTable[0x88] = new PropertyDeclaration("Children", knownTypeTable[0x2d9]);
            knownPropertyTable[0x8a] = new PropertyDeclaration("Child", knownTypeTable[2]);
            knownPropertyTable[0x8b] = new PropertyDeclaration("Child", knownTypeTable[4]);
            knownPropertyTable[140] = new PropertyDeclaration("Blocks", knownTypeTable[8]);
            knownPropertyTable[0x8d] = new PropertyDeclaration("Items", knownTypeTable[14]);
            knownPropertyTable[0x8e] = new PropertyDeclaration("Child", knownTypeTable[0x25]);
            knownPropertyTable[0x8f] = new PropertyDeclaration("Inlines", knownTypeTable[0x29]);
            knownPropertyTable[0x90] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x2d]);
            knownPropertyTable[0x91] = new PropertyDeclaration("Child", knownTypeTable[50]);
            knownPropertyTable[0x92] = new PropertyDeclaration("Child", knownTypeTable[0x36]);
            knownPropertyTable[0x93] = new PropertyDeclaration("Content", knownTypeTable[0x37]);
            knownPropertyTable[0x94] = new PropertyDeclaration("Content", knownTypeTable[0x38]);
            knownPropertyTable[0x95] = new PropertyDeclaration("KeyFrames", knownTypeTable[60]);
            knownPropertyTable[150] = new PropertyDeclaration("Children", knownTypeTable[0x42]);
            knownPropertyTable[0x97] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x45]);
            knownPropertyTable[0x98] = new PropertyDeclaration("Content", knownTypeTable[0x4a]);
            knownPropertyTable[0x99] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x54]);
            knownPropertyTable[0x9a] = new PropertyDeclaration("Items", knownTypeTable[0x5c]);
            knownPropertyTable[0x9b] = new PropertyDeclaration("Content", knownTypeTable[0x5d]);
            knownPropertyTable[0x9c] = new PropertyDeclaration("Items", knownTypeTable[0x69]);
            knownPropertyTable[0x9d] = new PropertyDeclaration("VisualTree", knownTypeTable[0x6c]);
            knownPropertyTable[0x9e] = new PropertyDeclaration("VisualTree", knownTypeTable[120]);
            knownPropertyTable[0x9f] = new PropertyDeclaration("Setters", knownTypeTable[0x7a]);
            knownPropertyTable[160] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x81]);
            knownPropertyTable[0xa1] = new PropertyDeclaration("Child", knownTypeTable[0x85]);
            knownPropertyTable[0xa2] = new PropertyDeclaration("Children", knownTypeTable[0xa3]);
            knownPropertyTable[0xa3] = new PropertyDeclaration("Document", knownTypeTable[0xa6]);
            knownPropertyTable[0xa4] = new PropertyDeclaration("KeyFrames", knownTypeTable[0xab]);
            knownPropertyTable[0xa5] = new PropertyDeclaration("Actions", knownTypeTable[0xc6]);
            knownPropertyTable[0xa6] = new PropertyDeclaration("Content", knownTypeTable[0xc7]);
            knownPropertyTable[0xa7] = new PropertyDeclaration("Blocks", knownTypeTable[0xca]);
            knownPropertyTable[0xa8] = new PropertyDeclaration("Pages", knownTypeTable[0xcd]);
            knownPropertyTable[0xa9] = new PropertyDeclaration("References", knownTypeTable[0xce]);
            knownPropertyTable[170] = new PropertyDeclaration("Children", knownTypeTable[0xcf]);
            knownPropertyTable[0xab] = new PropertyDeclaration("Blocks", knownTypeTable[0xd0]);
            knownPropertyTable[0xac] = new PropertyDeclaration("Blocks", knownTypeTable[0xd1]);
            knownPropertyTable[0xad] = new PropertyDeclaration("Document", knownTypeTable[210]);
            knownPropertyTable[0xae] = new PropertyDeclaration("VisualTree", knownTypeTable[0xe7]);
            knownPropertyTable[0xaf] = new PropertyDeclaration("Children", knownTypeTable[0xfe]);
            knownPropertyTable[0xb0] = new PropertyDeclaration("Columns", knownTypeTable[0x102]);
            knownPropertyTable[0xb1] = new PropertyDeclaration("Content", knownTypeTable[260]);
            knownPropertyTable[0xb2] = new PropertyDeclaration("Content", knownTypeTable[0x108]);
            knownPropertyTable[0xb3] = new PropertyDeclaration("Content", knownTypeTable[0x109]);
            knownPropertyTable[180] = new PropertyDeclaration("Content", knownTypeTable[0x10d]);
            knownPropertyTable[0xb5] = new PropertyDeclaration("Items", knownTypeTable[270]);
            knownPropertyTable[0xb6] = new PropertyDeclaration("VisualTree", knownTypeTable[0x10f]);
            knownPropertyTable[0xb7] = new PropertyDeclaration("Inlines", knownTypeTable[0x111]);
            knownPropertyTable[0xb8] = new PropertyDeclaration("Children", knownTypeTable[0x120]);
            knownPropertyTable[0xb9] = new PropertyDeclaration("Child", knownTypeTable[0x121]);
            knownPropertyTable[0xba] = new PropertyDeclaration("Child", knownTypeTable[0x124]);
            knownPropertyTable[0xbb] = new PropertyDeclaration("NameValue", knownTypeTable[300]);
            knownPropertyTable[0xbc] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x131]);
            knownPropertyTable[0xbd] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x138]);
            knownPropertyTable[190] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x143]);
            knownPropertyTable[0xbf] = new PropertyDeclaration("Inlines", knownTypeTable[0x147]);
            knownPropertyTable[0xc0] = new PropertyDeclaration("Items", knownTypeTable[0x149]);
            knownPropertyTable[0xc1] = new PropertyDeclaration("VisualTree", knownTypeTable[330]);
            knownPropertyTable[0xc2] = new PropertyDeclaration("Content", knownTypeTable[0x15a]);
            knownPropertyTable[0xc3] = new PropertyDeclaration("GradientStops", knownTypeTable[0x166]);
            knownPropertyTable[0xc4] = new PropertyDeclaration("ListItems", knownTypeTable[0x174]);
            knownPropertyTable[0xc5] = new PropertyDeclaration("Items", knownTypeTable[0x175]);
            knownPropertyTable[0xc6] = new PropertyDeclaration("Content", knownTypeTable[0x176]);
            knownPropertyTable[0xc7] = new PropertyDeclaration("Blocks", knownTypeTable[0x178]);
            knownPropertyTable[200] = new PropertyDeclaration("Items", knownTypeTable[0x179]);
            knownPropertyTable[0xc9] = new PropertyDeclaration("Content", knownTypeTable[0x17a]);
            knownPropertyTable[0xca] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x185]);
            knownPropertyTable[0xcb] = new PropertyDeclaration("Items", knownTypeTable[0x191]);
            knownPropertyTable[0xcc] = new PropertyDeclaration("Items", knownTypeTable[0x192]);
            knownPropertyTable[0xcd] = new PropertyDeclaration("Items", knownTypeTable[0x193]);
            knownPropertyTable[0xce] = new PropertyDeclaration("Children", knownTypeTable[0x199]);
            knownPropertyTable[0xcf] = new PropertyDeclaration("Bindings", knownTypeTable[0x1a0]);
            knownPropertyTable[0xd0] = new PropertyDeclaration("Setters", knownTypeTable[0x1a2]);
            knownPropertyTable[0xd1] = new PropertyDeclaration("Setters", knownTypeTable[0x1a3]);
            knownPropertyTable[210] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x1ac]);
            knownPropertyTable[0xd3] = new PropertyDeclaration("Child", knownTypeTable[0x1b3]);
            knownPropertyTable[0xd4] = new PropertyDeclaration("Content", knownTypeTable[0x1b4]);
            knownPropertyTable[0xd5] = new PropertyDeclaration("Children", knownTypeTable[0x1b5]);
            knownPropertyTable[0xd6] = new PropertyDeclaration("Inlines", knownTypeTable[0x1b6]);
            knownPropertyTable[0xd7] = new PropertyDeclaration("Children", knownTypeTable[0x1b7]);
            knownPropertyTable[0xd8] = new PropertyDeclaration("KeyFrames", knownTypeTable[460]);
            knownPropertyTable[0xd9] = new PropertyDeclaration("KeyFrames", knownTypeTable[470]);
            knownPropertyTable[0xda] = new PropertyDeclaration("Bindings", knownTypeTable[0x1e7]);
            knownPropertyTable[0xdb] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x1f1]);
            knownPropertyTable[220] = new PropertyDeclaration("GradientStops", knownTypeTable[0x1f6]);
            knownPropertyTable[0xdd] = new PropertyDeclaration("Content", knownTypeTable[0x1f7]);
            knownPropertyTable[0xde] = new PropertyDeclaration("KeyFrames", knownTypeTable[510]);
            knownPropertyTable[0xdf] = new PropertyDeclaration("Content", knownTypeTable[0x20a]);
            knownPropertyTable[0xe0] = new PropertyDeclaration("Document", knownTypeTable[0x20f]);
            knownPropertyTable[0xe1] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x215]);
            knownPropertyTable[0xe2] = new PropertyDeclaration("Text", knownTypeTable[0x21e]);
            knownPropertyTable[0xe3] = new PropertyDeclaration("Content", knownTypeTable[550]);
            knownPropertyTable[0xe4] = new PropertyDeclaration("Blocks", knownTypeTable[0x227]);
            knownPropertyTable[0xe5] = new PropertyDeclaration("Items", knownTypeTable[0x229]);
            knownPropertyTable[230] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x232]);
            knownPropertyTable[0xe7] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x23b]);
            knownPropertyTable[0xe8] = new PropertyDeclaration("Inlines", knownTypeTable[580]);
            knownPropertyTable[0xe9] = new PropertyDeclaration("Children", knownTypeTable[0x259]);
            knownPropertyTable[0xea] = new PropertyDeclaration("Items", knownTypeTable[0x25c]);
            knownPropertyTable[0xeb] = new PropertyDeclaration("Content", knownTypeTable[0x25d]);
            knownPropertyTable[0xec] = new PropertyDeclaration("Children", knownTypeTable[0x260]);
            knownPropertyTable[0xed] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x266]);
            knownPropertyTable[0xee] = new PropertyDeclaration("Setters", knownTypeTable[620]);
            knownPropertyTable[0xef] = new PropertyDeclaration("Items", knownTypeTable[0x26f]);
            knownPropertyTable[240] = new PropertyDeclaration("Content", knownTypeTable[0x270]);
            knownPropertyTable[0xf1] = new PropertyDeclaration("Children", knownTypeTable[0x271]);
            knownPropertyTable[0xf2] = new PropertyDeclaration("RowGroups", knownTypeTable[0x272]);
            knownPropertyTable[0xf3] = new PropertyDeclaration("Blocks", knownTypeTable[0x273]);
            knownPropertyTable[0xf4] = new PropertyDeclaration("Cells", knownTypeTable[0x275]);
            knownPropertyTable[0xf5] = new PropertyDeclaration("Rows", knownTypeTable[630]);
            knownPropertyTable[0xf6] = new PropertyDeclaration("Inlines", knownTypeTable[0x27e]);
            knownPropertyTable[0xf7] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x28e]);
            knownPropertyTable[0xf8] = new PropertyDeclaration("Content", knownTypeTable[0x29c]);
            knownPropertyTable[0xf9] = new PropertyDeclaration("Items", knownTypeTable[0x29d]);
            knownPropertyTable[250] = new PropertyDeclaration("Children", knownTypeTable[670]);
            knownPropertyTable[0xfb] = new PropertyDeclaration("Children", knownTypeTable[0x29f]);
            knownPropertyTable[0xfc] = new PropertyDeclaration("ToolBars", knownTypeTable[0x2a0]);
            knownPropertyTable[0xfd] = new PropertyDeclaration("Content", knownTypeTable[0x2a1]);
            knownPropertyTable[0xfe] = new PropertyDeclaration("Items", knownTypeTable[0x2ae]);
            knownPropertyTable[0xff] = new PropertyDeclaration("Items", knownTypeTable[0x2af]);
            knownPropertyTable[0x100] = new PropertyDeclaration("Setters", knownTypeTable[0x2b0]);
            knownPropertyTable[0x101] = new PropertyDeclaration("Inlines", knownTypeTable[0x2be]);
            knownPropertyTable[0x102] = new PropertyDeclaration("Children", knownTypeTable[0x2bf]);
            knownPropertyTable[0x103] = new PropertyDeclaration("Content", knownTypeTable[0x2c2]);
            knownPropertyTable[260] = new PropertyDeclaration("KeyFrames", knownTypeTable[0x2c8]);
            knownPropertyTable[0x105] = new PropertyDeclaration("KeyFrames", knownTypeTable[720]);
            knownPropertyTable[0x106] = new PropertyDeclaration("Child", knownTypeTable[0x2d8]);
            knownPropertyTable[0x107] = new PropertyDeclaration("Children", knownTypeTable[730]);
            knownPropertyTable[0x108] = new PropertyDeclaration("Children", knownTypeTable[0x2db]);
            knownPropertyTable[0x109] = new PropertyDeclaration("Children", knownTypeTable[0x2dc]);
            knownPropertyTable[0x10a] = new PropertyDeclaration("Content", knownTypeTable[0x2e3]);
            knownPropertyTable[0x10b] = new PropertyDeclaration("Children", knownTypeTable[0x2e6]);
            knownPropertyTable[0x10c] = new PropertyDeclaration("XmlSerializer", knownTypeTable[0x2f2]);
            this.knownResourceTable.Add(1, new ResourceName("ActiveBorderBrush"));
            this.knownResourceTable.Add(0x1f, new ResourceName("ActiveBorderColor"));
            this.knownResourceTable.Add(2, new ResourceName("ActiveCaptionBrush"));
            this.knownResourceTable.Add(0x20, new ResourceName("ActiveCaptionColor"));
            this.knownResourceTable.Add(3, new ResourceName("ActiveCaptionTextBrush"));
            this.knownResourceTable.Add(0x21, new ResourceName("ActiveCaptionTextColor"));
            this.knownResourceTable.Add(4, new ResourceName("AppWorkspaceBrush"));
            this.knownResourceTable.Add(0x22, new ResourceName("AppWorkspaceColor"));
            this.knownResourceTable.Add(0xc6, new ResourceName("Border"));
            this.knownResourceTable.Add(0xca, new ResourceName("BorderWidth"));
            this.knownResourceTable.Add(0x40, new ResourceName("CaptionFontFamily"));
            this.knownResourceTable.Add(0x3f, new ResourceName("CaptionFontSize"));
            this.knownResourceTable.Add(0x41, new ResourceName("CaptionFontStyle"));
            this.knownResourceTable.Add(0x43, new ResourceName("CaptionFontTextDecorations"));
            this.knownResourceTable.Add(0x42, new ResourceName("CaptionFontWeight"));
            this.knownResourceTable.Add(0xce, new ResourceName("CaptionHeight"));
            this.knownResourceTable.Add(0xcd, new ResourceName("CaptionWidth"));
            this.knownResourceTable.Add(0xc7, new ResourceName("CaretWidth"));
            this.knownResourceTable.Add(0xba, new ResourceName("ClientAreaAnimation"));
            this.knownResourceTable.Add(0xb9, new ResourceName("ComboBoxAnimation"));
            this.knownResourceTable.Add(210, new ResourceName("ComboBoxPopupAnimation"));
            this.knownResourceTable.Add(5, new ResourceName("ControlBrush"));
            this.knownResourceTable.Add(0x23, new ResourceName("ControlColor"));
            this.knownResourceTable.Add(6, new ResourceName("ControlDarkBrush"));
            this.knownResourceTable.Add(0x24, new ResourceName("ControlDarkColor"));
            this.knownResourceTable.Add(7, new ResourceName("ControlDarkDarkBrush"));
            this.knownResourceTable.Add(0x25, new ResourceName("ControlDarkDarkColor"));
            this.knownResourceTable.Add(8, new ResourceName("ControlLightBrush"));
            this.knownResourceTable.Add(0x26, new ResourceName("ControlLightColor"));
            this.knownResourceTable.Add(9, new ResourceName("ControlLightLightBrush"));
            this.knownResourceTable.Add(0x27, new ResourceName("ControlLightLightColor"));
            this.knownResourceTable.Add(10, new ResourceName("ControlTextBrush"));
            this.knownResourceTable.Add(40, new ResourceName("ControlTextColor"));
            this.knownResourceTable.Add(0x62, new ResourceName("CursorHeight"));
            this.knownResourceTable.Add(0xbb, new ResourceName("CursorShadow"));
            this.knownResourceTable.Add(0x61, new ResourceName("CursorWidth"));
            this.knownResourceTable.Add(11, new ResourceName("DesktopBrush"));
            this.knownResourceTable.Add(0x29, new ResourceName("DesktopColor"));
            this.knownResourceTable.Add(0xc9, new ResourceName("DragFullWindows"));
            this.knownResourceTable.Add(0xa7, new ResourceName("DropShadow"));
            this.knownResourceTable.Add(0x65, new ResourceName("FixedFrameHorizontalBorderHeight"));
            this.knownResourceTable.Add(0x66, new ResourceName("FixedFrameVerticalBorderWidth"));
            this.knownResourceTable.Add(0xa8, new ResourceName("FlatMenu"));
            this.knownResourceTable.Add(0xa5, new ResourceName("FocusBorderHeight"));
            this.knownResourceTable.Add(0xa4, new ResourceName("FocusBorderWidth"));
            this.knownResourceTable.Add(0x67, new ResourceName("FocusHorizontalBorderHeight"));
            this.knownResourceTable.Add(0x68, new ResourceName("FocusVerticalBorderWidth"));
            this.knownResourceTable.Add(0xd7, new ResourceName("FocusVisualStyle"));
            this.knownResourceTable.Add(200, new ResourceName("ForegroundFlashCount"));
            this.knownResourceTable.Add(0x6a, new ResourceName("FullPrimaryScreenHeight"));
            this.knownResourceTable.Add(0x69, new ResourceName("FullPrimaryScreenWidth"));
            this.knownResourceTable.Add(12, new ResourceName("GradientActiveCaptionBrush"));
            this.knownResourceTable.Add(0x2a, new ResourceName("GradientActiveCaptionColor"));
            this.knownResourceTable.Add(0xbc, new ResourceName("GradientCaptions"));
            this.knownResourceTable.Add(13, new ResourceName("GradientInactiveCaptionBrush"));
            this.knownResourceTable.Add(0x2b, new ResourceName("GradientInactiveCaptionColor"));
            this.knownResourceTable.Add(14, new ResourceName("GrayTextBrush"));
            this.knownResourceTable.Add(0x2c, new ResourceName("GrayTextColor"));
            this.knownResourceTable.Add(0xde, new ResourceName("GridViewItemContainerStyle"));
            this.knownResourceTable.Add(220, new ResourceName("GridViewScrollViewerStyle"));
            this.knownResourceTable.Add(0xdd, new ResourceName("GridViewStyle"));
            this.knownResourceTable.Add(0xa6, new ResourceName("HighContrast"));
            this.knownResourceTable.Add(15, new ResourceName("HighlightBrush"));
            this.knownResourceTable.Add(0x2d, new ResourceName("HighlightColor"));
            this.knownResourceTable.Add(0x10, new ResourceName("HighlightTextBrush"));
            this.knownResourceTable.Add(0x2e, new ResourceName("HighlightTextColor"));
            this.knownResourceTable.Add(0x6b, new ResourceName("HorizontalScrollBarButtonWidth"));
            this.knownResourceTable.Add(0x6c, new ResourceName("HorizontalScrollBarHeight"));
            this.knownResourceTable.Add(0x6d, new ResourceName("HorizontalScrollBarThumbWidth"));
            this.knownResourceTable.Add(0x11, new ResourceName("HotTrackBrush"));
            this.knownResourceTable.Add(0x2f, new ResourceName("HotTrackColor"));
            this.knownResourceTable.Add(0xbd, new ResourceName("HotTracking"));
            this.knownResourceTable.Add(0x59, new ResourceName("IconFontFamily"));
            this.knownResourceTable.Add(0x58, new ResourceName("IconFontSize"));
            this.knownResourceTable.Add(90, new ResourceName("IconFontStyle"));
            this.knownResourceTable.Add(0x5c, new ResourceName("IconFontTextDecorations"));
            this.knownResourceTable.Add(0x5b, new ResourceName("IconFontWeight"));
            this.knownResourceTable.Add(0x71, new ResourceName("IconGridHeight"));
            this.knownResourceTable.Add(0x70, new ResourceName("IconGridWidth"));
            this.knownResourceTable.Add(0x6f, new ResourceName("IconHeight"));
            this.knownResourceTable.Add(170, new ResourceName("IconHorizontalSpacing"));
            this.knownResourceTable.Add(0xac, new ResourceName("IconTitleWrap"));
            this.knownResourceTable.Add(0xab, new ResourceName("IconVerticalSpacing"));
            this.knownResourceTable.Add(110, new ResourceName("IconWidth"));
            this.knownResourceTable.Add(0x12, new ResourceName("InactiveBorderBrush"));
            this.knownResourceTable.Add(0x30, new ResourceName("InactiveBorderColor"));
            this.knownResourceTable.Add(0x13, new ResourceName("InactiveCaptionBrush"));
            this.knownResourceTable.Add(0x31, new ResourceName("InactiveCaptionColor"));
            this.knownResourceTable.Add(20, new ResourceName("InactiveCaptionTextBrush"));
            this.knownResourceTable.Add(50, new ResourceName("InactiveCaptionTextColor"));
            this.knownResourceTable.Add(0x15, new ResourceName("InfoBrush"));
            this.knownResourceTable.Add(0x33, new ResourceName("InfoColor"));
            this.knownResourceTable.Add(0x16, new ResourceName("InfoTextBrush"));
            this.knownResourceTable.Add(0x34, new ResourceName("InfoTextColor"));
            this.knownResourceTable.Add(0x3d, new ResourceName("InternalSystemColorsEnd"));
            this.knownResourceTable.Add(0, new ResourceName("InternalSystemColorsStart"));
            this.knownResourceTable.Add(0x5d, new ResourceName("InternalSystemFontsEnd"));
            this.knownResourceTable.Add(0x3e, new ResourceName("InternalSystemFontsStart"));
            this.knownResourceTable.Add(0xda, new ResourceName("InternalSystemParametersEnd"));
            this.knownResourceTable.Add(0x5e, new ResourceName("InternalSystemParametersStart"));
            this.knownResourceTable.Add(0xe8, new ResourceName("InternalSystemThemeStylesEnd"));
            this.knownResourceTable.Add(0xd6, new ResourceName("InternalSystemThemeStylesStart"));
            this.knownResourceTable.Add(0x95, new ResourceName("IsImmEnabled"));
            this.knownResourceTable.Add(150, new ResourceName("IsMediaCenter"));
            this.knownResourceTable.Add(0x97, new ResourceName("IsMenuDropRightAligned"));
            this.knownResourceTable.Add(0x98, new ResourceName("IsMiddleEastEnabled"));
            this.knownResourceTable.Add(0x99, new ResourceName("IsMousePresent"));
            this.knownResourceTable.Add(0x9a, new ResourceName("IsMouseWheelPresent"));
            this.knownResourceTable.Add(0x9b, new ResourceName("IsPenWindows"));
            this.knownResourceTable.Add(0x9c, new ResourceName("IsRemotelyControlled"));
            this.knownResourceTable.Add(0x9d, new ResourceName("IsRemoteSession"));
            this.knownResourceTable.Add(0x9f, new ResourceName("IsSlowMachine"));
            this.knownResourceTable.Add(0xa1, new ResourceName("IsTabletPC"));
            this.knownResourceTable.Add(0x91, new ResourceName("KanjiWindowHeight"));
            this.knownResourceTable.Add(0xad, new ResourceName("KeyboardCues"));
            this.knownResourceTable.Add(0xae, new ResourceName("KeyboardDelay"));
            this.knownResourceTable.Add(0xaf, new ResourceName("KeyboardPreference"));
            this.knownResourceTable.Add(0xb0, new ResourceName("KeyboardSpeed"));
            this.knownResourceTable.Add(190, new ResourceName("ListBoxSmoothScrolling"));
            this.knownResourceTable.Add(0x73, new ResourceName("MaximizedPrimaryScreenHeight"));
            this.knownResourceTable.Add(0x72, new ResourceName("MaximizedPrimaryScreenWidth"));
            this.knownResourceTable.Add(0x75, new ResourceName("MaximumWindowTrackHeight"));
            this.knownResourceTable.Add(0x74, new ResourceName("MaximumWindowTrackWidth"));
            this.knownResourceTable.Add(0xbf, new ResourceName("MenuAnimation"));
            this.knownResourceTable.Add(0x18, new ResourceName("MenuBarBrush"));
            this.knownResourceTable.Add(0x36, new ResourceName("MenuBarColor"));
            this.knownResourceTable.Add(0x92, new ResourceName("MenuBarHeight"));
            this.knownResourceTable.Add(0x17, new ResourceName("MenuBrush"));
            this.knownResourceTable.Add(0x79, new ResourceName("MenuButtonHeight"));
            this.knownResourceTable.Add(120, new ResourceName("MenuButtonWidth"));
            this.knownResourceTable.Add(0x77, new ResourceName("MenuCheckmarkHeight"));
            this.knownResourceTable.Add(0x76, new ResourceName("MenuCheckmarkWidth"));
            this.knownResourceTable.Add(0x35, new ResourceName("MenuColor"));
            this.knownResourceTable.Add(0xb6, new ResourceName("MenuDropAlignment"));
            this.knownResourceTable.Add(0xb7, new ResourceName("MenuFade"));
            this.knownResourceTable.Add(0x4a, new ResourceName("MenuFontFamily"));
            this.knownResourceTable.Add(0x49, new ResourceName("MenuFontSize"));
            this.knownResourceTable.Add(0x4b, new ResourceName("MenuFontStyle"));
            this.knownResourceTable.Add(0x4d, new ResourceName("MenuFontTextDecorations"));
            this.knownResourceTable.Add(0x4c, new ResourceName("MenuFontWeight"));
            this.knownResourceTable.Add(0xd1, new ResourceName("MenuHeight"));
            this.knownResourceTable.Add(0x19, new ResourceName("MenuHighlightBrush"));
            this.knownResourceTable.Add(0x37, new ResourceName("MenuHighlightColor"));
            this.knownResourceTable.Add(0xdb, new ResourceName("MenuItemSeparatorStyle"));
            this.knownResourceTable.Add(0xd3, new ResourceName("MenuPopupAnimation"));
            this.knownResourceTable.Add(0xb8, new ResourceName("MenuShowDelay"));
            this.knownResourceTable.Add(0x1a, new ResourceName("MenuTextBrush"));
            this.knownResourceTable.Add(0x38, new ResourceName("MenuTextColor"));
            this.knownResourceTable.Add(0xd0, new ResourceName("MenuWidth"));
            this.knownResourceTable.Add(0x54, new ResourceName("MessageFontFamily"));
            this.knownResourceTable.Add(0x53, new ResourceName("MessageFontSize"));
            this.knownResourceTable.Add(0x55, new ResourceName("MessageFontStyle"));
            this.knownResourceTable.Add(0x57, new ResourceName("MessageFontTextDecorations"));
            this.knownResourceTable.Add(0x56, new ResourceName("MessageFontWeight"));
            this.knownResourceTable.Add(0xc5, new ResourceName("MinimizeAnimation"));
            this.knownResourceTable.Add(0x7f, new ResourceName("MinimizedGridHeight"));
            this.knownResourceTable.Add(0x7e, new ResourceName("MinimizedGridWidth"));
            this.knownResourceTable.Add(0x7d, new ResourceName("MinimizedWindowHeight"));
            this.knownResourceTable.Add(0x7c, new ResourceName("MinimizedWindowWidth"));
            this.knownResourceTable.Add(0x7b, new ResourceName("MinimumWindowHeight"));
            this.knownResourceTable.Add(0x81, new ResourceName("MinimumWindowTrackHeight"));
            this.knownResourceTable.Add(0x80, new ResourceName("MinimumWindowTrackWidth"));
            this.knownResourceTable.Add(0x7a, new ResourceName("MinimumWindowWidth"));
            this.knownResourceTable.Add(180, new ResourceName("MouseHoverHeight"));
            this.knownResourceTable.Add(0xb3, new ResourceName("MouseHoverTime"));
            this.knownResourceTable.Add(0xb5, new ResourceName("MouseHoverWidth"));
            this.knownResourceTable.Add(0xd8, new ResourceName("NavigationChromeDownLevelStyle"));
            this.knownResourceTable.Add(0xd9, new ResourceName("NavigationChromeStyle"));
            this.knownResourceTable.Add(0xd5, new ResourceName("PowerLineStatus"));
            this.knownResourceTable.Add(0x83, new ResourceName("PrimaryScreenHeight"));
            this.knownResourceTable.Add(130, new ResourceName("PrimaryScreenWidth"));
            this.knownResourceTable.Add(0x86, new ResourceName("ResizeFrameHorizontalBorderHeight"));
            this.knownResourceTable.Add(0x87, new ResourceName("ResizeFrameVerticalBorderWidth"));
            this.knownResourceTable.Add(0x1b, new ResourceName("ScrollBarBrush"));
            this.knownResourceTable.Add(0x39, new ResourceName("ScrollBarColor"));
            this.knownResourceTable.Add(0xcc, new ResourceName("ScrollHeight"));
            this.knownResourceTable.Add(0xcb, new ResourceName("ScrollWidth"));
            this.knownResourceTable.Add(0xc0, new ResourceName("SelectionFade"));
            this.knownResourceTable.Add(0x9e, new ResourceName("ShowSounds"));
            this.knownResourceTable.Add(0x45, new ResourceName("SmallCaptionFontFamily"));
            this.knownResourceTable.Add(0x44, new ResourceName("SmallCaptionFontSize"));
            this.knownResourceTable.Add(70, new ResourceName("SmallCaptionFontStyle"));
            this.knownResourceTable.Add(0x48, new ResourceName("SmallCaptionFontTextDecorations"));
            this.knownResourceTable.Add(0x47, new ResourceName("SmallCaptionFontWeight"));
            this.knownResourceTable.Add(0x93, new ResourceName("SmallCaptionHeight"));
            this.knownResourceTable.Add(0xcf, new ResourceName("SmallCaptionWidth"));
            this.knownResourceTable.Add(0x89, new ResourceName("SmallIconHeight"));
            this.knownResourceTable.Add(0x88, new ResourceName("SmallIconWidth"));
            this.knownResourceTable.Add(0x8b, new ResourceName("SmallWindowCaptionButtonHeight"));
            this.knownResourceTable.Add(0x8a, new ResourceName("SmallWindowCaptionButtonWidth"));
            this.knownResourceTable.Add(0xb1, new ResourceName("SnapToDefaultButton"));
            this.knownResourceTable.Add(0xdf, new ResourceName("StatusBarSeparatorStyle"));
            this.knownResourceTable.Add(0x4f, new ResourceName("StatusFontFamily"));
            this.knownResourceTable.Add(0x4e, new ResourceName("StatusFontSize"));
            this.knownResourceTable.Add(80, new ResourceName("StatusFontStyle"));
            this.knownResourceTable.Add(0x52, new ResourceName("StatusFontTextDecorations"));
            this.knownResourceTable.Add(0x51, new ResourceName("StatusFontWeight"));
            this.knownResourceTable.Add(0xc1, new ResourceName("StylusHotTracking"));
            this.knownResourceTable.Add(160, new ResourceName("SwapButtons"));
            this.knownResourceTable.Add(0x63, new ResourceName("ThickHorizontalBorderHeight"));
            this.knownResourceTable.Add(100, new ResourceName("ThickVerticalBorderWidth"));
            this.knownResourceTable.Add(0x5f, new ResourceName("ThinHorizontalBorderHeight"));
            this.knownResourceTable.Add(0x60, new ResourceName("ThinVerticalBorderWidth"));
            this.knownResourceTable.Add(0xe0, new ResourceName("ToolBarButtonStyle"));
            this.knownResourceTable.Add(0xe3, new ResourceName("ToolBarCheckBoxStyle"));
            this.knownResourceTable.Add(0xe5, new ResourceName("ToolBarComboBoxStyle"));
            this.knownResourceTable.Add(0xe7, new ResourceName("ToolBarMenuStyle"));
            this.knownResourceTable.Add(0xe4, new ResourceName("ToolBarRadioButtonStyle"));
            this.knownResourceTable.Add(0xe2, new ResourceName("ToolBarSeparatorStyle"));
            this.knownResourceTable.Add(230, new ResourceName("ToolBarTextBoxStyle"));
            this.knownResourceTable.Add(0xe1, new ResourceName("ToolBarToggleButtonStyle"));
            this.knownResourceTable.Add(0xc2, new ResourceName("ToolTipAnimation"));
            this.knownResourceTable.Add(0xc3, new ResourceName("ToolTipFade"));
            this.knownResourceTable.Add(0xd4, new ResourceName("ToolTipPopupAnimation"));
            this.knownResourceTable.Add(0xc4, new ResourceName("UIEffects"));
            this.knownResourceTable.Add(0x8f, new ResourceName("VerticalScrollBarButtonHeight"));
            this.knownResourceTable.Add(0x94, new ResourceName("VerticalScrollBarThumbHeight"));
            this.knownResourceTable.Add(0x8e, new ResourceName("VerticalScrollBarWidth"));
            this.knownResourceTable.Add(0x8d, new ResourceName("VirtualScreenHeight"));
            this.knownResourceTable.Add(0xa2, new ResourceName("VirtualScreenLeft"));
            this.knownResourceTable.Add(0xa3, new ResourceName("VirtualScreenTop"));
            this.knownResourceTable.Add(140, new ResourceName("VirtualScreenWidth"));
            this.knownResourceTable.Add(0xb2, new ResourceName("WheelScrollLines"));
            this.knownResourceTable.Add(0x1c, new ResourceName("WindowBrush"));
            this.knownResourceTable.Add(0x85, new ResourceName("WindowCaptionButtonHeight"));
            this.knownResourceTable.Add(0x84, new ResourceName("WindowCaptionButtonWidth"));
            this.knownResourceTable.Add(0x90, new ResourceName("WindowCaptionHeight"));
            this.knownResourceTable.Add(0x3a, new ResourceName("WindowColor"));
            this.knownResourceTable.Add(0x1d, new ResourceName("WindowFrameBrush"));
            this.knownResourceTable.Add(0x3b, new ResourceName("WindowFrameColor"));
            this.knownResourceTable.Add(30, new ResourceName("WindowTextBrush"));
            this.knownResourceTable.Add(60, new ResourceName("WindowTextColor"));
            this.knownResourceTable.Add(0xa9, new ResourceName("WorkArea"));
        }

        private static bool IsExtension(object value)
        {
            Element element = value as Element;
            if (element != null)
            {
                if (element.Arguments.Count == 0)
                {
                    foreach (Property property in element.Properties)
                    {
                        if (property.PropertyType == PropertyType.Declaration)
                        {
                            return false;
                        }
                        if (!IsExtension(property.Value))
                        {
                            return false;
                        }
                        if (property.PropertyType == PropertyType.Content)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            if (value is IList)
            {
                return false;
            }
            return true;
        }

        private void ReadAssemblyInfo(BamlBinaryReader reader)
        {
            short key = reader.ReadInt16();
            string str = reader.ReadString();
            this.assemblyTable.Add(key, str);
        }

        private void ReadAttributeInfo(BamlBinaryReader reader)
        {
            short key = reader.ReadInt16();
            short identifier = reader.ReadInt16();
            reader.ReadByte();
            string name = reader.ReadString();
            TypeDeclaration typeDeclaration = this.GetTypeDeclaration(identifier);
            PropertyDeclaration declaration2 = new PropertyDeclaration(name, typeDeclaration);
            this.propertyTable.Add(key, declaration2);
        }

        private void ReadConstructorParametersEnd()
        {
            Element element = (Element)this.elementStack.Peek();
            this.constructorParameterTable.Remove(element);
        }

        private void ReadConstructorParametersStart()
        {
            Element element = (Element)this.elementStack.Peek();
            this.constructorParameterTable.Add(element);
        }

        private void ReadConstructorParameterType(BamlBinaryReader reader)
        {
            short identifier = reader.ReadInt16();
            TypeDeclaration typeDeclaration = this.GetTypeDeclaration(identifier);
            Element element = (Element)this.elementStack.Peek();
            element.Arguments.Add(typeDeclaration);
        }

        private void ReadContentProperty(BamlBinaryReader reader)
        {
            short identifier = reader.ReadInt16();
            Element parent = (Element)this.elementStack.Peek();
            Property contentProperty = this.GetContentProperty(parent);
            if (contentProperty == null)
            {
                contentProperty = new Property(PropertyType.Content);
                parent.Properties.Add(contentProperty);
            }
            PropertyDeclaration propertyDeclaration = this.GetPropertyDeclaration(identifier);
            if ((contentProperty.PropertyDeclaration != null) && (contentProperty.PropertyDeclaration != propertyDeclaration))
            {
                throw new NotSupportedException();
            }
            contentProperty.PropertyDeclaration = propertyDeclaration;
        }

        private void ReadDefAttribute(BamlBinaryReader reader)
        {
            string str = reader.ReadString();
            short identifier = reader.ReadInt16();
            Property property = new Property(PropertyType.Declaration);
            switch (identifier)
            {
                case -2:
                    property.PropertyDeclaration = new PropertyDeclaration("x:Uid");
                    break;

                case -1:
                    property.PropertyDeclaration = new PropertyDeclaration("x:Name");
                    break;

                default:
                    property.PropertyDeclaration = this.GetPropertyDeclaration(identifier);
                    break;
            }
            property.Value = str;
            Element element = (Element)this.elementStack.Peek();
            element.Properties.Add(property);
        }

        private void ReadDefAttributeKeyString(BamlBinaryReader reader)
        {
            short num = reader.ReadInt16();
            int position = reader.ReadInt32();
            reader.ReadBoolean();
            reader.ReadBoolean();
            string str = (string)stringTable[num];
            if (str == null)
            {
                throw new NotSupportedException();
            }
            Property keyProperty = new Property(PropertyType.Value);
            keyProperty.PropertyDeclaration = new PropertyDeclaration("x:Key");
            keyProperty.Value = str;
            Element dictionary = (Element)elementStack.Peek();
            AddDictionaryEntry(dictionary, position, keyProperty);
        }

        private void ReadDefAttributeKeyType(BamlBinaryReader reader)
        {
            short typeIdentifier = reader.ReadInt16();
            reader.ReadByte();
            int position = reader.ReadInt32();
            reader.ReadBoolean();
            reader.ReadBoolean();
            Property keyProperty = new Property(PropertyType.Complex);
            keyProperty.PropertyDeclaration = new PropertyDeclaration("x:Key");
            keyProperty.Value = this.CreateTypeExtension(typeIdentifier);
            Element dictionary = (Element)this.elementStack.Peek();
            this.AddDictionaryEntry(dictionary, position, keyProperty);
        }

        private void ReadElementEnd()
        {
            Property property = this.elementStack.Peek() as Property;
            if ((property != null) && (property.PropertyType == PropertyType.Dictionary))
            {
                property = null;
            }
            else
            {
                Element parent = (Element)this.elementStack.Pop();
                Property contentProperty = this.GetContentProperty(parent);
                if ((contentProperty != null) && (contentProperty.Value == null))
                {
                    parent.Properties.Remove(contentProperty);
                }
                if (parent.TypeDeclaration == this.GetTypeDeclaration(-120))
                {
                    bool flag = false;
                    for (int i = parent.Properties.Count - 1; i >= 0; i--)
                    {
                        if ((parent.Properties[i].PropertyDeclaration != null) && (parent.Properties[i].PropertyDeclaration.Name == "DataType"))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        for (int j = parent.Properties.Count - 1; j >= 0; j--)
                        {
                            if ((parent.Properties[j].PropertyDeclaration != null) && (parent.Properties[j].PropertyDeclaration.Name == "x:Key"))
                            {
                                parent.Properties.Remove(parent.Properties[j]);
                            }
                        }
                    }
                }
            }
        }

        private void ReadElementStart(BamlBinaryReader reader)
        {
            short identifier = reader.ReadInt16();
            reader.ReadByte();
            Element element = new Element();
            element.TypeDeclaration = this.GetTypeDeclaration(identifier);
            this.AddElementToTree(element, reader);
            this.elementStack.Push(element);
        }

        private void ReadKeyElementEnd()
        {
            Element element1 = (Element)this.elementStack.Pop();
        }

        private void ReadKeyElementStart(BamlBinaryReader reader)
        {
            short typeIdentifier = reader.ReadInt16();
            byte num2 = reader.ReadByte();
            int position = reader.ReadInt32();
            reader.ReadBoolean();
            reader.ReadBoolean();
            Property keyProperty = new Property(PropertyType.Complex);
            keyProperty.PropertyDeclaration = new PropertyDeclaration("x:Key");
            keyProperty.Value = this.CreateTypeExtension(typeIdentifier, false);
            Element dictionary = (Element)this.elementStack.Peek();
            this.AddDictionaryEntry(dictionary, position, keyProperty);
            this.elementStack.Push(keyProperty.Value);
        }

        private void ReadNamespaceMapping(BamlBinaryReader reader)
        {
            string xmlNamespace = reader.ReadString();
            string clrNamespace = reader.ReadString();
            short num = reader.ReadInt16();
            string assembly = (string)this.assemblyTable[num];
            this.namespaceManager.AddNamespaceMapping(xmlNamespace, clrNamespace, assembly);
        }

        private void ReadOptimizedStaticResource(BamlBinaryReader reader)
        {
            byte num = reader.ReadByte();
            short typeIdentifier = reader.ReadInt16();
            bool flag = (num & 1) == 1;
            bool flag2 = (num & 2) == 2;
            object obj2 = null;
            if (flag)
            {
                obj2 = this.CreateTypeExtension(typeIdentifier);
            }
            else if (flag2)
            {
                Element element2 = new Element();
                element2.TypeDeclaration = new TypeDeclaration("x:Static");
                ResourceName resourceName = (ResourceName)this.GetResourceName(typeIdentifier);
                element2.Arguments.Add(resourceName);
                obj2 = element2;
            }
            else
            {
                string str = (string)this.stringTable[typeIdentifier];
                obj2 = str;
            }
            this.staticResourceTable.Add(obj2);
        }

        private void ReadPresentationOptionsAttribute(BamlBinaryReader reader)
        {
            string str = reader.ReadString();
            short num = reader.ReadInt16();
            Property property = new Property(PropertyType.Value);
            property.PropertyDeclaration = new PropertyDeclaration("PresentationOptions:" + this.stringTable[num]);
            property.Value = str;
            Element element = (Element)this.elementStack.Peek();
            element.Properties.Add(property);
        }

        private void ReadPropertyComplexEnd()
        {
            Property property = (Property)this.elementStack.Pop();
            if (property.PropertyType != PropertyType.Complex)
            {
                throw new NotSupportedException();
            }
        }

        private void ReadPropertyComplexStart(BamlBinaryReader reader)
        {
            short identifier = reader.ReadInt16();
            Property property = new Property(PropertyType.Complex);
            property.PropertyDeclaration = this.GetPropertyDeclaration(identifier);
            Element element = (Element)this.elementStack.Peek();
            element.Properties.Add(property);
            this.elementStack.Push(property);
        }

        private void ReadPropertyCustom(BamlBinaryReader reader)
        {
            Element element;
            short identifier = reader.ReadInt16();
            short num2 = reader.ReadInt16();
            bool flag = (num2 & 0x4000) == 0x4000;
            num2 = (short)(num2 & -16385);
            Property property = new Property(PropertyType.Value);
            property.PropertyDeclaration = this.GetPropertyDeclaration(identifier);
            switch (num2)
            {
                case 0x2e8:
                    switch (reader.ReadByte())
                    {
                        case 1:
                            {
                                uint argb = reader.ReadUInt32();
                                if (argb == 0xffffff)
                                {
                                    property.Value = "Transparent";
                                }
                                else
                                {
                                    property.Value = KnownColors.KnownColorFromUInt(argb);
                                }
                                goto Label_038A;
                            }
                        case 2:
                            property.Value = reader.ReadString();
                            goto Label_038A;
                    }
                    throw new NotSupportedException();

                case 0x2e9:
                    {
                        using (StringWriter writer = new StringWriter())
                        {
                            int num8;
                            int num10;
                            int num12;
                            byte num6 = reader.ReadByte();
                            int num7 = reader.ReadInt32();
                            switch (num6)
                            {
                                case 1:
                                    num8 = 0;
                                    goto Label_01C9;

                                case 2:
                                    num10 = 0;
                                    goto Label_0202;

                                case 3:
                                    num12 = 0;
                                    goto Label_023C;

                                case 4:
                                    throw new NotSupportedException();

                                default:
                                    throw new NotSupportedException();
                            }
                        Label_0194:
                            if (num8 != 0)
                            {
                                writer.Write(",");
                            }
                            int num9 = reader.ReadInt32();
                            writer.Write(num9.ToString());
                            if (num9 > num7)
                            {
                                goto Label_0250;
                            }
                            num8++;
                        Label_01C9:
                            if (num8 < num7)
                            {
                                goto Label_0194;
                            }
                            goto Label_0250;
                        Label_01D6:
                            if (num10 != 0)
                            {
                                writer.Write(",");
                            }
                            writer.Write(((int)reader.ReadByte()).ToString());
                            num10++;
                        Label_0202:
                            if (num10 < num7)
                            {
                                goto Label_01D6;
                            }
                            goto Label_0250;
                        Label_020F:
                            if (num12 != 0)
                            {
                                writer.Write(",");
                            }
                            writer.Write(((int)reader.ReadUInt16()).ToString());
                            num12++;
                        Label_023C:
                            if (num12 < num7)
                            {
                                goto Label_020F;
                            }
                        Label_0250:
                            property.Value = writer.ToString();
                            goto Label_038A;
                        }
                    }
                case 0x2ea:
                    property.Value = PathDataParser.ParseStreamGeometry(reader);
                    goto Label_038A;

                case 0x2eb:
                case 0x2f0:
                    {
                        using (StringWriter writer3 = new StringWriter())
                        {
                            int num18 = reader.ReadInt32();
                            for (int i = 0; i < num18; i++)
                            {
                                if (i != 0)
                                {
                                    writer3.Write(" ");
                                }
                                for (int j = 0; j < 3; j++)
                                {
                                    if (j != 0)
                                    {
                                        writer3.Write(",");
                                    }
                                    writer3.Write(reader.ReadCompressedDouble().ToString());
                                }
                            }
                            property.Value = writer3.ToString();
                            goto Label_038A;
                        }
                    }
                case 0x2ec:
                    {
                        using (StringWriter writer2 = new StringWriter())
                        {
                            int num14 = reader.ReadInt32();
                            for (int k = 0; k < num14; k++)
                            {
                                if (k != 0)
                                {
                                    writer2.Write(" ");
                                }
                                for (int m = 0; m < 2; m++)
                                {
                                    if (m != 0)
                                    {
                                        writer2.Write(",");
                                    }
                                    writer2.Write(reader.ReadCompressedDouble().ToString());
                                }
                            }
                            property.Value = writer2.ToString();
                            goto Label_038A;
                        }
                    }
                case 0x2ed:
                case 750:
                case 0x2ef:
                    goto Label_0384;

                case 0x89:
                    {
                        short num3 = reader.ReadInt16();
                        if (flag)
                        {
                            TypeDeclaration typeDeclaration = this.GetTypeDeclaration(num3);
                            string name = reader.ReadString();
                            property.Value = new PropertyDeclaration(name, typeDeclaration);
                        }
                        else
                        {
                            property.Value = this.GetPropertyDeclaration(num3);
                        }
                        goto Label_038A;
                    }
                case 0x2e:
                    {
                        int num4 = reader.ReadByte();
                        property.Value = (num4 == 1) ? "true" : "false";
                        goto Label_038A;
                    }
            }
        Label_0384:
            throw new NotSupportedException();
        Label_038A:
            element = (Element)this.elementStack.Peek();
            element.Properties.Add(property);
        }

        private void ReadPropertyDictionaryEnd()
        {
            Property property = (Property)this.elementStack.Pop();
            if (property.PropertyType != PropertyType.Dictionary)
            {
                throw new NotSupportedException();
            }
        }

        private void ReadPropertyDictionaryStart(BamlBinaryReader reader)
        {
            short identifier = reader.ReadInt16();
            Property property = new Property(PropertyType.Dictionary);
            property.Value = new ArrayList();
            property.PropertyDeclaration = this.GetPropertyDeclaration(identifier);
            Element element = (Element)this.elementStack.Peek();
            element.Properties.Add(property);
            this.elementStack.Push(property);
        }

        private void ReadPropertyListEnd()
        {
            Property property = (Property)this.elementStack.Pop();
            if (property.PropertyType != PropertyType.List)
            {
                throw new NotSupportedException();
            }
        }

        private void ReadPropertyListStart(BamlBinaryReader reader)
        {
            short identifier = reader.ReadInt16();
            Property property = new Property(PropertyType.List);
            property.Value = new ArrayList();
            property.PropertyDeclaration = this.GetPropertyDeclaration(identifier);
            Element element = (Element)this.elementStack.Peek();
            element.Properties.Add(property);
            this.elementStack.Push(property);
        }

        private void ReadPropertyRecord(BamlBinaryReader reader)
        {
            short identifier = reader.ReadInt16();
            string str = reader.ReadString();
            Property property = new Property(PropertyType.Value);
            property.PropertyDeclaration = this.GetPropertyDeclaration(identifier);
            property.Value = str;
            Element element = (Element)this.elementStack.Peek();
            element.Properties.Add(property);
        }

        private void ReadPropertyTypeReference(BamlBinaryReader reader)
        {
            short identifier = reader.ReadInt16();
            short typeIdentifier = reader.ReadInt16();
            Property property = new Property(PropertyType.Complex);
            property.PropertyDeclaration = this.GetPropertyDeclaration(identifier);
            property.Value = this.CreateTypeExtension(typeIdentifier);
            Element element = (Element)this.elementStack.Peek();
            element.Properties.Add(property);
        }

        private void ReadPropertyWithConverter(BamlBinaryReader reader)
        {
            short identifier = reader.ReadInt16();
            string str = reader.ReadString();
            reader.ReadInt16();
            Property property = new Property(PropertyType.Value);
            property.PropertyDeclaration = this.GetPropertyDeclaration(identifier);
            property.Value = str;
            Element element = (Element)this.elementStack.Peek();
            element.Properties.Add(property);
        }

        private void ReadPropertyWithExtension(BamlBinaryReader reader)
        {
            short identifier = reader.ReadInt16();
            short num2 = reader.ReadInt16();
            short num3 = reader.ReadInt16();
            bool flag = (num2 & 0x4000) == 0x4000;
            bool flag2 = (num2 & 0x2000) == 0x2000;
            num2 = (short)(num2 & 0xfff);
            Property property = new Property(PropertyType.Complex);
            property.PropertyDeclaration = this.GetPropertyDeclaration(identifier);
            short num4 = (short)-(num2 & 0xfff);
            Element element = new Element();
            element.TypeDeclaration = this.GetTypeDeclaration(num4);
            switch (num2)
            {
                case 0x25a:
                    {
                        ResourceName resourceName = (ResourceName)this.GetResourceName(num3);
                        element.Arguments.Add(resourceName);
                        break;
                    }
                case 0x25b:
                case 0xbd:
                    if (flag)
                    {
                        Element element2 = this.CreateTypeExtension(num3);
                        element.Arguments.Add(element2);
                    }
                    else if (flag2)
                    {
                        Element element3 = new Element();
                        element3.TypeDeclaration = new TypeDeclaration("x:Static");
                        ResourceName name = (ResourceName)this.GetResourceName(num3);
                        element3.Arguments.Add(name);
                        element.Arguments.Add(element3);
                    }
                    else
                    {
                        string str = (string)this.stringTable[num3];
                        element.Arguments.Add(str);
                    }
                    break;

                case 0x27a:
                    {
                        PropertyDeclaration propertyDeclaration = this.GetPropertyDeclaration(num3);
                        element.Arguments.Add(propertyDeclaration);
                        break;
                    }
                default:
                    throw new NotSupportedException("Unknown property with extension");
            }
            property.Value = element;
            Element element4 = (Element)this.elementStack.Peek();
            element4.Properties.Add(property);
        }

        private void ReadPropertyWithStaticResourceIdentifier(BamlBinaryReader reader)
        {
            short identifier = reader.ReadInt16();
            short num2 = reader.ReadInt16();
            object staticResource = this.GetStaticResource(num2);
            Element element = new Element();
            element.TypeDeclaration = this.GetTypeDeclaration(-603);
            element.Arguments.Add(staticResource);
            Property property = new Property(PropertyType.Complex);
            property.PropertyDeclaration = this.GetPropertyDeclaration(identifier);
            property.Value = element;
            Element element2 = (Element)this.elementStack.Peek();
            element2.Properties.Add(property);
        }

        private void ReadStaticResourceEnd(BamlBinaryReader reader)
        {
            this.elementStack.Pop();
        }

        private void ReadStaticResourceIdentifier(BamlBinaryReader reader)
        {
            short identifier = reader.ReadInt16();
            object staticResource = this.GetStaticResource(identifier);
            Element element = new Element();
            element.TypeDeclaration = this.GetTypeDeclaration(-603);
            element.Arguments.Add(staticResource);
            this.AddElementToTree(element, reader);
        }

        private void ReadStaticResourceStart(BamlBinaryReader reader)
        {
            short identifier = reader.ReadInt16();
            reader.ReadByte();
            Element element = new Element();
            element.TypeDeclaration = this.GetTypeDeclaration(identifier);
            this.elementStack.Push(element);
            this.staticResourceTable.Add(element);
        }

        private void ReadStringInfo(BamlBinaryReader reader)
        {
            short num = reader.ReadInt16();
            string str = reader.ReadString();
            if ((str != null) && (str.Length == 1))
            {
                str = string.Format("[{0}] {1}", num, str);
            }
            this.stringTable.Add(num, str);
        }

        private void ReadText(BamlBinaryReader reader)
        {
            string str = reader.ReadString();
            this.ReadText(str);
        }

        private void ReadText(string value)
        {
            Element element = (Element)this.elementStack.Peek();
            if (this.constructorParameterTable.Contains(element))
            {
                element.Arguments.Add(value);
            }
            else
            {
                this.AddContent(element, value);
            }
        }

        private void ReadTextWithConverter(BamlBinaryReader reader)
        {
            string str = reader.ReadString();
            reader.ReadInt16();
            this.ReadText(str);
        }

        private void ReadTextWithId(BamlBinaryReader reader)
        {
            short num = reader.ReadInt16();
            string str = this.stringTable[num] as string;
            this.ReadText(str);
        }

        private void ReadTypeInfo(BamlBinaryReader reader)
        {
            short key = reader.ReadInt16();
            short num2 = reader.ReadInt16();
            string name = reader.ReadString();
            num2 = (short)(num2 & 0xfff);
            string assembly = (string)this.assemblyTable[num2];
            TypeDeclaration declaration = null;
            int length = name.LastIndexOf('.');
            if (length != -1)
            {
                string str3 = name.Substring(length + 1);
                string namespaceName = name.Substring(0, length);
                declaration = new TypeDeclaration(str3, namespaceName, assembly);
            }
            else
            {
                declaration = new TypeDeclaration(name, string.Empty, assembly);
            }
            this.typeTable.Add(key, declaration);
        }

        private void ReadXmlnsProperty(BamlBinaryReader reader)
        {
            string name = reader.ReadString();
            string xmlNamespace = reader.ReadString();
            string[] strArray = new string[(uint)reader.ReadInt16()];
            for (int i = 0; i < strArray.Length; i++)
            {
                strArray[i] = (string)this.assemblyTable[reader.ReadInt16()];
            }
            Property property = new Property(PropertyType.Namespace);
            property.PropertyDeclaration = new PropertyDeclaration(name, new TypeDeclaration("XmlNamespace", null, null));
            property.Value = xmlNamespace;
            this.namespaceManager.AddMapping(name, xmlNamespace);
            Element element = (Element)this.elementStack.Peek();
            element.Properties.Add(property);
        }

        public override string ToString()
        {
            using (StringWriter writer = new StringWriter())
            {
                using (IndentationTextWriter writer2 = new IndentationTextWriter(writer))
                {
                    WriteElement(this.rootElement, writer2);
                }
                return writer.ToString();
            }
        }

        private static void WriteComplexElement(Element element, TextWriter writer)
        {
            if (element.TypeDeclaration.Name == "Binding")
            {
                Console.WriteLine();
            }
            writer.Write("{");
            string str = element.TypeDeclaration.ToString();
            if (str.EndsWith("Extension"))
            {
                str = str.Substring(0, str.Length - 9);
            }
            writer.Write(str);
            if ((element.Arguments.Count > 0) || (element.Properties.Count > 0))
            {
                if (element.Arguments.Count > 0)
                {
                    writer.Write(" ");
                    for (int i = 0; i < element.Arguments.Count; i++)
                    {
                        if (i != 0)
                        {
                            writer.Write(", ");
                        }
                        if (element.Arguments[i] is string)
                        {
                            writer.Write((string)element.Arguments[i]);
                        }
                        else if (element.Arguments[i] is TypeDeclaration)
                        {
                            WriteTypeDeclaration((TypeDeclaration)element.Arguments[i], writer);
                        }
                        else if (element.Arguments[i] is PropertyDeclaration)
                        {
                            PropertyDeclaration declaration = (PropertyDeclaration)element.Arguments[i];
                            writer.Write(declaration.Name);
                        }
                        else if (element.Arguments[i] is ResourceName)
                        {
                            ResourceName name = (ResourceName)element.Arguments[i];
                            writer.Write(name.Name);
                        }
                        else
                        {
                            if (!(element.Arguments[i] is Element))
                            {
                                throw new NotSupportedException();
                            }
                            WriteComplexElement((Element)element.Arguments[i], writer);
                        }
                    }
                }
                if (element.Properties.Count > 0)
                {
                    writer.Write(" ");
                    for (int j = 0; j < element.Properties.Count; j++)
                    {
                        if ((j != 0) || (element.Arguments.Count > 0))
                        {
                            writer.Write(", ");
                        }
                        WritePropertyDeclaration(element.Properties[j].PropertyDeclaration, element.TypeDeclaration, writer);
                        writer.Write("=");
                        if (element.Properties[j].Value is string)
                        {
                            writer.Write((string)element.Properties[j].Value);
                        }
                        else if (element.Properties[j].Value is Element)
                        {
                            WriteComplexElement((Element)element.Properties[j].Value, writer);
                        }
                        else
                        {
                            if (!(element.Properties[j].Value is PropertyDeclaration))
                            {
                                throw new NotSupportedException();
                            }
                            Property property = element.Properties[j];
                            writer.Write(property.Value.ToString());
                        }
                    }
                }
            }
            writer.Write("}");
        }

        private static void WriteElement(Element element, IndentationTextWriter writer)
        {
            writer.Write("<");
            WriteTypeDeclaration(element.TypeDeclaration, writer);
            ArrayList list = new ArrayList();
            ArrayList list2 = new ArrayList();
            Property property = null;
            foreach (Property property2 in element.Properties)
            {
                switch (property2.PropertyType)
                {
                    case PropertyType.Value:
                    case PropertyType.Declaration:
                    case PropertyType.Namespace:
                        {
                            list.Add(property2);
                            continue;
                        }
                    case PropertyType.Content:
                        {
                            property = property2;
                            continue;
                        }
                    case PropertyType.List:
                    case PropertyType.Dictionary:
                        {
                            list2.Add(property2);
                            continue;
                        }
                    case PropertyType.Complex:
                        {
                            if (!IsExtension(property2.Value))
                            {
                                break;
                            }
                            list.Add(property2);
                            continue;
                        }
                    default:
                        {
                            continue;
                        }
                }
                list2.Add(property2);
            }
            foreach (Property property3 in list)
            {
                writer.Write(" ");
                WritePropertyDeclaration(property3.PropertyDeclaration, element.TypeDeclaration, writer);
                writer.Write("=");
                writer.Write("\"");
                switch (property3.PropertyType)
                {
                    case PropertyType.Value:
                    case PropertyType.Declaration:
                    case PropertyType.Namespace:
                        writer.Write(property3.Value.ToString());
                        break;

                    case PropertyType.Complex:
                        WriteComplexElement((Element)property3.Value, writer);
                        break;

                    default:
                        throw new NotSupportedException();
                }
                writer.Write("\"");
            }
            if ((property != null) || (list2.Count > 0))
            {
                writer.Write(">");
                if ((list2.Count > 0) || (property.Value is IList))
                {
                    writer.WriteLine();
                }
                writer.Indentation++;
                foreach (Property property4 in list2)
                {
                    writer.Write("<");
                    WriteTypeDeclaration(element.TypeDeclaration, writer);
                    writer.Write(".");
                    WritePropertyDeclaration(property4.PropertyDeclaration, element.TypeDeclaration, writer);
                    writer.Write(">");
                    writer.WriteLine();
                    writer.Indentation++;
                    WritePropertyValue(property4, writer);
                    writer.Indentation--;
                    writer.Write("</");
                    WriteTypeDeclaration(element.TypeDeclaration, writer);
                    writer.Write(".");
                    WritePropertyDeclaration(property4.PropertyDeclaration, element.TypeDeclaration, writer);
                    writer.Write(">");
                    writer.WriteLine();
                }
                if (property != null)
                {
                    WritePropertyValue(property, writer);
                }
                writer.Indentation--;
                writer.Write("</");
                WriteTypeDeclaration(element.TypeDeclaration, writer);
                writer.Write(">");
            }
            else
            {
                writer.Write(" />");
            }
            writer.WriteLine();
        }

        private static void WritePropertyDeclaration(PropertyDeclaration value, TypeDeclaration context, TextWriter writer)
        {
            writer.Write(value.ToString());
        }

        private static void WritePropertyValue(Property property, IndentationTextWriter writer)
        {
            object obj2 = property.Value;
            if (obj2 is IList)
            {
                IList list = (IList)obj2;
                if (((property.PropertyDeclaration != null) && (property.PropertyDeclaration.Name == "Resources")) && ((list.Count == 1) && (list[0] is Element)))
                {
                    Element element = (Element)list[0];
                    if ((((element.TypeDeclaration.Name == "ResourceDictionary") && (element.TypeDeclaration.Namespace == "System.Windows")) && ((element.TypeDeclaration.Assembly == "PresentationFramework") && (element.Arguments.Count == 0))) && ((element.Properties.Count == 1) && (element.Properties[0].PropertyType == PropertyType.Content)))
                    {
                        WritePropertyValue(element.Properties[0], writer);
                        return;
                    }
                }
                foreach (object obj3 in list)
                {
                    if (!(obj3 is string))
                    {
                        if (!(obj3 is Element))
                        {
                            throw new NotSupportedException();
                        }
                        WriteElement((Element)obj3, writer);
                    }
                    else
                    {
                        writer.Write((string)obj3);
                        continue;
                    }
                }
            }
            else if (obj2 is string)
            {
                string str = (string)obj2;
                writer.Write(str);
            }
            else
            {
                if (!(obj2 is Element))
                {
                    throw new NotSupportedException();
                }
                Element element2 = (Element)obj2;
                WriteElement(element2, writer);
            }
        }

        private static void WriteTypeDeclaration(TypeDeclaration value, TextWriter writer)
        {
            writer.Write(value.ToString());
        }

        // Nested Types
        private enum BamlAttributeUsage : short
        {
            Default = 0,
            RuntimeName = 3,
            XmlLang = 1,
            XmlSpace = 2
        }

        private class BamlBinaryReader : BinaryReader
        {
            // Methods
            public BamlBinaryReader(Stream stream)
                : base(stream)
            {
            }

            public virtual double ReadCompressedDouble()
            {
                switch (this.ReadByte())
                {
                    case 1:
                        return 0.0;

                    case 2:
                        return 1.0;

                    case 3:
                        return -1.0;

                    case 4:
                        {
                            double num2 = this.ReadInt32();
                            return (num2 * 1E-06);
                        }
                    case 5:
                        return this.ReadDouble();
                }
                throw new NotSupportedException();
            }

            public int ReadCompressedInt32()
            {
                return base.Read7BitEncodedInt();
            }
        }

        private enum BamlRecordType : byte
        {
            AssemblyInfo = 0x1c,
            AttributeInfo = 0x1f,
            ClrEvent = 0x13,
            Comment = 0x17,
            ConnectionId = 0x2d,
            ConstructorParametersEnd = 0x2b,
            ConstructorParametersStart = 0x2a,
            ConstructorParameterType = 0x2c,
            ContentProperty = 0x2e,
            DefAttribute = 0x19,
            DefAttributeKeyString = 0x26,
            DefAttributeKeyType = 0x27,
            DeferableContentStart = 0x25,
            DefTag = 0x18,
            DocumentEnd = 2,
            DocumentStart = 1,
            ElementEnd = 4,
            ElementStart = 3,
            EndAttributes = 0x1a,
            KeyElementEnd = 0x29,
            KeyElementStart = 40,
            LastRecordType = 0x39,
            LineNumberAndPosition = 0x35,
            LinePosition = 0x36,
            LiteralContent = 15,
            NamedElementStart = 0x2f,
            OptimizedStaticResource = 0x37,
            PIMapping = 0x1b,
            PresentationOptionsAttribute = 0x34,
            ProcessingInstruction = 0x16,
            Property = 5,
            PropertyArrayEnd = 10,
            PropertyArrayStart = 9,
            PropertyComplexEnd = 8,
            PropertyComplexStart = 7,
            PropertyCustom = 6,
            PropertyDictionaryEnd = 14,
            PropertyDictionaryStart = 13,
            PropertyListEnd = 12,
            PropertyListStart = 11,
            PropertyStringReference = 0x21,
            PropertyTypeReference = 0x22,
            PropertyWithConverter = 0x24,
            PropertyWithExtension = 0x23,
            PropertyWithStaticResourceId = 0x38,
            RoutedEvent = 0x12,
            StaticResourceEnd = 0x31,
            StaticResourceId = 50,
            StaticResourceStart = 0x30,
            StringInfo = 0x20,
            Text = 0x10,
            TextWithConverter = 0x11,
            TextWithId = 0x33,
            TypeInfo = 0x1d,
            TypeSerializerInfo = 30,
            Unknown = 0,
            XmlAttribute = 0x15,
            XmlnsProperty = 20
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ClrNamespace
        {
            public string Namespace;
            public string Assembly;
            public ClrNamespace(string clrNamespace, string assembly)
            {
                this.Namespace = clrNamespace;
                this.Assembly = assembly;
            }
        }

        private class Element
        {
            // Fields
            private IList arguments = new ArrayList();
            private BamlTranslator.PropertyCollection properties = new BamlTranslator.PropertyCollection();
            private BamlTranslator.TypeDeclaration typeDeclaration;

            // Methods
            public override string ToString()
            {
                return ("<" + this.TypeDeclaration.ToString() + ">");
            }

            // Properties
            public IList Arguments
            {
                get
                {
                    return this.arguments;
                }
            }

            public BamlTranslator.PropertyCollection Properties
            {
                get
                {
                    return this.properties;
                }
            }

            public BamlTranslator.TypeDeclaration TypeDeclaration
            {
                get
                {
                    return this.typeDeclaration;
                }
                set
                {
                    this.typeDeclaration = value;
                }
            }
        }

        private class IndentationTextWriter : TextWriter
        {
            // Fields
            private int indentation = 0;
            private bool indentationPending = false;
            private string indentText = "    ";
            private TextWriter writer = null;

            // Methods
            public IndentationTextWriter(TextWriter writer)
            {
                this.writer = writer;
            }

            public override void Write(bool value)
            {
                this.WriteIndentation();
                this.writer.Write(value);
            }

            public override void Write(char value)
            {
                this.WriteIndentation();
                this.writer.Write(value);
            }

            public override void Write(double value)
            {
                this.WriteIndentation();
                this.writer.Write(value);
            }

            public override void Write(int value)
            {
                this.WriteIndentation();
                this.writer.Write(value);
            }

            public override void Write(char[] buffer)
            {
                this.WriteIndentation();
                this.writer.Write(buffer);
            }

            public override void Write(long value)
            {
                this.WriteIndentation();
                this.writer.Write(value);
            }

            public override void Write(object value)
            {
                this.WriteIndentation();
                this.writer.Write(value);
            }

            public override void Write(float value)
            {
                this.WriteIndentation();
                this.writer.Write(value);
            }

            public override void Write(string s)
            {
                this.WriteIndentation();
                this.writer.Write(s);
            }

            public override void Write(string format, params object[] arg)
            {
                this.WriteIndentation();
                this.writer.Write(format, arg);
            }

            public override void Write(string format, object arg0)
            {
                this.WriteIndentation();
                this.writer.Write(format, arg0);
            }

            public override void Write(char[] buffer, int index, int count)
            {
                this.WriteIndentation();
                this.writer.Write(buffer, index, count);
            }

            public override void Write(string format, object arg0, object arg1)
            {
                this.WriteIndentation();
                this.writer.Write(format, arg0, arg1);
            }

            private void WriteIndentation()
            {
                if (this.indentationPending)
                {
                    for (int i = 0; i < this.indentation; i++)
                    {
                        this.writer.Write(this.indentText);
                    }
                    this.indentationPending = false;
                }
            }

            public override void WriteLine()
            {
                this.WriteIndentation();
                this.writer.WriteLine();
                this.indentationPending = true;
            }

            public override void WriteLine(bool value)
            {
                this.WriteIndentation();
                this.writer.WriteLine(value);
                this.indentationPending = true;
            }

            public override void WriteLine(char value)
            {
                this.WriteIndentation();
                this.writer.WriteLine(value);
                this.indentationPending = true;
            }

            public override void WriteLine(double value)
            {
                this.WriteIndentation();
                this.writer.WriteLine(value);
                this.indentationPending = true;
            }

            public override void WriteLine(char[] buffer)
            {
                this.WriteIndentation();
                this.writer.WriteLine(buffer);
                this.indentationPending = true;
            }

            public override void WriteLine(int value)
            {
                this.WriteIndentation();
                this.writer.WriteLine(value);
                this.indentationPending = true;
            }

            public override void WriteLine(long value)
            {
                this.WriteIndentation();
                this.writer.WriteLine(value);
                this.indentationPending = true;
            }

            public override void WriteLine(object value)
            {
                this.WriteIndentation();
                this.writer.WriteLine(value);
                this.indentationPending = true;
            }

            public override void WriteLine(float value)
            {
                this.WriteIndentation();
                this.writer.WriteLine(value);
                this.indentationPending = true;
            }

            public override void WriteLine(string s)
            {
                this.WriteIndentation();
                this.writer.WriteLine(s);
                this.indentationPending = true;
            }

            public override void WriteLine(string format, object arg0)
            {
                this.WriteIndentation();
                this.writer.WriteLine(format, arg0);
                this.indentationPending = true;
            }

            public override void WriteLine(string format, params object[] arg)
            {
                this.WriteIndentation();
                this.writer.WriteLine(format, arg);
                this.indentationPending = true;
            }

            public override void WriteLine(string format, object arg0, object arg1)
            {
                this.WriteIndentation();
                this.writer.WriteLine(format, arg0, arg1);
                this.indentationPending = true;
            }

            public override void WriteLine(char[] buffer, int index, int count)
            {
                this.WriteIndentation();
                this.writer.WriteLine(buffer, index, count);
                this.indentationPending = true;
            }

            // Properties
            public override Encoding Encoding
            {
                get
                {
                    return this.writer.Encoding;
                }
            }

            public int Indentation
            {
                get
                {
                    return this.indentation;
                }
                set
                {
                    this.indentation = value;
                }
            }
        }

        internal class KnownColors
        {
            // Fields
            private static readonly Hashtable colorTable = new Hashtable();

            // Methods
            static KnownColors()
            {
                colorTable[0xfff0f8ff] = "AliceBlue";
                colorTable[0xfffaebd7] = "AntiqueWhite";
                colorTable[0xff00ffff] = "Aqua";
                colorTable[0xff7fffd4] = "Aquamarine";
                colorTable[0xfff0ffff] = "Azure";
                colorTable[0xfff5f5dc] = "Beige";
                colorTable[0xffffe4c4] = "Bisque";
                colorTable[0xff000000] = "Black";
                colorTable[0xffffebcd] = "BlanchedAlmond";
                colorTable[0xff0000ff] = "Blue";
                colorTable[0xff8a2be2] = "BlueViolet";
                colorTable[0xffa52a2a] = "Brown";
                colorTable[0xffdeb887] = "BurlyWood";
                colorTable[0xff5f9ea0] = "CadetBlue";
                colorTable[0xff7fff00] = "Chartreuse";
                colorTable[0xffd2691e] = "Chocolate";
                colorTable[0xffff7f50] = "Coral";
                colorTable[0xff6495ed] = "CornflowerBlue";
                colorTable[0xfffff8dc] = "Cornsilk";
                colorTable[0xffdc143c] = "Crimson";
                colorTable[0xff00ffff] = "Cyan";
                colorTable[0xff00008b] = "DarkBlue";
                colorTable[0xff008b8b] = "DarkCyan";
                colorTable[0xffb8860b] = "DarkGoldenrod";
                colorTable[0xffa9a9a9] = "DarkGray";
                colorTable[0xff006400] = "DarkGreen";
                colorTable[0xffbdb76b] = "DarkKhaki";
                colorTable[0xff8b008b] = "DarkMagenta";
                colorTable[0xff556b2f] = "DarkOliveGreen";
                colorTable[0xffff8c00] = "DarkOrange";
                colorTable[0xff9932cc] = "DarkOrchid";
                colorTable[0xff8b0000] = "DarkRed";
                colorTable[0xffe9967a] = "DarkSalmon";
                colorTable[0xff8fbc8f] = "DarkSeaGreen";
                colorTable[0xff483d8b] = "DarkSlateBlue";
                colorTable[0xff2f4f4f] = "DarkSlateGray";
                colorTable[0xff00ced1] = "DarkTurquoise";
                colorTable[0xff9400d3] = "DarkViolet";
                colorTable[0xffff1493] = "DeepPink";
                colorTable[0xff00bfff] = "DeepSkyBlue";
                colorTable[0xff696969] = "DimGray";
                colorTable[0xff1e90ff] = "DodgerBlue";
                colorTable[0xffb22222] = "Firebrick";
                colorTable[0xfffffaf0] = "FloralWhite";
                colorTable[0xff228b22] = "ForestGreen";
                colorTable[0xffff00ff] = "Fuchsia";
                colorTable[0xffdcdcdc] = "Gainsboro";
                colorTable[0xfff8f8ff] = "GhostWhite";
                colorTable[0xffffd700] = "Gold";
                colorTable[0xffdaa520] = "Goldenrod";
                colorTable[0xff808080] = "Gray";
                colorTable[0xff008000] = "Green";
                colorTable[0xffadff2f] = "GreenYellow";
                colorTable[0xfff0fff0] = "Honeydew";
                colorTable[0xffff69b4] = "HotPink";
                colorTable[0xffcd5c5c] = "IndianRed";
                colorTable[0xff4b0082] = "Indigo";
                colorTable[0xfffffff0] = "Ivory";
                colorTable[0xfff0e68c] = "Khaki";
                colorTable[0xffe6e6fa] = "Lavender";
                colorTable[0xfffff0f5] = "LavenderBlush";
                colorTable[0xff7cfc00] = "LawnGreen";
                colorTable[0xfffffacd] = "LemonChiffon";
                colorTable[0xffadd8e6] = "LightBlue";
                colorTable[0xfff08080] = "LightCoral";
                colorTable[0xffe0ffff] = "LightCyan";
                colorTable[0xfffafad2] = "LightGoldenrodYellow";
                colorTable[0xffd3d3d3] = "LightGray";
                colorTable[0xff90ee90] = "LightGreen";
                colorTable[0xffffb6c1] = "LightPink";
                colorTable[0xffffa07a] = "LightSalmon";
                colorTable[0xff20b2aa] = "LightSeaGreen";
                colorTable[0xff87cefa] = "LightSkyBlue";
                colorTable[0xff778899] = "LightSlateGray";
                colorTable[0xffb0c4de] = "LightSteelBlue";
                colorTable[0xffffffe0] = "LightYellow";
                colorTable[0xff00ff00] = "Lime";
                colorTable[0xff32cd32] = "LimeGreen";
                colorTable[0xfffaf0e6] = "Linen";
                colorTable[0xffff00ff] = "Magenta";
                colorTable[0xff800000] = "Maroon";
                colorTable[0xff66cdaa] = "MediumAquamarine";
                colorTable[0xff0000cd] = "MediumBlue";
                colorTable[0xffba55d3] = "MediumOrchid";
                colorTable[0xff9370db] = "MediumPurple";
                colorTable[0xff3cb371] = "MediumSeaGreen";
                colorTable[0xff7b68ee] = "MediumSlateBlue";
                colorTable[0xff00fa9a] = "MediumSpringGreen";
                colorTable[0xff48d1cc] = "MediumTurquoise";
                colorTable[0xffc71585] = "MediumVioletRed";
                colorTable[0xff191970] = "MidnightBlue";
                colorTable[0xfff5fffa] = "MintCream";
                colorTable[0xffffe4e1] = "MistyRose";
                colorTable[0xffffe4b5] = "Moccasin";
                colorTable[0xffffdead] = "NavajoWhite";
                colorTable[0xff000080] = "Navy";
                colorTable[0xfffdf5e6] = "OldLace";
                colorTable[0xff808000] = "Olive";
                colorTable[0xff6b8e23] = "OliveDrab";
                colorTable[0xffffa500] = "Orange";
                colorTable[0xffff4500] = "OrangeRed";
                colorTable[0xffda70d6] = "Orchid";
                colorTable[0xffeee8aa] = "PaleGoldenrod";
                colorTable[0xff98fb98] = "PaleGreen";
                colorTable[0xffafeeee] = "PaleTurquoise";
                colorTable[0xffdb7093] = "PaleVioletRed";
                colorTable[0xffffefd5] = "PapayaWhip";
                colorTable[0xffffdab9] = "PeachPuff";
                colorTable[0xffcd853f] = "Peru";
                colorTable[0xffffc0cb] = "Pink";
                colorTable[0xffdda0dd] = "Plum";
                colorTable[0xffb0e0e6] = "PowderBlue";
                colorTable[0xff800080] = "Purple";
                colorTable[0xffff0000] = "Red";
                colorTable[0xffbc8f8f] = "RosyBrown";
                colorTable[0xff4169e1] = "RoyalBlue";
                colorTable[0xff8b4513] = "SaddleBrown";
                colorTable[0xfffa8072] = "Salmon";
                colorTable[0xfff4a460] = "SandyBrown";
                colorTable[0xff2e8b57] = "SeaGreen";
                colorTable[0xfffff5ee] = "SeaShell";
                colorTable[0xffa0522d] = "Sienna";
                colorTable[0xffc0c0c0] = "Silver";
                colorTable[0xff87ceeb] = "SkyBlue";
                colorTable[0xff6a5acd] = "SlateBlue";
                colorTable[0xff708090] = "SlateGray";
                colorTable[0xfffffafa] = "Snow";
                colorTable[0xff00ff7f] = "SpringGreen";
                colorTable[0xff4682b4] = "SteelBlue";
                colorTable[0xffd2b48c] = "Tan";
                colorTable[0xff008080] = "Teal";
                colorTable[0xffd8bfd8] = "Thistle";
                colorTable[0xffff6347] = "Tomato";
                colorTable[0xffffff] = "Transparent";
                colorTable[0xff40e0d0] = "Turquoise";
                colorTable[0xffee82ee] = "Violet";
                colorTable[0xfff5deb3] = "Wheat";
                colorTable[uint.MaxValue] = "White";
                colorTable[0xfff5f5f5] = "WhiteSmoke";
                colorTable[0xffffff00] = "Yellow";
                colorTable[0xff9acd32] = "YellowGreen";
            }

            internal static string KnownColorFromUInt(uint argb)
            {
                return (string)colorTable[argb];
            }
        }

        private class NamespaceManager
        {
            // Fields
            private Stack mappingStack = new Stack();
            private HybridDictionary reverseTable = new HybridDictionary();
            private HybridDictionary table = new HybridDictionary();

            // Methods
            internal NamespaceManager()
            {
            }

            internal void AddMapping(string prefix, string xmlNamespace)
            {
                ElementEntry entry = (ElementEntry)this.mappingStack.Peek();
                entry.MappingTable[xmlNamespace] = prefix;
            }

            public void AddNamespaceMapping(string xmlNamespace, string clrNamespace, string assembly)
            {
                BamlTranslator.ClrNamespace namespace2 = new BamlTranslator.ClrNamespace(clrNamespace, assembly);
                this.table[xmlNamespace] = namespace2;
                this.reverseTable[namespace2] = xmlNamespace;
            }

            internal string GetPrefix(string xmlNamespace)
            {
                foreach (ElementEntry entry in this.mappingStack)
                {
                    if (entry.HasMappingTable && entry.MappingTable.Contains(xmlNamespace))
                    {
                        return (string)entry.MappingTable[xmlNamespace];
                    }
                }
                return null;
            }

            internal string GetXmlNamespace(BamlTranslator.TypeDeclaration type)
            {
                BamlTranslator.ClrNamespace namespace2 = new BamlTranslator.ClrNamespace(type.Namespace, type.Assembly);
                return (string)this.reverseTable[namespace2];
            }

            internal void OnElementEnd()
            {
                this.mappingStack.Pop();
            }

            internal void OnElementStart()
            {
                ElementEntry entry = new ElementEntry();
                this.mappingStack.Push(entry);
            }

            // Nested Types
            private class ElementEntry
            {
                // Fields
                private HybridDictionary mappingTable;

                // Properties
                internal bool HasMappingTable
                {
                    get
                    {
                        return (null != this.mappingTable);
                    }
                }

                internal HybridDictionary MappingTable
                {
                    get
                    {
                        if (this.mappingTable == null)
                        {
                            this.mappingTable = new HybridDictionary();
                        }
                        return this.mappingTable;
                    }
                }
            }
        }

        private class PathDataParser
        {
            // Methods
            private static void AddPathCommand(char commandChar, ref char lastCommandChar, StringBuilder sb)
            {
                if (commandChar != lastCommandChar)
                {
                    lastCommandChar = commandChar;
                    sb.Append(commandChar);
                    sb.Append(' ');
                }
            }

            private static void AddPathPoint(BamlTranslator.BamlBinaryReader reader, StringBuilder sb)
            {
                sb.AppendFormat("{0},{1} ", reader.ReadCompressedDouble(), reader.ReadCompressedDouble());
            }

            private static void AddPathPoint(BamlTranslator.BamlBinaryReader reader, StringBuilder sb, bool flag1, bool flag2)
            {
                sb.AppendFormat("{0},{1} ", ReadPathDouble(reader, flag1), ReadPathDouble(reader, flag2));
            }

            internal static object ParseStreamGeometry(BamlTranslator.BamlBinaryReader reader)
            {
                byte num;
                StringBuilder sb = new StringBuilder();
                bool flag = false;
                char lastCommandChar = '\0';
            Label_000A:
                num = reader.ReadByte();
                bool flag2 = (num & 0x10) == 0x10;
                bool flag3 = (num & 0x20) == 0x20;
                bool flag4 = (num & 0x40) == 0x40;
                bool flag5 = (num & 0x80) == 0x80;
                switch ((num & 15))
                {
                    case 0:
                        flag = flag3;
                        AddPathCommand('M', ref lastCommandChar, sb);
                        AddPathPoint(reader, sb, flag4, flag5);
                        goto Label_000A;

                    case 1:
                        AddPathCommand('L', ref lastCommandChar, sb);
                        AddPathPoint(reader, sb, flag4, flag5);
                        goto Label_000A;

                    case 2:
                        AddPathCommand('Q', ref lastCommandChar, sb);
                        AddPathPoint(reader, sb, flag4, flag5);
                        AddPathPoint(reader, sb);
                        goto Label_000A;

                    case 3:
                        AddPathCommand('C', ref lastCommandChar, sb);
                        AddPathPoint(reader, sb, flag4, flag5);
                        AddPathPoint(reader, sb);
                        AddPathPoint(reader, sb);
                        goto Label_000A;

                    case 4:
                        {
                            AddPathCommand('L', ref lastCommandChar, sb);
                            int num2 = reader.ReadInt32();
                            for (int i = 0; i < num2; i++)
                            {
                                AddPathPoint(reader, sb);
                            }
                            goto Label_000A;
                        }
                    case 5:
                        {
                            AddPathCommand('Q', ref lastCommandChar, sb);
                            int num4 = reader.ReadInt32();
                            for (int j = 0; j < num4; j++)
                            {
                                AddPathPoint(reader, sb);
                            }
                            goto Label_000A;
                        }
                    case 6:
                        {
                            AddPathCommand('C', ref lastCommandChar, sb);
                            int num6 = reader.ReadInt32();
                            for (int k = 0; k < num6; k++)
                            {
                                AddPathPoint(reader, sb);
                            }
                            goto Label_000A;
                        }
                    case 7:
                        {
                            double num8 = ReadPathDouble(reader, flag4);
                            double num9 = ReadPathDouble(reader, flag5);
                            byte num10 = reader.ReadByte();
                            bool flag6 = (num10 & 15) != 0;
                            bool flag7 = (num10 & 240) != 0;
                            double num11 = reader.ReadCompressedDouble();
                            double num12 = reader.ReadCompressedDouble();
                            double num13 = reader.ReadCompressedDouble();
                            sb.AppendFormat("A {0},{1} {2} {3} {4} {5},{6} ", new object[] { num11, num12, num13, flag6 ? 1 : 0, flag7 ? 1 : 0, num8, num9 });
                            lastCommandChar = 'A';
                            goto Label_000A;
                        }
                    case 8:
                        if (!flag)
                        {
                            if (sb.Length > 0)
                            {
                                sb.Remove(sb.Length - 1, 0);
                            }
                            break;
                        }
                        sb.Append("Z");
                        break;

                    case 9:
                        sb.Insert(0, flag2 ? "F1 " : "F0 ");
                        lastCommandChar = 'F';
                        goto Label_000A;

                    default:
                        throw new InvalidOperationException();
                }
                return sb.ToString();
            }

            private static double ReadPathDouble(BamlTranslator.BamlBinaryReader reader, bool isInt)
            {
                if (isInt)
                {
                    return (reader.ReadInt32() * 1E-06);
                }
                return reader.ReadCompressedDouble();
            }
        }

        private class Property
        {
            // Fields
            private BamlTranslator.PropertyDeclaration propertyDeclaration;
            private BamlTranslator.PropertyType propertyType;
            private object value;

            // Methods
            public Property(BamlTranslator.PropertyType propertyType)
            {
                this.propertyType = propertyType;
            }

            public override string ToString()
            {
                return this.PropertyDeclaration.Name;
            }

            // Properties
            public BamlTranslator.PropertyDeclaration PropertyDeclaration
            {
                get
                {
                    return this.propertyDeclaration;
                }
                set
                {
                    this.propertyDeclaration = value;
                }
            }

            public BamlTranslator.PropertyType PropertyType
            {
                get
                {
                    return this.propertyType;
                }
            }

            public object Value
            {
                get
                {
                    return this.value;
                }
                set
                {
                    this.value = value;
                }
            }
        }

        private class PropertyCollection : IEnumerable
        {
            // Fields
            private ArrayList list = new ArrayList();

            // Methods
            public void Add(BamlTranslator.Property value)
            {
                this.list.Add(value);
            }

            public IEnumerator GetEnumerator()
            {
                return this.list.GetEnumerator();
            }

            public void Remove(BamlTranslator.Property value)
            {
                this.list.Remove(value);
            }

            // Properties
            public int Count
            {
                get
                {
                    return this.list.Count;
                }
            }

            public BamlTranslator.Property this[int index]
            {
                get
                {
                    return (BamlTranslator.Property)this.list[index];
                }
            }
        }

        private class PropertyDeclaration
        {
            // Fields
            private BamlTranslator.TypeDeclaration declaringType;
            private string name;

            // Methods
            public PropertyDeclaration(string name)
            {
                this.name = name;
                this.declaringType = null;
            }

            public PropertyDeclaration(string name, BamlTranslator.TypeDeclaration declaringType)
            {
                this.name = name;
                this.declaringType = declaringType;
            }

            public override string ToString()
            {
                if (((this.DeclaringType == null) || !(this.DeclaringType.Name == "XmlNamespace")) || ((this.DeclaringType.Namespace != null) || (this.DeclaringType.Assembly != null)))
                {
                    return this.Name;
                }
                if ((this.Name != null) && (this.Name.Length != 0))
                {
                    return ("xmlns:" + this.Name);
                }
                return "xmlns";
            }

            // Properties
            public BamlTranslator.TypeDeclaration DeclaringType
            {
                get
                {
                    return this.declaringType;
                }
            }

            public string Name
            {
                get
                {
                    return this.name;
                }
            }
        }

        private enum PropertyType
        {
            Value,
            Content,
            Declaration,
            List,
            Dictionary,
            Complex,
            Namespace
        }

        private class ResourceName
        {
            // Fields
            private string name;

            // Methods
            public ResourceName(string name)
            {
                this.name = name;
            }

            public override string ToString()
            {
                return this.Name;
            }

            // Properties
            public string Name
            {
                get
                {
                    return this.name;
                }
            }
        }

        private class TypeDeclaration
        {
            // Fields
            private string assembly;
            private string name;
            private string namespaceName;
            private string xmlPrefix;

            // Methods
            public TypeDeclaration(string name)
            {
                this.name = name;
                this.namespaceName = string.Empty;
                this.assembly = string.Empty;
            }

            public TypeDeclaration(string name, string namespaceName, string assembly)
            {
                this.name = name;
                this.namespaceName = namespaceName;
                this.assembly = assembly;
            }

            public BamlTranslator.TypeDeclaration Copy(string xmlPrefix)
            {
                BamlTranslator.TypeDeclaration declaration = new BamlTranslator.TypeDeclaration(this.name, this.namespaceName, this.assembly);
                declaration.xmlPrefix = xmlPrefix;
                return declaration;
            }

            public override string ToString()
            {
                if ((this.xmlPrefix != null) && (this.xmlPrefix.Length != 0))
                {
                    return (this.xmlPrefix + ":" + this.Name);
                }
                return this.Name;
            }

            // Properties
            public string Assembly
            {
                get
                {
                    return this.assembly;
                }
            }

            public string Name
            {
                get
                {
                    return this.name;
                }
            }

            public string Namespace
            {
                get
                {
                    return this.namespaceName;
                }
            }

            public string XmlPrefix
            {
                get
                {
                    return this.xmlPrefix;
                }
            }
        }
    }


}
