﻿//-----------------------------------------------------------------------
// <copyright file="EventCollector.cs" company="Brian Rogers">
// Copyright (c) Brian Rogers. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace TraceAnalysisSample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EventCollector
    {
        private EventWindow window;

        public EventCollector()
        {
        }

        public event EventHandler<WindowEventArgs> WindowClosed;

        public void OnStart(int eventId, Guid instanceId, DateTime timestamp)
        {
            this.EnsureWindow(timestamp);
            this.window.Add(eventId, instanceId);
        }

        public void OnEnd(int eventId, Guid instanceId, DateTime timestamp)
        {
            this.EnsureWindow(timestamp);
            this.window.Complete(eventId, instanceId);
        }

        public void CloseWindow()
        {
            this.WindowClosed(this, new WindowEventArgs(this.window));
        }

        private void EnsureWindow(DateTime timestamp)
        {
            if (this.window == null)
            {
                this.window = new EventWindow(timestamp);
            }
            else
            {
                DateTime nextStartTime = this.window.StartTime + TimeSpan.FromSeconds(1.0d);
                if (timestamp >= nextStartTime)
                {
                    EventWindow nextWindow = new EventWindow(this.window, nextStartTime);
                    nextWindow.ClearCompleted();
                    this.CloseWindow();
                    this.window = nextWindow;
                }
            }
        }
    }
}
