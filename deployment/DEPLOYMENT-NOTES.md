# Siliana IT Hub production notes

## Required configuration

- Use a supported SQL Server instance and run every numbered migration through `006`.
- Put the production connection string and SMTP credentials in deployment-time configuration, never in Git.
- Set `Mail:Mode` to `Smtp`, provide the relay host/port/SSL values, and use a real DRÄXLMAIER sender address.
- Generate an explicit, secret `machineKey` for production. Every IIS node must use the same keys.
- Require HTTPS and secure cookies. `Web.Release.config` supplies the release cookie settings.
- Give the IIS application-pool identity modify access only to `App_Data/MailPickup` if pickup mode is intentionally retained.

## Release procedure

1. Run `powershell -ExecutionPolicy Bypass -File scripts/Verify-Build.ps1`.
2. Back up and verify the database with `scripts/Backup-Database.ps1`.
3. Apply migrations with `scripts/Apply-Migrations.ps1`.
4. Create the deployment package with `scripts/Publish-Release.ps1`.
5. Deploy through IIS, recycle the pool, and verify login, password recovery, tickets, paginated lists, QR labels, maintenance calendar, reports, and CSV/PDF exports.

The report page's **Print / Save PDF** button uses the browser's native PDF output, so it does not transmit operational data to an external PDF service.
