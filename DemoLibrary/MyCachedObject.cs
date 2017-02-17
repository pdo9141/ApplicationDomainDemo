using System;

namespace DemoLibrary
{
    internal sealed class MyCachedObject
    {
        private static Random generator = new Random();
        private byte[] processedData;
        public MyCachedObject()
        {
            processedData = new byte[generator.Next(25, 70) * 1024];
        }
        public int Execute(int id)
        {
            return processedData.Length;
        }
    }
}
