using System.ComponentModel;

namespace ChristianMoser.WpfInspector.Utilities
{
    /// <summary>
    /// Extensions to the delegate type <see cref="PropertyChangedEventHandler"/>.
    /// </summary>
    public static class PropertyChangedEventHandlerExtensions
    {
        /// <summary>
        /// Changes a field and notifies all registered receivers about a changed property,
        /// if the value of its field has changed.
        /// </summary>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <param name="field">The field.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>True, if the field has changed and therefore the value has been updated, false otherwise</returns>
        public static bool ChangeAndNotify<TField>(this PropertyChangedEventHandler handler, ref TField field,
                                                   TField fieldValue, object sender, string propertyName)
        {
            if (!Equals(field, fieldValue))
            {
                field = fieldValue;
                handler.Notify(sender, propertyName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Notifies all receivers of a given <see cref="PropertyChangedEventHandler"/> handler.
        /// 
        /// The property name provided as event argument will be an empty string.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="sender">The sender.</param>
        public static void Notify(this PropertyChangedEventHandler handler, object sender)
        {
            Notify(handler, sender, string.Empty);
        }

        /// <summary>
        /// Notifies all receivers of a given <see cref="PropertyChangedEventHandler"/> handler 
        /// that a property with specific name has changed.
        /// </summary>
        /// <param name="handler">The handler.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="propertyName">Name of the property.</param>
        public static void Notify(this PropertyChangedEventHandler handler, object sender, string propertyName)
        {
            if (handler != null)
            {
                handler(sender, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

}
