---
title: Redis
layout: "default"
nav_order: 6
---
## Redis
Gaucho has a default inmemory storagem to save metrics and logs.  
The package Gaucho.Redis is used to persist the data in Redis.  
A Gaucho cluster can be setup by using the same Redis Database for all servers that create the cluster.

Installation
---
Gaucho.Redis is installed with NuGet  
```
https://www.nuget.org/packages/Gaucho.Redis/
```
  
Setup
---
Gaucho.Redis is added in the setup through the GlobalConfiguration Setup
```
GlobalConfiguration.Setup(c => c.UseRedisStorage("localhost:6379", new Redis.RedisStorageOptions {Db = 15}));
```