using System.Linq;
using System.Windows;
using System.Security.Cryptography;
using System.Text;
using Store.Data;
using Store.Models;

namespace Store
{
	public partial class LoginWindow : Window
	{
		public User LoggedInUser { get; private set; }
		private AppDbContext _db;

		public LoginWindow()
		{
			InitializeComponent();
			_db = new AppDbContext();
			SeedUsers();
		}


		private void Login_Click(object sender, RoutedEventArgs e)
		{
			var username = UsernameBox.Text.Trim();
			var password = PasswordBox.Password;

			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
			{ ErrorText.Text = "Заполните все поля."; return; }

			var user = _db.Users.FirstOrDefault(
				u => u.Username == username && u.PasswordHash == password);

			if (user == null)
			{ ErrorText.Text = "Неверное имя пользователя или пароль."; return; }

			LoggedInUser = user;
			LoggedInUser = user;
			var main = new MainWindow(user);
			main.Show();
			this.Close();
		}
		private void SeedUsers()
		{
			if (_db.Users.Any()) return;

			_db.Users.AddRange(
				new User
				{
					Username = "admin",
					Email = "admin@store.com",
					PasswordHash = "admin123",
					Role = "Admin"
				},
				new User
				{
					Username = "user",
					Email = "user@store.com",
					PasswordHash = "user123",
					Role = "User"
				}
			);
			_db.SaveChanges();
		}
	}
}