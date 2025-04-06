using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CesiZen_Backend.Models
{
    public class PercentageConverter : ValueConverter<Percentage, double>
    {
        public PercentageConverter()
            : base(
                percent => (double)(percent.Value * 100),
                value => new Percentage((decimal)(value / 100))
            )
        {
        }
    }
}
