using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Vaka.Web.Authorization;
using Vaka.Web.Data;
using Microsoft.AspNetCore.Mvc.Razor;
using Vaka.Web.Routing;
using Vaka.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllersWithViews(options =>
{
    options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseParameterTransformer()));
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    options.Filters.Add(typeof(FirstLoginGateFilter), -1000);
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

builder.Services.Configure<RazorViewEngineOptions>(options => options.ViewLocationExpanders.Add(new RecordViewLocationExpander()));

var connectionString = new SqliteConnectionStringBuilder(builder.Configuration.GetConnectionString("Default"));
if (!Path.IsPathRooted(connectionString.DataSource))
{
    connectionString.DataSource = Path.Combine(builder.Environment.ContentRootPath, connectionString.DataSource);
}

builder.Services.AddDbContext<VakaDbContext>(options => options.UseSqlite(connectionString.ToString()));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/account/login";
        options.AccessDeniedPath = "/account/access-denied";
        options.EventsType = typeof(VakaCookieEvents);
        options.SlidingExpiration = false;
        // Teknik tavan: işlemsizlik kararı OturumSüresi ayarından ve son istek anından gelir (VakaCookieEvents).
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
    });

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<ICredentialHasher, CredentialHasher>();
builder.Services.AddSingleton<ITurkeyClock, TurkeyClock>();
builder.Services.AddSingleton<IDurationCalculator, DurationCalculator>();
builder.Services.AddSingleton<IDurationFormatter, DurationFormatter>();
builder.Services.AddSingleton<ISessionRevocations, SessionRevocations>();
builder.Services.AddScoped<VakaCookieEvents>();
builder.Services.AddScoped<ISystemSettingsReader, SystemSettingsReader>();
builder.Services.AddScoped<IPinFormatValidator, PinFormatValidator>();
builder.Services.AddScoped<IPinService, PinService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IOpenWorkProvider, OpenWorkProvider>();
builder.Services.AddScoped<IMachineBlockProvider, MachineBlockProvider>();
builder.Services.AddScoped<IFacilityService, FacilityService>();
builder.Services.AddScoped<ILineService, LineService>();
builder.Services.AddScoped<IShiftService, ShiftService>();
builder.Services.AddScoped<IMachineService, MachineService>();
builder.Services.AddScoped<IMachineRetirement, MachineRetirement>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IMaterialLotService, MaterialLotService>();
builder.Services.AddSingleton<IMediaStore, MediaStore>();
builder.Services.AddScoped<UploadLimitFilter>();
builder.Services.AddScoped<IProcedureLinks, ProcedureLinks>();
builder.Services.AddScoped<ICleaningTypeBlockProvider, CleaningTypeBlockProvider>();
builder.Services.AddScoped<ICleaningTypeService, CleaningTypeService>();
builder.Services.AddScoped<IMachineMappingService, MachineMappingService>();
builder.Services.AddScoped<IProcedureVersionReader, ProcedureVersionReader>();
builder.Services.AddScoped<IProcedureVersionPublisher, ProcedureVersionPublisher>();
builder.Services.AddScoped<IProcedureDraftEditor, ProcedureDraftEditor>();
builder.Services.AddScoped<IProcedureStepEditor, ProcedureStepEditor>();
builder.Services.AddScoped<IStepMediaService, StepMediaService>();
builder.Services.AddScoped<IUserDirectory, UserDirectory>();
builder.Services.AddScoped<IUserAdministration, UserAdministration>();
builder.Services.AddSingleton<IEventDigest>(_ => new HmacEventDigest(EventChainKey.Parse(builder.Configuration[EventChainKey.ConfigurationKey])));
builder.Services.AddScoped<IWriteTransactionRunner, WriteTransactionRunner>();
builder.Services.AddScoped<ICleaningEventWriter, CleaningEventWriter>();
builder.Services.AddScoped<IProcessedCommands, ProcessedCommands>();
builder.Services.AddScoped<INumberSequences, NumberSequences>();
builder.Services.AddScoped<IShiftLocator, ShiftLocator>();
builder.Services.AddScoped<IRecordTimeouts, RecordTimeouts>();
builder.Services.AddScoped<ILotCatalog, LotCatalog>();
builder.Services.AddScoped<IRecordOptions, RecordOptions>();
builder.Services.AddScoped<IRecordCreateForm, RecordCreateForm>();
builder.Services.AddScoped<IRecordOpenValidator, RecordOpenValidator>();
builder.Services.AddScoped<IRecordOpener, RecordOpener>();
builder.Services.AddScoped<IRecordRunReader, RecordRunReader>();
builder.Services.AddScoped<IRecordMaterialService, RecordMaterialService>();
builder.Services.AddScoped<IBreakReader, BreakReader>();
builder.Services.AddScoped<IBreakService, BreakService>();
builder.Services.AddScoped<IParticipantDirectory, ParticipantDirectory>();
builder.Services.AddSingleton<IRecordTimelineBuilder, RecordTimelineBuilder>();
builder.Services.AddScoped<IParticipantService, ParticipantService>();
builder.Services.AddScoped<IRunBreakReader, RunBreakReader>();
builder.Services.AddScoped<IRunProgressReader, RunProgressReader>();
builder.Services.AddScoped<IRunCompletion, RunCompletion>();
builder.Services.AddScoped<IStepLocator, StepLocator>();
builder.Services.AddScoped<IStepStarter, StepStarter>();
builder.Services.AddScoped<IStepFinisher, StepFinisher>();
builder.Services.AddScoped<IMyWorkReader, MyWorkReader>();
builder.Services.AddScoped<IDashboardReader, DashboardReader>();
builder.Services.AddScoped<IMachineBoardReader, MachineBoardReader>();
builder.Services.AddScoped<IRecordListReader, RecordListReader>();
builder.Services.AddScoped<IRecordStateAudit, RecordStateAudit>();
builder.Services.AddScoped<IRecordIntegrity, RecordIntegrity>();
builder.Services.AddScoped<IIntegrityReportReader, IntegrityReportReader>();
builder.Services.AddScoped<IRecordPhaseReader, RecordPhaseReader>();
builder.Services.AddScoped<IRecordDetailReader, RecordDetailReader>();
builder.Services.AddScoped<IInterventionForms, InterventionForms>();
builder.Services.AddScoped<IInterventionService, InterventionService>();
builder.Services.AddScoped<IRecordBlockFinder, RecordBlockFinder>();
builder.Services.AddScoped<IRecordPhaseBuilder, RecordPhaseBuilder>();
builder.Services.AddScoped<IRecordLotChecker, RecordLotChecker>();
builder.Services.AddScoped<IRecordMachineChecker, RecordMachineChecker>();
builder.Services.AddScoped<IMaterialEntryResolver, MaterialEntryResolver>();
builder.Services.AddScoped<IStepStartApplier, StepStartApplier>();
builder.Services.AddScoped<IEmergencyOpenValidator, EmergencyOpenValidator>();
builder.Services.AddScoped<IEmergencyOpener, EmergencyOpener>();
builder.Services.AddScoped<IEmergencyForm, EmergencyForm>();
builder.Services.AddScoped<IReportOptions, ReportOptions>();
builder.Services.AddScoped<IDurationReportReader, DurationReportReader>();
builder.Services.AddScoped<IMaterialTraceReader, MaterialTraceReader>();

var app = builder.Build();

try
{
    app.Services.GetRequiredService<IEventDigest>();
}
catch (InvalidOperationException exception)
{
    app.Logger.LogCritical("Olay özeti anahtarı geçersiz, uygulama başlamıyor: {Reason}", exception.Message);
    return 1;
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}

app.UseStatusCodePagesWithReExecute("/error/{0}");
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
return 0;
