using System;
using System.Collections.Generic;

/// <summary>
/// Merkezi servis yöneticisi - Global servis erişimi sağlar
/// GameObject.Find() yerine hızlı dictionary lookup kullanır
/// </summary>
public static class ServiceLocator
{
    private static readonly Dictionary<Type, IService> services = new Dictionary<Type, IService>();

    public static void Register<T>(IService service) where T : IService
    {
        Type serviceType = typeof(T);
        if (services.ContainsKey(serviceType))
        {
            UnityEngine.Debug.LogWarning($"[ServiceLocator] {serviceType.Name} zaten kayıtlı!");
        }
        services[serviceType] = service;
    }

    public static T Get<T>() where T : IService
    {
        Type serviceType = typeof(T);
        if (!services.ContainsKey(serviceType))
        {
            throw new Exception($"[ServiceLocator] {serviceType.Name} bulunamadı!");
        }
        return (T)services[serviceType];
    }

    public static bool IsRegistered<T>() where T : IService
    {
        return services.ContainsKey(typeof(T));
    }

    public static void Reset()
    {
        foreach (var service in services.Values)
        {
            service.Cleanup();
        }
        services.Clear();
    }

    public static void InitializeAll()
    {
        foreach (var service in services.Values)
        {
            service.Initialize();
        }
    }

    public static void TickAll()
    {
        foreach (var service in services.Values)
        {
            service.Tick();
        }
    }
}
