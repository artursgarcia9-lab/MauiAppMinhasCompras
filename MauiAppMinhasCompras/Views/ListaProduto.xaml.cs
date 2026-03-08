using MauiAppMinhasCompras.Models;
using MauiAppMinhasCompras.Helpers;

namespace MauiAppMinhasCompras.Views
{
    public partial class ListaProduto : ContentPage
    {
        SQLiteDatabaseHelper _db; // Objeto responsável pela conexão e operações no banco de dados

        Produto _produtoSelecionado; // Variável que armazena o produto selecionado na lista

        // Construtor da página
        public ListaProduto()
        {
            InitializeComponent();

            string path = FileSystem.AppDataDirectory + "/compras.db3"; // Define o caminho onde o banco SQLite será armazenado

            _db = new SQLiteDatabaseHelper(path); // Cria a conexão com o banco
        }

        // Método executado sempre que a página aparece na tela
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var lista = await _db.GetAll(); // Busca todos os produtos no banco de dados

            lst_produtos.ItemsSource = lista; // Preenche a lista visual com os produtos

            CalcularTotal(lista); // Calcula o valor total dos produtos
        }

        // Evento executado quando o texto da busca é alterado
        private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string q = e.NewTextValue; // Texto digitado pelo usuário

            List<Produto> lista;

            // Se o campo estiver vazio, mostra todos os produtos
            if (string.IsNullOrWhiteSpace(q))
                lista = await _db.GetAll();
            else 
                lista = await _db.Search(q); // Caso contrário, pesquisa produtos pela descrição

            lst_produtos.ItemsSource = lista; // Atualiza a lista exibida na tela


            CalcularTotal(lista); // Recalcula o total com base na nova lista
        }

        // Evento executado quando um item da lista é selecionado
        private void lst_produtos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Verifica se algum item foi selecionado
            if (e.CurrentSelection.Count > 0)

               _produtoSelecionado = (Produto)e.CurrentSelection[0];  // Armazena o produto selecionado
        }

        // Evento do botão para adicionar um novo produto
        private async void btn_novo_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NovoProduto()); // Navega para a tela de cadastro de produto
        }

        // Evento do botão de editar produto
        private async void btn_editar_Clicked(object sender, EventArgs e)
        {
            // Verifica se um produto foi selecionado
            if (_produtoSelecionado == null)
            {
                await DisplayAlert("Aviso", "Selecione um produto na lista", "OK");
                return;
            }

            await Navigation.PushAsync(new EditarProduto(_produtoSelecionado)); // Abre a tela de edição passando o produto selecionado
        }

        // Evento do botão de excluir produto
        private async void btn_excluir_Clicked(object sender, EventArgs e)
        {
            // Verifica se um produto foi selecionado
            if (_produtoSelecionado == null)
            {
                await DisplayAlert("Aviso", "Selecione um produto na lista", "OK");
                return;
            }

            await Navigation.PushAsync(new ExcluirProduto(_produtoSelecionado)); // Abre a tela de confirmação de exclusão
        }

        // Método responsável por calcular o valor total dos produtos
        private void CalcularTotal(List<Produto> lista)
        {
            double total = 0;

            // Percorre todos os produtos da lista
            foreach (var p in lista)
            {
                total += p.Quantidade * p.Preco; // Soma quantidade × preço de cada produto
            }

            lbl_total.Text = $"Total: R$ {total:F2}"; // Mostra o valor total na tela formatado em moeda
        }
    }
}