namespace Vaka.Web.Authorization;

public static class Roles
{
    public const string Operator = "Operator";
    public const string Manager = "Manager";
    public const string Auditor = "Auditor";
    public const string SuperAdmin = "SuperAdmin";

    public const string ManagerOrAuditor = Manager + "," + Auditor;
    public const string ManagerOrSuperAdmin = Manager + "," + SuperAdmin;
    public const string RecordReaders = Operator + "," + Manager + "," + Auditor;
    public const string Any = Operator + "," + Manager + "," + Auditor + "," + SuperAdmin;
}
