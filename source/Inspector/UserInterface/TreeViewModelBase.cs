using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using ChristianMoser.WpfInspector.Services.ElementTree;
using ChristianMoser.WpfInspector.Utilities;

namespace ChristianMoser.WpfInspector.UserInterface
{
    public abstract class TreeViewModelBase
    {
        #region Private Members

        private readonly SelectedTreeItemService _selectedTreeItemService;
        private readonly Command<object> _invalidateVisualCommand;
        private readonly Command<object> _invalidateMeasureCommand;
        private readonly Command<object> _invalidateArrangeCommand;
        private readonly Command<object> _bringIntoViewCommand;
        private readonly Command<object> _focusCommand;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeViewModelBase"/> class.
        /// </summary>
        protected TreeViewModelBase()
        {
            _invalidateVisualCommand = new Command<object>(InvalidateVisual, CanInvalidateElement);
            _invalidateMeasureCommand = new Command<object>(InvalidateMeasure, CanInvalidateElement);
            _invalidateArrangeCommand = new Command<object>(InvalidateArrange, CanInvalidateElement);
            _bringIntoViewCommand = new Command<object>(BringIntoView, CanInvalidateElement);
            _focusCommand = new Command<object>(Focus, CanInvalidateElement);
        }

        #endregion

        protected TreeItem SelectedElement { get; set; }

        #region Public Functionality

        /// <summary>
        /// Gets the invalidate visual command.
        /// </summary>
        public ICommand InvalidateVisualCommand
        {
            get { return _invalidateVisualCommand; }
        }

        /// <summary>
        /// Gets the invalidate arrange command.
        /// </summary>
        public ICommand InvalidateArrangeCommand
        {
            get { return _invalidateArrangeCommand; }
        }

        /// <summary>
        /// Gets the bring into view command.
        /// </summary>
        public ICommand BringIntoViewCommand
        {
            get { return _bringIntoViewCommand; }
        }

        /// <summary>
        /// Gets the focus command command.
        /// </summary>
        public ICommand FocusCommandCommand
        {
            get { return _focusCommand; }
        }

        /// <summary>
        /// Gets the invalidate measure command.
        /// </summary>
        public ICommand InvalidateMeasureCommand
        {
            get { return _invalidateMeasureCommand; }
        }

        #endregion

        #region Private Helpers

        private bool CanInvalidateElement(object parameter)
        {
            return SelectedElement != null && SelectedElement.Instance as FrameworkElement != null;
        }

        private void InvalidateVisual(object parameter)
        {
            if (CanInvalidateElement(parameter))
            {
                ((FrameworkElement)SelectedElement.Instance).InvalidateVisual();
            }
        }

        private void InvalidateMeasure(object parameter)
        {
            if (CanInvalidateElement(parameter))
            {
                ((FrameworkElement)SelectedElement.Instance).InvalidateMeasure();
            }
        }

        private void BringIntoView(object parameter)
        {
            if (CanInvalidateElement(parameter))
            {
                ((FrameworkElement)SelectedElement.Instance).BringIntoView();
            }
        }

        private void Focus(object parameter)
        {
            if (CanInvalidateElement(parameter))
            {
                ((FrameworkElement)SelectedElement.Instance).Focus();
            }
        }

        private void InvalidateArrange(object parameter)
        {
            if (CanInvalidateElement(parameter))
            {
                ((FrameworkElement)SelectedElement.Instance).InvalidateArrange();
            }
        }

        #endregion
    }
}
