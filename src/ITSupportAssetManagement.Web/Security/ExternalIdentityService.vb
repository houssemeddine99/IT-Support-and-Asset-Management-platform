Imports System.Configuration
Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web
Imports System.Web.Script.Serialization
Imports System.Web.SessionState

Namespace Security
    Public NotInheritable Class ExternalIdentityService
        Private Const StateKey As String = "EntraState"
        Private Const VerifierKey As String = "EntraVerifier"

        Public Shared Function IsConfigured() As Boolean
            Return IsGuidSetting("Identity:TenantId") AndAlso IsGuidSetting("Identity:ClientId") AndAlso Not String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings("Identity:ClientSecret"))
        End Function

        Public Function CreateAuthorizationUrl(request As HttpRequest, session As HttpSessionState) As String
            EnsureConfigured()
            Dim state = RandomUrlValue(32), verifier = RandomUrlValue(48)
            session(StateKey) = state : session(VerifierKey) = verifier
            Dim redirectUri = CallbackUrl(request)
            Return Authority("oauth2/v2.0/authorize") & "?client_id=" & Encode(Setting("Identity:ClientId")) & "&response_type=code&redirect_uri=" & Encode(redirectUri) & "&response_mode=query&scope=" & Encode("openid profile email User.Read") & "&state=" & Encode(state) & "&code_challenge=" & Encode(Sha256Url(verifier)) & "&code_challenge_method=S256"
        End Function

        Public Function CompleteAuthorization(request As HttpRequest, session As HttpSessionState) As ExternalIdentity
            EnsureConfigured()
            Dim expectedState = Convert.ToString(session(StateKey)), verifier = Convert.ToString(session(VerifierKey))
            session.Remove(StateKey) : session.Remove(VerifierKey)
            If String.IsNullOrWhiteSpace(expectedState) OrElse Not String.Equals(expectedState, request.QueryString("state"), StringComparison.Ordinal) OrElse String.IsNullOrWhiteSpace(verifier) Then Throw New System.Security.SecurityException("Invalid external sign-in state.")
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
            Using client As New HttpClient()
                Dim values As New Dictionary(Of String, String) From {{"client_id",Setting("Identity:ClientId")},{"client_secret",Setting("Identity:ClientSecret")},{"grant_type","authorization_code"},{"code",request.QueryString("code")},{"redirect_uri",CallbackUrl(request)},{"code_verifier",verifier},{"scope","openid profile email User.Read"}}
                Dim tokenResponse = client.PostAsync(Authority("oauth2/v2.0/token"), New FormUrlEncodedContent(values)).GetAwaiter().GetResult()
                If Not tokenResponse.IsSuccessStatusCode Then Throw New System.Security.SecurityException("Identity token exchange failed.")
                Dim serializer As New JavaScriptSerializer()
                Dim token = serializer.Deserialize(Of Dictionary(Of String,Object))(tokenResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult())
                If Not token.ContainsKey("access_token") Then Throw New System.Security.SecurityException("Identity token was not returned.")
                client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", Convert.ToString(token("access_token")))
                Dim userResponse = client.GetAsync("https://graph.microsoft.com/oidc/userinfo").GetAwaiter().GetResult()
                If Not userResponse.IsSuccessStatusCode Then Throw New System.Security.SecurityException("Identity profile lookup failed.")
                Dim profile = serializer.Deserialize(Of Dictionary(Of String,Object))(userResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult())
                Dim email = Claim(profile,"email")
                If String.IsNullOrWhiteSpace(email) Then email = Claim(profile,"preferred_username")
                If String.IsNullOrWhiteSpace(email) Then Throw New System.Security.SecurityException("The company account has no email claim.")
                Return New ExternalIdentity With {.Email=email.Trim().ToLowerInvariant(),.DisplayName=Claim(profile,"name")}
            End Using
        End Function

        Private Shared Function CallbackUrl(request As HttpRequest) As String
            Return request.Url.GetLeftPart(UriPartial.Authority) & VirtualPathUtility.ToAbsolute("~/ExternalLogin.aspx")
        End Function
        Private Shared Function Authority(path As String) As String
            Return "https://login.microsoftonline.com/" & Setting("Identity:TenantId") & "/" & path
        End Function
        Private Shared Function Setting(key As String) As String
            Return ConfigurationManager.AppSettings(key)
        End Function
        Private Shared Function IsGuidSetting(key As String) As Boolean
            Dim value As Guid
            Return Guid.TryParse(Setting(key),value)
        End Function
        Private Shared Sub EnsureConfigured()
            If Not IsConfigured() Then Throw New ConfigurationErrorsException("Microsoft Entra sign-in is not configured.")
        End Sub
        Private Shared Function Encode(value As String) As String
            Return HttpUtility.UrlEncode(value)
        End Function
        Private Shared Function Claim(values As Dictionary(Of String,Object), key As String) As String
            Return If(values.ContainsKey(key),Convert.ToString(values(key)),String.Empty)
        End Function
        Private Shared Function RandomUrlValue(length As Integer) As String
            Dim bytes(length-1) As Byte
            Using random=RandomNumberGenerator.Create()
                random.GetBytes(bytes)
            End Using
            Return Base64Url(bytes)
        End Function
        Private Shared Function Sha256Url(value As String) As String
            Using sha=SHA256.Create()
                Return Base64Url(sha.ComputeHash(Encoding.ASCII.GetBytes(value)))
            End Using
        End Function
        Private Shared Function Base64Url(value As Byte()) As String
            Return Convert.ToBase64String(value).TrimEnd("="c).Replace("+","-").Replace("/","_")
        End Function

        Public NotInheritable Class ExternalIdentity
            Public Property Email As String
            Public Property DisplayName As String
        End Class
    End Class
End Namespace
