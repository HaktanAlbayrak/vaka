namespace Vaka.Web.Services;

/// <summary>Yürütme ekranı okuyucularının ortak girdisi: kayıt, görüntüleyen ve okuma anı.</summary>
public sealed record RunContext(int RecordId, int ViewerUserId, int OwnerUserId, RecordStatus Status, bool MaterialRequired, DateTimeOffset Now);
