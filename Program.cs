using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;
using System.Security.Cryptography;

namespace Chess
{   //https://www.chessvariants.com/d.chess/chess.html

    class Program
    {
        static void Main(string[] args)
        {
            Menu menu = new Menu();
            menu.Run();
        }
    } 

    //public class DelegateTest
    //{
    //    private delegate void StringOut(string s);
    //    private delegate double ReturnOut(params double[] values);
    //    private delegate void ArrayOut<T>(params T[][] arrays);
    //    StringOut stringOut;
    //    ReturnOut returnOut;
    //    ArrayOut<string> typeOut;
    //    ArrayOut<int> typeIntOut;
    //    //ArrayOut test;
    //    //private static event StringOut stringOutTest;
    //    Publisher pub;
    //    List<Subscriber> subs = new List<Subscriber>();
    //    public DelegateTest()
    //    {
    //        stringOut = StringWritter;
    //        stringOut += StringMehWritter;
    //        stringOut += StringFancyWritter;
    //        returnOut = MathCal;
    //        typeOut = OutDotPrintLine;
    //        typeIntOut = OutDotPrintLine<int>;

    //        pub = new Publisher();
    //        subs.Add(new Subscriber("sub1", pub));
    //        subs.Add(new Subscriber("sub2", pub));

    //    }

    //    public void Print<T>(params T[][] arrays)
    //    {

    //        //if (typeof(T) == typeof(int))
    //            //typeIntOut(Array.ConvertAll(arrays, item => (int)item); //https://stackoverflow.com/questions/2068120/c-sharp-cast-entire-array
    //        //else if
    //    }

    //    private void OutDotPrintLine<T>(params T[][] arrays)
    //    {
    //        foreach (T[] array in arrays)
    //        {
    //            foreach (T item in array)
    //            {
    //                Console.Write(item.ToString() + "");
    //            }
    //            Console.WriteLine();
    //        }
    //    }

    //    public double MathCalculator(params double[] values)
    //    {
    //        double result = returnOut(values);
    //        stringOut(result.ToString());
    //        return result;
    //    }

    //    private double MathCal(params double[] values)
    //    {
    //        double result = 0;
    //        foreach (double value in values)
    //        {
    //            result += value;
    //        }
    //        return result;
    //    } 

    //    public void WriteOutEvent()
    //    {
    //        pub.Test();
    //        pub.ArrowTest();
    //    }

    //    public void WriteOut(string message)
    //    {
    //        stringOut(message);
    //    }
        
    //    public void MathTest(params float[] values)
    //    {
    //        Math(stringOut, values);
    //    }

    //    private void Math(StringOut callBack, params float[] values)
    //    {
    //        float total = 0;
    //        foreach (float value in values)
    //        {
    //            total += value;
    //            callBack(value.ToString());
    //        }
    //        callBack($"Total is {total}. Average is {total/values.Length}");
    //    }

    //    private void StringWritter(string s)
    //    {
    //        Console.WriteLine(s);
    //    }

    //    private void StringMehWritter(string s)
    //    {
    //        Console.WriteLine("Meh... " + s);
    //    }

    //    private void StringFancyWritter(string s)
    //    {
    //        Console.WriteLine(Settings.CVTS.BrightRedForeground + s + Settings.CVTS.Reset);
    //    }


    //    public class Event : EventArgs
    //    {
    //        public Event(string s)
    //        {
    //            Message = s;
    //        }
    //        public Event(params double[] values)
    //        {
    //            Values = values;
    //        }
    //        public Event(ConsoleKeyInfo key)
    //        {
    //            Key = key;
    //        }
    //        public double[] Values { get; set; }
    //        public string Message { get; set; }
    //        public ConsoleKeyInfo Key { get; set; }
    //    }


    //    public class Publisher
    //    {
    //        public event EventTest StringEvent;

    //        public delegate void EventTest(object sender, Event args);

    //        public event EventHandler<Event> StringEvent2;

    //        public event EventTest ValueEvent;

    //        public event EventHandler<Event> KeyEvent;


    //        public void Test()
    //        {
    //            OnEventTest(new Event("Event Ran"));
    //            OnEventTest(new Event(5, 2, 1));
    //            //OnEventTest(new Event(5, -2, 1));
    //        }

    //        public void ArrowTest()
    //        {
    //            bool test = false;
    //            do
    //            {
    //                while (!Console.KeyAvailable) ;
    //                ConsoleKeyInfo key = Console.ReadKey(); //have a list of ConsoleKeys for the reworked movement system
    //                if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.RightArrow || key.Key == ConsoleKey.Escape || key.Key == ConsoleKey.Enter)
    //                    onArrowKeyPress(new Event(key));

    //            } while (!test);
    //        }

    //        protected virtual void onArrowKeyPress(Event e)
    //        {
    //            EventHandler<Event> eventHandler = KeyEvent;
    //            if(eventHandler != null)
    //            {
    //                eventHandler.Invoke(this, e);
    //            }
    //        }

    //        protected virtual void OnEventTest(Event e)
    //        {
    //            EventHandler<Event> testEvent = StringEvent2;
    //            var eValues = e.GetType().GetProperties();
    //            if (testEvent != null)
    //            {
    //                for (int n = 0; n < eValues.Length; n++)
    //                {
    //                    string type = eValues[n].PropertyType.Name;

    //                    if (type == "Double[]")
    //                    {
    //                        if (eValues[n].GetValue(e) != null)
    //                        {
    //                            double result = 0;
    //                            double[] values = (double[])e.GetType().GetProperty(eValues[n].Name).GetValue(e);
    //                            foreach (double value in values)
    //                            {
    //                                result += value;
    //                            }
    //                            e.Values = new double[] { result };
    //                            //e.Message += $"Result is {result}"; //
    //                            testEvent.Invoke(this, e);
    //                        }
    //                    }
    //                    else if (type == "String")
    //                    {
    //                        if (eValues[n].GetValue(e) != null) //even if Message is null, since Values is before it will add the results to Message and thus after Message is no longer null
    //                        {
    //                            e.Message += $" at {DateTime.Now}";
    //                            testEvent.Invoke(this, e);
    //                        }
    //                    }

    //                }
    //            }
    //        }
    //    }


    //    public class Subscriber
    //    {
    //        private readonly string ID;
    //        private int[] direction;
    //        public Subscriber(string ID, Publisher Pub)
    //        {
    //            Random rnd = new Random();
    //            this.ID = ID;
    //            Pub.StringEvent2 += HandleString2Event;
    //            Pub.KeyEvent += KeyEvent;
    //            direction = new int[2] {rnd.Next(-5,5), rnd.Next(-10,10) };
    //        }

    //        protected void KeyEvent(object sender, Event e)
    //        {
    //            Console.CursorLeft = 0;
    //            ConsoleKey key = e.Key.Key;
    //            Console.WriteLine($"{ID} recieved {key.ToString()}");
    //            if (key == ConsoleKey.DownArrow)
    //                direction[1]++;
    //            else if (key == ConsoleKey.UpArrow)
    //                direction[1]--;
    //            else if (key == ConsoleKey.RightArrow)
    //                direction[0]++;
    //            else if (key == ConsoleKey.LeftArrow)
    //                direction[0]--;
    //            //Console.CursorLeft = 0; //without this one, sub 1 will write at index 1 instead of index 0 for some reason. The formatted string does not contain an empty space
    //            Console.WriteLine($"{ID}'s Coordinates: {direction[0]} {direction[1]}"); //and both subs places the cursor the same location on the next line
    //            //found out of why, pressing a key will still "write" to the console and thus move the cursor
    //        }

    //        protected void HandleString2Event(object sender, Event e)
    //        {
    //            var eProperties = e.GetType().GetProperties();
    //            for (int n = 0; n < eProperties.Length; n++)
    //            {
    //                if(eProperties[n].GetValue(e) != null)
    //                {
    //                    string type = eProperties[n].PropertyType.Name;
    //                    if (type == "String")
    //                        Console.WriteLine($"{ID} received this message: {e.Message}");
    //                    else if (type == "Double[]")
    //                        Console.WriteLine($"{ID} received this value: {e.Values[0]}");
    //                }
    //            }
    //        }

    //    }



    //}

}
