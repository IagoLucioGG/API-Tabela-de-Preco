namespace APICrudEspecifica.Exceptions
{
    public class ExceptionProduto
    {
        public const string ProdutoNaoEncontrado = "Nenhum produto Encontrado na base.";
        public const string ProdutoDuplicado = "Já existe um produto com este nome na base de dados.";
        public const string ProdutoDeletado = "O produto foi excluído com sucesso.";
        public const string ProdutoDuplicadoChave = "Já Existe um produto com a ChaveExterna cadastrada.";
    }
}
