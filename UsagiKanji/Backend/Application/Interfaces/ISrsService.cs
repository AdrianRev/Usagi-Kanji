using Application.Dtos;
using FluentResults;

namespace Application.Interfaces
{
	public interface ISrsService
	{
		Task<IReadOnlyList<UserKanjiDto>> GetDueKanjiAsync(Guid userId);
		Task<Result> SubmitBatchReviewAsync(Guid userId, SubmitBatchReviewDto dto);
	}
}