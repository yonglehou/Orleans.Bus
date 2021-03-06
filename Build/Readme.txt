﻿How to use
-----------

1. Reference this package (Orleans.Bus.dll and Orleans.Bus.Observables.dll) 
   from both grain interface and grain implementation projects (and any client projects)

2. For assembly, which contains grain interfaces and code generated by Orleans, set description to:

   		[assembly: AssemblyDescription("Contains.Orleans.Generated.Code")]

   WARN: Otherwise Orleans.Bus won't be able to find assemblies wich contain static factory classes.

3. Make sure that you grain interface is marked with [ExtendedPrimaryKey] attribute. Then:
   	- For every command you want to handle - specify [Handles(TCommand)] attribute.
   	- For every query you want to answer   - specify [Answers(TQuery)] attribute.
   	- For every event you want to notify   - specify [Notifies(TEvent)] attribute.
   
4. Mark methods to be used for handling commands/queires with [Handler] attribute.
   - One to dispatch commands (single parameter of 'object' type, returns Task)
   - And another one to dispatch queries (single parameter of 'object' type, returns Task<object>)
   
   * Check sample interfaces in a bus test project 
     https://github.com/yevhen/Orleans.Bus/tree/master/Source/Bus.Tests.Grains.Interfaces

5. It is recommended to create your own custom application-specific base classes, by iheriting from
   MessageBasedGrain and MessageBasedGrain<TState> classes provided by Orleans.Bus.
   
   You can put any common application-specific logic there, for example:
   	- You can futher constrain types of messages with you own message interfaces
   	- You can implement automatic envelope wrapping/unwrapping
   	- You can implement support for dependency injection, using you favorite DI framework
   	- You can implement support for automatic persistence (via automatic state checking)
   	- Etc

   For simple applications, you can just use PocoGrain base class.

6. For every actor, you'll need to create 2 classes:

	- One serving as a shell, which you will inherit from your application-specific base grain class
	- Second - will be the POCO, containing actual logic
	
	Then from handler method implementation, you will simply dispatch message 
	to a specific handler method on you POCO using 'dynamic' feature of C#

7. If you care about unit testing, make sure that from within grain/client code,
   you're only using services provided by Orleans.Bus:
   
   - IMessageBus (MessageBus.Instance)
   - IActivation
   - ITimerCollection
   - IReminderCollection
   - IObserverCollection

   You can pass those interfaces to you POCO (DI), which will allow you to use mocks
   
6. For subscribing to observable grains, you'll need to create an observable client proxy. 
   Use ObservableProxy/GenericObservableProxy classes
   RX-based version could be found here https://www.nuget.org/packages/Orleans.Bus.Reactive 

7. For further details see project documentation on GitHub (https://github.com/yevhen/Orleans.Bus)

Happy Messaging and Happy Testing!
