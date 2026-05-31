namespace EwFrameworkAnalysis.UI.Pages.Docs;

public class DocsContext
{
    private readonly List<DocSectionInfo> _sections = [];

    public event Action? Changed;

    public void Register(DocSectionInfo info)
    {
        _sections.Add(info);
        Changed?.Invoke();
    }

    public void Unregister(string slug)
    {
        _sections.RemoveAll(s => s.Slug == slug);
        Changed?.Invoke();
    }

    public IEnumerable<DocSectionInfo> GetOrderedSections()
    {
        foreach (var root in _sections.Where(s => s.ParentSlug == null))
            foreach (var node in WalkDepthFirst(root))
                yield return node;
    }

    private IEnumerable<DocSectionInfo> WalkDepthFirst(DocSectionInfo node)
    {
        yield return node;
        foreach (var child in _sections.Where(s => s.ParentSlug == node.Slug))
            foreach (var descendant in WalkDepthFirst(child))
                yield return descendant;
    }

    public int GetDepth(string slug)
    {
        var depth = 0;
        var current = _sections.FirstOrDefault(s => s.Slug == slug);
        while (current?.ParentSlug != null)
        {
            depth++;
            current = _sections.FirstOrDefault(s => s.Slug == current.ParentSlug);
        }
        return depth;
    }
}

public record DocSectionInfo(string Slug, string Title, string? ParentSlug);
