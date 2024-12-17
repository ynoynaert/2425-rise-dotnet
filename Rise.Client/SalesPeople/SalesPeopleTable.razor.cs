using Microsoft.AspNetCore.Components;
using Rise.Shared.Helpers;
using Rise.Shared.Users;

namespace Rise.Client.SalesPeople;

public partial class SalesPeopleTable
{
    [Parameter]
    public IEnumerable<UserDto.Index>? SalesPeople { get; set; }
	[Parameter]
	public UserQueryObject Query { get; set; } = new UserQueryObject();
	[Inject]
	public required IUserService UserService { get; set; }
	[Inject]
	public UserQueryService QueryService { get; set; } = default!;

	protected override async Task OnInitializedAsync()
	{
		Query = QueryService.SavedQuery ?? new UserQueryObject();
		SalesPeople = await UserService.GetSalesPeopleAsync(Query);
	}
}
