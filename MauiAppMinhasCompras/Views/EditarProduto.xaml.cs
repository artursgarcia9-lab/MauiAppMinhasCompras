using MauiAppMinhasCompras.Models;
using MauiAppMinhasCompras.Helpers;

namespace MauiAppMinhasCompras.Views
{
    public partial class EditarProduto : ContentPage
    {
        // Objeto que receberá o produto enviado pela tela anterior
        Produto _produto;

        // Conexão com o banco
        SQLiteDatabaseHelper _db;

        // Construtor recebe o produto selecionado
        public EditarProduto(Produto p)
        {
            InitializeComponent();

            // Guarda o produto recebido
            _produto = p;

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

                // Chama o update no banco
                await _db.Update(_produto);

                // Mostra confirmação
                await DisplayAlert("Sucesso", "Produto atualizado!", "OK");

                // Volta para a tela anterior
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                // Mostra erro caso falhe conversão ou banco
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }


        private Produto? _produtoSelecionado;

        private async void btn_cancelar_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}