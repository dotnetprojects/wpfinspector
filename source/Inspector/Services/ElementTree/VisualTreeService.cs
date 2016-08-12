namespace ChristianMoser.WpfInspector.Services.ElementTree
{
    /// <summary>
    /// Service to browse the visual tree
    /// </summary>
    public class VisualTreeService : TreeElementService
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualTreeService"/> class.
        /// </summary>
        public VisualTreeService()
            : base(TreeType.VisualTree)
        {
        }

        #endregion
    }
}
