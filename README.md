# ODataClientManager

[![NuGet](https://img.shields.io/nuget/v/OData.Client.Manager.svg)](https://www.nuget.org/packages/OData.Client.Manager)
[![Azure DevOps Build Status](https://dev.azure.com/SiberaIndustries/OData.Client.Manager/_apis/build/status/SiberaIndustries.OData.Client.Manager?branchName=master)](https://dev.azure.com/SiberaIndustries/OData.Client.Manager/_build/latest?definitionId=1&branchName=master)
[![AppVeyor Build status](https://ci.appveyor.com/api/projects/status/6bx528e35dt43783/branch/master?svg=true)](https://ci.appveyor.com/project/SiberaIndustries/odata-client-manager/branch/master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SiberaIndustries_OData.Client.Manager&metric=alert_status)](https://sonarcloud.io/dashboard?id=SiberaIndustries_OData.Client.Manager)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=SiberaIndustries_OData.Client.Manager&metric=coverage)](https://sonarcloud.io/dashboard?id=SiberaIndustries_OData.Client.Manager)

## Purpose

This repository provides a C# based OData client manager library.
The Manager uses the `IODataClient` implementation of `Simple.OData.Client` to communicate with OData APIs and is able to handle authorization and versioning requirements.

## Getting started

The easiest way to start using the `ODataManager` is to install the Nuget package:

```sh
Install-Package OData.Client.Manager
```

In the source file where you will be using `ODataManager` import the namespace:

```cs
using OData.Client.Manager;
```

### Quickstart

The following code snipped shows an example of how to use the `IODataManger` implementation.

```cs
// Create the manager
var odataEndpoint = new Uri("http://localhost:12345/api");
var manager = new ODataManager(odataEndpoint);

// Use the client of the manager (example of the typed fluent API syntax)
IEnumerable<Product> entities = await manager.Client
    .For<Product>()
    .FindEntriesAsync();

// Use the client of the manager (example of the dynamic fluent API syntax)
var dx = ODataDynamic.Expression;
IEnumerable<dynamic> entities = await manager.Client
    .For(dx.Products)
    .FindEntriesAsync();
```

For more information about how to use the Odata client, please read the [Simple.OData.Client documentation](https://github.com/simple-odata-client/Simple.OData.Client/wiki).

### Make use of autenticated and versioned requests

* To make use of authentication use one of the existing authenticators in the `OData.Client.Manager.Authenticators` namespace, or create your own by implementing the `IAuthenticator` interface.
* To make use of authentication, just use one of the existing managers in the `OData.Client.Manager.Versioning` namespace or create your own by implementing the `IVersioningManager` interface.

```cs
// Setup the configuration
var config = new ODataManagerConfiguration(new Uri("http://localhost:12345/api"))
{
    // Authenticated requests
    Authenticator = new OidcAuthenticator(new OidcSettings
    {
        AuthUri = new Uri("http://localhost:5000"),
        ClientId = "ClientAppX",
        ClientSecret = "Secret",
        Username = "User",
        Password = "Password",
        Scope = "api1",

        GrantType = "Password", // Default
        DiscoveryPolicy = new DiscoveryPolicy { RequireHttps = false },
    }),

    // Versioned requests
    VersioningManager = new QueryParamVersioningManager("1.2", "api-version")
};

// Use the configuration in the ctor of the manager
var manager = new ODataManager(config);
```

## FAQ

1. Why do I get the error `Https required`?
    * OIDC endpoints must provide an encrypted connection (https) by default (except URIs of localhost). To disable this requirement, make use of the `OidcSettings` and set `RequireHttps` of the `DiscoveryPolicy` property to `false`: `settings.DiscoveryPolicy = new DiscoveryPolicy { RequireHttps = requireHttps };`.

## Links

* OData: <http://www.odata.org/getting-started> or <https://docs.microsoft.com/odata>
* Simple.OData.Client: <https://github.com/simple-odata-client/Simple.OData.Client/wiki>
* IdentityModel: <https://identitymodel.readthedocs.io>
* OIDC: <https://openid.net/connect>

## Open Source License Acknowledgements and Third-Party Copyrights

* Icon made by [Freepik](https://www.flaticon.com/authors/freepik)
