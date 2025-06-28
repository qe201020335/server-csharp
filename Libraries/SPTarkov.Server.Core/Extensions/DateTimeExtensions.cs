namespace SPTarkov.Server.Core.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        ///     Formats the time part of a date as a UTC string.
        /// </summary>
        /// <param name="dateTimeOffset">The date to format in UTC.</param>
        /// <returns>The formatted time as 'HH-MM-SS'.</returns>
        public static string FormatToBsgTime(this DateTimeOffset dateTimeOffset)
        {
            var universalTime = dateTimeOffset.ToUniversalTime();
            var hour = Pad(universalTime.Hour);
            var minute = Pad(universalTime.Minute);
            var second = Pad(universalTime.Second);

            return $"{hour}-{minute}-{second}";
        }

        /// <summary>
        ///     Formats the time part of a date as a UTC string.
        /// </summary>
        /// <param name="dateTime">The date to format in UTC.</param>
        /// <returns>The formatted time as 'HH-MM-SS'.</returns>
        public static string FormatToBsgTime(this DateTime dateTime)
        {
            var universalTime = dateTime.ToUniversalTime();
            var hour = Pad(universalTime.Hour);
            var minute = Pad(universalTime.Minute);
            var second = Pad(universalTime.Second);

            return $"{hour}-{minute}-{second}";
        }

        /// <summary>
        ///     Formats the date part of a date as a UTC string.
        /// </summary>
        /// <param name="dateTimeOffset">The date to format in UTC.</param>
        /// <returns>The formatted date as 'YYYY-MM-DD'.</returns>
        public static string FormatToBsgDate(this DateTimeOffset dateTimeOffset)
        {
            var universalTime = dateTimeOffset.ToUniversalTime();
            var day = Pad(universalTime.Day);
            var month = Pad(universalTime.Month);
            var year = Pad(universalTime.Year);

            return $"{year}-{month}-{day}";
        }

        /// <summary>
        ///     Formats the date part of a date as a UTC string.
        /// </summary>
        /// <param name="dateTime">The date to format in UTC.</param>
        /// <returns>The formatted date as 'YYYY-MM-DD'.</returns>
        public static string FormatToBsgDate(this DateTime dateTime)
        {
            var universalTime = dateTime.ToUniversalTime();
            var day = Pad(universalTime.Day);
            var month = Pad(universalTime.Month);
            var year = Pad(universalTime.Year);

            return $"{year}-{month}-{day}";
        }

        /// <summary>
        ///     Pads a number with a leading zero if it is less than 10.
        /// </summary>
        /// <param name="number">The number to pad.</param>
        /// <returns>The padded number as a string.</returns>
        private static string Pad(int number)
        {
            return number.ToString().PadLeft(2, '0');
        }

        /// <summary>
        ///     Get current time formatted to fit BSGs requirement
        /// </summary>
        /// <param name="date"> Date to format into bsg style </param>
        /// <returns> Time formatted in BSG format </returns>
        public static string GetBsgFormattedWeatherTime(this DateTime date)
        {
            return date.FormatToBsgTime().Replace("-", ":").Replace("-", ":");
        }
    }
}
