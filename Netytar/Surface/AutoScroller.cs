﻿using NITHdmis.Eyetracking.PointFilters;
using System;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Netytar
{
    /// <summary>
    /// Automatically scrolls a ScrollViewer following the mouse.
    /// </summary>
    public class AutoScroller
    {
        #region Params
        protected ScrollViewer scrollViewer;
        protected int radiusThreshold;
        protected int proportional;
        protected IPointFilter filter;
        #endregion

        #region Scrollviewer params
        protected System.Windows.Point scrollCenter;
        protected System.Windows.Point basePosition;
        #endregion

        #region Internal counters
        protected DispatcherTimer samplerTimer = new DispatcherTimer(DispatcherPriority.Render);
        // private Timer samplerTimer = new Timer();
        // private MicroTimer samplerTimer = new MicroTimer();
        protected Point lastSampledPoint;
        protected Point lastMean;
        protected double Xdifference;
        protected double Ydifference;
        #endregion

        protected bool enabled = false;
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
        public AutoScroller(ScrollViewer scrollViewer, int radiusThreshold, int proportional, IPointFilter filter)
        {
            this.radiusThreshold = radiusThreshold;
            this.filter = filter;
            this.scrollViewer = scrollViewer;
            this.proportional = proportional;

            // Setting scrollviewer dimensions
            lastSampledPoint = new Point();
            basePosition = scrollViewer.PointToScreen(new System.Windows.Point(0, 0));
            scrollCenter = new System.Windows.Point(scrollViewer.ActualWidth / 2, scrollViewer.ActualHeight / 2);

            // Setting sampling timer
            samplerTimer.Interval = new TimeSpan(10000);//1000; //1;
            //samplerTimer.MicroTimerElapsed += SamplerTimer_MicroTimerElapsed;
            samplerTimer.Tick += ListenPosition;
            samplerTimer.Start();
        }

        protected void ListenPosition(object sender, EventArgs e)
        {
            if (enabled)
            {
                lastSampledPoint.X = GetMousePos().X - (int)basePosition.X;
                lastSampledPoint.Y = GetMousePos().Y - (int)basePosition.Y;

                filter.Push(lastSampledPoint);
                lastMean = filter.GetOutput();

                Scroll();
            }
        }

        protected void Scroll()
        {
            Xdifference = (scrollCenter.X - lastMean.X);
            Ydifference = (scrollCenter.Y - lastMean.Y);
            if (Math.Abs(scrollCenter.Y - lastMean.Y) > radiusThreshold && Math.Abs(scrollCenter.X - lastMean.X) > radiusThreshold)
            {
                scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - Math.Pow((Xdifference / proportional), 2) * Math.Sign(Xdifference));
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - Math.Pow((Ydifference / proportional), 2) * Math.Sign(Ydifference));
            }
        }

        protected Point GetMousePos()
        {
            temp = scrollViewer.PointToScreen(Mouse.GetPosition(scrollViewer));
            return new Point((int)temp.X, (int)temp.Y);
        }
        protected System.Windows.Point temp = new System.Windows.Point();
    }
}
