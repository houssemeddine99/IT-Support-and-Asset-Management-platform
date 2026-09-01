# Database setup

The application uses SQL Server. The initial migration creates the user, ticket, asset-assignment, and maintenance data model.

## Create the local database

1. Open SQL Server Management Studio.
2. Create an empty database named `ITSupportAssetManagement`.
3. Select that database and execute `001_initial_schema.sql`, followed by each numbered migration in order.
4. Copy the `Web.config.example` connection string into `Web.config` and set the correct SQL Server instance.

The migrations are transactional and safe to execute again. They insert reference roles and categories only when their tables are empty. They never create a default user or store a plaintext password. Migration `005` makes administrator-issued passwords temporary and forces the employee to replace them after signing in. Migration `006` adds single-use, expiring password-reset tokens; only SHA-256 token hashes are stored.

## Development email delivery

`Web.config` defaults to pickup mode, so password-reset messages appear as `.eml` files under `App_Data/MailPickup`. For production, set `Mail:Mode` to `Smtp` and provide the relay settings shown in `Web.config.example`. Do not commit real SMTP credentials.
