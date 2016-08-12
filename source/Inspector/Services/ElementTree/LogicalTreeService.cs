namespace ChristianMoser.WpfInspector.Services.ElementTree
{
    /// <summary>
    /// Service to browse the logical tree
    /// </summary>
    public class LogicalTreeService : TreeElementService
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicalTreeService"/> class.
        /// </summary>
        public LogicalTreeService()
            : base(TreeType.LogicalTree)
        {
        }

        #endregion
    }
}
