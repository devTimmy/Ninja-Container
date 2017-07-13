using InversionOfControl.ClockManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace InversionOfControl.Interfaces
{
    public interface IClockManager
    {
        //add item to clock
        bool Add(string providerName);
        bool Add(string providerName, TimeSpan duration);
        //remove
        bool Remove(string providerName);
        //collection
        int Collection { get; }


        //added event handler
        event AddedEventHandler Added;
        //expired event handler
        event ExpiredEventHandler Expired;
        //removed event handler
        event RemoveEventHandler Removed;
        //tick
        event TimerTickEventHandler Tick;
    }

}
