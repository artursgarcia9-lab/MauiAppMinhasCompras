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
            try // Tenta converter os valores e atualizar o produto
            {
                // Validação da descrição (Nome)
                if (string.IsNullOrWhiteSpace(txt_descricao.Text))
                {
                    await DisplayAlert("Erro", "Informe descrição do produto.", "OK");
                    return;
                }

                string descricao = txt_descricao.Text?.Trim(); // Remove espaços extras

                // Verifica se existe outro produto com mesma descrição
                var lista = await _db.GetAll();

                if (lista.Any(p =>
                    p.Descricao.ToLower() == descricao.ToLower() &&
                    p.Id != _produto.Id))
                {
                    await DisplayAlert("Erro", "Já existe outro produto com essa descrição.", "OK");
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

                // Atualiza o objeto com os valores digitados validados
                _produto.Descricao = txt_descricao.Text;
                _produto.Quantidade = Convert.ToDouble(txt_quantidade.Text);
                _produto.Preco = Convert.ToDouble(txt_preco.Text);
                 
                await _db.Update(_produto); // Chama o update no banco

                await DisplayAlert("Sucesso", "Produto atualizado!", "OK"); // Mostra confirmação

                await Navigation.PopAsync(); // Volta para a tela anterior

            }
            catch (FormatException) // Captura erros de formatação (ex: texto em campo numérico)
            {
                await DisplayAlert("Erro", "Quantidade e preço devem ser números.", "OK"); // Mostra mensagem de erro específica
            }
            catch (Exception ex) // Captura qualquer erro de conversão ou banco
            {
                await DisplayAlert("Erro", "Falha ao editar o produto: ", ex.Message, "OK"); // Mostra erro caso falhe conversão ou banco
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