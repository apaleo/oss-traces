# How-to: Client generation

> see https://aka.ms/autorest

* load swagger json definition
e.g.  `curl https://identity.apaleo.com/swagger/identity-v1/swagger.json | jq . -rSM > identity.json`
* call `autorest`

From yaml file
``` yaml
input-file: ./identity.json    # or download another
csharp:
  namespace: Traces.ApaleoClients.Identity
  output-folder: ./
  override-client-name: IdentityApi
```

From the terminal
``` terminal
autorest --input-file=./identity.json --output-folder=Traces.ApaleoClients/Identity --namespace=Traces.ApaleoClients.Identity --override-client-name=IdentityApi --csharp
```
