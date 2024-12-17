namespace Rise.Shared.Quotes;

public abstract class TradedMachineryResult
{
    public class Create
    {
        public int Id { get; set; }
        public List<string> UploadUris { get; set; } = default!;
    }
}
