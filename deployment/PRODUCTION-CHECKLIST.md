# Production deployment checklist

1. Provision a supported SQL Server instance and a dedicated least-privilege application login.
2. Back up the database, then run `scripts/Apply-Migrations.ps1` with the production connection string.
3. Replace the release connection string through the deployment system; never commit production credentials.
4. Configure a unique explicit ASP.NET `machineKey` on every server in a farm.
5. Bind a valid TLS certificate in IIS and keep `requireSSL="true"` from `Web.Release.config`.
6. Grant the IIS application-pool identity read/execute access only to the published application directory.
7. Publish with `scripts/Publish-Release.ps1`, deploy the resulting `artifacts/publish` directory, and recycle the application pool.
8. Verify login, ticket creation, attachment download, SLA alerts, asset workflows, maintenance completion, reports, settings, and audit history.
9. Schedule `scripts/Backup-Database.ps1`, copy backups off-host, apply retention, and test a restore regularly.
10. Monitor HTTP 500 responses, failed logins, database storage, SLA breaches, and backup failures.

Before every deployment: create and verify a backup, test the Release build, and keep a copy of the previous published package for rollback.
