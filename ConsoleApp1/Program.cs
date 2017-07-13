using InversionOfControl;
using InversionOfControl.ClockManagement;
using InversionOfControl.Implementations;
using InversionOfControl.Interfaces;
using InversionOfControl.ModulesManagement;
using InversionOfControl.Services;
using System;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        private static INinjaContainer _ninjaContainer;
        //private static IServiceLocator _serviceLocator = new ServiceLocator(new TimeSpan(0, 0, 15));
        //private static IModuleLocator _moduleLocator = new ModuleLocator(new TimeSpan(0, 0, 10));
        //private static IClockManager _clock = new ClockManager(new TimeSpan(0, 0, 3));
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            _ninjaContainer = new NinjaContainer(new TimeSpan(0, 0, 5));

            _ninjaContainer.Bind<DispatcherTimer>();
            _ninjaContainer.Bind<IClockManager, ClockManager>(new object[] { new TimeSpan(0, 0, 10) });


            var a = _ninjaContainer.Resolve<DispatcherTimer>();
            a.Tick += A_Tick;
            //_moduleLocator.Add<DispatcherTimer>();
            //_moduleLocator.Add<ClockManager>(new object[] { new TimeSpan(0, 0, 4) });

            //_serviceLocator.Add<INinjaContainer, NinjaContainer>(new object[] { new TimeSpan(0,0,5) });

            //_clock.Added += _clock_Added;
            //_clock.Expired += _clock_Expired;
            //_clock.Removed += _clock_Removed;


            //_clock.Add("timothy");
            //Thread.Sleep(1000);
            //_clock.Add("maina");
            //Thread.Sleep(1000);
            //_clock.Add("macharia");
            //Thread.Sleep(1000);
            //_clock.Add("devTimmy");

            Console.ReadKey();
        }

        private static void A_Tick()
        {
            Console.WriteLine("Timer running in ninja container...");
        }

        //private static void _clock_Removed(string providerName)
        //{
        //    Console.WriteLine($"Successfully removed, {providerName}");
        //}

        //private static void _clock_Expired(string providerName)
        //{
        //    Console.WriteLine($"Item expired, {providerName}");
        //    Console.WriteLine("Removing item.");
        //    _clock.Remove(providerName);

        //    Console.WriteLine($"TRAPSOUL, Count = {_clock.Collection}");
        //    Console.WriteLine($"TRUE TO SELF, Count = {_clock.Collection}");

        //    Console.WriteLine("Adding item again..");
        //    _clock.Add(providerName);

        //}

        //private static void _clock_Added(string providerName, TimeSpan duration)
        //{
        //    Console.WriteLine($"{providerName}, added. Waiting time, {duration.ToString()}");
        //}

    }
}