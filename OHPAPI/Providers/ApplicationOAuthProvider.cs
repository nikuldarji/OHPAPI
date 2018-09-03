﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using OHPAPI.DBClass;
using OHPAPI.Models;

namespace OHPAPI.Providers
{
    public class ApplicationOAuthProvider: OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            //ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);
            
            //context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            // Below logic set for own database to check login user available in the database or not

            UserDAL ulogic = new UserDAL();
            var username = context.UserName;
            var password = context.Password;
            User userInfo = ulogic.GetAdminUser(username, password);

            if (userInfo == null)
            {
                var data = context.UserName + " " + context.Password;
                context.SetError("invalid_grant_type", data);
                return;
            }
            else
            {
                // If user available then go for create new token.
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.Name, userInfo.Email));
                var props = CreateProperties(username);
                var ticket = new AuthenticationTicket(identity, props);
                context.Validated(ticket);
            }

            //ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
            //   OAuthDefaults.AuthenticationType);
            //ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
            //    CookieAuthenticationDefaults.AuthenticationType);

            //AuthenticationProperties properties = CreateProperties(user.UserName);
            //AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            //context.Validated(ticket);
            //context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {

                context.AdditionalResponseParameters.Add(property.Key, property.Value);

            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }


        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }

    }
}