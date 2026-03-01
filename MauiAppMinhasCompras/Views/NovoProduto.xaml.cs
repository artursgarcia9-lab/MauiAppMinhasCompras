using MauiAppMinhasCompras.Models;
using MauiAppMinhasCompras.Helpers;

namespace MauiAppMinhasCompras.Views
{
    public partial class NovoProduto : ContentPage
    {
        SQLiteDatabaseHelper _db;

        public NovoProduto()
        {
            InitializeComponent();

            // Caminho do banco local do app
            string path = FileSystem.AppDataDirectory + "/compras.db3";

            // Instancia o helper SQLite
            _db = new SQLiteDatabaseHelper(path);
        }

        private async void btn_salvar_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Cria objeto Produto preenchendo com os dados digitados
                Produto p = new Produto
                {
                    Descricao = txt_descricao.Text,
                    Quantidade = Convert.ToDouble(txt_quantidade.Text),
                    Preco = Convert.ToDouble(txt_preco.Text)
                };

                // Insere no banco
                await _db.Insert(p);

                // Mostra confirmação
                await DisplayAlert("Sucesso", "Produto cadastrado!", "OK");

                // Limpa campos após salvar
                txt_descricao.Text = string.Empty;
                txt_quantidade.Text = string.Empty;
                txt_preco.Text = string.Empty;

                // Volta para a tela anterior
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                // Mostra erro caso algo falhe
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }
    }
}