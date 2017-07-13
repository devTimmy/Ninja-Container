using DMS.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InversionOfControl.Interfaces
{
    public interface INinjaContainer
    {
        //refresh
        void Refresh();


        //bind
        T Bind<T>();
        T Bind<T>(object[] dependencies);
        TInterface Bind<TInterface, TImplementation>();
        TInterface Bind<TInterface, TImplementation>(object[] dependencies);


        //with time restriction
        T Bind<T>(TimeSpan duration);
        T Bind<T>(TimeSpan duration, object[] dependencies);
        TInterface Bind<TInterface, TImplementation>(TimeSpan duration);
        TInterface Bind<TInterface, TImplementation>(TimeSpan duration, object[] dependencies);



        //resolve
        T Resolve<T>();
        IRepository<T> ResolveRepository<T>(object context) where T : class;
    }
}
