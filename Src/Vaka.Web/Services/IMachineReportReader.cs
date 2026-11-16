using Vaka.Web.ViewModels.Reports;

namespace Vaka.Web.Services;

public interface IMachineReportReader
{
    /// <summary>
    /// Makine raporu (PL-85 madde 5-7, E-34). Makine seçilmediyse hata olmadan boş döner. Tarih süzgeci kaydın açılış anına, Türkiye tarihine göre gün dahil uygulanır.
    /// Tip satırı kayıt sayısını süzgece uyan tüm kayıtlardan, ortalamaları yalnız Tamamlandı kayıtlardan alır (SH-15, versiyonlar birlikte); ortalama planlanan minimum
    /// SH-18 değerlerinin, plan farkı SH-19'dur (acil tipte boş). Süre altı faz ve değerlendirilen faz sayıları Tamamlandı kayıtların faz satırlarındandır; değerlendirilen faz,
    /// minimum anlık değeri ve net süresi dolu fazdır. Son kayıtlar açılış anına göre yeniden eskiye en çok 20 satırdır; durum sayıları süzgece uyan tüm kayıtlardandır.
    /// Malzeme satırları <see cref="IMaterialUsageReader"/> ile aynı süzgeçle gelir.
    /// <paramref name="inputError"/> doluysa ya da süzgeç geçersizse (bitiş başlangıçtan önce, olmayan ya da birbirine uymayan tesis, hat, makine, tanımsız tür) tablolar boş döner
    /// ve <c>ErrorMessage</c> dolar. Seçilen tesis ya da hatla uyuşmayan hat ve makine hata vermeden boşaltılır. Makine seçiliyken boş tesis ve hat makineden doldurulur.
    /// </summary>
    Task<MachineReportViewModel> BuildAsync(MachineReportFilterViewModel filter, string? inputError, CancellationToken cancellationToken);
}
