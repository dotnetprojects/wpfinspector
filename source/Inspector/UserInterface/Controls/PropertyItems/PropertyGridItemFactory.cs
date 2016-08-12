using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Markup.Primitives;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Automation;
using System.Collections;

namespace ChristianMoser.WpfInspector.UserInterface.Controls.PropertyItems
{
    public static class PropertyGridItemFactory
    {
        public static List<IPropertyGridItem> GetMethodItems(object instance)
        {
            var methodItems = new List<IPropertyGridItem>();
            if (instance != null)
            {
                foreach (var methodInfo in instance.GetType().GetMethods())
                {
                    if( methodInfo.Name.StartsWith("get_") || methodInfo.Name.StartsWith("set_") ||
                        methodInfo.Name.StartsWith("add_") || methodInfo.Name.StartsWith("remove_") ||
                        methodInfo.ReflectedType == typeof(object) )
                    {
                        continue;
                    }
                    methodItems.Add(new MethodItem(methodInfo, instance));
                }
            }
             
            return methodItems;
        }

        public static List<IPropertyGridItem> GetFieldItems(object instance)
        {
            var fieldItems = new List<IPropertyGridItem>();
            if (instance != null)
            {
                foreach (var fieldInfo in instance.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    fieldItems.Add(new FieldItem(fieldInfo, instance));
                }
            }

            return fieldItems;
        }

        public static List<IPropertyGridItem> GetPropertyItems(object instance)
        {
            var propertyItems = new List<IPropertyGridItem>();
            if (instance == null)
            {
                return propertyItems;
            }

            var properties = TypeDescriptor.GetProperties(instance.GetType(),
                                  new Attribute[] { new PropertyFilterAttribute(PropertyFilterOptions.All) });

            // Get all properties of the type
            propertyItems.AddRange(
                properties.Cast<PropertyDescriptor>().Where(p => p.IsBrowsable && p.Name != "GenericParameterAttributes").Select(property => CreatePropertyItem(property, instance)));

            if (instance is FrameworkElement)
            {
                propertyItems.Add(CreatePropertyItem(DependencyPropertyDescriptor.FromProperty(AutomationProperties.NameProperty, instance.GetType()), instance));
                propertyItems.Add(CreatePropertyItem(DependencyPropertyDescriptor.FromProperty(AutomationProperties.HelpTextProperty, instance.GetType()), instance));
            }

            // Get all set attached dependency properties
            var markupObject = MarkupWriter.GetMarkupObjectFor(instance);
            foreach (MarkupProperty mp in markupObject.Properties)
            {
                if (mp.IsAttached && mp.DependencyProperty != null)
                {
                    var dpd = DependencyPropertyDescriptor.FromProperty(mp.DependencyProperty, instance.GetType());
                    if (dpd != null)
                    {
                        propertyItems.Add(CreatePropertyItem(dpd, instance));
                    }
                }
            }

            return propertyItems;
        }

        public static IPropertyGridItem CreatePropertyItem(PropertyDescriptor property, object instance)
        {
            try
            {
                var type = property.PropertyType;
                if (type == typeof(bool))
                {
                    return new BooleanPropertyItem(property, instance);
                }
                if (type.IsEnum)
                {
                    return new EnumPropertyItem(property, instance);
                }
                Type valuesType = GetSpecialEnumType(type);
                if (valuesType != null)
                {
                    return new EnumPropertyItem(property, instance, valuesType);
                }
                if (type == typeof(FontFamily))
                {
                    return new EnumPropertyItem(property, instance, Fonts.SystemFontFamilies);
                }
                if (type == typeof(Brush))
                {
                    return new BrushPropertyItem(property, instance);
                }
                if (type == typeof(Style))
                {
                    return new StylePropertyItem(property, instance);
                }
                if (type == typeof(Thickness))
                {
                    return new ThicknessPropertyItem(property, instance);
                }
                if (type == typeof(CornerRadius))
                {
                    return new CornerRadiusPropertyItem(property, instance);
                }
                if (type == typeof(GridLength))
                {
                    return new GridLengthPropertyItem(property, instance);
                }
                if (typeof(ImageSource).IsAssignableFrom(type))
                {
                    return new ImagePropertyItem(property, instance);
                }
                //if (type.GetInterface(typeof(ICommand).FullName) != null)
                //{
                //    return new CommandPropertyItem(property, instance);
                //}
                if (type.GetInterface(typeof(IEnumerable).FullName) != null && type != typeof(string))
                {
                    return new ListPropertyItem(property, instance);
                }
                if ((type.IsClass || type.IsInterface) && type != typeof(string))
                {
                    return new CompositePropertyItem(property, instance);
                }
                return new PropertyItem(property, instance);
            }
            catch (Exception)
            {
                return new PropertyItem(property, instance);
            }
        }

        private static Type GetSpecialEnumType(Type type)
        {
            if (type == typeof(FontWeight))
                return typeof(FontWeights);
            if (type == typeof(FontStyle))
                return typeof(FontStyles);
            if (type == typeof(FontStyle))
                return typeof(FontStyles);
            if (type == typeof(FontStretch))
                return typeof(FontStretches);
            if (type == typeof(Cursor))
                return typeof(Cursors);
            return null;
        }


    }
}
