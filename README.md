# Dotnet Lambda Chaos

A proof of concept to enable easy configuration of chaos engineering experiments on AWS Lambda functions using .NET.

Inspired by the [aws-lambda-layer-chaos-injection repo](https://github.com/adhorn/aws-lambda-layer-chaos-injection) by [Adrian Hornsby](https://github.com/adhorn).

## Usage

Currently chaos is configured using an environment variable against the Lambda function itself. The configuration uses the JSON format specific below and should be added to a variables named CHAOS_PARAM

``` json
{	
	"FaultType": "exception", // Either 'latency' or 'exception'
	"Delay": 400, // Used if the fault type is set to latency. The ms delay to add
	"IsEnabled": true, // Enable or disable the chaos
	"ExceptionMsg": "chaos", // Used if the fault type is set to exception. The error message to return.
	"Rate": 1 // A value between 0 and 1, determines how often chaos will be induced. 0.2 = 20% of invokes
}
```

To trigger chaos in your function, add the below to your function handler. The LambdaChaos class could also be initialized from your function constructor.

``` csharp
var chaos = new LambdaChaos();
await this._chaos.ExecuteChaos();
```

An example Lambda function and SAM template can be found under the [example](./example) directory. To deploy into your own AWS account:

``` bash
cd example
sam build
sam deploy --guided
```

## Roadmap

- Update configuration to be pulled from SSM Parameter Store
- Add C# source generator support for simplified configuration
- Force Lambda to return a specific status code

