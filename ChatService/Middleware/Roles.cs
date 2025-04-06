namespace ChatService.Middleware;

public class Roles
{
    public string Name { get; }

    private Roles(string name)
    {
        Name = name;
    }

    public static readonly Roles Worker = new("worker");
    public static readonly Roles Startupper = new("startupper");

    public override string ToString() => Name;
}