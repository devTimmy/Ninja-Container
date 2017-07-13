using InversionOfControl.ClockManagement;
using InversionOfControl.Interfaces;
using InversionOfControl.ModulesManagement;
using InversionOfControl.Services;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using DMS.Common.Interfaces;
using DMS.Common.Implementations;

namespace InversionOfControl.Implementations
{
    public class NinjaContainer : INinjaContainer
    {
        #region Local Variables
        private static NinjaContainer Instance;

        private IModuleLocator _moduleLocator;
        private IServiceLocator _serviceLocator;
        #endregion

        public NinjaContainer(TimeSpan defaultRunTime)
        {
            _moduleLocator = new ModuleLocator(defaultRunTime);
            _serviceLocator = new ServiceLocator(defaultRunTime);
        }

        public static NinjaContainer GetInstance()
        {
            if (Instance == null)
            {
                Instance = new NinjaContainer(new TimeSpan(0, 0, 5));
                return Instance;
            }
            else
            {
                return Instance;
            }
        }

        #region Binding Modules
        public T Bind<T>()
        {
            return _moduleLocator.Add<T>();
        }

        public T Bind<T>(object[] dependencies)
        {
            return _moduleLocator.Add<T>(dependencies);
        }
        public T Bind<T>(TimeSpan duration)
        {
            throw new NotImplementedException();
        }
        public T Bind<T>(TimeSpan duration, object[] dependencies)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Binding Services
        public TInterface Bind<TInterface, TImplementation>()
        {
            return _serviceLocator.Add<TInterface, TImplementation>();
        }
        public TInterface Bind<TInterface, TImplementation>(object[] dependencies)
        {
            return _serviceLocator.Add<TInterface, TImplementation>(dependencies);
        }
        public TInterface Bind<TInterface, TImplementation>(TimeSpan duration)
        {
            throw new NotImplementedException();
        }
        public TInterface Bind<TInterface, TImplementation>(TimeSpan duration, object[] dependencies)
        {
            throw new NotImplementedException();
        }
        #endregion


        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>()
        {
            if (typeof(T).GetTypeInfo().IsInterface)
                return _serviceLocator.Get<T>();
            else if (typeof(T).GetTypeInfo().IsClass)
                return _moduleLocator.Get<T>();
            else
                return default(T);
        }

        public IRepository<T> ResolveRepository<T>(object context) where T : class
        {
            //check availability
            var obj = _serviceLocator.Get<IRepository<T>>();

            if (obj != null)
                return obj;
            else
            {
                //create the repository
                return _serviceLocator.Add<IRepository<T>, Repository<T>>(new object[] { context });
            }
        }
    }
}
