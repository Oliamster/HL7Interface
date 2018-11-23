# HL7Interface
A generic interface for HL7 communications

## Dependencies
* NHapi 2.5.0.6
* NHapiTools 1.10.0
* HL7api 1.0.0.0
* SuperSocket 1.6.6.1
* SuperSocket.CLientEngine 0.10.0.0
* .NET Framework >= 4.5

## Introduction 

The `HL7Interface` is a finite product that let the `NHapi`, `NHapiTools`, the `HL7api`, `SuperSocket`, `SuperSocket.Client.Engine` work togheder. 

~~~csharp
IHL7Message command = new EquipmentCommandRequest();
var ret = await SendHL7MessageAsync(command);

var ack = ret.Acknowledgment;
var commandResponse = ret.Response;
~~~