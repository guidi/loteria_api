using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace Loteria.API.Extension
{
    public static class HttpContextExtensions
    {
        private const string CloudflareHeader = "CF-Connecting-IP";
        private const string ForwardedForHeader = "X-Forwarded-For";

        public static string ObterIpOrigem(this HttpContext context)
        {
            if (context == null)
            {
                return string.Empty;
            }

            if (EstaEmProducao(context))
            {
                var cloudflareIp = ObterPrimeiroIpDoCabecalho(context, CloudflareHeader);
                if (!string.IsNullOrEmpty(cloudflareIp))
                {
                    return cloudflareIp;
                }
            }

            var forwardedIp = ObterPrimeiroIpDoCabecalho(context, ForwardedForHeader);
            if (!string.IsNullOrEmpty(forwardedIp))
            {
                return forwardedIp;
            }

            var remoteIp = context.Connection.RemoteIpAddress;
            if (remoteIp == null)
            {
                return string.Empty;
            }

            if (remoteIp.IsIPv4MappedToIPv6)
            {
                remoteIp = remoteIp.MapToIPv4();
            }

            return remoteIp.ToString();
        }

        private static bool EstaEmProducao(HttpContext context)
        {
            if (context.RequestServices.GetService(typeof(IHostEnvironment)) is IHostEnvironment hostEnvironment)
            {
                return hostEnvironment.IsProduction();
            }

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return string.Equals(environment, Environments.Production, StringComparison.OrdinalIgnoreCase);
        }

        private static string ObterPrimeiroIpDoCabecalho(HttpContext context, string cabecalho)
        {
            if (context.Request.Headers.TryGetValue(cabecalho, out var valores) != true)
            {
                return string.Empty;
            }

            var valorCabecalho = valores.ToString();
            if (string.IsNullOrWhiteSpace(valorCabecalho))
            {
                return string.Empty;
            }

            var possiveisIps = valorCabecalho
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(x => !string.IsNullOrWhiteSpace(x));

            foreach (var candidato in possiveisIps)
            {
                if (TryNormalizarIp(candidato, out var ip))
                {
                    return ip;
                }
            }

            return string.Empty;
        }

        private static bool TryNormalizarIp(string valor, out string ip)
        {
            ip = string.Empty;

            if (string.IsNullOrWhiteSpace(valor))
            {
                return false;
            }

            var candidato = valor.Trim();

            if (IPAddress.TryParse(candidato, out var endereco))
            {
                ip = NormalizarIp(endereco);
                return true;
            }

            var indiceUltimoDoisPontos = candidato.LastIndexOf(':');
            if (indiceUltimoDoisPontos > 0 && candidato.Count(x => x == ':') == 1)
            {
                var possivelIp = candidato[..indiceUltimoDoisPontos];
                if (IPAddress.TryParse(possivelIp, out var enderecoSemPorta))
                {
                    ip = NormalizarIp(enderecoSemPorta);
                    return true;
                }
            }

            return false;
        }

        private static string NormalizarIp(IPAddress endereco)
        {
            if (endereco.IsIPv4MappedToIPv6)
            {
                endereco = endereco.MapToIPv4();
            }

            return endereco.ToString();
        }
    }
}
