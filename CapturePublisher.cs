using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    /// <summary>
    /// 
    /// </summary>
    public class CapturePublisher
    {
        public delegate void captureEventHandler(object sender, ControlEvents.CaptureEventArgs args);

        public event captureEventHandler RaiseCaptureEvent;

        /// <summary>
        /// Should be called with the <paramref name="ID"/> of a captured piece to ensure the captured piece is removed.
        /// </summary>
        /// <param name="ID">The ID of the captured piece.</param>
        public void Capture(string ID)
        {
            OnCapture(new ControlEvents.CaptureEventArgs(ID));
        }

        /// <summary>
        /// Invokes the event RaiseCaptureEvent.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnCapture(ControlEvents.CaptureEventArgs e)
        {
            captureEventHandler captureEventHandler = RaiseCaptureEvent;
            if (captureEventHandler != null)
                captureEventHandler.Invoke(this, e);
        }
    }
}
