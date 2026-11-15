using System.Security.Cryptography;

namespace Vaka.Web.Services;

public sealed class MediaStore : IMediaStore
{
    private readonly string _root;

    public MediaStore(IConfiguration configuration, IHostEnvironment environment)
    {
        var configured = configuration["Media:RootPath"]
            ?? throw new InvalidOperationException("Media:RootPath yapılandırması yok.");
        _root = Path.IsPathRooted(configured) ? configured : Path.Combine(environment.ContentRootPath, configured);
    }

    public async Task<StoredMedia?> SaveAsync(Stream content, long maxBytes, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(_root);
        var temporary = Path.Combine(_root, $"{Guid.NewGuid():N}.tmp");
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        long total = 0;
        try
        {
            await using (var target = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, true))
            {
                var buffer = new byte[81920];
                int read;
                while ((read = await content.ReadAsync(buffer, cancellationToken)) > 0)
                {
                    total += read;
                    if (total > maxBytes)
                    {
                        return null;
                    }

                    hash.AppendData(buffer, 0, read);
                    await target.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
                }
            }

            var sha256 = Convert.ToHexString(hash.GetHashAndReset()).ToLowerInvariant();
            var destination = Path.Combine(_root, sha256);
            if (!File.Exists(destination))
            {
                File.Move(temporary, destination);
            }

            return new StoredMedia(sha256, total);
        }
        finally
        {
            if (File.Exists(temporary))
            {
                File.Delete(temporary);
            }
        }
    }

    public Stream? Open(string sha256)
    {
        if (sha256.Length != 64 || !sha256.All(char.IsAsciiHexDigitLower))
        {
            return null;
        }

        var path = Path.Combine(_root, sha256);
        return File.Exists(path)
            ? new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, true)
            : null;
    }
}
