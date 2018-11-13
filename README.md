# Structurizr.InfrastructureAsCode

[![Build Status](https://christianederzuehlke.visualstudio.com/aprg%20Structurizr%20Infrastructure%20as%20Code/_apis/build/status/ChristianEder.Structurizr.InfrastructureAsCode)](https://christianederzuehlke.visualstudio.com/aprg%20Structurizr%20Infrastructure%20as%20Code/_build/latest?definitionId=5)

[![Structurizr.InfrastructureAsCode](https://img.shields.io/nuget/v/Structurizr.InfrastructureAsCode.png "Latest nuget package for Structurizr.InfrastructureAsCode")](https://www.nuget.org/packages/Structurizr.InfrastructureAsCode/)
[![Structurizr.InfrastructureAsCode.Azure](https://img.shields.io/nuget/v/Structurizr.InfrastructureAsCode.Azure.png "Latest nuget package for Structurizr.InfrastructureAsCode.Azure")](https://www.nuget.org/packages/Structurizr.InfrastructureAsCode.Azure/)

Structurizr.InfrastructureAsCode is a library that uses both [Structurizr for .NET](https://github.com/structurizr/dotnet) and the [Azure SDK for .NET](https://github.com/Azure/azure-sdk-for-net/tree/Fluent) to provide an easy to use architecture- and infrastructure-as-code solution. Currently, there is only an implementation for Azure resources available.

## How to desribe your architecture and infrastructure

You can find a sample implementation [here](https://github.com/ChristianEder/Structurizr.InfrastructureAsCode/tree/master/Structurizr.InfrastructureAsCode.Azure.Sample).

### Create the architecture and infrastructure model

- Create a subclass of SoftwareSystemWithInfrastructure which will describe your software system. For this, you will use Structurizrs Workspace, SoftwareSystem and Container classes. See the [sample projects Monkey factory system](https://github.com/ChristianEder/Structurizr.InfrastructureAsCode/blob/master/Structurizr.InfrastructureAsCode.Azure.Sample/Model/MonkeyFactory.cs)
-  Create subclasses of ContainerWithInfrastructure which will describe individual containers and their corresponding cloud infrastructure. See the [sample projects UI container](https://github.com/ChristianEder/Structurizr.InfrastructureAsCode/blob/master/Structurizr.InfrastructureAsCode.Azure.Sample/Model/MonkeyUI.cs)

### Use Structurizr to render the model to architecture diagrams

Since the model you created in the first step uses Structurizrs SDK to describe the architecture of your software system, you can use the Strucurizr API to render / upload that model directly to Structurizr.

### Use Structurizr.InfrastructureAsCode to render the model to Azure resources

Before creating any Azure resoures, you will need to

- Implement a console application similar to the [sample](https://github.com/ChristianEder/Structurizr.InfrastructureAsCode/tree/master/Structurizr.InfrastructureAsCode.Azure.Sample)
- Create a service principal using a self signed certificate for authentication. You can use the latest infrastructurizr [release](https://github.com/ChristianEder/Structurizr.InfrastructureAsCode/releases) to do this
- Use the InfrastructureRendererBuilder class in the console app to create an Azure InfrastructureRenderer. See [the sample Program.cs](https://github.com/ChristianEder/Structurizr.InfrastructureAsCode/blob/master/Structurizr.InfrastructureAsCode.Azure.Sample/Program.cs). Here you will need to pass in the credentials to access your Azure subscription. These should **not** be your personal credentials, but the credentials (key) you just created for the Azure AD Application.
- Use pass your model (the SoftwareSystemWithInfrastructure subclass) to that renderer. This will create the resources defined as the infrastructure elements in your model in your Azure subscription.

## Contribute: How to extend Structurizr.InfrastructureAsCode to support additional resource types

TBD

