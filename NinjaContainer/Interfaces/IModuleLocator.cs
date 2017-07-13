using System;
using System.Collections.Generic;
using System.Text;

namespace InversionOfControl.Interfaces
{
    public interface IModuleLocator
    {
        //get a module
        TModule Get<TModule>();
        //add a module
        TModule Add<TModule>(object[] dependencies);
        TModule Add<TModule>();
    }
}
