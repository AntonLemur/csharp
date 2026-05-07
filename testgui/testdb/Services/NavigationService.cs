using System;
using Microsoft.Extensions.DependencyInjection;
using testdb.ViewModels;
using testdb.Abstractions;
using System.Threading.Tasks;

public class NavigationService
{
    private readonly IServiceProvider _provider;

    public NavigationService(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<T> NavigateTo<T>() where T : class
    {
        var vm = _provider.GetRequiredService<T>();

        // если есть InitializeAsync — вызываем
        if (vm is IAsyncInitializable init)
            await init.InitializeAsync();

        return vm;
    }
}
