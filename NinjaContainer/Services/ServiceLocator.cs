using InversionOfControl.Interfaces;
using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using InversionOfControl.ClockManagement;

namespace InversionOfControl.Services
{
    public class ServiceLocator : IServiceLocator
    {
        #region Local Variables
        private readonly IClockManager _clockManager;
        private readonly IDictionary<string, Tuple<object,object[]>> _services;
        #endregion

        public ServiceLocator(TimeSpan initial)
        {
            _clockManager = new ClockManager(initial);
            _services = new Dictionary<string, Tuple<object, object[]>>();


            //events
            _clockManager.Added += _clockManager_Added;
            _clockManager.Removed += _clockManager_Removed;
            _clockManager.Expired += _clockManager_Expired;
        }

        private void _clockManager_Expired(string providerName)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Service: {providerName}, Expired!");
            //get expired item
            Tuple<object, object[]> oldObject;

            _services.TryGetValue(providerName, out oldObject);


            if (oldObject != null)
            {
                //remove items from clock and modules locator
                _services.Remove(providerName);
                _clockManager.Remove(providerName);
                //create new instance
                var constructor = oldObject.Item1.GetType().GetTypeInfo().DeclaredConstructors.ToArray()[0];
                var parameters = constructor.GetParameters();

                //initialize constructor
                object newObject = null;

                if (parameters.Length == 1)
                {
                    newObject = constructor.Invoke(new object[] { oldObject.Item2[0] });
                }
                else if (parameters.Length > 1)
                {
                    newObject = constructor.Invoke(new object[] { oldObject.Item2 });
                }
                else
                    newObject = constructor.Invoke(null);

                //re-add
                _services.Add(providerName, new Tuple<object, object[]>(newObject, oldObject.Item2));
                _clockManager.Add(providerName);
            }

        }

        private void _clockManager_Removed(string providerName)
        {
            Console.WriteLine($"Service Removed...[{providerName}]");

        }

        private void _clockManager_Added(string providerName, TimeSpan duration)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Service Added:{providerName}, Duration : {duration.ToString()}");
        }

        public TService Add<TService, TImplementation>()
        {
            return ResolveInner<TService, TImplementation>(null);
        }
        private TService ResolveInner<TService, TImplementation>(object[] dependencies)
        {
            //compatibility check
            if (typeof(TImplementation).GetTypeInfo().IsAssignableFrom(typeof(TService).GetTypeInfo()))
                throw new Exception("Incompatible service and implementation. Provided implemetation cannot be assigned from service");


            object obj = ResolveImplementation(typeof(TImplementation), dependencies);


            if(!_services.ContainsKey(typeof(TService).FullName))
            {
                _services.Add(typeof(TService).FullName, new Tuple<object, object[]>(obj, dependencies));
                _clockManager.Add(typeof(TService).FullName);
            }

            //return newly created service
            return Get<TService>();
        }

        private object ResolveImplementation(Type type, object[] dependencies)
        {
            var constructors = type.GetTypeInfo().DeclaredConstructors.ToArray();


            if (constructors.Length < 1)
                return null;
            else
            {
                var constructor = constructors[0];
                var parameters = constructor.GetParameters();

                if (parameters.Length > 0)
                {
                    if (dependencies != null && dependencies.Length > 0)
                        return constructor.Invoke(dependencies);
                    else
                        throw new Exception("Parameter count mismatch. This instance requires dependencies passed to it through its constructor.");
                }
                else
                    return constructor.Invoke(null);
            }
        }

        public TService Add<TService, TImplementation>(object[] dependencies)
        {
            return ResolveInner<TService, TImplementation>(dependencies);
        }

        public TService Get<TService>()
        {
            Tuple<object, object[]> obj;

            _services.TryGetValue(typeof(TService).FullName, out obj);

            if (obj != null)
                return (TService)obj.Item1;
            else
                return default(TService);
        }
    }
}
