using Decoherence.SystemExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
#pragma warning disable 8777

namespace Decoherence
{
#if HIDE_DECOHERENCE
    internal static class ThrowUtil
#else
    public static class ThrowUtil
#endif
    {
        public static void ThrowIfArgumentNull([NotNull] object? arg, string? argName = null, string? message = null)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(argName, message);
            }
        }
        
        public static void ThrowIfArgumentNullOrWhiteSpace([NotNull] string? arg, string? paramName = null, string? message = null)
        {
            if (string.IsNullOrWhiteSpace(arg))
            {
                throw new ArgumentException(message ?? "Value cannot be null or whitespace.", paramName);
            }
        }
        
        public static void ThrowIfArgument([DoesNotReturnIf(true)] bool condition, string? paramName = null, string? message = null)
        {
            if (condition)
            {
                throw new ArgumentException(message ?? "Invalid argument.", paramName);
            }
        }

        public static void ThrowIfPathNotFound([NotNull] string? path)
        {
            if (!Directory.Exists(path) && !File.Exists(path))
            {
                throw new PathNotFoundException($"The path of '{path}' contains neither files nor directories.");
            }
        }

        public static void ThrowIfFileNotFound([NotNull] string? path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"The file could not be found under the path of '{path}'.", path);
            }
        }

        public static void ThrowIfDirectoryNotFound([NotNull] string? path)
        {
            if (Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"The directory could not be found under the path of '{path}'.");
            }
        }

        
    }
}
