using MauiAppMinhasCompras.Models;
using MauiAppMinhasCompras.Helpers;

namespace MauiAppMinhasCompras.Views
{
    public partial class ListaProduto : ContentPage
    {
        SQLiteDatabaseHelper _db;

        // variável que guarda o item selecionado
        Produto _produtoSelecionado;

        public ListaProduto()
        {
            InitializeComponent();

            string path = FileSystem.AppDataDirectory + "/compras.db3";
            _db = new SQLiteDatabaseHelper(path);
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
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

        // salva o item selecionado
        private void lst_produtos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count > 0)
                _produtoSelecionado = (Produto)e.CurrentSelection[0];
        }

        private async void btn_novo_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NovoProduto());
        }

        // BOTÃO EDITAR FUNCIONAL
        private async void btn_editar_Clicked(object sender, EventArgs e)
        {
            if (_produtoSelecionado == null)
            {
                await DisplayAlert("Aviso", "Selecione um produto na lista", "OK");
                return;
            }

            await Navigation.PushAsync(new EditarProduto(_produtoSelecionado));
        }

    }
}