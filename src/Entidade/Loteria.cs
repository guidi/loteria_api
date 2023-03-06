namespace Loteria.API.Entidade
{
    public class Loteria
    {
        public Int32? Id { get; set; }
        public String CodigoLoteria { get; set; }
        public Int32 Concurso { get; set; }
        public String Resultado { get; set; }
        /// <summary>
        /// Apenas as 6 dezenas da mega
        /// </summary>
        public String Dezena1 { get; set; }
        public String Dezena2 { get; set; }
        public String Dezena3 { get; set; }
        public String Dezena4 { get; set; }
        public String Dezena5 { get; set; }
        public String Dezena6 { get; set; }
        public DateTime? DataCadastro { get; set; }
        public DateTime? DataProximoConcurso { get; set; }
    }
}
