using System;
using System.Collections.Generic;
using System.Reflection;
using Decoherence.SystemExtensions;

namespace Decoherence.CommandLineSerialization
{
    public class CommandLineApp : ICommandLineApp
    {
        private readonly Dictionary<string, Tuple<MethodBase, Func<object?>?>> mCommand2Method = new Dictionary<string, Tuple<MethodBase, Func<object?>?>>();
        private readonly List<Tuple<MethodBase, Func<object?>?>> mGlobalMethods = new List<Tuple<MethodBase, Func<object?>?>>();

        public event Action? AfterGlobalMethod;

        public int Start(IEnumerable<string> args)
        {
            LinkedList<string> argList = new LinkedList<string>(args);

            CommandLineSerializer commandLineSerializer = new CommandLineSerializer();

            foreach (var (methodBase, objGetter) in mGlobalMethods)
            {
                MethodInvoker.InvokeMethod(commandLineSerializer, methodBase, objGetter?.Invoke(), argList);
            }
            
            AfterGlobalMethod?.Invoke();

            var command = commandLineSerializer.DeserializeObject<string>(argList);
            if (string.IsNullOrWhiteSpace(command) || !mCommand2Method.TryGetValue(command, out var tuple))
            {
                throw new Exception($"No input command: {command}");
            }
            
            var ret = MethodInvoker.InvokeMethod(commandLineSerializer, tuple.Item1, tuple.Item2?.Invoke(), argList);
            if (ret is int retInt)
            {
                return retInt;
            }

            return 0;
        }

        public void AddGlobalMethod(MethodBase method, Func<object?>? objGetter)
        {
            ThrowUtil.ThrowIfArgumentNull(method);
            
            mGlobalMethods.Add(new Tuple<MethodBase, Func<object?>?>(method, objGetter));
        }

        public void AddCommand(MethodBase method, Func<object?>? objGetter, string? commandName = null)
        {
            ThrowUtil.ThrowIfArgumentNull(method);
            
            mCommand2Method.Add(commandName ?? method.Name, new Tuple<MethodBase, Func<object?>?>(method, objGetter));
        }
    }
}