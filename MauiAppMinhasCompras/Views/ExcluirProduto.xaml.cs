using MauiAppMinhasCompras.Models;
using MauiAppMinhasCompras.Helpers;

namespace MauiAppMinhasCompras.Views
{
    public partial class ExcluirProduto : ContentPage
    {
        SQLiteDatabaseHelper _db; // Objeto responsável pelas operações no banco de dados SQLite

        Produto _produto; // Objeto que armazena o produto que será exibido e possivelmente excluído

        // Construtor da página, recebe como parâmetro o produto selecionado
        public ExcluirProduto(Produto p)
        {
            InitializeComponent();

            _produto = p; // Armazena o produto recebido no atributo da classe

            // Preenche os campos da tela com os dados do produto selecionado
            txt_descricao.Text = _produto.Descricao;
            txt_quantidade.Text = _produto.Quantidade.ToString();
            txt_preco.Text = _produto.Preco.ToString();

            string path = FileSystem.AppDataDirectory + "/compras.db3"; // Define o caminho do banco de dados no diretório interno do aplicativo

            _db = new SQLiteDatabaseHelper(path); // Cria uma instância do helper responsável por acessar o banco SQLite
        }

        // Método executado quando o botão "Excluir" é clicado
        private async void btn_excluir_Clicked(object sender, EventArgs e)
        {
            // Exibe uma mensagem de confirmação para o usuário
            bool confirm = await DisplayAlert(
                "Confirmar", "Deseja realmente excluir este produto?", "Sim", "Não"
            );

            // Se o usuário escolher "Não", a operação é cancelada
            if (!confirm)
                return;

            await _db.Delete(_produto.Id); // Executa a exclusão do produto no banco de dados usando o Id

            await DisplayAlert("Sucesso", "Produto excluído!", "OK");  // Exibe uma mensagem informando que a exclusão foi realizada com sucesso

            await Navigation.PopAsync(); // Retorna para a página anterior da navegação
        }

        // Método executado quando o botão "Cancelar" é clicado
        private async void btn_cancelar_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // Apenas retorna para a tela anterior sem realizar nenhuma ação
        }
    }
}