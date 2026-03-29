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

            try
            {
                string path = FileSystem.AppDataDirectory + "/compras.db3"; // Define o caminho onde o banco SQLite será armazenado
                _db = new SQLiteDatabaseHelper(path); // Cria a conexão com o banco
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Falha ao criar/abrir banco de dados: {ex}");
                // Garantir que _db não seja nulo para evitar NREs posteriores
                _db = null;
                // Opcional: mostrar mensagem simples ao usuário (não bloqueante)
                // Note: em construtor não podemos usar await, então apenas logamos e permitimos a página carregar
            }

            lst_produtos.ItemsSource = produtos; // Liga a lista visual à ObservableCollection
        }

        // Método executado sempre que a página aparece na tela
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            try // Tenta carregar os produtos do banco de dados
            {
                var categorias = await _db.GetCategorias(); // Busca todas as categorias no banco de dados        

                categorias.Insert(0, "Todas"); // opção padrão

                picker_categoria.ItemsSource = categorias; // Define a lista de categorias no Picker
               
                if (categorias.Count > 0)
                    picker_categoria.SelectedIndex = 0; // Define "Todas" como padrão

                await Filtrar(); // Carrega os produtos aplicando o filtro inicial
            }
            catch (Exception ex) // Trata qualquer erro que possa ocorrer durante a carga dos produtos
            {
                await DisplayAlert("Erro", $"Falha ao carregar os produtos: {ex.Message}", "OK");
            }
        }

        // Método responsável por aplicar os filtros de Categoria e Descrição
        private async Task Filtrar()
        {
            try
            {
                // Garante que o Picker já foi carregado
                if (picker_categoria.ItemsSource == null)
                    return;

                // Proteção contra null
                string texto = txt_search.Text ?? string.Empty; // evita null
                texto = texto.Trim().ToLower();

                string categoria = picker_categoria.SelectedItem?.ToString() ?? "Todas"; // valor padrão

                List<Produto> lista = await _db.GetAll(); // Busca todos os produtos no banco de dados

                // Filtro por Categoria
                if (!string.IsNullOrWhiteSpace(categoria) && categoria != "Todas")
                {
                    lista = lista.Where(p => p.Categoria != null && p.Categoria == categoria).ToList();
                }

                // Filtro por Descrição
                if (!string.IsNullOrWhiteSpace(texto))
                {
                    lista = lista.Where(p =>
                        p.Descricao != null &&
                        p.Descricao.ToLower().Contains(texto)
                    ).ToList();
                }

                produtos.Clear();

                foreach (var p in lista)
                {
                    produtos.Add(p);
                }

                CalcularTotal(lista);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao filtrar: {ex.Message}", "OK");
            }
        }

        // Evento executado quando o texto da busca é alterado
        private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            string texto = e.NewTextValue?.Trim(); // Texto digitado pelo usuário
            string categoria = picker_categoria.SelectedItem?.ToString(); // Categoria selecionada no Picker

            List<Produto> lista = await _db.GetAll(); // Busca todos os produtos no banco de dados

            // Filtro por Categoria (caso não seja "Todas")
            if (!string.IsNullOrWhiteSpace(categoria) && categoria != "Todas")
            {
                lista = lista.Where(p =>
                    !string.IsNullOrEmpty(p.Categoria) && // Evita erro se Categoria for null
                    p.Categoria == categoria
                ).ToList();
            }

            // Filtro por Descrição (nome do produto)
            if (!string.IsNullOrWhiteSpace(texto))
            {
                lista = lista.Where(p =>
                    !string.IsNullOrEmpty(p.Descricao) && // Evita erro se Descricao for null
                    p.Descricao.ToLower().Contains(texto.ToLower())
                ).ToList();
            }

            produtos.Clear(); // Limpa a coleção atual

            // Adiciona os produtos filtrados na ObservableCollection
            foreach (var p in lista)
            {
                produtos.Add(p);
            }

            CalcularTotal(lista); // Recalcula o total com base na lista filtrada
        }

        // Evento executado quando o filtro de categoria é alterado
        private async void Filtro_Changed(object sender, EventArgs e)
        {
            try
            {
                // Evita erro se o SearchBar ainda não estiver pronto
                string textoAtual = txt_search?.Text ?? "";

                // Reutiliza a lógica da busca para evitar duplicação de código
                txt_search_TextChanged(sender, new TextChangedEventArgs(textoAtual, textoAtual));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao aplicar filtro: {ex.Message}", "OK");
            }
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

                await Filtrar(); // Reaplica o filtro atual após a exclusão (corrigido - sem chamar evento manualmente)

                _produtoSelecionado = null; // Limpa a seleção após exclusão
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
                // Evita erro caso algum valor esteja nulo
                total += (p.Quantidade) * (p.Preco);
            }

            lbl_total.Text = $"Total: R$ {total:F2}"; // Mostra o valor total na tela formatado em moeda
        }

        private async void btn_relatorio_Clicked(object sender, EventArgs e)
        {
            try
            {
                var dados = await _db.GetRelatorioCategoria();

                if (dados == null || dados.Count == 0)
                {
                    await DisplayAlert("Relatório", "Nenhum dado encontrado.", "OK");
                    return;
                }

                // Monta o texto do relatório
                string mensagem = "Relatório de Gastos por Categoria:\n\n";

                double totalGeral = 0;

                foreach (var item in dados)
                {
                    mensagem += $"{item.Categoria}: R$ {item.Total:F2}\n";
                    totalGeral += item.Total;
                }

                mensagem += $"\nTOTAL GERAL: R$ {totalGeral:F2}";

                // Exibe o alerta
                await DisplayAlert("Relatório", mensagem, "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao gerar relatório: {ex.Message}", "OK");
            }
        }
    }
}