using System;

namespace HermiteInterpolation.Numerics
{
    public static class RandomNumber
    {
        private static volatile Random _instance;
        private static readonly object SyncRoot = new Object();

        public static Random Instance
        {
            get
            {
                if (_instance != null) return _instance;
                lock (SyncRoot)
                {
                    if (_instance == null)
                        _instance = new Random();
                }

                return _instance;
            }
        }
    }
}