using System.Collections.Generic;
using ChristianMoser.WpfInspector.Services;

namespace ChristianMoser.WpfInspector.Process32Helper
{
    public class ProcessService : IProcessService
    {
        #region IProcessService Members

        public List<ManagedApplicationInfo> GetProcessInfos()
        {
            var applicationsService = ServiceLocator.Resolve<ManagedApplicationsService>();
            var processes = new List<ManagedApplicationInfo>();
            processes.AddRange(applicationsService.GetManagedApplications());
            return processes;
        }

        public string Inspect(ManagedApplicationInfo applicationInfo)
        {
            return ServiceLocator.Resolve<InspectionService>().Inspect(applicationInfo);
        }

        #endregion
    }
}
