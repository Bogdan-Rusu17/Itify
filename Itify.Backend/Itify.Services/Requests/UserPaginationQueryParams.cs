using Itify.Database.Repository.Enums;
using Itify.Infrastructure.Requests;

namespace Itify.Services.Requests;

public class UserPaginationQueryParams : PaginationSearchQueryParams
{
    public UserRoleEnum? Role { get; set; }
}
