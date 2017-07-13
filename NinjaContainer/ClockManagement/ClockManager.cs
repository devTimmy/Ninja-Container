using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using InversionOfControl.Interfaces;
using System.Threading.Tasks;

namespace InversionOfControl.ClockManagement
{
    public delegate void AddedEventHandler(string providerName,TimeSpan duration);
    public delegate void ExpiredEventHandler(string providerName);
    public delegate void RemoveEventHandler(string providerName);
    public delegate void TimerTickEventHandler();

    public class ClockManager : IClockManager
    {
        #region Local Variables
        private readonly DispatcherTimer _dispatcherTimer;
        private readonly IDictionary<string, DateTime> _providers;
        private TimeSpan _defaultTime;
        #endregion


        #region Event Handlers
        public event AddedEventHandler Added;
        public event ExpiredEventHandler Expired;
        public event RemoveEventHandler Removed;
        public event TimerTickEventHandler Tick;
        #endregion

        public ClockManager(TimeSpan defaultValue)
        {
            _dispatcherTimer = new DispatcherTimer();
            _providers = new Dictionary<string, DateTime>();
            _defaultTime = defaultValue;

            //event handler - for every 1 second
            _dispatcherTimer.Tick += OnTimerTicked;
            Expired += ClockManager_Expired;
        }

        private void ClockManager_Expired(string providerName)
        {
        }

        public int Collection => _providers.Count;


        public bool Add(string providerName)
        {
            return Add(providerName, new TimeSpan(0, 0, 0));
        }
        public bool Add(string providerName, TimeSpan duration)
        {
            //perform validation
            if (!Validation(providerName, duration))
                return false;


            //resolve duration
            TimeSpan span = ResolveDuration(duration);


            //check if exists
            if(!_providers.ContainsKey(providerName))
            {
                //add
                DateTime expiry = DateTime.Now.Add(span);
                _providers.Add(providerName, expiry);

                //call added event handler
                if (Added != null)
                    Added(providerName, span);
                else { };
            }

            return true;
        }
        public bool Remove(string providerName)
        {
            if (!_providers.ContainsKey(providerName))
                return false;


            //remove
            _providers.Remove(providerName);

            if (Removed != null)
            {
                Removed(providerName);
                return true;
            }
            else
                return false;
        }


        private TimeSpan ResolveDuration(TimeSpan duration)
        {
            if(duration.TotalMilliseconds <= 1)
            {
                //set default time as passed by developer
                duration = _defaultTime;
                return duration;
            }
            else
            {
                return duration;
            }
        }
        private bool Validation(string providerName, TimeSpan duration)
        {
            if (string.IsNullOrWhiteSpace(providerName))
                return false;


            if (duration == null)
                return false;


            return true;
        }
        protected virtual void OnTimerTicked()
        {
            if (Tick != null)
                Tick();
            else { };

            Task task = new Task(TimerTickerTask);
            task.Start();
        }
        private void TimerTickerTask()
        {
            if (_providers.Count > 0)
            {
                IList<string> expiredItems = new List<string>();
                var array = _providers.ToArray();

                //check expired items
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].Value <= DateTime.Now)
                    {
                        expiredItems.Add(array[i].Key);
                    }
                }


                //raise event handlers for expired items
                if(expiredItems.Count > 0)
                {
                    for (int i = 0; i < expiredItems.Count; i++)
                    {
                        //event
                        if (Expired != null)
                            Expired(expiredItems[i]);
                        else { };

                    }
                }
            }

        }
    }
}
