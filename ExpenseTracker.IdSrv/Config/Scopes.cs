using System.Collections.Generic;
using Thinktecture.IdentityServer.Core.Models;

namespace ExpenseTracker.IdSrv.Config
{
    public static class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            var scopes = new List<Scope>
                {
                    // identity scopes

                    StandardScopes.OpenId,
                    StandardScopes.Profile
                 };

            return scopes;
        }
    }
}