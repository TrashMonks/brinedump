using System;

namespace TrashMonks.Brinedump
{
	public static partial class Static
	{
		/// <summary>
		/// Returns first valid dice range with flat distribution.
		/// </summary>
		public static string DiceForRange(int v1, int v2) {
			if (v1 == v2) return $"{v1:+#;-#;+0}";

			int max = v1, min = v2;
			if (v2 > v1) {
				max = v2;
				min = v1;
			}

			for (int i = 1, c = max - min; i <= c; i++) {
				if (c % i > 0) continue;

				var y = (i + c) / i;
				var z = i - min;
				if (z > 0) return $"+{i}d{y}-{z}";
				else return $"+{i}d{y}+{-z}";
			}

			return $"[invalid range: {min}...{max}]";
		}
	}
}
