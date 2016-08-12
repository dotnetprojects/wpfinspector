using System;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;
using System.Globalization;

namespace ChristianMoser.WpfInspector.UserInterface.Helpers
{
    public class SelectionAdorner : Adorner
    {
        private static readonly Typeface TypeFace = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

        public SelectionAdorner(UIElement uiElement)
            : base(uiElement)
        {
            IsHitTestVisible = false;
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

            var dashPen = new Pen(brush, 1) { DashStyle = new DashStyle(new double[] { 1, 6 }, 0) };
            dashPen.Freeze();

            var guidelineSet = new GuidelineSet();
            guidelineSet.GuidelinesX.Add(0.5);
            guidelineSet.GuidelinesY.Add(0.5);

            //var outlinePen = new Pen(new SolidColorBrush(Color.FromArgb(0x70, 0xFF, 0xFF, 0xFF)), 5);
            //outlinePen.Freeze();

            drawingContext.PushGuidelineSet(guidelineSet);

            //drawingContext.DrawRectangle(null, outlinePen, rect);
            drawingContext.DrawRectangle(null, pen, rect);

            //var parent = VisualTreeHelper.GetParent(fe) as FrameworkElement;
            //if (parent != null)
            //{
            //    var thisLeft = new Point(0, fe.ActualHeight / 2);
            //    var thisRight = new Point(fe.ActualWidth, fe.ActualHeight / 2);
            //    var thisTop = new Point(fe.ActualWidth / 2, 0);
            //    var thisBottom = new Point(fe.ActualWidth / 2, fe.ActualHeight);
            //    var ancestorLeft = new Point(parent.TranslatePoint(thisLeft, fe).X, thisLeft.Y);
            //    var ancestorRight = ancestorLeft + new Vector(parent.ActualWidth, 0);
            //    var ancestorTop = new Point(thisTop.X, parent.TranslatePoint(new Point(), fe).Y);
            //    var ancestorBottom = new Point(thisBottom.X, parent.TranslatePoint(new Point(), fe).Y + parent.ActualHeight);

            //    var leftPen = fe.HorizontalAlignment == HorizontalAlignment.Left || fe.HorizontalAlignment == HorizontalAlignment.Stretch ? pen : dashPen;
            //    var rightPen = fe.HorizontalAlignment == HorizontalAlignment.Right || fe.HorizontalAlignment == HorizontalAlignment.Stretch ? pen : dashPen;
            //    var topPen = fe.VerticalAlignment == VerticalAlignment.Top || fe.VerticalAlignment == VerticalAlignment.Stretch ? pen : dashPen;
            //    var bottomPen = fe.VerticalAlignment == VerticalAlignment.Bottom || fe.VerticalAlignment == VerticalAlignment.Stretch ? pen : dashPen;

            //    drawingContext.DrawLine(leftPen, thisLeft, ancestorLeft);
            //    drawingContext.DrawLine(rightPen, thisRight, ancestorRight);
            //    drawingContext.DrawLine(topPen, thisTop, ancestorTop);
            //    drawingContext.DrawLine(bottomPen, thisBottom, ancestorBottom);
            //}

            var formattedHeight = new FormattedText(string.Format("{0:0}", fe.ActualHeight), CultureInfo.InvariantCulture, FlowDirection.LeftToRight, TypeFace, 10, brush);
            var formattedWidth = new FormattedText(string.Format("{0:0}", fe.ActualWidth), CultureInfo.InvariantCulture, FlowDirection.LeftToRight, TypeFace, 10, brush);
            drawingContext.DrawText(formattedHeight, new Point(rect.Width + 5, (rect.Height / 2) - (formattedHeight.Height / 2)));
            drawingContext.DrawText(formattedWidth, new Point(rect.Width / 2 - formattedWidth.Width / 2, rect.Height + 5));

            drawingContext.Pop();

        }
    }
}

