# Database setup

The application uses SQL Server. The initial migration creates the user, ticket, asset-assignment, and maintenance data model.

## Create the local database

1. Open SQL Server Management Studio.
2. Create an empty database named `ITSupportAssetManagement`.
3. Select that database and execute `001_initial_schema.sql`.
4. Copy the `Web.config.example` connection string into `Web.config` and set the correct SQL Server instance.

The migration is transactional and safe to execute again. It inserts reference roles and categories only when their tables are empty. It never creates a default user or stores a password.

