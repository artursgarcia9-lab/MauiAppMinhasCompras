using MauiAppMinhasCompras.Models;
using SQLite;
namespace MauiAppMinhasCompras.Helpers 
{ 
    public class SQLiteDatabaseHelper 
    {
        readonly SQLiteAsyncConnection _conn; // Cria uma conexão assíncrona com o banco SQLite

        // Construtor da classe - recebe o caminho do banco de dados
        public SQLiteDatabaseHelper(string path) 
        {
            _conn = new SQLiteAsyncConnection(path); // Cria a conexão com o banco

            _conn.CreateTableAsync<Produto>().Wait(); // Cria a tabela Produto caso ela ainda não exista
        }

        // Método responsável por inserir um produto no banco de dados
        public Task<int> Insert(Produto p)
        {
            return _conn.InsertAsync(p); // Insere o objeto Produto na tabela Produto
        }

        // Método responsável por atualizar um produto existente
        public Task<int> Update(Produto p)
        {
            return _conn.UpdateAsync(p); // Atualiza o registro usando a chave primária (Id)
        }

        // Método responsável por excluir um produto pelo Id
        public Task<int> Delete(int id)
        {
            string sql = "DELETE FROM Produto WHERE Id = ?"; // Comando SQL para remover o produto da tabela

            return _conn.ExecuteAsync(sql, id); // Executa o comando passando o Id como parâmetro
        }

        // Método que retorna todos os produtos cadastrados
        public Task<List<Produto>> GetAll()
        {
            return _conn.Table<Produto>().ToListAsync(); // Consulta a tabela Produto e retorna todos os registros
        }

        // Método responsável por pesquisar produtos pela descrição
        public Task<List<Produto>> Search(string q)
        {
            string sql = "SELECT * FROM Produto WHERE descricao LIKE '%" + q + "%'"; // Comando SQL que busca produtos que contenham o texto informado

            return _conn.QueryAsync<Produto>(sql); // Executa a consulta e retorna a lista de produtos encontrados
        }

        // Método que verifica se já existe um produto com a mesma descrição (case-insensitive)
        public async Task<bool> ExistsDescricao(string descricao)
        {
            string sql = "SELECT * FROM Produto WHERE LOWER(Descricao) = LOWER(?)";

            var resultado = await _conn.QueryAsync<Produto>(sql, descricao);

            return resultado.Count > 0; // Retorna true se encontrar algum produto com a mesma descrição, caso contrário retorna false
        }
    } 
}