﻿using System.Management;
using System.Runtime.Versioning;
using KrasnyyOktyabr.Application.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace KrasnyyOktyabr.Application.Services;

[SupportedOSPlatform("windows")]
public class WmiService(IMemoryCache cache, ILogger<WmiService> logger) : IWmiService
{
    public class CacheKeys
    {
        public static string Logons => nameof(WmiService) + "_" + nameof(Logons);
    }

    private static string Win32TerminalServicesWmiPath => @"\\.\root\cimv2\TerminalServices:Win32_TerminalServiceSetting.ServerName="".""";

    private static string LogonsPropertyName => "Logons";

    /// <returns><c>null</c> when read value is not neither <c>"0"</c> (<c>true</c>)
    /// or <c>"1"</c> (<c>false</c>)</returns>.
    public bool? AreRdSessionsAllowed()
    {
        object? logons = GetRdSessionsLogons();

        return logons switch
        {
            "0" => true,
            "1" => false,
            _ => null
        };
    }

    private object? GetRdSessionsLogons()
    {
        if (cache.TryGetValue(CacheKeys.Logons, out object? logons))
        {
            return logons;
        }

        if (logons != null)
        {
            return logons;
        }

        ManagementObject managementObject = new(Win32TerminalServicesWmiPath);

        logons = managementObject[LogonsPropertyName];

        logger.ReadingProperty(LogonsPropertyName, Win32TerminalServicesWmiPath);

        cache.Set(CacheKeys.Logons, logons, TimeSpan.FromMinutes(2));

        return logons;
    }
}