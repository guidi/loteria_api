namespace Loteria.API.DTO
{
    public class LoteriaDTO
    {
        public List<String> trevosSorteados { get; set; }
        public Boolean acumulado { get; set; }
        public String dataApuracao { get; set; }
        public String dataProximoConcurso { get; set; }
        public List<String> dezenasSorteadasOrdemSorteio { get; set; }
        public Boolean exibirDetalhamentoPorCidade { get; set; }
        public object id { get; set; }
        public int indicadorConcursoEspecial { get; set; }
        public List<String> listaDezenas { get; set; }
        public List<String> listaDezenasSegundoSorteio { get; set; }
        public List<UFGanhadores> listaMunicipioUFGanhadores { get; set; }
        public List<ListaRateioPremio> listaRateioPremio { get; set; }
        public object listaResultadoEquipeEsportiva { get; set; }
        public String localSorteio { get; set; }
        public String nomeMunicipioUFSorteio { get; set; }
        public String nomeTimeCoracaoMesSorte { get; set; }
        public int numero { get; set; }
        public int numeroConcursoAnterior { get; set; }
        public int numeroConcursoFinal_0_5 { get; set; }
        public int numeroConcursoProximo { get; set; }
        public int numeroJogo { get; set; }
        public String observacao { get; set; }
        public object premiacaoContingencia { get; set; }
        public String tipoJogo { get; set; }
        public int tipoPublicacao { get; set; }
        public Boolean ultimoConcurso { get; set; }
        public double valorArrecadado { get; set; }
        public double valorAcumuladoConcurso_0_5 { get; set; }
        public double valorAcumuladoConcursoEspecial { get; set; }
        public double valorAcumuladoProximoConcurso { get; set; }
        public double valorEstimadoProximoConcurso { get; set; }
        public double valorSaldoReservaGarantidora { get; set; }
        public double valorTotalPremioFaixaUm { get; set; }
    }

    public class UFGanhadores
    {
        public int ganhadores { get; set; }
        public string municipio { get; set; }
        public string nomeFatansiaUL { get; set; }
        public int posicao { get; set; }
        public string serie { get; set; }
        public string uf { get; set; }
    }

    public class ListaRateioPremio
    {
        public string? descricaoFaixa { get; set; }
        public int faixa { get; set; }
        public int numeroDeGanhadores { get; set; }
        public double valorPremio { get; set; }
    }
}
