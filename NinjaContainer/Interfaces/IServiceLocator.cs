using System;
using System.Collections.Generic;
using System.Text;

namespace InversionOfControl.Interfaces
{
    public interface IServiceLocator
    {
        //get a service
        TService Get<TService>();
        //add a service
        TService Add<TService,TImplementation>();
        TService Add<TService, TImplementation>(object[] dependencies);
    }
}
