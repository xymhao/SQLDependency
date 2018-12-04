using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security;
using Utils;

namespace Filter
{
    public class TokenFilterAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var request = actionContext.Request;

            if (!TryRetrieveToken(request, out string jwtToken, out string random))
            {
                HttpResponseMessage response = BuildResponseErrorMessage(HttpStatusCode.Unauthorized);
                actionContext.Response = response;
                return;
            }
            try
            {
                if (!TokenUtility.ValidateToken(jwtToken, random))
                {
                    HttpResponseMessage response = BuildResponseErrorMessage(HttpStatusCode.Unauthorized);
                    actionContext.Response = response;
                }
            }
            catch 
            {
                HttpResponseMessage response = BuildResponseErrorMessage(HttpStatusCode.Unauthorized);
                actionContext.Response = response;
            }
        }

        private HttpResponseMessage BuildResponseErrorMessage(HttpStatusCode statusCode)
        {
            HttpResponseMessage response = new HttpResponseMessage(statusCode);

            // the Scheme should be "Bearer"
            // authorization_uri should point to the tenant url and resource_id should point to the audience
            AuthenticationHeaderValue authenticateHeader = new AuthenticationHeaderValue("Bearer", "authorization error");
            response.Headers.WwwAuthenticate.Add(authenticateHeader);
            return response;
        }

        // Reads the token from the authorization header on the incoming request
        private static bool TryRetrieveToken(HttpRequestMessage request, out string token, out string random)
        {
            token = null;
            random = null;
            if (!request.Headers.Contains("Authorization"))
            {
                return false;
            }

            string authzHeader = request.Headers.GetValues("Authorization").First<string>();

            // Verify Authorization header contains 'Bearer' scheme
            token = authzHeader.StartsWith("Bearer ") ? authzHeader.Split(' ')[1] : null;
            random = authzHeader.StartsWith("Bearer ") ? authzHeader.Split(' ')[2] : null;
            if (null == token)
            {
                return false;
            }
            return true;
        }
    }
}
