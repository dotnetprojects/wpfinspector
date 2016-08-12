using System;
using System.ServiceModel;
using System.Threading;
using ChristianMoser.WpfInspector.Services;

namespace ChristianMoser.WpfInspector.Process32Helper
{
    public static class Application
    {
        /// <summary>
        /// Static application entry point
        /// </summary>
        /// <param name="args">The args.</param>
        public static void Main( string[] args)
        {
            // Start a serivce host to query 32-bit processes
            try
            {
                var host = new ServiceHost(typeof(ProcessService));
                host.AddServiceEndpoint(typeof(IProcessService), new NetNamedPipeBinding(), Process32Service.ProcessServiceAddress);
                host.Open();

                // Wait forever
                var evt = new AutoResetEvent(false);
                evt.WaitOne();
            }
            catch (AddressAlreadyInUseException)
            {
            }
            
        }
    }
}
