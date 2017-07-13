using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace InversionOfControl.ClockManagement
{
    public delegate void TickEventHandler();

    public class DispatcherTimer
    {
        private readonly Timer timer;
        public event TickEventHandler Tick;

        public DispatcherTimer()
        {
            timer = new Timer(OnTick, null, 0, 1000);
        }

         

        protected virtual void OnTick(object state)
        {
            if (Tick != null)
                Tick();
            else { };
        }
        
    }
}
