namespace Rise.Shared.Helpers
{
	public class UnacceptedTranslationQueryObject
	{
		public int PageNumber { get; set; } = 1;

		public int PageSize { get; set; } = 5;

		public bool HasNext { get; set; } = true;
	}
}
