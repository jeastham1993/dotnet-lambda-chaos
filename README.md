# Dotnet Lambda Chaos

A proof of concept to enable easy configuration of chaos engineering experiments on AWS Lambda functions using .NET.

Inspired by the [aws-lambda-layer-chaos-injection repo](https://github.com/adhorn/aws-lambda-layer-chaos-injection) by [Adrian Hornsby](https://github.com/adhorn).

## Usage

Chaos is configured using a JSON object stored as an SSM Parameter. At runtime the SSM Parameter is retrieved and cached in the Lambda execution environment. The time to live (TTL) of the cached configuration can also be configured. After TTL expiry an API call will be made to SSM to retrieve the latest parameter version.

The SSM parameter should be in the below format:

``` json
{	
	"FaultType": "exception", // Either 'latency' or 'exception'
	"Delay": 400, // Used if the fault type is set to latency. The ms delay to add
	"IsEnabled": true, // Enable or disable the chaos
	"ExceptionMsg": "chaos", // Used if the fault type is set to exception. The error message to return.
	"Rate": 1, // A value between 0 and 1, determines how often chaos will be induced. 0.2 = 20% of invokes
	"CacheTTL" : 30 // The number of seconds to cache the configuration for. Default is 60 seconds.
}
```

Your Lambda function also needs to have permission to retrieve the configuration value. If you are using AWS SAM for deployment this can be provided using the below policy template

``` yaml
ChaosFunction:
    Type: AWS::Serverless::Function
    Properties:
	...
      Policies:
        - SSMParameterReadPolicy:
            ParameterName: <Your parameter name>
```

If you are not using AWS SAM, the below IAM permissions are required:

``` json
{
    "Statement": [
        {
            "Action": [
                "ssm:DescribeParameters"
            ],
            "Resource": "*",
            "Effect": "Allow"
        },
        {
            "Action": [
                "ssm:GetParameters",
                "ssm:GetParameter",
                "ssm:GetParametersByPath"
            ],
            "Resource": <PARAMETER_ARN>,
            "Effect": "Allow"
        }
    ]
}
```

The name of the SSM parameter needs to be provided to your Lambda function as an environment variable.

``` yaml
ChaosFunction:
    Type: AWS::Serverless::Function
    Properties:
      ...
      Environment:
        Variables:
          CHAOS_PARAM: SSM_PARAMETER_NAME
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

- ~~Update configuration to be pulled from SSM Parameter Store~~
- ~~Cache configuration to reduce overhead and API calls to SSM parameter store~~
- Add C# source generator support for simplified configuration
- Force Lambda to return a specific status code

## Show your support
Give a ⭐️ if this project helped you!

## License

This library is licensed under the MIT-0 License. See the LICENSE file.
