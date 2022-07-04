using Microsoft.AspNetCore.Components;

namespace CheckAvailability.Client.Pages;

public class PageBase : ComponentBase, IDisposable
{
    protected CancellationTokenSource Cts = new();
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (Cts.IsCancellationRequested) return;
            Cts.Cancel();
            Cts.Dispose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}