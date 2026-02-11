// Copyright 2025 by PeopleWare n.v..
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace PPWCode.Util.DI.I;

public abstract class AbstractFactory
{
    private readonly MethodInfo? _captureDisposableMethod;
    private readonly IServiceProvider _serviceProvider;

    protected AbstractFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _captureDisposableMethod =
            serviceProvider
                .GetType()
                .GetMethod("CaptureDisposable", BindingFlags.NonPublic | BindingFlags.Instance);
    }

    private T Create<T>(IServiceProvider serviceProvider, params object[] parameters)
    {
        T service = ActivatorUtilities.CreateInstance<T>(serviceProvider, parameters);
        _captureDisposableMethod?.Invoke(serviceProvider, [service]);
        return service;
    }

    private void ExecuteInNewScope<TService, TContext>(
        Action<TService, TContext> action,
        TContext context,
        IServiceProvider serviceProvider,
        params object[] parameters)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        TService service = Create<TService>(scope.ServiceProvider, parameters);
        action(service, context);
    }

    private TResult ExecuteInNewScope<TService, TContext, TResult>(
        Func<TService, TContext, TResult> action,
        TContext context,
        IServiceProvider serviceProvider,
        params object[] parameters)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        TService service = Create<TService>(scope.ServiceProvider, parameters);
        return action(service, context);
    }

    private async Task ExecuteInNewScopeAsync<TService, TContext>(
        Func<TService, TContext, CancellationToken, Task> action,
        TContext context,
        IServiceProvider serviceProvider,
        object[] parameters,
        CancellationToken cancellationToken = default)
    {
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        TService service = Create<TService>(scope.ServiceProvider, parameters);
        await action(service, context, cancellationToken).ConfigureAwait(false);
    }

    private async Task<TResult> ExecuteInNewScopeAsync<TService, TContext, TResult>(
        Func<TService, TContext, CancellationToken, Task<TResult>> action,
        TContext context,
        IServiceProvider serviceProvider,
        object[] parameters,
        CancellationToken cancellationToken = default)
    {
        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        TService service = Create<TService>(scope.ServiceProvider, parameters);
        return await action(service, context, cancellationToken);
    }

    protected T Create<T>(params object[] parameters)
        => Create<T>(_serviceProvider, parameters);

    protected void ExecuteInNewScope<TService, TContext>(
        Action<TService, TContext> action,
        TContext context,
        object[]? parameters = null)
        => ExecuteInNewScope(action, context, _serviceProvider, parameters ?? []);

    protected TResult ExecuteInNewScope<TService, TContext, TResult>(
        Func<TService, TContext, TResult> action,
        TContext context,
        object[]? parameters = null)
        => ExecuteInNewScope(action, context, _serviceProvider, parameters ?? []);

    protected void ExecuteInNewScope<TService>(
        Action<TService> action,
        object[]? parameters = null)
        => ExecuteInNewScope<TService, int>((service, _) => action(service), 0, _serviceProvider, parameters ?? []);

    protected TResult ExecuteInNewScope<TService, TResult>(
        Func<TService, TResult> action,
        object[]? parameters = null)
        => ExecuteInNewScope<TService, int, TResult>((service, _) => action(service), 0, _serviceProvider, parameters ?? []);

    protected async Task ExecuteInNewScopeAsync<TService, TContext>(
        Func<TService, TContext, CancellationToken, Task> action,
        TContext context,
        object[]? parameters = null,
        CancellationToken cancellationToken = default)
        => await ExecuteInNewScopeAsync(action, context, _serviceProvider, parameters ?? [], cancellationToken).ConfigureAwait(false);

    protected async Task<TResult> ExecuteInNewScopeAsync<TService, TContext, TResult>(
        Func<TService, TContext, CancellationToken, Task<TResult>> action,
        TContext context,
        object[]? parameters = null,
        CancellationToken cancellationToken = default)
        => await ExecuteInNewScopeAsync(action, context, _serviceProvider, parameters ?? [], cancellationToken).ConfigureAwait(false);

    protected async Task ExecuteInNewScopeAsync<TService>(
        Func<TService, CancellationToken, Task> action,
        object[]? parameters = null,
        CancellationToken cancellationToken = default)
        => await ExecuteInNewScopeAsync<TService, int>(async (service, _, can) => await action(service, can), 0, _serviceProvider, parameters ?? [], cancellationToken).ConfigureAwait(false);

    protected async Task<TResult> ExecuteInNewScopeAsync<TService, TResult>(
        Func<TService, CancellationToken, Task<TResult>> action,
        object[]? parameters = null,
        CancellationToken cancellationToken = default)
        => await ExecuteInNewScopeAsync<TService, int, TResult>(async (service, _, can) => await action(service, can), 0, _serviceProvider, parameters ?? [], cancellationToken).ConfigureAwait(false);
}
