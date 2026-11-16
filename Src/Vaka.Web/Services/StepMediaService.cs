using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public sealed class StepMediaService : IStepMediaService
{
    private const int MaxFileNameLength = 255;
    private const int SignatureLength = 12;

    private static readonly Dictionary<string, MediaFormat> Formats = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"] = new(StepMediaType.Photo, "image/jpeg", IsJpeg),
        [".jpeg"] = new(StepMediaType.Photo, "image/jpeg", IsJpeg),
        [".png"] = new(StepMediaType.Photo, "image/png", IsPng),
        [".webp"] = new(StepMediaType.Photo, "image/webp", IsWebP),
        [".mp4"] = new(StepMediaType.Video, "video/mp4", IsMp4)
    };

    private readonly VakaDbContext _db;
    private readonly ISystemSettingsReader _settings;
    private readonly IDurationCalculator _calculator;
    private readonly IMediaStore _store;

    public StepMediaService(VakaDbContext db, ISystemSettingsReader settings, IDurationCalculator calculator, IMediaStore store)
    {
        _db = db;
        _settings = settings;
        _calculator = calculator;
        _store = store;
    }

    public async Task<CommandResult> UploadAsync(int stepId, MediaUploadRequest? upload, CancellationToken cancellationToken)
    {
        var step = await _db.ProcedureSteps
            .Include(item => item.Phase)
            .ThenInclude(phase => phase.Version)
            .FirstOrDefaultAsync(item => item.Id == stepId, cancellationToken);

        if (step is null)
        {
            return CommandResult.NotFound();
        }

        var versionId = step.Phase.ProcedureVersionId;
        if (DraftRules.CheckEditable(step.Phase.Version) is { } blocked)
        {
            return blocked;
        }

        if (upload is null || string.IsNullOrWhiteSpace(upload.FileName))
        {
            return CommandResult.Rejected(versionId, "File", "Dosya seçin.");
        }

        if (upload.Length <= 0)
        {
            return CommandResult.Rejected(versionId, "File", "Dosya boş. İçeriği olan bir fotoğraf ya da video seçin.");
        }

        var extension = Path.GetExtension(upload.FileName);
        if (!Formats.TryGetValue(extension, out var format))
        {
            return CommandResult.Rejected(versionId, "File", "Dosya biçimi desteklenmiyor. İzinli biçimler: JPEG, PNG, WebP, MP4.");
        }

        var isPhoto = format.Type == StepMediaType.Photo;
        var maxMegabytes = await _settings.GetValueAsync(isPhoto ? SettingKeys.MaxPhotoMb : SettingKeys.MaxVideoMb, cancellationToken);
        var tooLarge = CommandResult.Rejected(versionId, "File", $"{ProcedureLabels.MediaType(format.Type)} en çok {maxMegabytes} MB olabilir.");
        if (!_calculator.IsMediaWithinLimit(upload.Length, maxMegabytes))
        {
            return tooLarge;
        }

        var header = new byte[SignatureLength];
        var headerRead = await upload.Content.ReadAtLeastAsync(header, SignatureLength, false, cancellationToken);
        if (headerRead < SignatureLength || !format.HasSignature(header))
        {
            return CommandResult.Rejected(versionId, "File", "Dosya içeriği uzantısıyla uyuşmuyor.");
        }

        upload.Content.Position = 0;
        var stored = await _store.SaveAsync(upload.Content, _calculator.GetMediaLimitBytes(maxMegabytes), cancellationToken);
        if (stored is null)
        {
            return tooLarge;
        }

        var fileName = Path.GetFileName(upload.FileName.Trim());
        _db.StepMedia.Add(new StepMedia
        {
            ProcedureStepId = stepId,
            MediaType = format.Type,
            Sha256 = stored.Sha256,
            OriginalFileName = fileName.Length > MaxFileNameLength ? fileName[..MaxFileNameLength] : fileName,
            ContentType = format.ContentType,
            SizeBytes = stored.SizeBytes
        });

        return await DraftRules.SaveAsync(_db, versionId, cancellationToken);
    }

    public async Task<CommandResult> RemoveAsync(int mediaId, CancellationToken cancellationToken)
    {
        var media = await _db.StepMedia
            .Include(item => item.Step)
            .ThenInclude(step => step.Phase)
            .ThenInclude(phase => phase.Version)
            .FirstOrDefaultAsync(item => item.Id == mediaId, cancellationToken);

        if (media is null)
        {
            return CommandResult.NotFound();
        }

        var version = media.Step.Phase.Version;
        if (DraftRules.CheckEditable(version) is { } blocked)
        {
            return blocked;
        }

        _db.StepMedia.Remove(media);
        return await DraftRules.SaveAsync(_db, version.Id, cancellationToken);
    }

    public async Task<int?> FindStepIdAsync(int mediaId, CancellationToken cancellationToken) =>
        await _db.StepMedia
            .AsNoTracking()
            .Where(item => item.Id == mediaId)
            .Select(item => (int?)item.ProcedureStepId)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<LookupResult<MediaFile>> FindAsync(int mediaId, bool includeDraft, CancellationToken cancellationToken)
    {
        var media = await _db.StepMedia
            .AsNoTracking()
            .Where(item => item.Id == mediaId
                && (includeDraft || item.Step.Phase.Version.Status != ProcedureVersionStatus.Draft))
            .Select(item => new MediaFile(item.Sha256, item.ContentType, item.SizeBytes, item.OriginalFileName))
            .FirstOrDefaultAsync(cancellationToken);

        return media is null ? LookupResult<MediaFile>.Missing() : LookupResult<MediaFile>.Found(media);
    }

    private static bool IsJpeg(byte[] header) => header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;

    private static bool IsPng(byte[] header) =>
        header.AsSpan(0, 8).SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A });

    private static bool IsWebP(byte[] header) =>
        header.AsSpan(0, 4).SequenceEqual("RIFF"u8) && header.AsSpan(8, 4).SequenceEqual("WEBP"u8);

    private static bool IsMp4(byte[] header) => header.AsSpan(4, 4).SequenceEqual("ftyp"u8);

    private sealed record MediaFormat(StepMediaType Type, string ContentType, Func<byte[], bool> HasSignature);
}
