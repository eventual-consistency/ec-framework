# EventualConsistency Framework
The EventualConsistency framework, a collection of .NET libraries for rapid creation of CQRS/Event-Sourcing systems.

Current Build Status
====================
![Build Status](https://travis-ci.org/eventual-consistency/ec-framework.svg?branch=master)

Project Structure
=================
The framework is broken up into several libraries, allowing you to leverage as much or as little as you would like.

* Core Libraries
  * EventualConsistency.Framework
  * EventualConsistency.Framework.Infrastructure
  * EventualConsistency.Framework.Testing

* Integration with 3rd Party Platforms
  * EventualConsistency.Integration.Nancy

* Providers (for Event Stores, Message Bus, Projection hosting)
  * Azure
  * Elastic (Work in progress)
  * InProcess
  * RabbitMQ
  * Redis (Work in progress)
