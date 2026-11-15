namespace Vaka.Web.Services;

/// <summary>Molanın kapanış türü (PL-19). Faz 5'te yalnız <see cref="Person"/> yazılır; diğerleri Faz 6 ve Faz 8'dedir.</summary>
public enum BreakEndKind
{
    Person = 1,
    Manager = 2,
    Intervention = 3
}
