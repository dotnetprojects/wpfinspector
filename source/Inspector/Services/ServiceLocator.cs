using System;
using System.Collections.Generic;

namespace ChristianMoser.WpfInspector.Services
{
    public class ServiceLocator
    {
        private static readonly Dictionary<Type, object> Instances = new Dictionary<Type, object>();

        public static void RegisterInstance<TInterface>(object instance)
        {
            var type = typeof(TInterface);
            if (!Instances.ContainsKey(type))
            {
                Instances.Add(type, instance);
            }
            else
            {
                Instances[type] = instance;
            }
        }

        public static T Resolve<T>()
            where T : class, new()
        {
            var type = typeof(T);
            if (!Instances.ContainsKey(type))
            {
                var instance = new T();
                Instances.Add(type, instance);
            }
            return (T)Instances[type];
        }

        public static void ShutDown()
        {
            foreach (var instance in Instances.Values)
            {
                var disposable = instance as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
