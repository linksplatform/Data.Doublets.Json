using System.Collections.Generic;
using Platform.Interfaces;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Platform.Data.Doublets.Json
{
    /// <summary>
    ///     <para>
    ///         Represents the json array element criterion matcher.
    ///     </para>
    ///     <para></para>
    /// </summary>
    /// <seealso cref="ICriterionMatcher{TArgument}" />
    public class JsonArrayElementCriterionMatcher<TLink> : ICriterionMatcher<TLink>
    {
        /// <summary>
        ///     <para>
        ///         The storage.
        ///     </para>
        ///     <para></para>
        /// </summary>
        public readonly IJsonStorage<TLink> Storage;

        /// <summary>
        ///     <para>
        ///         Initializes a new <see cref="JsonArrayElementCriterionMatcher" /> instance.
        ///     </para>
        ///     <para></para>
        /// </summary>
        /// <param name="storage">
        ///     <para>A storage.</para>
        ///     <para></para>
        /// </param>
        public JsonArrayElementCriterionMatcher(IJsonStorage<TLink> storage)
        {
            Storage = storage;
        }

        /// <summary>
        ///     <para>
        ///         Determines whether this instance is matched.
        ///     </para>
        ///     <para></para>
        /// </summary>
        /// <param name="link">
        ///     <para>The link.</para>
        ///     <para></para>
        /// </param>
        /// <returns>
        ///     <para>The bool</para>
        ///     <para></para>
        /// </returns>
        public bool IsMatched(TLink link)
        {
            return EqualityComparer<TLink>.Default.Equals(Storage.Links.GetSource(link), Storage.ValueMarker);
        }
    }
}
