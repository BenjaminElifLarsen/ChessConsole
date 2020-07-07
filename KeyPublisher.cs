using System;
using System.Collections.Generic;
using System.Text;

namespace Chess
{
    /// <summary>
    /// Class contains events, delegates and functions related to the movement and selection using a keyboard.  
    /// </summary>
    public class KeyPublisher
    {
        public delegate void keyEventHandler(object sender, ControlEvents.KeyEventArgs args);

        public event keyEventHandler RaiseKeyEvent;

        /// <summary>
        /// Runs as long time the program is running. If a a key is pressed it will trigger an event that transmit the <c>ConsoleKeyInfo key</c> to all subscribers.  
        /// </summary>
        public void KeyPresser()
        {
            while (true)
            {
                while (!Console.KeyAvailable) ;
                ConsoleKeyInfo key = Console.ReadKey(true);
                OnKeyPress(new ControlEvents.KeyEventArgs(key));
                while (Console.KeyAvailable)
                    Console.ReadKey(true);
            }
        }

        /// <summary>
        /// Invokes the event RaiseKeyEvent. 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnKeyPress(ControlEvents.KeyEventArgs e)
        {
            keyEventHandler eventHandler = RaiseKeyEvent;
            if (eventHandler != null)
                eventHandler.Invoke(this, e);
        }
    }
}
