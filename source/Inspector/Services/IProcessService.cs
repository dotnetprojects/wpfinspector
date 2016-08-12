using System.Collections.Generic;
using System.ServiceModel;

namespace ChristianMoser.WpfInspector.Services
{
    [ServiceContract]
    public interface IProcessService
    {
        /// <summary>
        /// Gets the process infos.
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<ManagedApplicationInfo> GetProcessInfos();

        [OperationContract]
        string Inspect(ManagedApplicationInfo applicationInfo);
    }
}
