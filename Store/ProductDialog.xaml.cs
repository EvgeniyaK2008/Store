using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Store.Models;

namespace Store
{
	public partial class ProductDialog : Window
	{
		public Product Result { get; private set; }
		private Product _existing;

		public ProductDialog()
		{
			InitializeComponent();
		}

		public ProductDialog(Product p) : this()
		{
			_existing = p;
			NameBox.Text = p.Name;
			PriceBox.Text = p.Price.ToString();
			StockBox.Text = p.StockQuantity.ToString();
			DescBox.Text = p.Description;
			Title = "Редактировать товар";
		}

		private void Save_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(NameBox.Text))
			{ MessageBox.Show("Введите название."); return; }

			if (!decimal.TryParse(PriceBox.Text, out var price) || price < 0)
			{ MessageBox.Show("Некорректная цена."); return; }

			if (!int.TryParse(StockBox.Text, out var stock) || stock < 0)
			{ MessageBox.Show("Некорректный остаток."); return; }

			if (_existing != null)
			{
				_existing.Name = NameBox.Text.Trim();
				_existing.Price = price;
				_existing.StockQuantity = stock;
				_existing.Description = DescBox.Text.Trim();
				Result = _existing;

			}
			else
			{
				Result = new Product
				{
					Name = NameBox.Text.Trim(),
					Price = price,
					StockQuantity = stock,
					Description = DescBox.Text.Trim(),
					ProductionDate = DateTime.Now,
					ShelfLifeDays = 0
				};
			}

			DialogResult = true;
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
		}
	}
}