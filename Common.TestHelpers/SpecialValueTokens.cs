using System.Globalization;

namespace Common.TestHelpers
{
    /// <summary>
    /// A static class containing special string tokens used in test cases to represent specific values such as null, true, and false when testing the API's handling of form field values.
    /// </summary>
    public static class SpecialValueTokens
    {
        /// <summary>
        /// The string representation of a null value in JSON, used for testing how the API handles null values in form fields.
        /// This is not a standard JSON token, but rather a placeholder string to represent null values in test cases where we want to explicitly check for null handling.
        /// </summary>
        public static readonly string Null = "<null>";

        /// <summary>
        /// The string representation of a boolean true value in JSON, used for testing how the API handles boolean values in form fields.
        /// This is not a standard JSON token, but rather a placeholder string to represent true values in test cases where we want to explicitly check for boolean handling.
        /// </summary>
        public static readonly string True = "<true>";

        /// <summary>
        /// The string representation of a boolean false value in JSON, used for testing how the API handles boolean values in form fields.
        /// This is not a standard JSON token, but rather a placeholder string to represent false values in test cases where we want to explicitly check for boolean handling.
        /// </summary>
        public static readonly string False = "<false>";

        /// <summary>
        /// The string representation of a boolean value in JSON, used for testing how the API handles boolean values in form fields.
        /// </summary>
        public static readonly string BooleanTrue = "<boolean:true>";

        /// <summary>
        /// The string representation of a boolean value in JSON, used for testing how the API handles boolean values in form fields.
        /// </summary>
        public static readonly string BooleanFalse = "<boolean:false>";

        /// <summary>
        /// The string representation of a string value in JSON, used for testing how the API handles string values in form fields.
        /// </summary>
        /// <param name="value">The string value to be represented in the token. This value will be included in the token to allow for testing of specific string values in form fields.</param>
        /// <returns></returns>
        public static string String(string value) => $"<string:{value}>";

        /// <summary>
        /// The string representation of an integer value in JSON, used for testing how the API handles integer values in form fields.
        /// </summary>
        /// <param name="value">The integer value to be represented in the token. This value will be included in the token to allow for testing of specific integer values in form fields.</param>
        /// <returns>The string token representing the integer value, formatted as "<integer:{value}>". The value is converted to a string using invariant culture formatting to ensure consistent representation across different cultures.</returns>
        public static string Integer(long value) => $"<integer:{value.ToString(CultureInfo.InvariantCulture)}>";

        /// <summary>
        /// The string representation of a decimal value in JSON, used for testing how the API handles decimal values in form fields.
        /// </summary>
        /// <param name="value">The decimal value to be represented in the token. This value will be included in the token to allow for testing of specific decimal values in form fields.</param>
        /// <returns>The string token representing the decimal value, formatted as "<decimal:{value}>". The value is converted to a string using invariant culture formatting to ensure consistent representation across different cultures.</returns>
        public static string Decimal(decimal value) => $"<decimal:{value.ToString(CultureInfo.InvariantCulture)}>";

        /// <summary>
        /// The string representation of a date value in JSON, used for testing how the API handles date values in form fields.
        /// </summary>
        /// <param name="date">The date value to be represented in the token. This value will be included in the token to allow for testing of specific date values in form fields. The date is formatted as "yyyy-MM-dd" to ensure a consistent representation of date values in test cases.</param>
        /// <returns>The string token representing the date value, formatted as "<date:{date}>". The date is formatted using the "yyyy-MM-dd" format to ensure a consistent representation of date values in test cases.</returns>
        public static string Date(DateOnly date) => $"<date:{date:yyyy-MM-dd}>";

        /// <summary>
        /// The string representation of today's date in JSON, used for testing how the API handles date values in form fields. This token can also include an optional offset in days to represent a date that is a certain number of days in the past or future relative to today.
        /// </summary>
        /// <param name="offsetDays">The number of days to offset from today's date. A positive value represents a future date, while a negative value represents a past date. If the offset is zero, the token will represent today's date without any offset. The offset is included in the token to allow for testing of date values that are relative to today's date in form fields.</param>
        /// <returns>The string token representing today's date, optionally offset by the specified number of days, formatted as "<date:today:{offsetDays}>".</returns>
        public static string DateToday(int offsetDays = 0)
            => offsetDays == 0 ? "<date:today>" : $"<date:today:{offsetDays.ToString(CultureInfo.InvariantCulture)}>";
    }
}
