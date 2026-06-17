using System.Windows;
using System.Windows.Controls;
using Store.Data;

namespace Store
{
	public partial class StatsWindow : Window
	{
		private AppDbContext _db;

		public StatsWindow(AppDbContext db)
		{
			InitializeComponent();
			_db = new AppDbContext();
			LoadStats();
		}

		private void LoadStats()
		{
			TotalProducts.Text = _db.Products.Count().ToString();
			TotalOrders.Text = _db.Orders.Count().ToString();
			var lowStock = DemandForecast.LowStock(_db);
			LowStockCount.Text = lowStock.Count.ToString();
			LowStockGrid.ItemsSource = lowStock;
			TopGrid.ItemsSource = DemandForecast.TopProducts(_db);
			RevenueGrid.ItemsSource = DemandForecast.RevenueByMonth(_db);


			ForecastProductCombo.ItemsSource = _db.Products.ToList();
		}

		private void ForecastProductCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ForecastProductCombo.SelectedValue is int id)
			{
				var result = DemandForecast.Forecast(id, 3, _db);
				ForecastResult.Text = result == 0 ? "нет данных" : $"{result} ед.";
			}
		}

		private void Close_Click(object sender, RoutedEventArgs e) => Close();
	}
}