# How-to: Client generation

> see https://aka.ms/autorest

* load swagger json definition
e.g.  `curl https://api.apaleo.com/swagger/inventory-v1/swagger.json | jq . -rSM > inventory.json`
* call `autorest`

From yaml file
``` yaml
input-file: ./inventory.json    # or download another
csharp:
  namespace: Traces.ApaleoClients.Inventory
  output-folder: ./
  override-client-name: InventoryApi
```

From the terminal
``` terminal
autorest --input-file=./inventory.json --output-folder=Traces.ApaleoClients/Inventory --namespace=Traces.ApaleoClients.Inventory --override-client-name=InventoryApi --csharp
```
