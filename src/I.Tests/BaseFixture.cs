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

using NUnit.Framework;

namespace PPWCode.Util.DI.I.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Fixtures)]
public abstract class BaseFixture
{
    [SetUp]
    public void Setup()
    {
        OnSetup();
    }

    [TearDown]
    public void TearDown()
    {
        OnTearDown();
    }

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        OnOneTimeSetup();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        OnOneTimeTearDown();
    }

    protected virtual void OnOneTimeTearDown()
    {
    }

    protected virtual void OnOneTimeSetup()
    {
    }

    protected virtual void OnSetup()
    {
    }

    protected virtual void OnTearDown()
    {
    }
}
