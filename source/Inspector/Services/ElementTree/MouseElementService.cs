using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Documents;
using ChristianMoser.WpfInspector.UserInterface.Helpers;
using ChristianMoser.WpfInspector.Utilities;
using ChristianMoser.WpfInspector.Win32;
using System.Windows.Controls;

namespace ChristianMoser.WpfInspector.Services.ElementTree
{
    public enum SearchStrategy
    {
        VisualTree,
        LogicalTree
    }

    public class MouseElementService : IDisposable
    {
        private readonly DispatcherTimer _adornerTimer;
        private readonly DispatcherTimer _refreshTimer;
        private readonly SelectedTreeItemService _selectedTreeItemService;
        private readonly MouseElementServiceSettings _settings = new MouseElementServiceSettings();

        private SelectionAdorner _selectionAdorner;
        private AdornerLayer _adornerLayer;
        private bool _lastPressed;

        public FrameworkElement DireclyOverElement { get; private set; }
        private TreeItem _selectedTreeItem;

        public event EventHandler MouseElementChanged;

        public MouseElementService()
        {
            _selectedTreeItemService = ServiceLocator.Resolve<SelectedTreeItemService>();
            _selectedTreeItemService.SelectedTreeItemChanged += OnSelectedTreeItemChanged;

            _adornerTimer = new DispatcherTimer(DispatcherPriority.Normal) { Interval = TimeSpan.FromMilliseconds(100) };
            _adornerTimer.Tick += OnAdornerTimerTick;
            _adornerTimer.Start();

            _settings.ShowAdornerChanged += (s, e) => UpdateAdorner(DireclyOverElement);

            _refreshTimer = new DispatcherTimer(DispatcherPriority.Background);
            _refreshTimer.Interval = TimeSpan.FromSeconds(1);
            _refreshTimer.Tick += (s, e) => ApplySelectedTreeItem();
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        public MouseElementServiceSettings Settings
        {
            get { return _settings; }
        }

        private void OnSelectedTreeItemChanged(object sender, EventArgs<TreeItem> e)
        {
            UIElement uiElement = null;
            if (e.Data != null)
            {
                uiElement = e.Data.Instance as UIElement;
            }
            UpdateAdorner(uiElement);
        }

        private void OnAdornerTimerTick(object sender, EventArgs e)
        {
            bool modifierPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

            // Start hovering
            if( modifierPressed && !_lastPressed)
            {
                _refreshTimer.Start();
            }

            // Stop hovering
            if (_lastPressed && !modifierPressed)
            {
                ApplySelectedTreeItem();
                _refreshTimer.Stop();
            }

            _lastPressed = modifierPressed;

            if (!modifierPressed)
                return;

            ActivateWindowAtMousePosition();

            var directlyOverElement = GetElementAtMousePos();
            if (DireclyOverElement != directlyOverElement)
            {
                FrameworkElement element = directlyOverElement;

                if (_selectedTreeItemService.SearchStrategy == SearchStrategy.LogicalTree)
                {
                    element = FindLogicalElement(element);
                }
                else
                {
                    // If SHIFT is pressed, automatically skip template parts and move up to the control itself
                    if (!Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift))
                    {
                        while (element != null && element.TemplatedParent != null)
                        {
                            element = VisualTreeHelper.GetParent(element) as FrameworkElement;
                        }
                    }
                    _selectedTreeItem = VisualTreeItem.FindVisualTreeItem(element);
                }

                UpdateAdorner(element);
                DireclyOverElement = element;
                NotifyMouseElementChanged();
            }
        }

        private FrameworkElement FindLogicalElement(FrameworkElement element)
        {
            _selectedTreeItem = LogicalTreeItem.FindLogicalTreeItem(element);

            while (_selectedTreeItem == null && element != null && element.TemplatedParent != null)
            {
                element = element.TemplatedParent as FrameworkElement;
                _selectedTreeItem = LogicalTreeItem.FindLogicalTreeItem(element);
            }

            while (_selectedTreeItem == null && element != null)
            {
                element = VisualTreeHelper.GetParent(element) as FrameworkElement;
                _selectedTreeItem = LogicalTreeItem.FindLogicalTreeItem(element);
            }
            return element;
        }

        private void ApplySelectedTreeItem()
        {
            if (_selectedTreeItemService.SearchStrategy == SearchStrategy.LogicalTree)
            {
                _selectedTreeItemService.SelectedLogicalTreeItem = _selectedTreeItem;
            }
            else
            {
                _selectedTreeItemService.SelectedVisualTreeItem = _selectedTreeItem;
            }
        }

        private static FrameworkElement GetElementAtMousePos()
        {
            FrameworkElement directlyOverElement = null;
            var activeWindow = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window.IsActive);

            if (activeWindow != null)
            {
                VisualTreeHelper.HitTest(activeWindow, FilterCallback, r => ResultCallback(r, ref directlyOverElement), new PointHitTestParameters(Mouse.GetPosition(activeWindow)));
            }
            
            return directlyOverElement;
        }

        private static HitTestFilterBehavior FilterCallback(DependencyObject target)
        {
            return HitTestFilterBehavior.Continue;
        }


        private static HitTestResultBehavior ResultCallback(HitTestResult result, ref FrameworkElement directlyOverElement)
        {
            if (result != null && result.VisualHit is FrameworkElement)
            {
                directlyOverElement = result.VisualHit as FrameworkElement;
                if (directlyOverElement.IsVisible)
                {
                    return HitTestResultBehavior.Stop;
                }
            }
            return HitTestResultBehavior.Continue;
        }


        private static void ActivateWindowAtMousePosition()
        {
            System.Drawing.Point mousePos;
            if (NativeMethods.GetCursorPos(out mousePos))
            {
                IntPtr hWnd = NativeMethods.WindowFromPoint(mousePos);
                NativeMethods.SetForegroundWindow(hWnd);
            }
        }

        private void UpdateAdorner(UIElement directlyOverElement)
        {
            if (_selectionAdorner != null)
            {
                _adornerLayer.Remove(_selectionAdorner);
            }

            if (directlyOverElement != null && _settings.IsShowAdorner)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(directlyOverElement);
                if (adornerLayer != null)
                {
                    _adornerLayer = adornerLayer;
                    if (directlyOverElement is Grid)
                    {
                        _selectionAdorner = new GridAdorner(directlyOverElement);
                    }
                    else
                    {
                        _selectionAdorner = new SelectionAdorner(directlyOverElement); 
                    }
                    
                    _adornerLayer.Add(_selectionAdorner);
                }
            }
        }

        private void NotifyMouseElementChanged()
        {
            if (MouseElementChanged != null)
            {
                MouseElementChanged(this, EventArgs.Empty);
            }
        }

        internal void RemoveAdorner()
        {
            if (_adornerLayer != null && _selectionAdorner != null)
            {
                _adornerLayer.Remove(_selectionAdorner);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            RemoveAdorner();
            _adornerTimer.Stop();
            _refreshTimer.Stop();
        }

        #endregion
    }
}
