using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChristianMoser.WpfInspector.UserInterface.Helpers
{
    /// <summary>
    /// Draws the grid lines of a grid
    /// </summary>
    public class GridAdorner : SelectionAdorner
    {
        #region Private Members

        private Grid _grid;
        
        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="GridAdorner"/> class.
        /// </summary>
        public GridAdorner(UIElement uiElement)
            : base(uiElement)
        {
            _grid = uiElement as Grid;
        }

        #endregion


        protected override void OnRender(DrawingContext drawingContext)
        {
            var dashPen = new Pen(Brushes.Red, 1) { DashStyle = new DashStyle(new double[] { 1, 6 }, 0) };
            dashPen.Freeze();

            if( _grid  != null )
            {
                if (_grid.ColumnDefinitions != null)
                {
                    double xPos = 0;
                    foreach (var column in _grid.ColumnDefinitions)
                    {
                        xPos += column.ActualWidth;
                        drawingContext.DrawLine(dashPen, new Point(xPos, 0), new Point(xPos,ActualHeight));
                    }
                }
                if (_grid.RowDefinitions != null)
                {
                    double yPos = 0;
                    foreach (var column in _grid.RowDefinitions)
                    {
                        yPos += column.ActualHeight;
                        drawingContext.DrawLine(dashPen, new Point(0, yPos), new Point(ActualWidth, yPos));
                    }
                }
            }
            base.OnRender(drawingContext);
        }
    }
}
