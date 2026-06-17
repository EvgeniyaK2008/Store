using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Store.Data;
using Store.Models;

namespace Store
{
	public partial class MainWindow : Window
	{
		private AppDbContext _db;
		private User _currentUser;
		private List<Product> _allProducts;


		public MainWindow(User currentUser)
		{
			InitializeComponent();
			try
			{
				_db = new AppDbContext();
				_currentUser = currentUser;
				ApplyRoleVisibility();
				LoadCategories();
				LoadProducts();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Ошибка запуска: " + ex.Message);
			}
		}

		public MainWindow() : this(new User { Role = "Admin", Username = "Admin"}) { }

		private void ApplyRoleVisibility()
		{
			bool isAdmin = _currentUser?.Role == "Admin";
			var vis = isAdmin ? Visibility.Visible : Visibility.Collapsed;

			AdminAddButton.Visibility = vis;
			AdminDeleteButton.Visibility = vis;
			AdminSeparator.Visibility = vis;
			NewButton.Visibility = vis; 
		}

		private void LoadCategories()
		{
			try
			{
				var categories = new List<Category>
				{
					new Category { Id = 0, Name = "Все категории" }
				};
				categories.AddRange(_db.Categories.OrderBy(c => c.Name).ToList());

				CategoriesComboBox.ItemsSource = categories;
				CategoriesComboBox.SelectedIndex = 0;
			}
			catch (Exception ex)
			{
				ShowError(ex.Message);
			}
		}

		private void LoadProducts(int? categoryId = null)
		{
			_db = new AppDbContext();
			try
			{
				var query = _db.Products.AsQueryable();

				if (categoryId.HasValue && categoryId.Value != 0)
					query = query.Where(p => p.CategoryId == categoryId.Value);

				string search = SearchBox.Text?.Trim().ToLower();
				if (!string.IsNullOrEmpty(search))
					query = query.Where(p => p.Name.ToLower().Contains(search));

				_allProducts = query.ToList();

				if (_allProducts.Count > 1)
				{
					ProductsItemsControl.ItemsSource = _allProducts
						.Take(_allProducts.Count - 1).ToList();

					var promo = _allProducts.Last();
					PromoCard.DataContext = promo;
					PromoCard.Tag = promo.Id;
					PromoCard.Visibility = Visibility.Visible;
				}
				else
				{
					ProductsItemsControl.ItemsSource = _allProducts;
					PromoCard.Visibility = Visibility.Collapsed;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.InnerException?.Message ?? ex.Message);
			}
		}

		private void CategoriesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (CategoriesComboBox.SelectedItem is Category selected)
			{
				int? catId = selected.Id == 0 ? (int?)null : selected.Id;
				LoadProducts(catId);
			}
		}

		private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			int? catId = null;
			if (CategoriesComboBox.SelectedItem is Category sel && sel.Id != 0)
				catId = sel.Id;

			LoadProducts(catId);
		}

		private void ProductCardButton_Click(object sender, RoutedEventArgs e)
		{
			if (sender is Button btn && btn.Tag is int productId && productId != 0)
			{
				var product = _db.Products.Find(productId);
				if (product == null) return;

				if (_currentUser?.Role != "Admin")
				{
					MessageBox.Show(
						$"{product.Name}\nЦена: {product.Price:N2} ₽\nОстаток: {product.StockQuantity} шт.\n\n{product.Description}",
						"Информация о товаре");
					return;
				}

				var dialog = new ProductDialog(product);
				if (dialog.ShowDialog() == true)
				{
					try
					{
						_db.SaveChanges();
						LoadProducts();
					}
					catch (Exception ex) { ShowError(ex.Message); }
				}
			}
		}


		private void AdminAddButton_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new ProductDialog();
			if (dialog.ShowDialog() == true)
			{
				try
				{
					_db.Products.Add(dialog.Result);
					_db.SaveChanges();
					LoadProducts();
				}
				catch (Exception ex) { ShowError(ex.Message); }
			}
		}

		private void AdminDeleteButton_Click(object sender, RoutedEventArgs e)
		{
			var input = Microsoft.VisualBasic.Interaction.InputBox(
				"Введите ID товара для удаления:", "Удаление", "");

			if (int.TryParse(input, out int id))
			{
				using var db = new AppDbContext();
				var product = db.Products.Find(id);
				if (product == null) { ShowError("Товар не найден."); return; }

				var confirm = MessageBox.Show(
					$"Удалить «{product.Name}»?", "Подтверждение",
					MessageBoxButton.YesNo, MessageBoxImage.Warning);

				if (confirm == MessageBoxResult.Yes)
				{
					try
					{
						db.Products.Remove(product);
						db.SaveChanges();
						_db = new AppDbContext();
						LoadProducts();
					}
					catch (Exception ex) { ShowError(ex.Message); }
				}
			}
		}
		private void NewFilterButton_Click(object sender, RoutedEventArgs e)
		{
			new StatsWindow(_db).ShowDialog();
			try
			{
				var cutoff = DateTime.Now.AddDays(-30);
				var newProducts = _db.Products
					.Where(p => p.ProductionDate >= cutoff)
					.ToList();

				ProductsItemsControl.ItemsSource = newProducts;
				PromoCard.Visibility = Visibility.Collapsed;
			}
			catch (Exception ex)
			{
				var msg = ex.Message;
				if (ex.InnerException != null)
					msg += "\n\nInner: " + ex.InnerException.Message;
				if (ex.InnerException?.InnerException != null)
					msg += "\n\nInner2: " + ex.InnerException.InnerException.Message;
				MessageBox.Show(msg);
			}
		}

		private void ShowError(string message)
		{
			MessageBox.Show($"Ошибка: {message}", "Ошибка",
				MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
}
