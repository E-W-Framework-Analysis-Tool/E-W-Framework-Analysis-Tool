namespace EwFrameworkAnalysis.UI.Services;

public sealed class BusyService
{
    private readonly Dictionary<Guid, string> _tokens = [];
    private readonly Lock _lock = new();

    public event Action? Changed;

    public bool IsBusy
    {
        get { lock (_lock) return _tokens.Count > 0; }
    }

    private IDisposable Begin(string reason = "Loading…")
    {
        var id = Guid.NewGuid();
        lock (_lock) _tokens[id] = reason;
        NotifyChanged();
        return new BusyToken(id, this);
    }

    public async Task<IDisposable> BeginAsync(string reason = "Loading…")
    {
        var token = Begin(reason);
        await Task.Yield();
        return token;
    }

    /// <summary>
    /// Convenience wrapper: runs the task while holding a busy token.
    /// Clears the token even if the task throws.
    /// </summary>
    public async Task WhileBusyAsync(Func<Task> work, string reason = "Loading…")
    {
        using var _ = Begin(reason);
        await Task.Yield(); // ensure renderer paints before work starts
        await work();
    }

    public async Task<T> WhileBusyAsync<T>(Func<Task<T>> work, string reason = "Loading…")
    {
        using var _ = Begin(reason);
        await Task.Yield(); // ensure renderer paints before work starts
        return await work();
    }

    internal void Release(Guid id)
    {
        lock (_lock) _tokens.Remove(id);
        NotifyChanged();
    }

    private void NotifyChanged() => Changed?.Invoke();

    // --- Token ---

    private sealed class BusyToken : IDisposable
    {
        private readonly Guid _id;
        private readonly BusyService _svc;
        private bool _disposed;

        public BusyToken(Guid id, BusyService svc) { _id = id; _svc = svc; }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _svc.Release(_id);
        }
    }
}
