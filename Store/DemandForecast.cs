using System;
using System.Collections.Generic;
using System.Linq;
using Store.Data;
using Store.Models;

namespace Store
{
	public static class DemandForecast	
	{

		public static decimal Forecast(int productId, int periods, AppDbContext db)
		{
			var cutoff = DateTime.Now.AddMonths(-periods);
			var orders = db.Orders.Where(o => o.OrderDate >= cutoff).ToList();
			var items = db.OrderItems.Where(oi => oi.ProductId == productId).ToList();

			var sales = items
				.Join(orders, oi => oi.OrderId, o => o.Id,
					  (oi, o) => new { oi.Quantity, o.OrderDate })
				.GroupBy(x => new { x.OrderDate.Year, x.OrderDate.Month })
				.Select(g => (decimal)g.Sum(x => x.Quantity))
				.ToList();

			return sales.Any() ? Math.Round(sales.Average(), 2) : 0;
		}

		public static List<TopItem> TopProducts(AppDbContext db)
		{
			var products = db.Products.ToList();
			var items = db.OrderItems.ToList();

			return items
				.GroupBy(oi => oi.ProductId)
				.Select(g => new { ProductId = g.Key, Total = g.Sum(x => x.Quantity) })
				.OrderByDescending(x => x.Total)
				.Take(5)
				.Join(products, x => x.ProductId, p => p.Id,
					  (x, p) => new TopItem { Name = p.Name, TotalSold = x.Total })
				.ToList();
		}

		public class TopItem
		{
			public string Name { get; set; }
			public int TotalSold { get; set; }
		}

		public class RevenueItem 
		{
			public string Month { get; set; }
			public decimal Revenue { get; set; }
		}
		public static List<Product> LowStock(AppDbContext db, int threshold = 5)
		{
			return db.Products
				.Where(p => p.StockQuantity <= threshold)
				.OrderBy(p => p.StockQuantity)
				.ToList();
		}

		public static List<RevenueItem> RevenueByMonth(AppDbContext db)
		{
			var cutoff = DateTime.Now.AddMonths(-6);
			var orders = db.Orders.Where(o => o.OrderDate >= cutoff).ToList();
			var items = db.OrderItems.ToList();

			return items
				.Join(orders, oi => oi.OrderId, o => o.Id,
					  (oi, o) => new { oi.PriceAtPurchase, oi.Quantity, o.OrderDate })
				.GroupBy(x => new { x.OrderDate.Year, x.OrderDate.Month })
				.Select(g => new RevenueItem
				{
					Month = $"{g.Key.Month:D2}/{g.Key.Year}",
					Revenue = g.Sum(x => x.PriceAtPurchase * x.Quantity)
				})
				.OrderBy(x => x.Month)
				.ToList();
		}
	}
}