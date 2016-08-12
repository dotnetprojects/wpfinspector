using System;

namespace ChristianMoser.WpfInspector.Utilities
{
    public class EventArgs<T> : EventArgs
    {
        public T Data { get; set; }
    }
}
