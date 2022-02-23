using System;
using System.Collections.Generic;
using System.Reflection;

namespace Decoherence.CommandLineSerialization
{
    public class CommandLineApp
    {
        private readonly Dictionary<string, MethodBase> mCommand2Method = new Dictionary<string, MethodBase>();
        private MethodBase? mGlobalMethod;

        public int Start(IEnumerable<string> args)
        {
            LinkedList<string> argList = new LinkedList<string>(args);

            CommandLineSerializer commandLineSerializer = new CommandLineSerializer();

            if (mGlobalMethod != null)
            {
                MethodInvoker.InvokeMethod(commandLineSerializer, mGlobalMethod, null, argList);
            }

            var command = commandLineSerializer.DeserializeObject<string>(argList);
            if (string.IsNullOrWhiteSpace(command) || !mCommand2Method.TryGetValue(command, out var method))
            {
                throw new Exception($"No input command: {command}");
            }
            
            var ret = MethodInvoker.InvokeMethod(commandLineSerializer, method, null, argList);
            if (ret is int retInt)
            {
                return retInt;
            }

            return 0;
        }

        public void SetGlobalMethod(Type type, string methodName)
        {
            var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            ThrowUtil.ThrowIfArgumentNull(method);

            mGlobalMethod = method;
        }
        
        public void AddCommand(Type type, string methodName)
        {
            var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            ThrowUtil.ThrowIfArgumentNull(method);
            
            mCommand2Method.Add(methodName, method);
        }
    }
}