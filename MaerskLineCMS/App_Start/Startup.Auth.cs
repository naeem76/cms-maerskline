using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Owin;
using MaerskLineCMS.Models;
using System.Collections;
using System.Security.Principal;
using System.Security.Claims;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Azure.ActiveDirectory.GraphClient.Extensions;

namespace MaerskLineCMS
{
    public partial class Startup
    {
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string appKey = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];

        public static readonly string Authority = aadInstance + tenantId;

        // This is the resource ID of the AAD Graph API.  We'll need this to request a token to call the Graph API.
        string graphResourceId = "https://graph.windows.net";

        public void ConfigureAuth(IAppBuilder app)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = clientId,
                    Authority = Authority,
                    PostLogoutRedirectUri = postLogoutRedirectUri,

                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        // If there is a code in the OpenID Connect response, redeem it for an access token and refresh token, and store those away.
                        AuthorizationCodeReceived = (context) =>
                        {
                            var code = context.Code;
                            ClientCredential credential = new ClientCredential(clientId, appKey);
                            string signedInUserID = context.AuthenticationTicket.Identity.FindFirst(System.IdentityModel.Claims.ClaimTypes.NameIdentifier).Value;
                            AuthenticationContext authContext = new AuthenticationContext(Authority, new ADALTokenCache(signedInUserID));
                            AuthenticationResult result = authContext.AcquireTokenByAuthorizationCode(
                            code, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), credential, graphResourceId);

                            return Task.FromResult(0);
                        },

                        RedirectToIdentityProvider = (context) =>
                        {
                            // Ensure the URI is picked up dynamically from the request;
                            string appBaseUrl = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase + context.Request.Uri.PathAndQuery;
                            context.ProtocolMessage.RedirectUri = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase + context.Request.Uri.PathAndQuery;
                            context.ProtocolMessage.PostLogoutRedirectUri = appBaseUrl;
                            return Task.FromResult(0);
                        },

                        AuthenticationFailed = (context) =>
                        {
                            if (context.Exception.Message.StartsWith("OICE_20004") || context.Exception.Message.Contains("IDX10311"))
                            {
                                context.SkipToNextMiddleware();
                                return Task.FromResult(0);
                            }
                            return Task.FromResult(0);
                        },
                    }

                });
        }

        /*public ArrayList Groups()
        {
            ArrayList groups = new ArrayList();
            foreach (System.Security.Principal.IdentityReference group in
                System.Web.HttpContext.Current.Request.LogonUserIdentity.Groups)
            {
                groups.Add(group.Translate(typeof
                    (System.Security.Principal.NTAccount)).ToString());
            }
            return groups;
        }*/

        /*public static IEnumerable<string> GetGroupMembershipsByObjectId(string id = null)
        {
            if (string.IsNullOrEmpty(id))
                id = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

            IList<string> groupMembership = new List<string>();
            try
            {
                Uri servicePointUri = new Uri("https://graph.windows.net");
                Uri serviceRoot = new Uri(servicePointUri, tenantId);
                ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(serviceRoot, async () => await GetAppTokenAsync());
                IUser user = activeDirectoryClient.Users.Where(u => u.ObjectId == id).ExecuteSingleAsync().Result;
                var userFetcher = (IUserFetcher)user;

                IPagedCollection<IDirectoryObject> pagedCollection = userFetcher.MemberOf.ExecuteAsync().Result;
                do
                {
                    List<IDirectoryObject> directoryObjects = pagedCollection.CurrentPage.ToList();
                    foreach (IDirectoryObject directoryObject in directoryObjects)
                    {
                        if (directoryObject is Group)
                        {
                            var group = directoryObject as Group;
                            groupMembership.Add(group.DisplayName);
                        }
                    }
                    pagedCollection = pagedCollection.GetNextPageAsync().Result;
                } while (pagedCollection != null);

            }
            catch (Exception e)
            {
                
            }

            int i = 0;

            return groupMembership;
        }*/

        /*private static async Task<string> GetAppTokenAsync()
        {
            string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
            string appKey = ConfigurationManager.AppSettings["ida:AppKey"];
            string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
            string tenant = ConfigurationManager.AppSettings["ida:Tenant"];
            string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
            string azureAdGraphApiEndPoint = ConfigurationManager.AppSettings["ida:AzureAdGraphApiEndPoint"];
            // This is the resource ID of the AAD Graph API.  We'll need this to request a token to call the Graph API.
            string graphResourceId = ConfigurationManager.AppSettings["ida:GraphResourceId"];

            string Authority = String.Format(CultureInfo.InvariantCulture, aadInstance, tenant);

            // Instantiate an AuthenticationContext for my directory (see authString above).
            AuthenticationContext authenticationContext = new AuthenticationContext(Authority, false);

            // Create a ClientCredential that will be used for authentication.
            // This is where the Client ID and Key/Secret from the Azure Management Portal is used.
            ClientCredential clientCred = new ClientCredential(clientId, appKey);

            // Acquire an access token from Azure AD to access the Azure AD Graph (the resource)
            // using the Client ID and Key/Secret as credentials.
            AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenAsync(graphResourceId, clientCred);

            // Return the access token.
            return authenticationResult.AccessToken;
        }*/
    }
}
