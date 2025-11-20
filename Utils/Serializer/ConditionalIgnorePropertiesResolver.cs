using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Replay.Utils
{
    /// <summary>
    /// Contract resolver that conditionally ignores properties based on a predicate function.
    /// Used for version-based serialization where deprecated fields are omitted in newer versions.
    /// </summary>
    public class ConditionalIgnorePropertiesResolver : DefaultContractResolver
    {
        private readonly HashSet<string> alwaysIgnoreProps;
        private readonly HashSet<string> conditionalIgnoreProps;
        private readonly Func<object, bool> shouldIgnoreConditionalProps;

        /// <summary>
        /// Creates a resolver that can conditionally ignore properties based on the object being serialized.
        /// </summary>
        /// <param name="alwaysIgnore">Properties to always ignore</param>
        /// <param name="conditionalIgnore">Properties to ignore based on predicate</param>
        /// <param name="shouldIgnorePredicate">Function that determines if conditional properties should be ignored</param>
        public ConditionalIgnorePropertiesResolver(
            IEnumerable<string> alwaysIgnore,
            IEnumerable<string> conditionalIgnore,
            Func<object, bool> shouldIgnorePredicate)
        {
            alwaysIgnoreProps = new HashSet<string>(alwaysIgnore ?? new string[0]);
            conditionalIgnoreProps = new HashSet<string>(conditionalIgnore ?? new string[0]);
            shouldIgnoreConditionalProps = shouldIgnorePredicate ?? (_ => false);
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            // Always ignore these properties
            if (alwaysIgnoreProps.Contains(property.PropertyName))
            {
                property.ShouldSerialize = _ => false;
            }
            // Conditionally ignore based on predicate
            else if (conditionalIgnoreProps.Contains(property.PropertyName))
            {
                property.ShouldSerialize = instance => !shouldIgnoreConditionalProps(instance);
            }

            return property;
        }
    }
}