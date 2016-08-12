using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;

namespace ChristianMoser.WpfInspector.Services
{
    public class Process32Service : IDisposable
    {
        #region Private Members

        private readonly Process _process32Helper;

        #endregion

        #region Constants

        public const string ProcessServiceAddress = "net.pipe://localhost/ProcessService";
        public const string Process32ExeName = "Process32Helper.exe";

        #endregion

        public string Inspect(ManagedApplicationInfo applicationInfo)
        {
            var binding = new NetNamedPipeBinding();
            var channelFactory = new ChannelFactory<IProcessService>(binding, ProcessServiceAddress);
            IProcessService processService = channelFactory.CreateChannel();
            return processService.Inspect(applicationInfo);
        }

        public IEnumerable<ManagedApplicationInfo> GetManagedApplications()
        {
            // Client
            try
            {
                var binding = new NetNamedPipeBinding();
                var channelFactory = new ChannelFactory<IProcessService>(binding, ProcessServiceAddress);
                IProcessService processService = channelFactory.CreateChannel();
                List<ManagedApplicationInfo> processInfos = processService.GetProcessInfos();
                return processInfos;
            }
            catch (EndpointNotFoundException)
            {
                return new List<ManagedApplicationInfo>();
            }   
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_process32Helper != null && !_process32Helper.HasExited)
            {
                _process32Helper.Kill();
            }
        }

        #endregion

        public Process Start32BitProcessHelper()
        {
            try
            {
                var processes = Process.GetProcessesByName(Process32ExeName);
                if (processes.Length == 0)
                {
                    return Process.Start(Process32ExeName);
                }
                return processes[0];
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
