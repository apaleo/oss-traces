# How-to: Client generation

> see https://aka.ms/autorest

* load swagger json definition
e.g.  `curl https://api.apaleo.com/swagger/booking-v1/swagger.json | jq . -rSM > booking.json`
* call `autorest`

From yaml file
``` yaml
input-file: ./booking.json    # or download another
csharp:
  namespace: Traces.ApaleoClients.Booking
  output-folder: ./
  override-client-name: BookingApi
```

From the terminal
``` terminal
autorest --input-file=./booking.json --output-folder=Traces.ApaleoClients/Booking --namespace=Traces.ApaleoClients.Booking --override-client-name=BookingApi --csharp
```
