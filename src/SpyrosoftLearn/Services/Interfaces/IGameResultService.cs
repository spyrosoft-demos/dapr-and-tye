using SpyrosoftLearn.Dtos;

namespace SpyrosoftLearn.Services.Interfaces
{
    public interface IGameResultService
    {
        List<GameResultDto> GetGameResults();
    }
}
