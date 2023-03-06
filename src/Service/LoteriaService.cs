using Loteria.API.DTO;
using Loteria.API.Util;
using Newtonsoft.Json;

namespace Loteria.API.Service
{
    public class LoteriaService : ILoteriaService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LoteriaService> _logger;

        private const String BASE_URL = "https://servicebus2.caixa.gov.br/portaldeloterias/api/";

        public LoteriaService(HttpClient httpClient, ILogger<LoteriaService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task<LoteriaDTO> ObterPelaLoteriaEConcurso(String Loteria, String Concurso)
        {
            String url = (!String.IsNullOrEmpty(Concurso) && Concurso.ToUpper() == Constantes.ULTIMO)  ? String.Concat(BASE_URL, Loteria) : String.Concat(BASE_URL, Loteria, "/", Concurso);
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(url);
            _logger.LogInformation($"Resultado da loteria {Loteria} Concurso {Concurso} encontrado na API externa.");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                String Resposta = await httpResponseMessage.Content.ReadAsStringAsync();

                LoteriaDTO dto = JsonConvert.DeserializeObject<LoteriaDTO>(Resposta);

                return dto;
            }

            return null;
        }
    }
}
