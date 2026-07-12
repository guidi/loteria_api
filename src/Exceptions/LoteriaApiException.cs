using Microsoft.AspNetCore.Http;

namespace Loteria.API.Exceptions
{
    public abstract class LoteriaApiException : Exception
    {
        protected LoteriaApiException(int statusCode, string title, string message, Exception? innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            Title = title;
        }

        public int StatusCode { get; }

        public string Title { get; }
    }

    public sealed class ExternalLotteryServiceUnavailableException : LoteriaApiException
    {
        public ExternalLotteryServiceUnavailableException(string message, Exception? innerException = null)
            : base(
                StatusCodes.Status503ServiceUnavailable,
                "Servico externo indisponivel",
                message,
                innerException)
        {
        }
    }

    public sealed class ExternalLotteryServiceBadResponseException : LoteriaApiException
    {
        public ExternalLotteryServiceBadResponseException(string message, Exception? innerException = null)
            : base(
                StatusCodes.Status502BadGateway,
                "Resposta invalida do servico externo",
                message,
                innerException)
        {
        }
    }
}
