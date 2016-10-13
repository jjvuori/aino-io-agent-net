# .NET Agent for Aino.io

.NET implementation of Aino.io logging agent

## What is [Aino.io](http://aino.io) and what does this Agent have to do with it?

[Aino.io](http://aino.io) is an analytics and monitoring tool for integrated enterprise applications and digital business processes. Aino.io can help organizations manage, develop, and run the digital parts of their day-to-day business. Read more from our [web pages](http://aino.io).

Aino.io works by analyzing transactions between enterprise applications and other pieces of software. This Agent helps to store data about the transactions to Aino.io platform using Aino.io Data API (version 2.0). See [API documentation](http://www.aino.io/api) for detailed information about the API.

## Installation

This Aino Agent can be installed 

### Via nuget
Are we gonna use nuget.org or bintray for this? If using bintray, user must add new source for nuget. If using nuget.org, then it is just installing through it.

### From sources
Download this repository and build. Then add the created library to your project.



## Technical requirements
* .NET Framework 4

## Example usage

#### Minimal example (only required fields)

```c#
using Aino;

var conf = new Configuration {
			ApiKey = "Your API key here!",
			SendInterval = 5000,				// Interval at which to send the data
			SizeThreshold = 3,					// If send buffer message size exceeds this threshold, sends immediately
			Gzip = true							// Should the content be gzipped to reduce bandwidth
		};


Agent agent = new Agent()
agent.Configuration = conf;
agent.Initialize(); 

var msg = new AinoMessage();
msg.From = "System 0";
msg.To = "System 1";
msg.Status = AinoMessage.MessageStatus.Success;

agent.AddMessage(msg);

// ... 
// Dispose the agent object after you don't need logging anymore (Or use `using`).
agent.Dispose();

```

#### Full example

```c#
var conf = new Configuration {
			ApiKey = "Your API key here!",
			SendInterval = 5000,				// Interval at which to send the data
			SizeThreshold = 3,					// If send buffer message size exceeds this threshold, sends immediately
			Gzip = true							// Should the content be gzipped to reduce bandwidth
		};


Agent agent = new Agent()
agent.Configuration = conf;
agent.Initialize(); 

var msg = new AinoMessage();
msg.From = "System 0";
msg.To = "System 1";
msg.Status = AinoMessage.MessageStatus.Failure;

msg.AddMetadata("MetadataKey1", "MetadataValue1");
msg.AddMetadata("meta 2", "meta 2 value");

msg.AddId("single value", "value1");
msg.AddId("single value", "value 2");

msg.AddId("Multiple values 1", new List<string> { "val1", "val2", "val3" });
msg.AddId("Multiple values 2", new List<string> { "val11", "val22", "val33" });
msg.AddId("Multiple values 2", new List<string> { "val44", "val55" });

msg.Message = "Failed to parse the invoice message from System 0.";

agent.AddMessage(msg);

// ... 
// Dispose the agent object after you don't need logging anymore (Or use `using`).
agent.Dispose();

```


<!--
# What to use as flow ID
The flow ID, also known as correlation ID or correlation key is an identifier that allows aino
to group several aino transaction invocations into a single transaction.

If user does not set flow Id, it will be automatically generated.
By calling `Aino::Lib->init_flow_id`, all subsequent messages will have the same flow id.
This can be overridden by setting the messages flow id manually:

Calling `Aino::Lib->clear_flow_id` will clear the flow id set in `Aino::Lib->init_flow_id`.
-->

## Contributing

### Technical Requirements

### Contributors

* [Jussi Mikkonen](https://github.com/jussi-mikkonen)
* [Ville Harvala](https://github.com/vharvala)

## [License](LICENSE)

Copyright &copy; 2016 [Aino.io](http://aino.io). Licensed under the [Apache 2.0 License](LICENSE).