using Loteria.API.Data.Context;
using Loteria.API.DTO;
using Loteria.API.Service;
using Loteria.API.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Globalization;

namespace Loteria.API.Controllers
{
    [Route("api/v1/loteria")]
    [ApiController]
    public class LoteriaController : ControllerBase
    {
        private readonly ILogger<LoteriaController> _logger;
        private readonly ILoteriaService _loteriaService;
        private readonly LoteriaContext _loteriaContext;
        private readonly String[] LoteriasPermitidas = new String[]{ "megasena", "quina", "lotofacil", "lotomania", "duplasena", "timemania", "diadesorte", "federal", "loteca", "supersete", "maismilionaria" }; 
        public LoteriaController(ILogger<LoteriaController> logger, ILoteriaService loteriaService, LoteriaContext loteriaContext)
        {
            _logger = logger;
            _loteriaService = loteriaService;
            _loteriaContext = loteriaContext;
        }

        /// <summary>
        /// Consulta um concurso pelo código da loteria e número do concurso
        /// </summary>
        /// <param name="CodigoLoteria"></param>
        /// <param name="Concurso"></param>
        /// <returns></returns>
        [HttpGet("healthcheck")]
        public IActionResult HealtCheck()
        {
            return Ok("Tudo certo :D");
        }

        /// <summary>
        /// Consulta um concurso pelo código da loteria e número do concurso
        /// </summary>
        /// <param name="CodigoLoteria"></param>
        /// <param name="Concurso"></param>
        /// <returns></returns>
        [HttpGet("{CodigoLoteria}/{Concurso}")]
        public async Task<IActionResult> Get(String CodigoLoteria, String Concurso)
        {
            if (String.IsNullOrEmpty(CodigoLoteria))
            {
                return BadRequest("O código da loteria não foi informado.");
            }

            if (!LoteriasPermitidas.Contains(CodigoLoteria))
            {
                return BadRequest("A API não é compatível com a loteria informada.");
            }

            Loteria.API.Entidade.Loteria loteriaDoBD = null;
            LoteriaDTO DTODaLoteria = null;
            Boolean EhUltimo = Concurso.ToUpper() == Constantes.ULTIMO;

            if (EhUltimo)
            {
                //Se for o último concurso procuramos pelo com o maior número do concurso no BD
                loteriaDoBD = await _loteriaContext.Loterias.Where(x => x.CodigoLoteria == CodigoLoteria).OrderByDescending(x => x.Concurso).FirstOrDefaultAsync();

                //Pode ser que o último que temos no BD não seja o mais atualizado, nesse caso verifica a data do próximo concurso.
                if (loteriaDoBD != null && loteriaDoBD.DataProximoConcurso != null)
                {
                    //Se a data do próximo concurso for menor ou igual a data de hoje busca o concurso externamente
                    if (loteriaDoBD.DataProximoConcurso.Value.Date <= DateTime.Now.Date)
                    {
                        DTODaLoteria = await _loteriaService.ObterPelaLoteriaEConcurso(CodigoLoteria, Concurso);

                        if (DTODaLoteria != null && DTODaLoteria.numero != loteriaDoBD.Concurso)
                        {
                            await SalvarLoteriaNoBD(DTODaLoteria, CodigoLoteria);
                            return Ok(DTODaLoteria);
                        }
                    }
                }
            }
            else
            {
                //Se o usuário não quiser o último concurso, procuramos no BD e caso não exista, buscamos na api externa e atualizamos o BD.
                Int32 ConcursoInt;
                Int32.TryParse(Concurso, out ConcursoInt);
                loteriaDoBD = await _loteriaContext.Loterias.Where(x => x.CodigoLoteria == CodigoLoteria && x.Concurso == ConcursoInt).FirstOrDefaultAsync();
            }

            if (loteriaDoBD != null)
            {
                return Ok(JsonConvert.DeserializeObject<LoteriaDTO>(loteriaDoBD.Resultado));
            }

            //Se não achou no BD, pega na API
            if (DTODaLoteria == null)
            {
                DTODaLoteria = await _loteriaService.ObterPelaLoteriaEConcurso(CodigoLoteria, Concurso);
            }

            if (DTODaLoteria == null)
            {
                return NotFound("Loteria não foi encontrada para os parâmetros informados");
            }
            else
            {
                await SalvarLoteriaNoBD(DTODaLoteria, CodigoLoteria);
            }

            return Ok(DTODaLoteria);
        }



        private async Task SalvarLoteriaNoBD(LoteriaDTO DTOLoteria, String CodigoLoteria)
        {
            var loteriaDoBD = new Loteria.API.Entidade.Loteria();
            loteriaDoBD.Concurso = DTOLoteria.numero;
            loteriaDoBD.CodigoLoteria = CodigoLoteria;
            loteriaDoBD.Resultado = JsonConvert.SerializeObject(DTOLoteria);
            loteriaDoBD.DataCadastro = DateTime.Now;

            loteriaDoBD.Dezena1 = ObterDezena(DTOLoteria.listaDezenas, 0);
            loteriaDoBD.Dezena2 = ObterDezena(DTOLoteria.listaDezenas, 1);
            loteriaDoBD.Dezena3 = ObterDezena(DTOLoteria.listaDezenas, 2);
            loteriaDoBD.Dezena4 = ObterDezena(DTOLoteria.listaDezenas, 3);
            loteriaDoBD.Dezena5 = ObterDezena(DTOLoteria.listaDezenas, 4);
            loteriaDoBD.Dezena6 = ObterDezena(DTOLoteria.listaDezenas, 5);
            if (!String.IsNullOrEmpty(DTOLoteria.dataProximoConcurso))
            {
                loteriaDoBD.DataProximoConcurso = DateTime.ParseExact(DTOLoteria.dataProximoConcurso, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }

            await _loteriaContext.Loterias.AddAsync(loteriaDoBD);
            await _loteriaContext.SaveChangesAsync();
        }

        private string ObterDezena(List<string> listaDezenas, int posicao)
        {
            return (listaDezenas != null && listaDezenas.Count > posicao) ? listaDezenas[posicao] : String.Empty;
        }
    }
}
