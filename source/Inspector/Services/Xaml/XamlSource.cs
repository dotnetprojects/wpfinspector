using System.IO;
using ChristianMoser.WpfInspector.Baml;

namespace ChristianMoser.WpfInspector.Services.Xaml
{
    public class XamlSource
    {
        #region Private Members

        private string _xaml;
        private readonly Stream _bamlStream;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlSource"/> class.
        /// </summary>
        public XamlSource(Stream bamlStream)
        {
            _bamlStream = bamlStream;
        }

        #endregion

        /// <summary>
        /// Gets the xaml.
        /// </summary>
        public string Xaml
        {
            get
            {
                if (_xaml == null)
                {
                    _xaml = new BamlTranslator(_bamlStream).ToString();
                }
                return _xaml;
            }
        }
        
    }
}
