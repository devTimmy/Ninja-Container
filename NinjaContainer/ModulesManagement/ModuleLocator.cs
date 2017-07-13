using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using InversionOfControl.Interfaces;
using InversionOfControl.ClockManagement;

namespace InversionOfControl.ModulesManagement
{
    public class ModuleLocator : IModuleLocator
    {
        #region Local Variables
        private ClockManager _clockManager;
        private readonly IDictionary<string, Tuple<object,object[]>> _modules;
        #endregion

        public ModuleLocator(TimeSpan defaultRunTime)
        {
            _clockManager = new ClockManager(defaultRunTime);
            _modules = new Dictionary<string, Tuple<object,object[]>>();

            //event handler
            _clockManager.Added += _clockManager_Added;
            _clockManager.Expired += _clockManager_Expired;
            _clockManager.Removed += _clockManager_Removed;

        }

        private void _clockManager_Removed(string providerName)
        {
            //set console color to green
            //Console.ForegroundColor = ConsoleColor.Red;

            //Console.WriteLine($"Module Removed...[{providerName}]");
        }

        private void _clockManager_Added(string providerName, TimeSpan duration)
        {
            //set console color to green
            //Console.ForegroundColor = ConsoleColor.DarkCyan;

            //Console.WriteLine($"Module Added:{providerName}, Duration : {duration.ToString()}");
        }

        private void _clockManager_Expired(string providerName)
        {
            //set console color to green
            //Console.ForegroundColor = ConsoleColor.Yellow;

            //Console.WriteLine($"Module: {providerName}, Expired!");

            //get expired item
            Tuple<object,object[]> oldObject;

            _modules.TryGetValue(providerName, out oldObject);

            
            if(oldObject != null)
            {
                //remove items from clock and modules locator
                _modules.Remove(providerName);
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
                else if(parameters.Length > 1)
                {
                    newObject = constructor.Invoke(new object[] { oldObject.Item2 });
                }
                else
                    newObject = constructor.Invoke(null);

                //re-add
                _modules.Add(providerName, new Tuple<object, object[]>(newObject,oldObject.Item2));
                _clockManager.Add(providerName);
            }

        }

        public TModule Add<TModule>()
        {
            return Add<TModule>(null);
        }
        public TModule Add<TModule>(object[] dependencies)
        {
            string moduleName = typeof(TModule).GetTypeInfo().FullName;


            //exists?
            Tuple<object,object[]> value;
            _modules.TryGetValue(moduleName, out value);
            if (value != null)
                return (TModule)value.Item1;

            //add
            object implementation = ResolveModuleInner(typeof(TModule), dependencies);

            if (implementation == null)
                throw new Exception("Module does contain a constructor. It may be a static class or an interface which cannot be initialized.");
            else
                _modules.Add(moduleName, new Tuple<object, object[]>(implementation,dependencies));

            //add to clock manager
            _clockManager.Add(moduleName);
            //return added object
            _modules.TryGetValue(moduleName,out value);

            return (TModule)value.Item1;
        }
        private object ResolveModuleInner(Type type,object[] dependencies)
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
                        throw new Exception("Parameter count mismatch. This instance requires some dependencies passed to it through its constructor.");
                }
                else
                    return constructor.Invoke(null);
            }
        }
        public TModule Get<TModule>()
        {
            Tuple<object,object[]> value;

            _modules.TryGetValue(typeof(TModule).GetTypeInfo().FullName, out value);

            return (TModule)value.Item1;
        }

        
    }
}
