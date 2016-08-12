using System;
using System.Linq;
using System.Reflection;

namespace ChristianMoser.WpfInspector.Utilities
{
    public static class AssemblyHelper
    {
        public static Assembly FindAssemblyByPartialName(string partialName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name.ToLower().Contains(partialName.ToLower())).FirstOrDefault();
        }
    }
}
