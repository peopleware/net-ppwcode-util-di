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

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

namespace PPWCode.Util.DI.I.Tests;

public class AbstractHistoryTests : ServiceBasedTests
{
    [Test]
    public void can_create_when_all_dependencies_are_registered()
    {
        ServiceCollection.AddTransient<IFactory, Factory>();
        ServiceCollection.AddTransient<IServiceA, ServiceA>();
        ServiceCollection.AddTransient<IServiceB, ServiceB>();

        IFactory factory = ServiceProvider.GetRequiredService<IFactory>();
        IServiceA serviceA = factory.CreateServiceA();

        Assert.That(serviceA, Is.Not.Null);
        Assert.That(serviceA.DisposeCount, Is.EqualTo(0));
        Assert.That(serviceA.ServiceB, Is.Not.Null);
        Assert.That(serviceA.ServiceB.DisposeCount, Is.EqualTo(0));
    }

    [Test]
    public void use_dispose_properly_when_scope_goes_out_of_scope()
    {
        ServiceCollection.AddTransient<IFactory, Factory>();
        ServiceCollection.AddTransient<IServiceA, ServiceA>();
        ServiceCollection.AddTransient<IServiceB, ServiceB>();

        IServiceA serviceA;
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            IFactory factory = scope.ServiceProvider.GetRequiredService<IFactory>();
            serviceA = factory.CreateServiceA();
        }

        Assert.That(serviceA, Is.Not.Null);
        Assert.That(serviceA.DisposeCount, Is.EqualTo(1));
        Assert.That(serviceA.ServiceB, Is.Not.Null);
        Assert.That(serviceA.ServiceB.DisposeCount, Is.EqualTo(1));
    }

    [Test]
    public void can_create_when_not_all_dependencies_are_registered()
    {
        ServiceCollection.AddTransient<IFactory, Factory>();
        ServiceCollection.AddTransient<IServiceB, ServiceB>();

        IFactory factory = ServiceProvider.GetRequiredService<IFactory>();
        Context context = new();
        IServiceC serviceC = factory.CreateServiceC(context);

        Assert.That(serviceC, Is.Not.Null);
        Assert.That(serviceC.DisposeCount, Is.EqualTo(0));
        Assert.That(serviceC.Context, Is.EqualTo(context));
        Assert.That(serviceC.ServiceB, Is.Not.Null);
        Assert.That(serviceC.ServiceB.DisposeCount, Is.EqualTo(0));
    }

    [Test]
    public void use_dispose_properly_when_scope_goes_out_of_scope_when_not_all_dependencies_are_registered()
    {
        ServiceCollection.AddTransient<IFactory, Factory>();
        ServiceCollection.AddTransient<IServiceB, ServiceB>();

        Context context = new();
        IServiceC serviceC;
        using (IServiceScope scope = ServiceProvider.CreateScope())
        {
            IFactory factory = scope.ServiceProvider.GetRequiredService<IFactory>();
            serviceC = factory.CreateServiceC(context);
        }

        Assert.That(serviceC, Is.Not.Null);
        Assert.That(serviceC.DisposeCount, Is.EqualTo(1));
        Assert.That(serviceC.Context, Is.EqualTo(context));
        Assert.That(serviceC.ServiceB, Is.Not.Null);
        Assert.That(serviceC.ServiceB.DisposeCount, Is.EqualTo(1));
    }

    [Test]
    public void can_create_when_all_dependencies_are_registered_using_scope()
    {
        ServiceCollection.AddTransient<IFactory, Factory>();
        ServiceCollection.AddTransient<IServiceA, ServiceA>();
        ServiceCollection.AddTransient<IServiceB, ServiceB>();

        IFactory factory = ServiceProvider.GetRequiredService<IFactory>();
        ServiceA? serviceA = null;
        factory.ExecuteInNewScopeUsingServiceA(service =>
        {
            Assert.That(service, Is.Not.Null);
            Assert.That(service.DisposeCount, Is.EqualTo(0));
            Assert.That(service.ServiceB, Is.Not.Null);
            Assert.That(service.ServiceB.DisposeCount, Is.EqualTo(0));
            serviceA = service;
        });

        Assert.That(serviceA, Is.Not.Null);
        Assert.That(serviceA.DisposeCount, Is.EqualTo(1));
        Assert.That(serviceA.ServiceB, Is.Not.Null);
        Assert.That(serviceA.ServiceB.DisposeCount, Is.EqualTo(1));
    }

    [Test]
    public async Task can_create_when_all_dependencies_are_registered_using_scope_async()
    {
        ServiceCollection.AddTransient<IFactory, Factory>();
        ServiceCollection.AddTransient<IServiceA, ServiceA>();
        ServiceCollection.AddTransient<IServiceB, ServiceB>();

        IFactory factory = ServiceProvider.GetRequiredService<IFactory>();
        ServiceA? serviceA = null;
        await factory.ExecuteInNewScopeUsingServiceAAsync((service, _) =>
            {
                Assert.That(service, Is.Not.Null);
                Assert.That(service.DisposeCount, Is.EqualTo(0));
                Assert.That(service.ServiceB, Is.Not.Null);
                Assert.That(service.ServiceB.DisposeCount, Is.EqualTo(0));
                serviceA = service;
                return Task.CompletedTask;
            })
            .ConfigureAwait(false);

        Assert.That(serviceA, Is.Not.Null);
        Assert.That(serviceA.DisposeCount, Is.EqualTo(1));
        Assert.That(serviceA.ServiceB, Is.Not.Null);
        Assert.That(serviceA.ServiceB.DisposeCount, Is.EqualTo(1));
    }

    [Test]
    public void can_create_when_not_all_dependencies_are_registered_using_scope()
    {
        ServiceCollection.AddTransient<IFactory, Factory>();
        ServiceCollection.AddTransient<IServiceB, ServiceB>();

        IFactory factory = ServiceProvider.GetRequiredService<IFactory>();
        Context context = new();
        ServiceC? serviceC = null;
        factory.ExecuteInNewScopeUsingServiceC(
            (service, c) =>
            {
                Assert.That(service, Is.Not.Null);
                Assert.That(service.DisposeCount, Is.EqualTo(0));
                Assert.That(service.Context, Is.EqualTo(c));
                Assert.That(service.ServiceB, Is.Not.Null);
                Assert.That(service.ServiceB.DisposeCount, Is.EqualTo(0));
                serviceC = service;
            },
            context);

        Assert.That(serviceC, Is.Not.Null);
        Assert.That(serviceC.DisposeCount, Is.EqualTo(1));
        Assert.That(serviceC.Context, Is.EqualTo(context));
        Assert.That(serviceC.ServiceB, Is.Not.Null);
        Assert.That(serviceC.ServiceB.DisposeCount, Is.EqualTo(1));
    }

    [Test]
    public async Task can_create_when_not_all_dependencies_are_registered_using_scope_async()
    {
        ServiceCollection.AddTransient<IFactory, Factory>();
        ServiceCollection.AddTransient<IServiceB, ServiceB>();

        IFactory factory = ServiceProvider.GetRequiredService<IFactory>();
        Context context = new();
        ServiceC? serviceC = null;
        await factory.ExecuteInNewScopeUsingServiceCAsync(
            (service, c, _) =>
            {
                Assert.That(service, Is.Not.Null);
                Assert.That(service.DisposeCount, Is.EqualTo(0));
                Assert.That(service.Context, Is.EqualTo(c));
                Assert.That(service.ServiceB, Is.Not.Null);
                Assert.That(service.ServiceB.DisposeCount, Is.EqualTo(0));
                serviceC = service;
                return Task.CompletedTask;
            },
            context);

        Assert.That(serviceC, Is.Not.Null);
        Assert.That(serviceC.DisposeCount, Is.EqualTo(1));
        Assert.That(serviceC.Context, Is.EqualTo(context));
        Assert.That(serviceC.ServiceB, Is.Not.Null);
        Assert.That(serviceC.ServiceB.DisposeCount, Is.EqualTo(1));
    }

    [Test]
    public void can_create_when_all_dependencies_are_registered_using_scope_and_lambda_returns_data()
    {
        ServiceCollection.AddTransient<IFactory, Factory>();
        ServiceCollection.AddTransient<IServiceA, ServiceA>();
        ServiceCollection.AddTransient<IServiceB, ServiceB>();

        IFactory factory = ServiceProvider.GetRequiredService<IFactory>();
        ServiceA? serviceA = null;
        int result =
            factory.ExecuteInNewScopeUsingServiceA(service =>
            {
                Assert.That(service, Is.Not.Null);
                Assert.That(service.DisposeCount, Is.EqualTo(0));
                Assert.That(service.ServiceB, Is.Not.Null);
                Assert.That(service.ServiceB.DisposeCount, Is.EqualTo(0));
                serviceA = service;
                return 1;
            });
        Assert.That(result, Is.EqualTo(1));
        Assert.That(serviceA, Is.Not.Null);
        Assert.That(serviceA.DisposeCount, Is.EqualTo(1));
        Assert.That(serviceA.ServiceB, Is.Not.Null);
        Assert.That(serviceA.ServiceB.DisposeCount, Is.EqualTo(1));
    }

    [Test]
    public async Task can_create_when_all_dependencies_are_registered_using_scope_and_lambda_returns_data_async()
    {
        ServiceCollection.AddTransient<IFactory, Factory>();
        ServiceCollection.AddTransient<IServiceA, ServiceA>();
        ServiceCollection.AddTransient<IServiceB, ServiceB>();

        IFactory factory = ServiceProvider.GetRequiredService<IFactory>();
        ServiceA? serviceA = null;
        int result =
            await factory
                .ExecuteInNewScopeUsingServiceAAsync((service, _) =>
                {
                    Assert.That(service, Is.Not.Null);
                    Assert.That(service.DisposeCount, Is.EqualTo(0));
                    Assert.That(service.ServiceB, Is.Not.Null);
                    Assert.That(service.ServiceB.DisposeCount, Is.EqualTo(0));
                    serviceA = service;
                    return Task.FromResult(1);
                });

        Assert.That(result, Is.EqualTo(1));
        Assert.That(serviceA, Is.Not.Null);
        Assert.That(serviceA.DisposeCount, Is.EqualTo(1));
        Assert.That(serviceA.ServiceB, Is.Not.Null);
        Assert.That(serviceA.ServiceB.DisposeCount, Is.EqualTo(1));
    }
}
