---
title: Redis
layout: "default"
nav_order: 6
---
## Redis
Gaucho uses the default inmemory storage to save metrics and logs.  
The library Gaucho.Redis is used to persist the data in a Redis server.  
A Gaucho swarm can be setup by configuring all server instances with the same Redis Database that makeup the cluster.

Installation
---
Gaucho.Redis is installed by NuGet  
```
https://www.nuget.org/packages/Gaucho.Redis/
```
  
Setup
---
Gaucho.Redis is added in the setup through the GlobalConfiguration Setup
```
GlobalConfiguration.Setup(c => c.UseRedisStorage("localhost:6379", new Redis.RedisStorageOptions {Db = 15, Prefix = "{Gaucho}"}));
```