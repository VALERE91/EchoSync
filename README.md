# EchoSync

A Bring your own Transport replication Library in .NET Standard 2.1 C# compatible with .NET Core servers, Unity and Godot.

> [!CAUTION]
> This library is a week-end project for a technical test. I plan on continuing it but it is absolutely not ready for prime-time. Use at your own risks. 

## Basic Architecture

### Inversion of Control

Except in some exceptional cases, every piece of the application uses interfaces pass in constructors.

### Singletons and managers

The replication system sometimes needs unique and global instances (Linking Context, Replication Engine, etc.) instead of simple singleton, those types are provided by a Service Locator responsible of their "global" behaviour. You are reponsible to create your instance and provide it to the Service Locator. Anybody knowing your interface will then be able to query the Service Locator for your implementation.

### Projects and solution

- Examples : 
  - Common Gameplay Code : code share between the Example Client and Example Server (Engine, Controller, World, etc.)
  - Client Example : A simple client to test
  - Server Example : A simple server to test
- EchoSync : the main library doing the replication
- LiteNetLib : a Git Subtree of the well-known LiteNetLib. I went with the subtree to better support Unity builds where having the code (instead of NuGet or DLLs) is mandatory for LiteNetLib.
- LiteNetLibAdapters : implement the Transport Interface of EchoSyn for LitNetLib. This means that if you want to use any other transport library you don't need LiteNetLib, you can simply bring your own transport
- Tests : simple Unit tests of mainly the serialization

## Features

- Unity packages
- Network Objects
- Automatic serialization of Net Properties
- Snapshot
- Input forwarding
- Reliable RPCs

## Remaining todo

- Tick system
- Tick synchronization
- Prediction
- Interpolation
- Tooling
- Implement code generation for RPCs and Net Properties by using Roslyn Generators
