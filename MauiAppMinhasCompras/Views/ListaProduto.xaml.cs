using MauiAppMinhasCompras.Models;
using MauiAppMinhasCompras.Helpers;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views
{
    public partial class ListaProduto : ContentPage
    {
        SQLiteDatabaseHelper _db; // Objeto responsável pela conexão e operações no banco de dados
        Produto _produtoSelecionado; // Variável que armazena o produto selecionado na lista

        ObservableCollection<Produto> produtos = new ObservableCollection<Produto>(); // Coleção observável que atualiza a interface automaticamente

        // Construtor da página
        public ListaProduto()
        {
            InitializeComponent();

            string path = FileSystem.AppDataDirectory + "/compras.db3"; // Define o caminho onde o banco SQLite será armazenado
            _db = new SQLiteDatabaseHelper(path); // Cria a conexão com o banco

            lst_produtos.ItemsSource = produtos; // Liga a lista visual à ObservableCollection
        }

        // Método executado sempre que a página aparece na tela
        protected async override void OnAppearing() 
        { 
            base.OnAppearing();

            try // Tenta carregar os produtos do banco de dados
            {
                var lista = await _db.GetAll(); // Busca todos os produtos no banco de dados         
                produtos.Clear(); // Limpa a coleção atual
                // Adiciona os produtos na ObservableCollection
                foreach (var p in lista)
                {
                    produtos.Add(p);
                }

                CalcularTotal(lista); // Calcula o valor total dos produtos
            } 
            catch (Exception ex) // Trata qualquer erro que possa ocorrer durante a carga dos produtos
            {
                await DisplayAlert("Erro", $"Falha ao carregar os produtos: {ex.Message}", "OK");
            }
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

            produtos.Clear(); // Limpa a coleção atual

            // Adiciona os resultados da busca na ObservableCollection
            foreach (var p in lista)
            {
                produtos.Add(p);
            }

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

            bool confirm = await DisplayAlert("Confirmar", "Deseja realmente excluir este produto?", "Sim", "Não"); // Confirma a exclusão

            if (!confirm)
                return;

            try // Tenta excluir o produto selecionado
            {

                await _db.Delete(_produtoSelecionado.Id); // Remove o produto do banco de dados

                produtos.Remove(_produtoSelecionado); // Remove o produto da ObservableCollection (atualiza a interface automaticamente)

                CalcularTotal(produtos.ToList()); // Recalcula o total após a exclusão

                _produtoSelecionado = null; // Limpa a seleção de exclusão
            }
            catch (Exception ex) // Trata qualquer erro que possa ocorrer durante a exclusão
            {
                await DisplayAlert("Erro", $"Falha ao excluir o produto: {ex.Message}", "OK");
            }
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