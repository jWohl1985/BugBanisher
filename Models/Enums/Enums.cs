namespace BugBanisher.Models.Enums;

public enum Roles
{
    Admin,
    ProjectManager,
    Developer,
	Member,
}

public enum NotificationType
{
	CompanyInvite = 0,
	CompanyInviteAccepted = 1,
	CompanyInviteRejected = 2,
	RemovedFromCompany = 3,
}
