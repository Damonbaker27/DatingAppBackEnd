using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Extensions
{
    public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
    {
        //converts dateonly to datetime
        public DateOnlyConverter()
            : base(dateOnly =>
                    dateOnly.ToDateTime(TimeOnly.MinValue),
                dateTime => DateOnly.FromDateTime(dateTime))
        { }
    }
}
