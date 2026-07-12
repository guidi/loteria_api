using Loteria.API.DTO;
using Loteria.API.Exceptions;
using Loteria.API.Util;
using Newtonsoft.Json;
using System.Net;

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

        public async Task<LoteriaDTO?> ObterPelaLoteriaEConcurso(String Loteria, String Concurso)
        {
            String url = (!String.IsNullOrEmpty(Concurso) && Concurso.ToUpper() == Constantes.ULTIMO)
                ? String.Concat(BASE_URL, Loteria)
                : String.Concat(BASE_URL, Loteria, "/", Concurso);

            HttpResponseMessage httpResponseMessage;
            try
            {
                httpResponseMessage = await _httpClient.GetAsync(url);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Timeout ao consultar a API externa da loteria {Loteria} concurso {Concurso}.",
                    Loteria,
                    Concurso);

                throw new ExternalLotteryServiceUnavailableException(
                    "Nao foi possivel consultar a API externa da loteria no tempo esperado.",
                    ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Falha de rede ao consultar a API externa da loteria {Loteria} concurso {Concurso}.",
                    Loteria,
                    Concurso);

                throw new ExternalLotteryServiceUnavailableException(
                    "Nao foi possivel consultar a API externa da loteria neste momento.",
                    ex);
            }

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                String resposta = await httpResponseMessage.Content.ReadAsStringAsync();

                try
                {
                    LoteriaDTO? dto = JsonConvert.DeserializeObject<LoteriaDTO>(resposta);
                    if (dto == null)
                    {
                        throw new JsonException("A API externa retornou um payload vazio ou invalido.");
                    }

                    _logger.LogInformation(
                        "Resultado da loteria {Loteria} concurso {Concurso} encontrado na API externa.",
                        Loteria,
                        Concurso);

                    return dto;
                }
                catch (JsonException ex)
                {
                    _logger.LogWarning(
                        ex,
                        "Payload invalido retornado pela API externa da loteria {Loteria} concurso {Concurso}.",
                        Loteria,
                        Concurso);

                    throw new ExternalLotteryServiceBadResponseException(
                        "A API externa da loteria retornou um payload invalido.",
                        ex);
                }
            }

            if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound
                || httpResponseMessage.StatusCode == HttpStatusCode.BadRequest)
            {
                return null;
            }

            _logger.LogWarning(
                "A API externa da loteria retornou status {StatusCode} para a loteria {Loteria} concurso {Concurso}.",
                (int)httpResponseMessage.StatusCode,
                Loteria,
                Concurso);

            throw new ExternalLotteryServiceUnavailableException(
                $"A API externa da loteria retornou status {(int)httpResponseMessage.StatusCode}.");
        }
    }
}
