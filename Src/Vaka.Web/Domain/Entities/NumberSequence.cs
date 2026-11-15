namespace Vaka.Web.Domain.Entities;

public class NumberSequence
{
    public string Scope { get; set; } = string.Empty;
    public int Year { get; set; }
    public int LastValue { get; set; }
}
