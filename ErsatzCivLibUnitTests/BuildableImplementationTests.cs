using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ErsatzCivLib.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ErsatzCivLibTests
{
    [TestClass]
    [Ignore]
    public class BuildableImplementationTests
    {
        private const string DefaultFieldName = "Default";
        private const string CreateMethodName = "CreateAtLocation";

        [TestMethod]
        public void Check_Default_FieldOnBuildableConcreteClasses()
        {
            var types = GetBuildableConcreteTypes();
            foreach (var type in types)
            {
                var field = type.GetField(DefaultFieldName, BindingFlags.NonPublic | BindingFlags.Static);
                Assert.IsNotNull(field);
                Assert.IsTrue(field.IsAssembly);
                Assert.IsTrue(field.IsInitOnly);
                Assert.AreEqual(type, field.FieldType);
            }
        }

        [TestMethod]
        public void Check_CreateAtLocation_MethodOnBuildableConcreteClasses()
        {
            var types = GetBuildableConcreteTypes();
            foreach (var type in types)
            {
                var method = type.GetMethod(CreateMethodName, BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(CityPivot) }, null);
                Assert.IsNotNull(method);
                Assert.IsTrue(method.IsAssembly);
                Assert.AreEqual(type, method.ReturnType);
            }
        }

        private List<Type> GetBuildableConcreteTypes()
        {
            return Assembly
                .GetAssembly(typeof(UnitPivot))
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(UnitPivot)) && !t.IsAbstract)
                .ToList();
        }
    }
}
