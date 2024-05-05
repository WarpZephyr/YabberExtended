using System.Collections.Generic;

namespace YabberExtended.Extensions
{
    /// <summary>
    /// <see cref="Dictionary{TKey, TValue}"/> extension methods.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Get a value using the given key if found, or throw a <see cref="KeyNotFoundException"/> with the given error message if not found.
        /// </summary>
        /// <typeparam name="TKey">The type of key in this <see cref="Dictionary{TKey, TValue}"/>.</typeparam>
        /// <typeparam name="TValue">The type of value in this <see cref="Dictionary{TKey, TValue}"/>.</typeparam>
        /// <param name="dictionary">A <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="errorMessage">The error message to use if throwing.</param>
        /// <returns>The value of the given key.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the given key is not present in the given <see cref="Dictionary{TKey, TValue}"/>.</exception>
        public static TValue GetValueOrThrow<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, string? errorMessage) where TKey : notnull
        {
            if (!dictionary.TryGetValue(key, out TValue? value))
            {
                throw new KeyNotFoundException(errorMessage);
            }

            return value;
        }
    }
}
