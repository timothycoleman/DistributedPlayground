using System.Diagnostics;

namespace DistributedPlayground.Infrastructure;

[DebuggerDisplay("{ToString()}")]
public readonly struct Delay
{
	public static readonly Delay None = new();
	public long Ticks { get; }

	public Delay(long ticks)
	{
		Ticks = ticks;
	}

	public override string ToString() => $"Ticks: {Ticks}";

	public static Delay operator +(Delay a, Delay b) => new(a.Ticks + b.Ticks);
	public static Delay operator -(Delay a, Delay b) => new(a.Ticks + b.Ticks);
	public static Delay operator *(Delay a, long x) => new(a.Ticks * x);
	public static Delay operator *(long x, Delay a) => new(a.Ticks * x);
	public static Delay operator /(Delay a, long x) => new(a.Ticks / x);
	public static Delay operator /(long x, Delay a) => new(a.Ticks / x);

	public static bool operator <(Delay a, Delay b) => a.Ticks < b.Ticks;
	public static bool operator >(Delay a, Delay b) => a.Ticks > b.Ticks;
	public static bool operator <=(Delay a, Delay b) => a.Ticks <= b.Ticks;
	public static bool operator >=(Delay a, Delay b) => a.Ticks >= b.Ticks;
}
