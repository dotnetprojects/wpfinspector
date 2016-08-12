using System;

namespace ChristianMoser.WpfInspector.Utilities
{
    public static class EventHandlerExtensions
    {
        /// <summary>
        /// Notifies all receivers of a given <see cref="EventHandler"/> handler 
        /// that a property with specific name has changed.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="sender">The sender.</param>
        public static void Notify(this EventHandler handler, object sender, EventArgs e)
        {
            if (handler != null)
            {
                handler(sender, e);
            }
        }
    }
}
