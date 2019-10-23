﻿using IdentityModel;
using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OData.Client.Manager.Authenticators
{
    public class OidcAuthenticator : AuthenticatorBase
    {
        private readonly OidcSettings oidcSettings;
        private readonly HttpClient httpClient;
        private readonly DiscoveryCache discoveryCache;
        private TokenResponse? token;
        private DateTime tokenExpiry = DateTime.Now;

        public OidcAuthenticator(OidcSettings oidcSettings)
        {
            this.oidcSettings = oidcSettings ?? throw new ArgumentNullException(nameof(oidcSettings));

            httpClient = this.oidcSettings.HttpClient ?? new HttpClient();
            var discoveryPolicy = this.oidcSettings.DiscoveryPolicy ?? new DiscoveryPolicy();
            discoveryCache = new DiscoveryCache(this.oidcSettings.AuthUri?.ToString(), () => httpClient, discoveryPolicy);
        }

        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private OidcAuthenticator()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc cref="IAuthenticator" />
        public override Task<bool> AuthenticateAsync(HttpRequestMessage requestMessage, CancellationToken ct = default)
        {
            if (requestMessage == null)
            {
                throw new ArgumentNullException(nameof(requestMessage));
            }

            if (!ReplaceAuthorizationHeader && requestMessage.Headers.Authorization != null)
            {
                OnTrace?.Invoke($"Request header '{nameof(requestMessage.Headers.Authorization)}' already set");
                return Task.FromResult(false);
            }

            return GetAndSetTokenAsync(requestMessage, ct);
        }

        /// <inheritdoc cref="IAuthenticator" />
        public override Task<bool> AuthenticateAsync(HttpClient httpClient, CancellationToken ct = default)
        {
            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            if (!ReplaceAuthorizationHeader && httpClient.DefaultRequestHeaders.Authorization != null)
            {
                OnTrace?.Invoke($"Request header '{nameof(httpClient.DefaultRequestHeaders.Authorization)}' already set");
                return Task.FromResult(false);
            }

            return GetAndSetTokenAsync(httpClient, ct);
        }

        private async Task<bool> GetAndSetTokenAsync(HttpRequestMessage requestMessage, CancellationToken ct = default)
        {
            var localToken = await GetTokenAsync(ct).ConfigureAwait(false);
            if (localToken != null)
            {
                requestMessage.SetBearerToken(localToken.AccessToken);
                Header = requestMessage.Headers.Authorization;
                return true;
            }

            OnTrace?.Invoke($"{nameof(localToken)} response could not be set.");
            return false;
        }

        private async Task<bool> GetAndSetTokenAsync(HttpClient httpClient, CancellationToken ct = default)
        {
            var localToken = await GetTokenAsync(ct).ConfigureAwait(false);
            if (localToken != null)
            {
                httpClient.SetBearerToken(localToken.AccessToken);
                Header = httpClient.DefaultRequestHeaders.Authorization;
                return true;
            }

            OnTrace?.Invoke($"{nameof(localToken)} response could not be set.");
            return false;
        }

        public async Task<TokenResponse?> GetTokenAsync(CancellationToken ct = default)
        {
            if (token == null || DateTime.Now >= tokenExpiry)
            {
                var discovery = await discoveryCache.GetAsync().ConfigureAwait(false);
                if (discovery.IsError)
                {
                    OnTrace?.Invoke($"{nameof(discovery)} response has errors: {discovery.Error}");
                    return token;
                }

                if (token == null)
                {
                    token = oidcSettings.GrantType switch
                    {
                        OidcConstants.GrantTypes.Password => await httpClient.RequestPasswordTokenAsync(
                            new PasswordTokenRequest
                            {
                                Address = discovery.TokenEndpoint,
                                ClientId = oidcSettings.ClientId,
                                ClientSecret = oidcSettings.ClientSecret,
                                Scope = oidcSettings.Scope,
                                UserName = oidcSettings.Username,
                                Password = oidcSettings.Password
                            }, ct).ConfigureAwait(false),
                        OidcConstants.GrantTypes.ClientCredentials => await httpClient.RequestClientCredentialsTokenAsync(
                            new ClientCredentialsTokenRequest
                            {
                                Address = discovery.TokenEndpoint,
                                ClientId = oidcSettings.ClientId,
                                ClientSecret = oidcSettings.ClientSecret,
                                Scope = oidcSettings.Scope
                            }, ct).ConfigureAwait(false),
                        OidcConstants.GrantTypes.AuthorizationCode => await httpClient.RequestAuthorizationCodeTokenAsync(
                            new AuthorizationCodeTokenRequest
                            {
                                Address = discovery.TokenEndpoint,
                                ClientId = oidcSettings.ClientId,
                                ClientSecret = oidcSettings.ClientSecret,
                                Code = oidcSettings.Code,
                                RedirectUri = oidcSettings.RedirectUri?.ToString(),
                                CodeVerifier = oidcSettings.CodeVerifier
                            }, ct).ConfigureAwait(false),
                        _ => throw new NotSupportedException($"Grant type '{oidcSettings.GrantType}' is not supported")
                    };
                }
                else
                {
                    using var request = new RefreshTokenRequest
                    {
                        Address = discovery.TokenEndpoint,
                        ClientId = oidcSettings.ClientId,
                        ClientSecret = oidcSettings.ClientSecret,
                        Scope = oidcSettings.Scope,
                        RefreshToken = token.RefreshToken
                    };
                    token = await httpClient.RequestRefreshTokenAsync(request, ct).ConfigureAwait(false);
                }

                if (token.IsError)
                {
                    OnTrace?.Invoke($"{nameof(token)} response has errors: {token.Error}");
                    return token;
                }

                tokenExpiry = DateTime.Now.AddSeconds(token.ExpiresIn - 10);
            }

            return token;
        }
    }
}