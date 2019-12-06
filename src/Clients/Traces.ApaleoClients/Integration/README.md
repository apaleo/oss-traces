# How-to: Client generation

> see https://aka.ms/autorest

* load swagger json definition
e.g.  `curl https://integration.apaleo.com/swagger/integration-v1/swagger.json | jq . -rSM > integration.json`
* call `autorest`

From yaml file
``` yaml
input-file: ./integration.json    # or download another
csharp:
  namespace: Traces.ApaleoClients.Integration
  output-folder: ./
  override-client-name: IntegrationApi
```

From the terminal
``` terminal
autorest --input-file=./integration.json --output-folder=Traces.ApaleoClients/Integration --namespace=Traces.ApaleoClients.Integration --override-client-name=IntegrationApi --csharp
```
