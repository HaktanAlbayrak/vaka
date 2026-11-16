namespace Vaka.Web.Services;

/// <summary>Görevlinin adımdaki durumu (Faz 6E): listede girişsiz, adım başlamadan girişli, çalışıyor ya da ayrılmış.</summary>
public enum ParticipantState
{
    Pending,
    Confirmed,
    Working,
    Left
}
