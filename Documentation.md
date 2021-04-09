# AspNetCore.Logging.Common Documentation
(c) 2020-2021 Singtone. All rights reserved.

## Overview
This library is a common interface and modules for ASP.NET Core logging and auditing.

## Install

Install as a NuGet package: [AspNetCore.Logging.Common](https://www.nuget.org/packages/AspNetCore.Logging.Common)

## Usage

### Microsoft.AspNetCore.Builder.IApplicationBuilder Extensions

##### `public static IApplicationBuilder UseHttpLogging(this IApplicationBuilder builder);`
```
public void Configure(IApplicationBuilder app)
{
    ...
    app.UseHttpsRedirection();
    app.UseRouting();
    
    app.UseHttpLogging();

    ...
}
```
Act as a ASP.NET Core Middleware to log the http request and response contents. `ILoggerFactory` is required via dependency injection.

- Detailed contents are only logged when the log level is set to `Trace`.

##### `public static IApplicationBuilder UseHttpAuditing(this IApplicationBuilder builder);`
```
public void Configure(IApplicationBuilder app)
{
    ...
    app.UseHttpsRedirection();
    app.UseRouting();
    
    app.UseHttpAuditing();

    ...
}
```
Act as a ASP.NET Core Middleware to log the auditing logs via the auditing logger. Various implementations of auditing logger is able to log the auditing information to different channels such as file system, database or message queue etc.

* `IAuditingLogger`, `IOptions<AuditingConfig>`, `ILoggerFactory` and `IHttpAuditingLogTypeLocator` are required via dependency injection.
* The auditing behaviour is driven by the config from `AuditingConfig` and `AuditingLogType` configuration.
* Implementation of `IAuditingLogger` and `IHttpAuditingLogTypeLocator` may be required by your own or via third party packages.

