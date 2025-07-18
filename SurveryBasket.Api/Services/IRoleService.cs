namespace SurveryBasket.Api.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled = false, CancellationToken cancellation = default);
    Task<Result<RoleDetailResponse>> GetDetailAsync(string roleid, CancellationToken cancellation = default);
    Task<Result<RoleDetailResponse>> AddAsync(RoleRequest roleRequest, CancellationToken cancellation = default);
    Task<Result> UpdateAsync(string id, RoleRequest roleRequest, CancellationToken cancellation = default);
    Task<Result> ToggleStatusAsync(string id,CancellationToken cancellation = default);


}
