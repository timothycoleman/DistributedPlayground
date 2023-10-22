using System.Diagnostics;

namespace DistributedPlayground.Infrastructure;

[DebuggerDisplay("{ToString()}")]
public readonly struct Instant : IComparable<Instant>
{
	public static readonly Instant Zero = new(0);
	public long Ticks { get; }

	public Instant(long ticks)
	{
		Ticks = ticks;
	}

	public override string ToString() => $"Ticks: {Ticks}";

	public readonly Instant Add(Delay delay)
	{
		return new(Ticks + delay.Ticks);
	}

	public int CompareTo(Instant other)
	{
		return Ticks.CompareTo(other.Ticks);
	}

	public static bool operator <(Instant a, Instant b) => a.Ticks < b.Ticks;
	public static bool operator >(Instant a, Instant b) => a.Ticks > b.Ticks;
	public static bool operator <=(Instant a, Instant b) => a.Ticks <= b.Ticks;
	public static bool operator >=(Instant a, Instant b) => a.Ticks >= b.Ticks;
	public static Instant operator +(Instant a, Delay b) => new(a.Ticks + b.Ticks);
}
