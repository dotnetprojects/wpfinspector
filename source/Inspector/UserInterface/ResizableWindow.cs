using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace ChristianMoser.WpfInspector.UserInterface
{
    public class ResizeableWindow : Window
    {
        private const int WM_SYSCOMMAND = 0x112;
        private HwndSource _hwndSource;

        private readonly Dictionary<ResizeDirection, Cursor> _cursors = new Dictionary<ResizeDirection, Cursor> 
        {
            {ResizeDirection.Top, Cursors.SizeNS},
            {ResizeDirection.Bottom, Cursors.SizeNS},
            {ResizeDirection.Left, Cursors.SizeWE},
            {ResizeDirection.Right, Cursors.SizeWE},
            {ResizeDirection.TopLeft, Cursors.SizeNWSE},
            {ResizeDirection.BottomRight, Cursors.SizeNWSE},
            {ResizeDirection.TopRight, Cursors.SizeNESW},
            {ResizeDirection.BottomLeft, Cursors.SizeNESW} 
        };

        private enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public ResizeableWindow()
        {
            SourceInitialized += InitializeWindowSource;
            PreviewMouseLeftButtonUp += ResetCursor;
        }

        protected void InitializeWindowSource(object sender, EventArgs e)
        {
            _hwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
        }

        protected void ResizeIfPressed(object sender, MouseEventArgs e)
        {
            var element = sender as FrameworkElement;
            ResizeDirection direction = GetDirectionFromName(element.Name);

            Cursor = _cursors[direction];

            if (e.LeftButton == MouseButtonState.Pressed)
                ResizeWindow(direction);
        }

        private static ResizeDirection GetDirectionFromName(string name)
        {
            //Hack - Assumes the drag handels are all named *DragHandle
            string enumName = name.Replace("DragHandle", "");
            return (ResizeDirection)Enum.Parse(typeof(ResizeDirection), enumName);
        }

        protected void ResetCursor(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                var element = e.OriginalSource as FrameworkElement;

                //Hack - only reset cursors if the orginal source isn't a draghandle
                if (element != null && !element.Name.Contains("DragHandle"))
                    Cursor = Cursors.Arrow;
            }
        }

        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(_hwndSource.Handle, WM_SYSCOMMAND, (IntPtr)(61440 + direction), IntPtr.Zero);
        }
    }
}
