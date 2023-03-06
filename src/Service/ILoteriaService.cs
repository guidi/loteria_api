using Loteria.API.DTO;

namespace Loteria.API.Service
{
    public interface ILoteriaService
    {
        Task<LoteriaDTO> ObterPelaLoteriaEConcurso(String Loteria, String Concurso);
    }
}
