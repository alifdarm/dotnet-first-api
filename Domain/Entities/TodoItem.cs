namespace MyFirstApi.Domain.Entities;

public sealed class TodoItem
{
    public TodoItem(int id, string title, bool isCompleted)
    {
        Id = id;
        Title = title;
        IsCompleted = isCompleted;
    }

    public int Id { get; }

    public string Title { get; private set; }

    public bool IsCompleted { get; private set; }

    public void UpdateTitle(string title)
    {
        Title = title;
    }

    public void SetCompletion(bool isCompleted)
    {
        IsCompleted = isCompleted;
    }
}
