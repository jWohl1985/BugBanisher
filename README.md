# BugBanisher

An ASP.NET Core MVC issue tracking application. The live website can be found here: http://bugbanisher.up.railway.app

This is a multi-tenant application. Each user can create or join a company. Companies have employees and projects, and projects have tickets which represent a task that needs to be completed. Within a company, a user can be an administrator, project manager, developer, or member.

Administrators can do pretty much everything -- invite people to join the company, remove users from the company, assign users to projects, create/edit/archive/delete projects, etc.

Project Managers can view and edit the projects they are assigned to manage. They can also create and edit tickets for those projects, and assign developers/members for those projects. They cannot view or edit projects that they are not managing.

Developers can create and edit tickets for projects they are assigned to.

Members are limited to viewing the projects and tickets they are assigned to.

All users can post comments and add attachments to a ticket. Each ticket keeps a record of its history.

