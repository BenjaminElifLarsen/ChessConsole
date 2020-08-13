using System;

namespace Support
{
    /// <summary>
    /// Contains extended functions for arrays. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ArrayExtent<T>
    {
        /// <summary>
        /// Deep copy of <paramref name="arrayToCopy"/>, ensures that the fields are deferenced. 
        /// </summary>
        /// <param name="arrayToCopy">The array to deep copy.</param>
        /// <returns>Returns a deep copty of <paramref name="arrayToCopy"/>.</returns>
        public static T[] DeepCore(T[] arrayToCopy)
        {
            T[] array = new T[arrayToCopy.Length];
            for (int i = 0; i < arrayToCopy.Length; i++)
                array[i] = arrayToCopy[i];
            return array;
        }



    }

    public class Test
    {
        void test()
        {
            ArrayExtent<int>.DeepCore(new int[] { 5, 3 });
        }
    }

}
