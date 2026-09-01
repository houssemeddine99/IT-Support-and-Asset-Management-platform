# Microsoft Entra ID setup

Company sign-in is hidden until all three identity settings are supplied.

1. Register a Web application in the DRÄXLMAIER Microsoft Entra tenant.
2. Add the redirect URI `https://your-host/ExternalLogin.aspx` (for local development: `https://localhost:44372/ExternalLogin.aspx`).
3. Add delegated Microsoft Graph permissions `openid`, `profile`, `email`, and `User.Read`, then grant the consent required by company policy.
4. Create a client secret and store it in the deployment secret store.
5. Set `Identity:TenantId`, `Identity:ClientId`, and `Identity:ClientSecret` at deployment time.

The flow uses authorization code, PKCE, a server-side state value, TLS, and Microsoft Graph's OIDC user-info endpoint. It never auto-creates accounts: the Entra email must already match an active Siliana IT Hub user. This keeps roles and access approval under the local administrator's control.

Rotate the client secret before expiry and never commit it to source control.
