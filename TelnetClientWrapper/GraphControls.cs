using GraphShape.Controls;
using System;
using System.Windows;
namespace IsengardClient
{
    public class EdgeControl : System.Windows.Controls.Control, IPoolObject, IDisposable
    {
        #region Dependency Properties

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source",
                                                     typeof(VertexControl),
                                                     typeof(EdgeControl),
                                                     new UIPropertyMetadata(null));

        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target",
                                                     typeof(VertexControl),
                                                     typeof(EdgeControl),
                                                     new UIPropertyMetadata(null));

        public static readonly DependencyProperty RoutePointsProperty = DependencyProperty.Register("RoutePoints",
                                                      typeof(Point[]),
                                                      typeof(EdgeControl),
                                                      new UIPropertyMetadata(
                                                        null));

        public static readonly DependencyProperty EdgeProperty = DependencyProperty.Register("Edge", typeof(object),
                                                   typeof(EdgeControl),
                                                   new PropertyMetadata(null));


        public static readonly DependencyProperty DeletedProperty = DependencyProperty.Register("Deleted", typeof(bool), typeof(EdgeControl), new UIPropertyMetadata(null));

        #endregion

        #region Properties
        public VertexControl Source
        {
            get { return (VertexControl)GetValue(SourceProperty); }
            internal set { SetValue(SourceProperty, value); }
        }

        public VertexControl Target
        {
            get { return (VertexControl)GetValue(TargetProperty); }
            internal set { SetValue(TargetProperty, value); }
        }

        public Point[] RoutePoints
        {
            get { return (Point[])GetValue(RoutePointsProperty); }
            set { SetValue(RoutePointsProperty, value); }
        }

        public object Edge
        {
            get { return GetValue(EdgeProperty); }
            set { SetValue(EdgeProperty, value); }
        }

        /// <summary>
        /// whether the edge has been deleted
        /// </summary>
        public bool Deleted
        {
            get { return (bool)GetValue(DeletedProperty); }
            set { SetValue(DeletedProperty, value); }
        }

        #endregion

        static EdgeControl()
        {
            //override the StyleKey
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EdgeControl), new FrameworkPropertyMetadata(typeof(EdgeControl)));
        }

        #region IPoolObject Members

        public void Reset()
        {
            Edge = null;
            RoutePoints = null;
            Source = null;
            Target = null;
            Deleted = false;
        }

        public void Terminate()
        {
            //nothing to do, there are no unmanaged resources
        }

        public event DisposingHandler Disposing;

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (Disposing != null)
                Disposing(this);
        }

        #endregion
    }
}
