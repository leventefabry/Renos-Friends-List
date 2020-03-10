using System;

namespace RenosFriendsList.API.Helpers
{
    public static class DateTimeExtensions
    {
        public static int? GetCurrentAge(this DateTime? dateTime)
        {
            if (dateTime.HasValue)
            {
                var currentDate = DateTime.UtcNow;
                var age = currentDate.Year - dateTime.Value.Year;

                if (currentDate < dateTime.Value.AddYears(age))
                {
                    age--;
                }

                return age;
            }

            return null;
        }
    }
}
