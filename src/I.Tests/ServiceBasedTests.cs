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

namespace PPWCode.Util.DI.I.Tests;

public abstract class ServiceBasedTests : BaseFixture
{
    protected ServiceCollection ServiceCollection { get; } = new();
    private IServiceProvider? _serviceProvider;

    protected IServiceProvider ServiceProvider
        => _serviceProvider ??= ServiceCollection.BuildServiceProvider();

    /// <inheritdoc />
    protected override void OnTearDown()
    {
        _serviceProvider = null;
        ServiceCollection.Clear();

        base.OnTearDown();
    }
}
