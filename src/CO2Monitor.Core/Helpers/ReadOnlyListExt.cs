using System;
using System.Collections.Generic;

namespace CO2Monitor.Core.Helpers {
	internal static class ReadOnlyListExt {
		public static int IndexOf<T>(this IReadOnlyList<T> list, T elem) {
			if (list is null) {
				throw new ArgumentNullException(nameof(list));
			}

			for (int i = 0; i < list.Count; i++) {
				if (list[i].Equals(elem)) {
					return i;
				}
			}

			return -1;
		}
	}
}
