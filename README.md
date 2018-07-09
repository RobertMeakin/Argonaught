# Argonaught
The aim of Argonaught is to make access and refresh token authentication in ASP.NET Core simple.

It's possible (admittedly at a push) to go from creating a blank webapi project to having the demo up and running in less than a minute.

This code has not yet been used in a production environment. It is currently in BETA. Feedback welcome.

### Quick Tutorial
In command line / terminal:
```
mkdir ArgonaughtTest
cd ArgonaughtTest
dotnet new webapi
dotnet add package Argonaught -v 1.0.2-beta
dotnet restore
```

Open the application folder in a code editor. I use Visual Studio Code:

```
code .
```

Open *Startup.cs* and add the following imports:
```csharp
using Argonaught;
using Argonaught.Model;
using System.Security.Claims;
```

In the  **ConfigureServices** method add the following policy using, services.Configure:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add framework services.
    services.AddMvc();

    //New code.
    services.Configure<Microsoft.AspNetCore.Authorization.AuthorizationOptions>(options =>
    {
        options.AddPolicy("ExamplePolicy", policy => policy.RequireClaim("canDoThis", "True").RequireClaim("canDoThat", "True"));
    });

    //The audience details will normally be stored as environment variables in the app using the tokens for authorisation / authenticaion.
    var audience = ArgonaughtTesting.Audiences.FirstOrDefault(x => x.Id == "ExampleAudience1");
    services.AddArgonaught(audience);
    //End new code.
}
```

Underneath the *Startup* class, copy and paste the following static class, this contains an example of everything needed for Argonaught configuration. There is no need to pay attention to this yet. Details are provided below this tutorial.
```csharp
public static class ArgonaughtTesting
{
    private static readonly string _issuer = "ExampleIssuer";
    public static Audience ExampleAudience1 { get { return new Audience("ExampleAudience1", _issuer, "mysecret123mysecret123", true, 4320, "*"); } }
    public static Audience ExampleAudience2 { get { return new Audience("ExampleAudience2", _issuer, "321mysecret321mysecret", true, 2160, "http://localhost:4200, http://localhost:9900,http://localhost:9900"); } }

    public static List<Audience> Audiences { get { return new List<Audience> { ExampleAudience1, ExampleAudience2 }; } }

    public static RefreshToken ExampleRefreshToken
    {
        get
        {
            var issued = DateTime.Today.AddDays(-1);
            var expires = DateTime.Today.AddDays(1);

            //61422b7c3843478184772fb75313123b is what's passed in. It is hashed to oCG3Gy0biuFNEDpXUSsCc97XRQs5u1yCOuVyB0dX3KI=
            var rt = new RefreshToken("oCG3Gy0biuFNEDpXUSsCc97XRQs5u1yCOuVyB0dX3KI=", "doej", ExampleAudience1.Id, issued, expires, "yfnwhLWpOoyD/2pH7Sb/XPhzRXpA86m0PZpoeJzzadWprravn/94Yo9OKhqnacbrge8K+ust5xOWqWpsRInKvBQRdMGFfcz09TLi/yeo+3vK7tPK1nJXM2DHOuRVkA7I3m9k4IF0+b0dCG1Gll9PCNauMkLhmsid8b8dppI1lGN/P/hptw37P4bIJ8E7t2asGXbDnx790EzrNIEJPrwPQbyuzxhKfu2PHb0s54htgbJTioSiw/A/nPhuUpm2pxyg3WEM6xvQxY4XwcGcRehQK94ROxNwmV2SLhMV8kIW4di1xSFZlujWcs6R67FFmSA4j/b110ta8LvVWpDoexYvlxfDwlXPboC/BwVPAjzWpYu44lvJZ9duVJK4HS+n+v9VtqnWEhSWdPBn42tz+qyCgSkwV/vm9KORjb2pf7qC3yF4haze2HqNZSLpvg5Kn5lx8Skp+Vhc/4XrofZON3ThRA==");
            return rt;
        }
    }

    //Custom classes for persistence to database. 
    //Note that the interfaces for audience and refresh token allow for private setters. This is to encourage invariant protection. Values can be set through initialisation only.
    public class Audience : IAudience
    {
        public Audience(string id, string issuer, string secret, bool active, int refreshTokenLifetimeMinutes, string allowedOrigins)
        {
            this.Id = id;
            this.Issuer = issuer;
            this.Secret = secret;
            this.Active = active;
            this.RefreshTokenLifetimeMinutes = refreshTokenLifetimeMinutes;
            this.AllowedOrigins = allowedOrigins;
        }

        private Audience() { } //Blank constructor for ORM

        public string Id { get; private set; }

        public string Issuer { get; private set; }

        public string Secret { get; private set; }

        public bool Active { get; private set; }

        public int RefreshTokenLifetimeMinutes { get; private set; }

        public string AllowedOrigins { get; private set; }

    }

    public class RefreshToken : IRefreshToken
    {
        public RefreshToken(string id, string subject, string audienceId, DateTime issuedUtc, DateTime expiresUtc, string protectedTicket)
        {
            this.Id = id;
            this.Subject = subject;
            this.AudienceId = audienceId;
            this.IssuedUtc = issuedUtc;
            this.ExpiresUtc = expiresUtc;
            this.ProtectedTicket = protectedTicket;
        }
        
        public static RefreshToken NewFromObjectImplementingInterface(IRefreshToken rt)
        {
            return new RefreshToken(rt.Id, rt.Subject, rt.AudienceId, rt.IssuedUtc, rt.ExpiresUtc, rt.ProtectedTicket);
        }

        private RefreshToken() { } //Blank constructor for ORM

        public string Id { get; private set; }

        public string Subject { get; private set; }

        public string AudienceId { get; private set; }

        public DateTime IssuedUtc { get; private set; }

        public DateTime ExpiresUtc { get; private set; }

        public string ProtectedTicket { get; private set; }

        public override string ToString()
        {
            return "Refresh Token. Id: " + this.Id + "; Subject: " + this.Subject + "; AudienceId: " + this.AudienceId
            + "; IssuedUtc: " + this.IssuedUtc + "; ExpiresUtc" + this.ExpiresUtc + "; ProtectedTicket: " + this.ProtectedTicket;
        }

    }

    //Methods and Functions
    public static ArgonaughtUser ExampleValidateUserFunction(string username, string password, string audienceId)
    {
        //Called by Argonaught when a user attempts to get an access token using a username and password.
        //Validate user with database and return claims
        //This function should also validate that this user is allowed to be part of the audience passed in above. It will then return the full audience object.
        //There may be one or several audiences available to the app.
        //If this user does not match the audience, the user should be marked as invalid and null passed as the audience.
        var userValidated = true;
        var au = ArgonaughtTesting.ExampleAudience1;
        var user = new ArgonaughtUser(userValidated, au);
        user.Claims.Add(new Claim("canDoThis", "True"));
        user.Claims.Add(new Claim("canDoThat", "True"));

        return user;
    }

    public static RefreshResponse ExampleRefreshResponse(string refreshToken)
    {
        //Called by Argonaught when the client requests a new access token using a refresh token.
        //Requires 2 objects from db: 
        var rfshToken = ArgonaughtTesting.ExampleRefreshToken;//The full RefreshToken object, found using the refreshTokenId (the hashed client refresh token)
        var aud = ArgonaughtTesting.ExampleAudience1;//... and the audience object associated with the refresh token.

        var refreshResponse = new RefreshResponse(rfshToken, aud);
        return refreshResponse;
    }

    public static void ExampleRefreshTokenGeneratedHandler(IRefreshToken refreshToken)
    {
        //Called by Argonaught whenever it creates a new refresh token.
        //Save new refresh token to database and delete any relevant existing tokens for this user (i.e. same audience).
        var myRefreshToken = RefreshToken.NewFromObjectImplementingInterface(refreshToken);

        System.Console.WriteLine(nameof(ExampleRefreshTokenGeneratedHandler));
        System.Console.WriteLine(myRefreshToken);
    }
}
```

In Startup.cs, **Configure** method add Argonaught to the pipeline, as below. **Note**, it is important that Argonaught is added **before**  `app.UseMvc();`:
```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    loggerFactory.AddConsole(Configuration.GetSection("Logging"));
    loggerFactory.AddDebug();
    
    //New code. Place Argonaught before app.UseMvc();
    //This will be persisted on the authorisation/authentication server.
    var argonautOptions = new ArgonautOptions(
        ArgonaughtTesting.Audiences, 
        ArgonaughtTesting.ExampleValidateUserFunction, 
        ArgonaughtTesting.ExampleRefreshResponse, 
        ArgonaughtTesting.ExampleRefreshTokenGeneratedHandler);
    argonautOptions.AccessTokenLifetimeMinutes = 60;

    app.UseArgonaut(argonautOptions);
    //End new code.

    app.UseMvc();
}
```

Finally, in *Controllers\ValuesController.cs*, add the following import.

```csharp
using Microsoft.AspNetCore.Authorization;
```

And add the authorize attribute to the class (or the functions individually) to require a valid access token to access these functions.

```csharp
[Route("api/[controller]")]
[Authorize(Policy = "ExamplePolicy")]//<-- New code
public class ValuesController : Controller
{
...
```

The setup is now complete and can be tested by running or debugging your application and opening an API testing tool, like [Postman](https://www.getpostman.com/).


**Or, Authenticate and Authorise On Controllers Using Custom Attribute**

```csharp
[Route("api/[controller]")]
[AuthorizeClaim("can_do_this", "true")]//<-- New code
public class ValuesController : Controller
{
...
```

`AuthorizeClaim` is available here: `using Argonaught.Authentication.Authorization;`

The attribute code is also included at the bottom in case developers want to modify it.

___

### Seperate Authentication Server

It is possible to use Argonaught on both the authentication server (whose job it is to check the user's password and permissions and return the access token) and the data server (that only needs to check the user has the correct claims).

If a server only needs to verify permissions and not create the tokens, only pass in the audiences to the ArgonautOptions object. That feature will then be turned off.

```csharp
var argonautOptions = new ArgonautOptions(AudienceList);
```

___

### Consuming the API
Open Postman and create a new query with the following parameters (assuming that your web api is on http://localhost:5000):


**New Access Token (1 of 3)**

`POST: http://localhost:5000/token`

**Headers**
```
Accept:application/json
Origin:http://localhost:4200
```
*\*Note: Origin is optional in Postman and determines if an Access-Control-Allow-Origin is sent with the response. In a browser this header is automatic. It is designed to prevent unauthorised cross origin requests from malicious websites. In the demo code an asterisk (\*) is used to allow requests from any origin.*

**Body** *x-www-form-urlencoded*
```
grant_type:password
username:doej
password:test123
audience:ExampleAudience1
```

<img src="http://i58.photobucket.com/albums/g257/Argonaught/AccessToken_zpso1yxam6b.jpg" alt="Access Token Image" width=400px/>

___

 
**New RefreshToken (2 of 3)**

`POST: http://localhost:5000/token`

**Headers**
```
Accept:application/json
Origin:http://localhost:4200
```

**Body** *x-www-form-urlencoded*

```
grant_type:refresh_token
refresh_token:61422b7c3843478184772fb75313123b
```
*\*Note: If using the testing code above it is important that this refresh token is used (61422b7c3843478184772fb75313123b).*

<img src="http://i58.photobucket.com/albums/g257/Argonaught/RefreshToken_zps2o1wb8jt.jpg" alt="Refresh Token Image" width=400px/>


___

**Data (3 of 3)**

`GET: http://localhost:5000/api/values`

**Headers**
```
Accept:application/json
Origin:http://localhost:4200
Authorization:Bearer generated_access_token
```
<img src="http://i58.photobucket.com/albums/g257/Argonaught/Data_zpsrnwct6ek.jpg" alt="Data Image" width=400px/>

___


### Argonaught Options Object

This is needed to add Argonaught to the pipeline.

Required parameters must be passed in at the point of initialisation.

`IEnumerable<Audience> Audiences`
**Required** A collection of one or more audiences (audience object outlined below) that the application will use to validate against. An access token will contain the name and terms of an audience. When validating the application will check that the bearer access token passed in by the request matches one of the audience objects in this collection.

`Func<string, string, string, ArgonaughtUser> ValidateUser`
**Required** Called by Argonaught when a user attempts to login by passing a username, password and audience Id. This function should check with the database that the user is allowed access to the requested audience and pass back the user's claims, along with the full audience object.

`Func<string, RefreshResponse> RefreshAccessToken`
**Required** This function is called when the client requests a new access token using a refresh token. Argonaught will hash the refresh token passed in by the client and pass it to this function. Use it to find the full refresh token object (that will have been saved using the `RefreshTokenGenerated` method noted below) and the associated audience object in the db and pass these back in a RefreshReponse object. Argonaught will then take care to validate that it hasn't expired and generate a new access token for the user. If this function returns null it is assumed a refresh token couldn't be found and the user will receive an Unauthorized 401 response.

`Action<RefreshToken> RefreshTokenGenerated`
**Required** Called whenever a refresh token has been generated. Use this function to store the RefreshToken object in the database for later validation when a new access token is requested by a client using their refresh token. It is recommended to delete any existing relevant tokens for the user before saving the new one. The RefreshToken object contains the encrypted ticket (listing authenticaion claims) and the hashed id of the refresh token itself.

`int AccessTokenLifetimeMinutes`
**Optional** *Default: 5*. The time in minutes that the access token will be valid, starting from the moment it is created.

`string TokenPath`
**Optional** *Default: "/token"*. The path used to access the api token end point, starting from the root url. Demo is normally http://localhost:5000/token.

`string APIPath`
**Optional** *Default: "api/"*. This value needs to be found in the url in order for preflight GET requests to succeed. Only change if your api will not include 'api/' in the url.

`bool VisualiseClaims`
**Optional** *Default: false*. If true the claims will be printed under the access token in English. Otherwise, they are only encoded in the access token.

___

### Audience

To be persisted by the application, presumably in a database.

`Id` *String. Primary key. The name of the audience. The access token will need to match this name to pass validation.*

`Issuer` *The name of the issuer. The access token will need to match this name to pass validation.*

`Secret` *Used to authenticate the access token to prevent tampering. Keep this as hidden as possible.*

`Active` *Not used by Argonaught. To be used at the user's discretion.*

`RefreshTokenLifetimeMinutes` *The refresh token can be used to generate another access token without the user needing to login again. The duration of this feature can be set here.*

`AllowedOrigins` *Comma separated. Use an asterisk (\*) to allow any origin (which may be required for an API where the end consumer cannot be predicted). Otherwise, only the specified origins will receive the Access-Control-Allow-Origin header.*


### RefreshToken

To be persisted by the application, presumably in a database.

`Id` *String. Primary key. Created by Argonaught. This is the hashed refresh token.*

`Subject` *The sub of the access token. This will be the username passed in by the user when logging in and can be used to revoke refresh tokens for users by deleting them from the database.*

`AudienceId` *Foreign key to the associated audience object.*

`IssuedUtc` *Universal time of issue.*

`ExpiresUtc` *Universal time of expiry.*

`ProtectedTicket` *The encrypted access token containing the claims for the user. Argonaught will decrypt and use this to regenerate the claims for a new access token. This means that if a user's claims are changed they either need to log out and log in again or you can force them to do so by deleting their refresh token in the databse.*


### AuthorizeClaimAttribute

```csharp
using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Argonaught.Authentication.Authorization {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeClaimAttribute : TypeFilterAttribute {
        public AuthorizeClaimAttribute(string claimType, string claimValue) : base(typeof(AuthorizeClaimFilter)) {
            Arguments = new object[] { new Claim(claimType, claimValue) };
        }
    }

    internal class AuthorizeClaimFilter : IAuthorizationFilter {
        readonly Claim _claim;

        public AuthorizeClaimFilter(Claim claim) {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context) {
            var authenticated = context.HttpContext.User.Identities.FirstOrDefault(x => x.IsAuthenticated == true) != null ? true : false;
            if (!authenticated) {
                context.Result = new UnauthorizedResult();
                return;
            }

            var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value);
            if (!hasClaim) {
                context.Result = new ForbidResult();
                return;
            }
        }
    }

}
```