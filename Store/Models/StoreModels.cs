using System;
using System.Collections.Generic;
using System.Text;

namespace Store.Models
	{
		public class User
		{
			public int Id { get; set; }
			public string Username { get; set; }
			public string Email { get; set; }
			public string PasswordHash { get; set; }
			public string Role { get; set; } 
		}

		public class Category
		{
			public int Id { get; set; }
			public string Name { get; set; }
		}

		public class Product
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public decimal Price { get; set; }
			public string Description { get; set; }
			public int? CategoryId { get; set; }
			public int StockQuantity { get; set; }
			public DateTime ProductionDate { get; set; }
			public int ShelfLifeDays { get; set; }
			public string? ImagePath { get; set; }
	}

		public class Order
		{
			public int Id { get; set; }
			public int UserId { get; set; }
			public DateTime OrderDate { get; set; }
			public string Status { get; set; }
		}

		public class OrderItem
		{
			public int Id { get; set; }
			public int OrderId { get; set; }
			public int ProductId { get; set; }
			public int Quantity { get; set; }
			public decimal PriceAtPurchase { get; set; }
		}

		public class Review
		{
			public int Id { get; set; }
			public int UserId { get; set; }
			public int ProductId { get; set; }
			public int? Rating { get; set; }
			public string Comment { get; set; }
		}

		public class ViewLog
		{
			public int Id { get; set; }
			public int UserId { get; set; }
			public int ProductId { get; set; }
			public DateTime ViewDate { get; set; }
		}
	}
