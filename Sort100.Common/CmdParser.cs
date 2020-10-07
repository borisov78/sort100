using System;
using Microsoft.Extensions.Configuration;

namespace Sort100.Common
{
    public sealed class CmdParser
    {
        private readonly IConfigurationRoot _config;
        public bool IsEmpty { get; }

        public CmdParser(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args);
            _config = builder.Build();
            IsEmpty = args == null || args.Length == 0;
        }

        private T InternalGet<T>(string paramName, T defaultValue, bool raiseIfNotExists)
        {
            var paramValue = _config[paramName];
            return paramValue switch
            {
                null when raiseIfNotExists => throw new ArgumentException($"Invalid parameter '{paramName}.'"),
                null => defaultValue,
                _ => (T) Convert.ChangeType(paramValue, typeof(T))
            };
        }
        
        public T GetOrDefault<T>(string paramName, T defaultValue)
        {
            return InternalGet(paramName, defaultValue, false);
        }

        public T Get<T>(string paramName)
        {
            return InternalGet(paramName, default(T), true);
        }
    }
}