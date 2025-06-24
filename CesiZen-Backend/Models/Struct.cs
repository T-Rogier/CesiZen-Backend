namespace CesiZen_Backend.Models
{
    public readonly struct Percentage
    {
        public decimal Value { get; }

        public Percentage(decimal value)
        {
            if (value < 0 || value > 1)
                throw new ArgumentOutOfRangeException();
            Value = value;
        }

        public override string ToString() => $"{Value * 100}%";
    }
}
