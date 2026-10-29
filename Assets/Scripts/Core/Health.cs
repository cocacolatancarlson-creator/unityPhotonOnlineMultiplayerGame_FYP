using System;

namespace Portfolio.Core
{
	// Engine-free health rules, so they can be unit-tested without Unity or Photon.
	public sealed class Health
	{
		public float Max { get; }
		public float Current { get; private set; }
		public bool IsDead => Current <= 0f;
		public float Fraction => Max <= 0f ? 0f : Current / Max;

		public Health(float max)
		{
			if (max <= 0f) throw new ArgumentOutOfRangeException(nameof(max));
			Max = max;
			Current = max;
		}

		// Returns true only on the hit that kills, so death is handled exactly once.
		public bool TakeDamage(float damage)
		{
			if (IsDead || damage <= 0f) return false;
			Current = Math.Max(0f, Current - damage);
			return IsDead;
		}

		public void Reset()
		{
			Current = Max;
		}
	}
}
