using System;
using System.Linq;
using System.Collections.Generic;

namespace GitManager.Extensions
{
    /// <summary>
	/// Класс проверки аргументов.
	/// </summary>
	public static class Argument
    {
        /// <summary>
        /// Проверка, что аргумент не равен null.
        /// </summary>
        /// <param name="value"> Аргумент. </param>
        /// <param name="message"> Сообщение об ошибке. </param>
        public static void NotNull(object value, string message)
        {
            if (null == value)
            {
                throw new ArgumentNullException(message);
            }
        }

        /// <summary>
        /// Проверяет строку на null или пустоту.
        /// </summary>
        /// <param name="value"> Аргумент для проверки. </param>
        /// <param name="message"> Сообщение об ошибке. </param>
        /// <exception cref="ArgumentException"><paramref name="value" /> is <c>null</c>.</exception>
        public static void NotNullOrEmpty(string value, string message)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// Проверяет строку на null, пустоту и состоит ли она только из символов-разделителей.
        /// </summary>
        /// <param name="value"> Аргумент для проверки. </param>
        /// <param name="message"> Сообщение об ошибке. </param>
        /// <exception cref="ArgumentException"><paramref name="value" /> is <c>null</c>.</exception>
        public static void NotNullOrWhiteSpace(string value, string message)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// Проверяет аргумент на равенство <see cref="Guid.Empty"/>.
        /// </summary>
        /// <param name="value"> Значение аргумента. </param>
        /// <param name="parameterName"> Название проверяемого аргумента. </param>
        /// <exception cref="ArgumentException"><paramref name="value" /> is <c>Guid.Empty</c>.</exception>
        public static void IsGuidNotEmpty(Guid value, string parameterName)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentException($"The '{parameterName}' argument must not be empty.");
            }
        }

        /// <summary>
        /// Проверяет коллекцию на null или пустоту.
        /// </summary>
        /// <param name="value"> Аргумент для проверки. </param>
        /// <param name="message"> Сообщение об ошибке. </param>
        /// <exception cref="ArgumentException"><paramref name="value" /> is <c>null</c>.</exception>
        public static void IsNullOrEmpty<T>(IEnumerable<T> value, string message)
        {
            if (value is null || !value.Any())
            {
                throw new ArgumentException(message);
            }
        }
    }
}
