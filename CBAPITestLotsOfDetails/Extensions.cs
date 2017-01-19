using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBAPITestLotsOfDetails
{
	public static class Extensions
	{
		//Source: http://blog.slaks.net/2010/12/nested-iterators-part-2.html
		public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> sequence, int size)
		{
			List<T> partition = new List<T>(size);
			foreach (var item in sequence)
			{
				partition.Add(item);
				if (partition.Count == size)
				{
					yield return partition;
					partition = new List<T>(size);
				}
			}
			if (partition.Count > 0)
				yield return partition;
		}
	}
}
