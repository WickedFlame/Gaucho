---
title: DependencyInjection
layout: "default"
nav_order: 6
---
## DependencyInjection
There is a very basic builtin implementation of DependencyInjection.  
Services can be added to the ActivationContext through the GlobalConfiguration when configuring the Server.  
```
GlobalConfiguration.Setup(c => c.UseProcessingServer(p =>
    {
        // create the configuration of a pipeline
        var config = new PipelineConfiguration
        {
            Id = "pipeline1",
            OutputHandlers = new List<HandlerNode>
            {
                new HandlerNode(typeof(ConsoleOutputHandler)),
                new HandlerNode(typeof(SqlOuputHandler))
            },
            InputHandler = new HandlerNode(typeof(InputHandler<LogMessage>))
        };
        p.BuildPipeline(config);
    })
    .AddService<IDependency>(() => new Dependency()));
```
  
### ActivationContext
A custom DI Container can be added by implementing the interface IActivationContext and registering this to the GlobalConfiguration.  
```
var container = new TinyIoCCContainer();
GlobalConfiguration.Setup(c => c.Register<IActivationContext>(new TinyIoCActivationContext(container)));
```
  
#### Example implementation
```
public class TinyIoCActivationContext : IActivationContext
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