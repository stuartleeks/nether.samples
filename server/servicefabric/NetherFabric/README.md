# Nether.Fabric

Nether Fabric is a sample implementation of Nether in Service Fabric. In this case, the Nether Analytics pipeline was implemented as a Stateless Service.

Using Nether in Service Fabric is straight forward. You just need to reference the Nether Nuget Packages in your Service that should use Nether functionality.

Because Nether compiles against .NET core 1.1 and .NET Framework 4.6 you need to make sure that the Service that references Nether is set to compile as > .NET Framework 4.6. There should be no problem to upgrade a standard Service Fabric Service to a higher version.

## Nether.Analytics

The analytics part of Nether is different from the other modules because the data ingress happens through an Event Hub not an REST API. The Service was implemented in a way that you could spawn as many processors as needed to process the incoming data in parallel.

Blob storage is used for Service synchronization, the reason for that is that this is the standard behavior of Nether.

With a tighter integration into Service Fabric you could use Reliable Dictionaries to do this in a more efficient and Service Fabric Way.

## Custom Service Instance Listener

The Nether Event Hub Listener is registered as a Service Instance Listener on the Stateless Service in Service Fabric. The reason for this is to make sure that the Service instances are listening on startup to the Service and make it possible to Name Listeners based on the Event Hubs they are listening to.

It is important to note that you should not execute `ProcessAndBlockAsync()` of the Message Processor in this method because it will block the thread which results in a Service that never get fully registered to service fabric and will result in an internal Service Fabric warning.
Because of this the Process statement was executed in the `RundAsnyc()` Method. Here it is important to use the cancelation Token to allow Service Fabric to interrupt the exception of necessary.