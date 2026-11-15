namespace Vaka.Web.Services;

public interface IRecordBlockFinder
{
    /// <summary>
    /// Makine kilidi (BR-23) ve sahip kilidi (BR-37) için okuma denetimi; kilidi tutan kayıt varsa nedenli ret ve kayıt kimliği döner, yoksa boş.
    /// Eşzamanlı yarışta asıl garanti veritabanı index'leridir.
    /// </summary>
    Task<OpenOutcome?> FindAsync(int machineId, int ownerUserId, CancellationToken cancellationToken);
}
