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
                // Validação da descrição (Nome)
                if (string.IsNullOrWhiteSpace(txt_descricao.Text))
                {
                    await DisplayAlert("Erro", "Informe descrição do produto.", "OK");
                    return;
                }

                string descricao = txt_descricao.Text?.Trim(); // Remove espaços extras

                // Verifica se já existe produto com mesma descrição
                if (await _db.ExistsDescricao(descricao))
                {
                    await DisplayAlert("Erro", "Já existe um produto com essa descrição.", "OK");
                    return;
                }

                // Validação da quantidade
                if (!double.TryParse(txt_quantidade.Text, out double quantidade) || quantidade <= 0)
                {
                    await DisplayAlert("Erro", "Quantidade deve ser maior que zero.", "OK");
                    return;
                }

                // Validação do preço
                if (!double.TryParse(txt_preco.Text, out double preco) || preco <= 0)
                {
                    await DisplayAlert("Erro", "Preço deve ser maior que zero.", "OK");
                    return;
                }

                // Cria objeto Produto preenchendo com os dados digitados validados
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
            catch (FormatException) // Captura erros de formatação (ex: texto em campo numérico)
            {
                await DisplayAlert("Erro", "Quantidade e preço devem ser números.", "OK"); // Mostra mensagem de erro específica
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