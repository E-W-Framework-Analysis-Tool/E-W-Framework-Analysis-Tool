namespace EwFrameworkAnalysis.UI.Services;

public sealed class BusyService
{
    // A List (not a Dictionary) so insertion order is preserved — CurrentReason
    // reports the most recently begun (or updated) operation.
    private readonly List<(Guid Id, string Reason)> _tokens = [];
    private readonly Lock _lock = new();

    public event Action? Changed;

    public bool IsBusy
    {
        get { lock (_lock) return _tokens.Count > 0; }
    }

    /// <summary>The reason text for the most recently begun (or updated) busy token, if any.</summary>
    public string? CurrentReason
    {
        get { lock (_lock) return _tokens.Count > 0 ? _tokens[^1].Reason : null; }
    }

    private BusyToken Begin(string reason = "Loading…")
    {
        var id = Guid.NewGuid();
        lock (_lock) _tokens.Add((id, reason));
        NotifyChanged();
        return new BusyToken(id, this);
    }

    public async Task<BusyToken> BeginAsync(string reason = "Loading…")
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

    private void UpdateReason(Guid id, string reason)
    {
        lock (_lock)
        {
            var idx = _tokens.FindIndex(t => t.Id == id);
            if (idx >= 0) _tokens[idx] = (id, reason);
        }
        NotifyChanged();
    }

    private void Release(Guid id)
    {
        lock (_lock) _tokens.RemoveAll(t => t.Id == id);
        NotifyChanged();
    }

    private void NotifyChanged() => Changed?.Invoke();

    // --- Token ---

    public sealed class BusyToken : IDisposable
    {
        private readonly Guid _id;
        private readonly BusyService _svc;
        private bool _disposed;

        internal BusyToken(Guid id, BusyService svc) { _id = id; _svc = svc; }

        /// <summary>Updates the reason text shown for this operation while it is still in progress.</summary>
        public void UpdateReason(string reason) => _svc.UpdateReason(_id, reason);

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _svc.Release(_id);
        }
    }
}
