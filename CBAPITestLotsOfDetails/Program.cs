using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBAPITestLotsOfDetails.Default;

namespace CBAPITestLotsOfDetails
{
	class Program
	{
		static DefaultSoapClient _client = new DefaultSoapClient();

		static void Main(string[] args)
		{
			_client.Login("admin", "admin", null, null, null);

			var orderDetails = GenerateOrderDetails(500);

			double totalTime = 0;
			for (int i = 0; i < 3; i++)
			{
				totalTime+=CreateOrder(orderDetails, 25);
			}
			Console.WriteLine("================= Average time {0}s", totalTime / 3);

			totalTime = 0;
			for (int i = 0; i < 3; i++)
			{
				totalTime += CreateOrder(orderDetails, 50);
			}

			Console.WriteLine("================= Average time {0}s", totalTime / 3);

			totalTime = 0;
			for (int i = 0; i < 3; i++)
			{
				totalTime += CreateOrder(orderDetails, 100);
			}
			Console.WriteLine("================= Average time {0}s", totalTime / 3);

			totalTime = 0;
			for (int i = 0; i < 3; i++)
			{
				totalTime += CreateOrder(orderDetails, 500);
			}
			Console.WriteLine("================= Average time {0}s", totalTime / 3);

			_client.Logout();
			Console.ReadLine();
		}

		private static List<SalesOrderDetail> GenerateOrderDetails(int lineCount)
		{
			var list = new List<SalesOrderDetail>();
			for (int i = 0; i < lineCount; i++)
			{
				list.Add(new SalesOrderDetail()
				{
					ReturnBehavior = ReturnBehavior.None,
					InventoryID = new StringValue { Value = "AACOMPUT01" },
					Quantity = new DecimalValue { Value = 1 },
					LineDescription = new StringValue { Value = String.Format("Line {0}", i + 1) }
				});
			}
			return list;
		}

		private static double CreateOrder(List<SalesOrderDetail> orderLines, int batchSize)
		{
			Console.WriteLine("================= STARTING - SO with {0} lines in batch of {1}", orderLines.Count, batchSize);
			var sw = new Stopwatch();
			sw.Start();

			var so = new SalesOrder
			{
				ReturnBehavior = ReturnBehavior.None,
				CustomerID = new StringValue { Value = "ABARTENDE" },
			};

			foreach (var partition in orderLines.Partition(batchSize))
			{
				var swPartition = new Stopwatch();
				swPartition.Start();
				so.Details = partition.ToArray();
				so = (SalesOrder) _client.Put(so);
				so.ReturnBehavior = ReturnBehavior.None; // Very important, since it will be reset by system after every Put(). Maybe this behaviour should be change to respect what I initially requested???
				swPartition.Stop();
				Console.WriteLine("Inserted {0} lines in {1}s", partition.Count(), swPartition.Elapsed.TotalSeconds);
				System.Diagnostics.Debug.Assert(so.Details == null);
			}

			sw.Stop();
			return sw.Elapsed.TotalSeconds;
		}
	}
}
