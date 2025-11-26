using Application.Dtos;
using FluentResults;

namespace Application.Interfaces
{
	public interface ISrsService
	{
		Task<IReadOnlyList<UserKanjiDto>> GetDueKanjiAsync(Guid userId);
		Task<Result> SubmitReviewAsync(Guid userId, Guid kanjiId, string rating);
	}
}
