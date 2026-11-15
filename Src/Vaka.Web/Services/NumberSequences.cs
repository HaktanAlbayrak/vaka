using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public sealed class NumberSequences : INumberSequences
{
    private readonly VakaDbContext _db;

    public NumberSequences(VakaDbContext db)
    {
        _db = db;
    }

    public async Task<int> NextAsync(string scope, int year, CancellationToken cancellationToken)
    {
        var row = await _db.NumberSequences.FirstOrDefaultAsync(item => item.Scope == scope && item.Year == year, cancellationToken);
        if (row is null)
        {
            row = new NumberSequence { Scope = scope, Year = year, LastValue = 1 };
            _db.NumberSequences.Add(row);
        }
        else
        {
            row.LastValue++;
        }

        await _db.SaveChangesAsync(cancellationToken);
        return row.LastValue;
    }
}
