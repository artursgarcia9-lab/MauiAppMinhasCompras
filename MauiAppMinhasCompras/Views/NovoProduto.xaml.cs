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

            string path = FileSystem.AppDataDirectory + "/compras.db3"; // Caminho do banco local do app

            _db = new SQLiteDatabaseHelper(path); // Instancia o helper SQLite
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

                await _db.Insert(p); // Insere no banco

                await DisplayAlert("Sucesso", "Produto cadastrado!", "OK"); // Mostra confirmação

                // Limpa campos após salvar
                txt_descricao.Text = string.Empty;
                txt_quantidade.Text = string.Empty;
                txt_preco.Text = string.Empty;

               await Navigation.PopAsync();  // Volta para a tela anterior
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK"); // Mostra erro caso algo falhe
            }
        }

        // Cancela a criação e volta para a tela anterior
        private async void btn_cancelar_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Retorna para a tela anterior sem salvar as alterações
        }
    }
}