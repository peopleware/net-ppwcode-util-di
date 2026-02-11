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

namespace PPWCode.Util.DI.I.Tests;

public interface IServiceA : IDisposable
{
    ServiceB? ServiceB { get; }
    int DisposeCount { get; }
}

public interface IServiceB : IDisposable
{
    int DisposeCount { get; }
}

public interface IServiceC : IDisposable
{
    ServiceB? ServiceB { get; }
    Context Context { get; }
    int DisposeCount { get; }
}

public class Context
{
}

public interface IFactory
{
    IServiceA CreateServiceA();
    IServiceB CreateServiceB();
    IServiceC CreateServiceC(Context context);

    void ExecuteInNewScopeUsingServiceA(Action<ServiceA> action);
    int ExecuteInNewScopeUsingServiceA(Func<ServiceA, int> action);

    Task ExecuteInNewScopeUsingServiceAAsync(Func<ServiceA, CancellationToken, Task> action, CancellationToken cancellationToken = default);
    Task<int> ExecuteInNewScopeUsingServiceAAsync(Func<ServiceA, CancellationToken, Task<int>> action, CancellationToken cancellationToken = default);

    void ExecuteInNewScopeUsingServiceC(Action<ServiceC, Context> action, Context context);
    Task ExecuteInNewScopeUsingServiceCAsync(Func<ServiceC, Context, CancellationToken, Task> action, Context context, CancellationToken cancellationToken = default);
}

public sealed class ServiceA : IServiceA
{
    private int _disposeCount;

    public ServiceA(IServiceB serviceB)
    {
        ServiceB = serviceB as ServiceB;
    }

    public ServiceB? ServiceB { get; }

    public int DisposeCount
        => _disposeCount;

    /// <inheritdoc />
    public void Dispose()
        => _disposeCount++;
}

public sealed class ServiceB : IServiceB
{
    private int _disposeCount;

    public int DisposeCount
        => _disposeCount;

    /// <inheritdoc />
    public void Dispose()
        => _disposeCount++;
}

public sealed class ServiceC : IServiceC
{
    private int _disposeCount;

    public ServiceC(IServiceB serviceB, Context context)
    {
        Context = context;
        ServiceB = serviceB as ServiceB;
    }

    public Context Context { get; }

    public ServiceB? ServiceB { get; }

    public int DisposeCount
        => _disposeCount;

    /// <inheritdoc />
    public void Dispose()
        => _disposeCount++;
}

public class Factory
    : AbstractFactory,
      IFactory
{
    public Factory(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    public IServiceA CreateServiceA()
        => Create<ServiceA>();

    public IServiceB CreateServiceB()
        => Create<ServiceB>();

    public IServiceC CreateServiceC(Context context)
        => Create<ServiceC>(context);

    public void ExecuteInNewScopeUsingServiceA(Action<ServiceA> action)
        => ExecuteInNewScope(action);

    public int ExecuteInNewScopeUsingServiceA(Func<ServiceA, int> action)
        => ExecuteInNewScope(action);

    public async Task ExecuteInNewScopeUsingServiceAAsync(Func<ServiceA, CancellationToken, Task> action, CancellationToken cancellationToken = default)
        => await ExecuteInNewScopeAsync(action, null, cancellationToken);

    public async Task<int> ExecuteInNewScopeUsingServiceAAsync(Func<ServiceA, CancellationToken, Task<int>> action, CancellationToken cancellationToken = default)
        => await ExecuteInNewScopeAsync(action, null, cancellationToken);

    public void ExecuteInNewScopeUsingServiceC(Action<ServiceC, Context> action, Context context)
        => ExecuteInNewScope(action, context, [context]);

    public async Task ExecuteInNewScopeUsingServiceCAsync(Func<ServiceC, Context, CancellationToken, Task> action, Context context, CancellationToken cancellationToken = default)
        => await ExecuteInNewScopeAsync(action, context, [context], cancellationToken);
}
