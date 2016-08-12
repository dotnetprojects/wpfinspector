using System;
using System.Net;
using System.IO;
using System.ComponentModel;
using ChristianMoser.WpfInspector.Utilities;
using System.Reflection;

namespace ChristianMoser.WpfInspector.Services
{
    public enum UpdateState
    {
        Querying,
        Available,
        Error
    }

    public class UpdateInformation : INotifyPropertyChanged
    {
        private bool _isLatestVersion;
        private string _releaseNotes;
        private Version _installedVersion;
        private Version _recentVersion;
        private UpdateState _state;

        /// <summary>
        /// Gets if the installed version is the latest
        /// </summary>
        public bool IsLatestVersion
        {
            get { return _isLatestVersion; }
            set { PropertyChanged.ChangeAndNotify(ref _isLatestVersion, value, this, "IsLatestVersion"); }
        }

        /// <summary>
        /// Gets the release notes of the most recent version
        /// </summary>
        public string ReleaseNotes
        {
            get { return _releaseNotes; }
            set { PropertyChanged.ChangeAndNotify(ref _releaseNotes, value, this, "ReleaseNotes"); }
        }

        /// <summary>
        /// Gets the state of the query
        /// </summary>
        public UpdateState State
        {
            get { return _state; }
            set { PropertyChanged.ChangeAndNotify(ref _state, value, this, "State"); }
        }

        /// <summary>
        /// Gets the installed version
        /// </summary>
        public Version InstalledVersion
        {
            get { return _installedVersion; }
            set { PropertyChanged.ChangeAndNotify(ref _installedVersion, value, this, "InstalledVersion"); }
        }

        /// <summary>
        /// Gets the most recent version
        /// </summary>
        public Version RecentVersion
        {
            get { return _recentVersion; }
            set { PropertyChanged.ChangeAndNotify(ref _recentVersion, value, this, "RecentVersion"); }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    /// <summary>
    /// Service to check for software updates
    /// </summary>
    public class UpdateCheckService
    {
        #region Private Members

        private UpdateInformation _updateInformation;

        #endregion

        /// <summary>
        /// Gets the update information.
        /// </summary>
        /// <value>The update information.</value>
        public UpdateInformation UpdateInformation
        {
            get
            {
                if( _updateInformation == null )
                {
                    _updateInformation = new UpdateInformation();
                    var bgw = new BackgroundWorker();
                    bgw.DoWork += (s, e) => CheckForUpdates();
                    bgw.RunWorkerAsync();
                }
                return _updateInformation;
            }
        }

        #region Private Members

        public void CheckForUpdates()
        {
            try
            {
                _updateInformation.State = UpdateState.Querying;
                var webRequest = WebRequest.Create("http://www.wpftutorial.net/inspectorversion.txt");
                webRequest.Proxy = WebRequest.GetSystemWebProxy();
                WebResponse response = webRequest.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        _updateInformation.RecentVersion = new Version(reader.ReadLine());
                        _updateInformation.ReleaseNotes = reader.ReadToEnd();
                        _updateInformation.InstalledVersion = Assembly.GetExecutingAssembly().GetName().Version;
                        _updateInformation.IsLatestVersion = _updateInformation.RecentVersion <=
                                                             _updateInformation.InstalledVersion;
                        _updateInformation.State = UpdateState.Available;
                    }
                }
            }
            catch (Exception)
            {
                _updateInformation.State = UpdateState.Error;
            }
        }

        #endregion
    }
}
