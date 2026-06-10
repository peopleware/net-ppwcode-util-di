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

using PPWCode.Vernacular.Contracts.I;

namespace PPWCode.Util.DI.I;

/// <summary>
///     Helper class to help with registration of services and implementations
///     in the DI container.
/// </summary>
/// <remarks>
///     The full potential of these helpers is reached when they are combined
///     with the Scrutor library for bulk registration of services.
/// </remarks>
public static class RegistrationHelper
{
    /// <summary>
    ///     Retrieves all interfaces implemented by the given type
    ///     <paramref name="implementationType" />, filters those on the types that
    ///     are a descendant of the given interface type
    ///     <paramref name="markerInterfaceType" />, and finally only keeps the
    ///     most specific types of those.  Typically, the result will contain
    ///     only one type, but in principle can contain multiple ones.
    /// </summary>
    /// <param name="markerInterfaceType">
    ///     The given base interface type, each returned interface type is either
    ///     a descendant of that type, or that type itself.
    /// </param>
    /// <param name="implementationType">
    ///     The given implementation type, the code filters the interfaces
    ///     implemented by this given type.
    /// </param>
    /// <returns>
    ///     An <see cref="IEnumerable{T}" /> of the most specific interfaces
    ///     implemented by the given <paramref name="implementationType" />, that
    ///     are a descendant of the given <paramref name="markerInterfaceType" />.
    /// </returns>
    /// <remarks>
    ///     Note that both the given <paramref name="markerInterfaceType" /> and
    ///     the given <paramref name="implementationType" /> can be (open or
    ///     closed) generic types.
    /// </remarks>
    public static IEnumerable<Type> GetMostSpecificInterfaces(
        Type markerInterfaceType,
        Type implementationType)
    {
        Contract.Requires(markerInterfaceType is not null);
        Contract.Requires(markerInterfaceType.IsInterface);
        Contract.Requires(implementationType is not null);
        Contract.Requires(implementationType.IsClass);

        bool IsSameAsMarkerInterface(Type type)
            => (type.IsGenericType && (type.GetGenericTypeDefinition() == markerInterfaceType))
               || (!type.IsGenericType && (type == markerInterfaceType));

        // build dictionary of interfaces implemented by the implementation type
        IDictionary<Type, HashSet<Type>> implementedInterfaces =
            implementationType
                .GetInterfaces()
                .ToDictionary(it => it, it => new HashSet<Type>(it.GetInterfaces()));

        // keep only the most specific interfaces
        // - filter away interfaces not linked to the marker interface
        // - keep only the most specific ones
        HashSet<Type> mostSpecificInterfaces =
            new(
                implementedInterfaces
                    .Where(kv => IsSameAsMarkerInterface(kv.Key) || kv.Value.Any(IsSameAsMarkerInterface))
                    .Select(kv => kv.Key));
        mostSpecificInterfaces.ExceptWith(implementedInterfaces.SelectMany(it => it.Value));

        // check whether the class is an open generic type
        // - class is open generic => use the generic type definition of the interface
        // - class is not generic, or a closed generic type => use the interface as-is
        bool isOpenGeneric = implementationType is { IsGenericType: true, IsConstructedGenericType: false };

        return mostSpecificInterfaces
            .Select(it =>
                        isOpenGeneric
                            ? it.GetGenericTypeDefinition()
                            : it);
    }

    /// <summary>
    ///     Retrieves all interfaces implemented by the given type
    ///     <paramref name="implementationType" />, filters those on the types that
    ///     are a descendant of the given interface type
    ///     <typeparamref name="TMarkerInterface" />, and finally only keeps the
    ///     most specific types of those.  Typically, the result will contain
    ///     only one type, but in principle can contain multiple ones.
    /// </summary>
    /// <param name="implementationType">
    ///     The given implementation type, the code filters the interfaces
    ///     implemented by this given type.
    /// </param>
    /// <typeparam name="TMarkerInterface">
    ///     The given base interface type, each returned interface type is either
    ///     a descendant of that type or that type itself.
    /// </typeparam>
    /// <returns>
    ///     An <see cref="IEnumerable{T}" /> of the most specific interfaces
    ///     implemented by the given <paramref name="implementationType" />, that
    ///     are a descendant of the given <typeparamref name="TMarkerInterface" />.
    /// </returns>
    /// <remarks>
    ///     Note that both the given <typeparamref name="TMarkerInterface" /> and
    ///     the given <paramref name="implementationType" /> can be (open or
    ///     closed) generic types.
    /// </remarks>
    public static IEnumerable<Type> GetMostSpecificInterfaces<TMarkerInterface>(Type implementationType)
    {
        Type markerInterfaceType = typeof(TMarkerInterface);
        return GetMostSpecificInterfaces(markerInterfaceType, implementationType);
    }
}
