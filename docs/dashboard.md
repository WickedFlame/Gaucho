---
title: Dashboard
layout: "default"
nav_order: 5
---
## Dashboard
The dashboard show metrics and logs of all pipelines  
The Dashboard is displaed in a Webapplication when navigating to the url \gaucho

Installation
---
Gaucho.Dashboard is installed with NuGet  
```
https://www.nuget.org/packages/Gaucho.Dashboard/  
```
  
Setup
---
Gaucho.Dashboard is added in the setup through a ExtensionMethod of the IApplicationBuilder
```
public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
{
    app.UseGauchoDashboard(pathMatch:"/gaucho", options: new DashboardOptions
    {
        Title = "Gaucho Dashboard"
    });
}
```