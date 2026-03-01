using MauiAppMinhasCompras.Models;
using MauiAppMinhasCompras.Helpers;

namespace MauiAppMinhasCompras.Views
{
    public partial class ListaProduto : ContentPage
    {
        SQLiteDatabaseHelper _db;

        public ListaProduto()
        {
            InitializeComponent();

            string path = FileSystem.AppDataDirectory + "/compras.db3";
            _db = new SQLiteDatabaseHelper(path);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            // Carrega todos os produtos do banco
            lst_produtos.ItemsSource = await _db.GetAll();
        }

        private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string q = e.NewTextValue;

            if (string.IsNullOrWhiteSpace(q))
                lst_produtos.ItemsSource = await _db.GetAll();
            else
                lst_produtos.ItemsSource = await _db.Search(q);
        }
    }
}