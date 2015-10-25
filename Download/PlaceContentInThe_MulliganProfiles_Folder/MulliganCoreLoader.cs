using System;
using System.Collections.Generic;
using System.Reflection;
using SmartBot.Plugins.API;

namespace SmartBot.Mulligan
{
    [Serializable]
    public class DefaultMulliganProfile : MulliganProfile
    {
        private static string dllPath = Environment.CurrentDirectory +
                                        @"\HearthstoneMulligan.dll";

        public List<Card.Cards> HandleMulligan(List<Card.Cards> choices, Card.CClass opponentClass, Card.CClass ownClass)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

            AppDomain domain = AppDomain.CreateDomain("MulliganCoreDomain");
            object result = null;

            try
            {
                AssemblyName assemblyName = new AssemblyName { CodeBase = dllPath };

                Assembly assembly = domain.Load(assemblyName);
                Type type = assembly.GetType("HearthstoneMulligan.DllMain");

                MethodInfo methodInfo = type.GetMethod("HandleMulligan");

                object classInstance = Activator.CreateInstance(type, null);
                object[] parametersArray = { choices, opponentClass, ownClass };
                result = methodInfo.Invoke(classInstance, parametersArray);
            }
            finally
            {
                AppDomain.Unload(domain);
            }

            return (List<Card.Cards>)result;
        }
    }
}
