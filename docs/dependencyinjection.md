---
title: DependencyInjection
layout: "default"
nav_order: 6
---
## DependencyInjection
There is a very basic builtin implementation of DependencyInjection. 
Services can be added to the ActivationContext when configuring the Server.
```
GlobalConfiguration.Configuration
    .UseProcessingServer(p =>
    {
        var config = new PipelineConfiguration
        {
            Id = "dependency_handler",
            OutputHandlers = new List<HandlerNode>
            {
                new HandlerNode(typeof(DependencyHandler))
            },
            InputHandler = new HandlerNode("CustomInput")
        };
        p.BuildPipeline(config);
    })
    .AddService<IDependency>(() => new Dependency());
```

### ActivationContext
The ActivationContext can be overriden with a own implementation that uses a real DI Container.
```
public class CustomActivationContext : IActivationContext
{
    private readonly TinyIoCContainer _container;

    publi CustomActivationContext(TinyIoCContainer container)
    {
        _container = container;
    }

    public void Register<TService, TImpl>() where TImpl : TService
    {
        // not needed
    }

    public void Register<TService>(Func<TService> instanceCreator)
    {
        // not needed
    }

    public T Resolve<T>()
    {
        return _container.Resolve<TResolveType>();
    }

    public T Resolve<T>(Type serviceType)
    {
        return _container.Resolve<TResolveType>();
    }

    public object Resolve(Type serviceType)
    {
       return _container.Resolve(serviceType);
    }

    public IActivationContext ChildContext()
    {
        return new CustomActivationContext(_container.GetChildContainer());
    }
}
```

```
var container = new TinyIoCCContainer();
GlobalConfiguration.Configuration
    .Register<IActivationContext>(new CustomActivationContext(container));
```