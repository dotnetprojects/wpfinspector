using System;
using System.Collections;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace ChristianMoser.WpfInspector.UserInterface.Controls
{
    public class UpdateTriggerControl : FrameworkElement
    {
        #region DependencyProperty UpdateAction

        /// <summary>
        /// Gets or sets the update action.
        /// </summary>
        /// <value>The update action.</value>
        public UpdateTrigger UpdateTrigger
        {
            get { return (UpdateTrigger)GetValue(UpdateTriggerProperty); }
            set { SetValue(UpdateTriggerProperty, value); }
        }

        /// <summary>
        /// Registers a dependency property to get or set the update action
        /// </summary>
        public static readonly DependencyProperty UpdateTriggerProperty =
            DependencyProperty.Register("UpdateTrigger", typeof(UpdateTrigger), typeof(UpdateTriggerControl),
            new FrameworkPropertyMetadata(null, OnUpdateTriggerChanged));

        #endregion

        #region Overrides

        protected override void OnRender(DrawingContext drawingContext)
        {
            if (UpdateTrigger != null && UpdateTrigger.UpdateAction != null)
            {
                Dispatcher.BeginInvoke(UpdateTrigger.UpdateAction, DispatcherPriority.Loaded);
            }
            base.OnRender(drawingContext);
        }

        /// <summary>
        /// Let the content measure.
        /// </summary>
        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(0, 0);
        }

        /// <summary>
        /// Let the arrange measure.
        /// </summary>
        protected override Size ArrangeOverride(Size finalSize)
        {
            return new Size(0, 0);
        }

        /// <summary>
        /// There is no visual child.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get { yield break; }
        }

        /// <summary>
        /// There is no visual child.
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// We have no visual child.
        /// </summary>
        protected override int VisualChildrenCount { get { return 0; } }

        #endregion

        private static void OnUpdateTriggerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var triggerControl = sender as UpdateTriggerControl;
            var updateTrigger = e.NewValue as UpdateTrigger;
            if (triggerControl != null && updateTrigger != null)
            {
                updateTrigger.IsUpdateRequiredChanged += triggerControl.OnIsUpdateRequiredChanged;
            }
        }

        private void OnIsUpdateRequiredChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }
    }
}
