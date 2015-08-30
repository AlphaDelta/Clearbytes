using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Clearbytes
{
    public class Async
    {
        public delegate void Action();

        public static void RunAsync(Action action)
        {
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += delegate { action(); bg.Dispose(); };
            bg.RunWorkerAsync();
        }
    }
}
