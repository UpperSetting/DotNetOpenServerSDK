using System;

namespace US.OpenServer
{
    /// <summary>
    /// Static class used to create a unique identifier that is associated with each
    /// socket connection.
    /// </summary>
    public static class SessionId
    {
        private static ushort nextId = 0;
        private static object synchObject = new object();

        /// <summary>
        /// Gets the next unique identifier.
        /// </summary>
        /// <value>A UInt16 that specifies the unique identifier.</value>
        public static ushort NextId
        {
            get
            {
                lock (synchObject)
                {
                    if (nextId == UInt16.MaxValue)
                        nextId = UInt16.MinValue;
                    else
                        nextId++;

                    return nextId;
                }
            }
        }
    }
}
