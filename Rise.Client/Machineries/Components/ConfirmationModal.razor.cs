using Blazorise;
using Microsoft.AspNetCore.Components;

namespace Rise.Client.Machineries.Components;

public partial class ConfirmationModal
{
    [Parameter]
    public required string Title { get; set; }
    private string? Message { get; set; }

    [Parameter]
    public EventCallback<bool> OnConfirm { get; set; }

    private Modal? modalRef;
    private bool cancelClose;

    public async Task ShowAsync(string message)
    {
        Message = message; 
        await modalRef?.Show()!;
    }

    private Task CloseModal()
    {
        cancelClose = false;
        return modalRef?.Hide()!;
    }

    private Task OnModalClosing(ModalClosingEventArgs e)
    {
        e.Cancel = cancelClose || e.CloseReason != CloseReason.UserClosing;
        return Task.CompletedTask;
    }

    private async Task Confirm()
    {
        await OnConfirm.InvokeAsync(true);
        await CloseModal();
    }
}
