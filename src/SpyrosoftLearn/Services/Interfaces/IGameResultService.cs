using SpyrosoftLearn.Dtos;

namespace SpyrosoftLearn.Services.Interfaces;

public interface IGameResultService
{
    Task<List<GameResultDto>> GetGameResults();
}
