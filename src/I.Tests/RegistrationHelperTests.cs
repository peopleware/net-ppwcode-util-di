// Copyright 2026 by PeopleWare n.v..
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections;

using NUnit.Framework;

namespace PPWCode.Util.DI.I.Tests;

public class RegistrationHelperTests : BaseFixture
{
    private static HashSet<Type> HashSet(params HashSet<Type> types)
        => types;

    public static IEnumerable RegistrationHelperTestCases
    {
        get
        {
            // non-generic cases
            yield return new TestCaseData(typeof(IA), typeof(A), HashSet(typeof(IA)));
            yield return new TestCaseData(typeof(IA), typeof(B), HashSet(typeof(IB)));
            yield return new TestCaseData(typeof(IA), typeof(C), HashSet(typeof(IC)));
            yield return new TestCaseData(typeof(IA), typeof(Bz), HashSet(typeof(IB), typeof(IZ)));
            yield return new TestCaseData(typeof(IA), typeof(Cz), HashSet(typeof(IC), typeof(IZ)));

            // generic cases
            yield return new TestCaseData(typeof(IRa<>), typeof(Ra<>), HashSet(typeof(IRa<>)));
            yield return new TestCaseData(typeof(IRa<>), typeof(Rb<>), HashSet(typeof(IRb<>)));
            yield return new TestCaseData(typeof(IRa<>), typeof(Rc), HashSet(typeof(IRc)));
            yield return new TestCaseData(typeof(IRa<>), typeof(Rd), HashSet(typeof(IRd)));
            yield return new TestCaseData(typeof(IRa<>), typeof(Rcd), HashSet(typeof(IRc), typeof(IRd), typeof(IRb<Eb>)));
            yield return new TestCaseData(typeof(IRa<>), typeof(RGenC), HashSet(typeof(IRb<Ec>)));
        }
    }

    [Test, TestCaseSource(nameof(RegistrationHelperTestCases))]
    public void verify_get_most_specific_interfaces(
        Type markerInterfaceType,
        Type implementationType,
        HashSet<Type> expected)
    {
        IEnumerable<Type> mostSpecificInterfaces = RegistrationHelper.GetMostSpecificInterfaces(markerInterfaceType, implementationType);
        Assert.That(mostSpecificInterfaces, Is.EquivalentTo(expected));
    }

    // @formatter:off
    // non-generic cases
    private interface IA;
    private interface IB : IA;
    private interface IC : IB;
    private interface IZ : IA;
    private abstract class A : IA;
    private class B : A, IB;
    private class C : A, IC;
    private class Bz : IB, IZ;
    private class Cz : IC, IZ;

    // generic cases
    private abstract class Ea;
    private abstract class Eb : Ea;
    private class Ec : Eb;
    private class Ed : Eb;
    private interface IRa<T>
        where T : Ea;
    private interface IRb<T> : IRa<T>
        where T : Eb;
    private interface IRc : IRb<Ec>;
    private interface IRd : IRb<Ed>;
    private class Ra<T> : IRa<T>
        where T : Ea;
    private class Rb<T> : IRb<T>
        where T : Eb;
    private class Rc : Rb<Ec>, IRc;
    private class Rd : Rb<Ed>, IRd;
    private class Rcd : Rb<Eb>, IRc, IRd;
    private class RGenC : Rb<Ec>;
}
