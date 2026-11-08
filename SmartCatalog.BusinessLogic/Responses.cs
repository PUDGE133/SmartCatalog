namespace SmartCatalog.BusinessLogic;

public static class Responses
{
    public static string Patch { get; private set; } = "Object successfully patched.";
    public static string Delete { get; private set; } = "Object successfully removed.";
    public static string AlreadyExists { get; private set; } = "Object with this given Key(s) already exists.";
    public static string NotFound { get; private set; } = "Object with this Key(s) not found.";
    public static string NotFoundRelatedObject { get; private set; } = "No related object found for the entity being added.";
    public static string NegativeArgs { get; private set; } = "Arguments cannot be negative numbers.";
    public static string ConflictVersions { get; private set; } = "Version conflict. The object was previously updated.";
}