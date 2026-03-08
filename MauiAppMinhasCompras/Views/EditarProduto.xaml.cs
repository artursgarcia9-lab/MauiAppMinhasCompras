using MauiAppMinhasCompras.Models;
using MauiAppMinhasCompras.Helpers;

namespace MauiAppMinhasCompras.Views
{
    public partial class EditarProduto : ContentPage
    {
        // Objeto que receberá o produto enviado pela tela anterior
        Produto _produto; // Contém dados que serão exibidos e editados

        SQLiteDatabaseHelper _db; // Conexão com o banco

        // Construtor recebe o produto selecionado
        public EditarProduto(Produto p)
        {
            InitializeComponent();

            _produto = p; // Guarda o produto recebido

            // Preenche os campos da tela com os dados atuais do produto
            txt_descricao.Text = _produto.Descricao;
            txt_quantidade.Text = _produto.Quantidade.ToString();
            txt_preco.Text = _produto.Preco.ToString();

            // Cria conexão com o banco
            string path = FileSystem.AppDataDirectory + "/compras.db3";
            _db = new SQLiteDatabaseHelper(path);
        }

        // Evento do botão salvar
        private async void btn_salvar_Clicked(object sender, EventArgs e)
        {
            try
            {
                // Atualiza o objeto com os valores digitados
                _produto.Descricao = txt_descricao.Text;
                _produto.Quantidade = Convert.ToDouble(txt_quantidade.Text);
                _produto.Preco = Convert.ToDouble(txt_preco.Text);
                 
                await _db.Update(_produto); // Chama o update no banco

                await DisplayAlert("Sucesso", "Produto atualizado!", "OK"); // Mostra confirmação

                await Navigation.PopAsync(); // Volta para a tela anterior

            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK"); // Mostra erro caso falhe conversão ou banco
            }
        }


        // Variável opcional para armazenar um produto selecionado
        private Produto? _produtoSelecionado; // O "?" indica que a variável pode ser nula

        // Evento executado quando o botão "Cancelar" é clicado
        private async void btn_cancelar_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();  // Retorna para a tela anterior sem salvar as alterações
        }
    }
}