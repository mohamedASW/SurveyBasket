namespace SurveryBasket.Api.Services;

public interface IUserService
{
    Task<PaginatedList<UserResponse>> GetAll(PaginationFilter filter, CancellationToken cancellationToken = default);
    Task<Result<UserResponse>> GetAsync(string id);
    Task<Result<UserResponse>> CreateAsync(UserRequest request);
    Task<Result> UpdateAsync(string id , UpdateUserRequest request , CancellationToken cancellation);
    Task<Result> UnLock(string id);
    Task<Result> ToggleStatus(string id);
    
}
