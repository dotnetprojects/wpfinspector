using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ChristianMoser.WpfInspector.UserInterface.Helpers
{

    public class ResizingAdorner : Adorner
    {
        #region Private Members

        private readonly Thumb _top, _bottom, _left, _right;
        private readonly Thumb _topLeft, _topRight, _bottomLeft, _bottomRight;
        private readonly VisualCollection _visualChildren;
        private static readonly Typeface TypeFace = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ResizingAdorner"/> class.
        /// </summary>
        public ResizingAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            _visualChildren = new VisualCollection(this);

            BuildAdornerCorner(ref _top, Cursors.SizeNS);
            BuildAdornerCorner(ref _bottom, Cursors.SizeNS);
            BuildAdornerCorner(ref _left, Cursors.SizeWE);
            BuildAdornerCorner(ref _right, Cursors.SizeWE);
            BuildAdornerCorner(ref _topLeft, Cursors.SizeNWSE);
            BuildAdornerCorner(ref _topRight, Cursors.SizeNESW);
            BuildAdornerCorner(ref _bottomLeft, Cursors.SizeNESW);
            BuildAdornerCorner(ref _bottomRight, Cursors.SizeNWSE);

            _left.DragDelta += HandleLeft;
            _right.DragDelta += HandleRight;
            _bottom.DragDelta += HandleBottom;
            _top.DragDelta += HandleTop;                
            _bottomLeft.DragDelta += HandleBottomLeft;
            _bottomRight.DragDelta += HandleBottomRight;
            _topLeft.DragDelta += HandleTopLeft;
            _topRight.DragDelta += HandleTopRight;
        }

        #endregion

        // Handler for resizing from the bottom-right.
        private void HandleBottomRight(object sender, DragDeltaEventArgs args)
        {
            var adornedElement = AdornedElement as FrameworkElement;
            var hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
            adornedElement.Height = Math.Max(args.VerticalChange + adornedElement.Height, hitThumb.DesiredSize.Height);
        }

        // Handler for resizing from the bottom-left.
        private void HandleBottomLeft(object sender, DragDeltaEventArgs args)
        {
            var adornedElement = AdornedElement as FrameworkElement;
            var hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            adornedElement.Width = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            adornedElement.Height = Math.Max(args.VerticalChange + adornedElement.Height, hitThumb.DesiredSize.Height);
        }

        // Handler for resizing from the top-right.
        private void HandleTopRight(object sender, DragDeltaEventArgs args)
        {
            var adornedElement = AdornedElement as FrameworkElement;
            var hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
            adornedElement.Height = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
        }

        // Handler for resizing from the top-left.
        private void HandleTopLeft(object sender, DragDeltaEventArgs args)
        {
            var adornedElement = AdornedElement as FrameworkElement;
            var hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            adornedElement.Width = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
            adornedElement.Height = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
        }

        // Handler for resizing from the top-right.
        private void HandleTop(object sender, DragDeltaEventArgs args)
        {
            var adornedElement = AdornedElement as FrameworkElement;
            var hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            adornedElement.Height = Math.Max(adornedElement.Height - args.VerticalChange, hitThumb.DesiredSize.Height);
        }

        // Handler for resizing from the bottom-left.
        private void HandleBottom(object sender, DragDeltaEventArgs args)
        {
            var adornedElement = AdornedElement as FrameworkElement;
            var hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            adornedElement.Height = Math.Max(args.VerticalChange + adornedElement.Height, hitThumb.DesiredSize.Height);
        }


        // Handler for resizing from the top-left.
        private void HandleLeft(object sender, DragDeltaEventArgs args)
        {
            var adornedElement = AdornedElement as FrameworkElement;
            var hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            adornedElement.Width = Math.Max(adornedElement.Width - args.HorizontalChange, hitThumb.DesiredSize.Width);
        }

        // Handler for resizing from the top-right.
        private void HandleRight(object sender, DragDeltaEventArgs args)
        {
            var adornedElement = AdornedElement as FrameworkElement;
            var hitThumb = sender as Thumb;

            if (adornedElement == null || hitThumb == null) return;

            // Ensure that the Width and Height are properly initialized after the resize.
            EnforceSize(adornedElement);

            // Change the size by the amount the user drags the mouse, as long as it's larger 
            // than the width or height of an adorner, respectively.
            adornedElement.Width = Math.Max(adornedElement.Width + args.HorizontalChange, hitThumb.DesiredSize.Width);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var fe = AdornedElement as FrameworkElement;
            if (fe == null)
            {
                return;
            }

            var rect = new Rect(1, 1, Math.Max(0, fe.ActualWidth - 2), Math.Max(0, fe.ActualHeight - 2));
            var color = Colors.Red;
            var brush = new SolidColorBrush(color);
            var pen = new Pen(brush, 1);
            pen.Freeze();

            var dashPen = new Pen(brush, 1) {DashStyle = new DashStyle(new double[]{1,6},0)};
            dashPen.Freeze();

            var guidelineSet = new GuidelineSet();
            guidelineSet.GuidelinesX.Add(0.5);
            guidelineSet.GuidelinesY.Add(0.5);

            //var outlinePen = new Pen(new SolidColorBrush(Color.FromArgb(0x70, 0xFF, 0xFF, 0xFF)), 5);
            //outlinePen.Freeze();

            drawingContext.PushGuidelineSet(guidelineSet);

            //drawingContext.DrawRectangle(null, outlinePen, rect);
            drawingContext.DrawRectangle(null, pen, rect);

            // desiredWidth and desiredHeight are the width and height of the element that's being adorned.  
            // These will be used to place the ResizingAdorner at the corners of the adorned element.  
            double desiredWidth = fe.ActualWidth;
            double desiredHeight = fe.ActualHeight;
            double adornerWidth = fe.ActualWidth;
            double adornerHeight = fe.ActualHeight;

            _top.Arrange(new Rect(desiredWidth / 2 - adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            _bottom.Arrange(new Rect(desiredWidth / 2 - adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));
            _left.Arrange(new Rect(-adornerWidth / 2, desiredHeight / 2 - adornerHeight / 2, adornerWidth, adornerHeight));
            _right.Arrange(new Rect(desiredWidth - adornerWidth / 2, desiredHeight / 2 - adornerHeight / 2, adornerWidth, adornerHeight));
            _topLeft.Arrange(new Rect(-adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            _topRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, -adornerHeight / 2, adornerWidth, adornerHeight));
            _bottomLeft.Arrange(new Rect(-adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));
            _bottomRight.Arrange(new Rect(desiredWidth - adornerWidth / 2, desiredHeight - adornerHeight / 2, adornerWidth, adornerHeight));


            var parent = VisualTreeHelper.GetParent(fe) as FrameworkElement;
            if (parent != null)
            {
                var thisLeft = new Point(0, fe.ActualHeight / 2);
                var thisRight = new Point(fe.ActualWidth, fe.ActualHeight / 2);
                var thisTop = new Point(fe.ActualWidth/2, 0);
                var thisBottom = new Point(fe.ActualWidth / 2, fe.ActualHeight);
                var ancestorLeft = new Point(parent.TranslatePoint(thisLeft, fe).X, thisLeft.Y);
                var ancestorRight = ancestorLeft + new Vector(parent.ActualWidth, 0);
                var ancestorTop = new Point(thisTop.X, parent.TranslatePoint(new Point(), fe).Y);
                var ancestorBottom = new Point(thisBottom.X, parent.TranslatePoint(new Point(), fe).Y + parent.ActualHeight);

                var leftPen = fe.HorizontalAlignment == HorizontalAlignment.Left || fe.HorizontalAlignment == HorizontalAlignment.Stretch ? pen : dashPen;
                var rightPen = fe.HorizontalAlignment == HorizontalAlignment.Right || fe.HorizontalAlignment == HorizontalAlignment.Stretch ? pen : dashPen;
                var topPen = fe.VerticalAlignment == VerticalAlignment.Top || fe.VerticalAlignment == VerticalAlignment.Stretch ? pen : dashPen;
                var bottomPen = fe.VerticalAlignment == VerticalAlignment.Bottom || fe.VerticalAlignment == VerticalAlignment.Stretch ? pen : dashPen;

                drawingContext.DrawLine(leftPen, thisLeft, ancestorLeft);
                drawingContext.DrawLine(rightPen, thisRight, ancestorRight);
                drawingContext.DrawLine(topPen, thisTop, ancestorTop);
                drawingContext.DrawLine(bottomPen, thisBottom, ancestorBottom);
            }

            var formattedHeight = new FormattedText(string.Format("{0:0}", fe.ActualHeight), CultureInfo.InvariantCulture, FlowDirection.LeftToRight, TypeFace, 10, brush);
            var formattedWidth = new FormattedText(string.Format("{0:0}", fe.ActualWidth), CultureInfo.InvariantCulture, FlowDirection.LeftToRight, TypeFace, 10, brush);
            drawingContext.DrawText(formattedHeight, new Point(rect.Width + 5, (rect.Height / 2) - (formattedHeight.Height / 2)));
            drawingContext.DrawText(formattedWidth, new Point(rect.Width / 2 - formattedWidth.Width / 2, rect.Height + 5));

            drawingContext.Pop();
        }

        // Override the VisualChildrenCount and GetVisualChild properties to interface with 
        // the adorner's visual collection.
        protected override int VisualChildrenCount
        {
            get
            {
                return _visualChildren.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visualChildren[index];
        }

        // Helper method to instantiate the corner Thumbs, set the Cursor property, 
        // set some appearance properties, and add the elements to the visual tree.
        private void BuildAdornerCorner(ref Thumb cornerThumb, Cursor customizedCursor)
        {
            if (cornerThumb != null) return;

            cornerThumb = new Thumb {Cursor = customizedCursor};

            // Set some arbitrary visual characteristics.
            cornerThumb.Height = cornerThumb.Width = 8;
            cornerThumb.Opacity = 0.40;
            cornerThumb.Background = new SolidColorBrush(Colors.Red);

            _visualChildren.Add(cornerThumb);
        }

        // This method ensures that the Widths and Heights are initialized.  Sizing to content produces
        // Width and Height values of Double.NaN.  Because this Adorner explicitly resizes, the Width and Height
        // need to be set first.  It also sets the maximum size of the adorned element.
        private static void EnforceSize(FrameworkElement adornedElement)
        {
            if (adornedElement.Width.Equals(Double.NaN))
                adornedElement.Width = adornedElement.DesiredSize.Width;
            if (adornedElement.Height.Equals(Double.NaN))
                adornedElement.Height = adornedElement.DesiredSize.Height;

            var parent = adornedElement.Parent as FrameworkElement;
            if (parent != null)
            {
                adornedElement.MaxHeight = parent.ActualHeight;
                adornedElement.MaxWidth = parent.ActualWidth;
            }
        }
        
    }

}
