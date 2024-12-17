namespace Rise.Client;
public partial class App
{
    protected override void OnInitialized()
    {
        // Redirect direct naar /machines bij het opstarten
        if (Navigation.Uri.EndsWith('/'))
        {
            Navigation.NavigateTo("/machines");
        }
    }
}
