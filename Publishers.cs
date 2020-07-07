using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    /// <summary>
    /// Contains the publisher class that all objects, that needs to react when a key is pressed, should subscribe too.
    /// </summary>
    public class Publishers
    {
        private static KeyPublisher pubKey;
        private static NetPublisher pubNet;
        private static CapturePublisher pubCapture;

        private Publishers() { }
        /// <summary>
        /// Get the key publisher class instant.
        /// </summary>
        public static KeyPublisher PubKey { get => pubKey; }
        /// <summary>
        /// Gets the network publisher class instant.
        /// </summary>
        public static NetPublisher PubNet { get => pubNet; }
        /// <summary>
        /// Gets the capture publisher class instant.
        /// </summary>
        public static CapturePublisher PubCapture { get => pubCapture; }
        /// <summary>
        /// Ensures that the key publisher instant is not null if it is null.
        /// </summary>
        public static void SetKeyClass()
        {
            if (pubKey == null)
                pubKey = new KeyPublisher();
        }
        /// <summary>
        /// Ensures that the net publisher instant is not null if it is null.
        /// </summary>
        public static void SetNetClass()
        {
            if (pubNet == null)
                pubNet = new NetPublisher();
        }
        /// <summary>
        /// Ensures that the capture publisher instant is not null if it is null.
        /// </summary>
        public static void SetCaptureClass()
        {
            if (pubCapture == null)
                pubCapture = new CapturePublisher();
        }
    }
}
