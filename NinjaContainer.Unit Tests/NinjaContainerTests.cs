using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InversionOfControl;
using InversionOfControl.Implementations;
using InversionOfControl.Interfaces;
using InversionOfControl.ClockManagement;

namespace NinjaContainer.Unit_Tests
{
    [TestClass]
    public class NinjaContainerTests
    {
        private readonly INinjaContainer _ninjaContainer = new InversionOfControl.Implementations.NinjaContainer(new TimeSpan(0, 0, 2));

        [TestMethod]
        public void BindModule()
        {
            var x = _ninjaContainer.Bind<Random>();

            Assert.IsNotNull(x);
        }
        [TestMethod]
        public void BindService()
        {
            var x = _ninjaContainer.Bind<IClockManager, ClockManager>(new object[] { new TimeSpan(0,0,2) });

            Assert.IsNotNull(x);
        }

        [TestMethod]
        public void ResolveModule()
        {
            _ninjaContainer.Bind<Random>();
            var x = _ninjaContainer.Resolve<Random>();

            Assert.IsNotNull(x);
        }
        [TestMethod]
        public void ResolveService()
        {
            _ninjaContainer.Bind<IClockManager, ClockManager>(new object[] { new TimeSpan(0, 0, 2) });
            var x = _ninjaContainer.Resolve<IClockManager>();

            Assert.IsNotNull(x);
        }
    }
}
