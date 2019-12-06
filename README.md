# oss-traces
This is an open source blueprint for a Traces/To-Do application to be coupled with apaleo UI via apaleo One integrations.

## Prerequisites

- You need to have Docker installed
- For deployment you need to have heroku cli
    - [how to install heroku cli](https://devcenter.heroku.com/articles/heroku-cli)

## Testing locally

### Run postgres container

**This step should not be skipped as it is required for the project database**

While in the root directory of the project

- `docker-compose up -d` will spin up the postgres container.
- `docker-compose down` will remove all containers and all the data.
- `docker-compose stop` will only stop the container without deleting data.
- `docker-compose start` will spin up the containers in case they have been stopped.

### Build and run with your IDE of preference or command line

**For this step you must have .Net Core 3.0 installed ([Install .Net Core 3.0](https://dotnet.microsoft.com/download/dotnet-core/3.0))**

1. You must have already followed the steps to spin up the database container explained above.

1. You will need to override the ClientId and ClientSecret variables in the appsettings.json in `/src/Traces.Web/appsettings.json` file with your apaleo client information **You should NEVER post your credentials publicly on Github or any public repository**.
    - To achieve this you must define two environment variables `APALEO__CLIENTID` and `APALEO__CLIENTSECRET`.
    - The value of such variables should be your respective `apaleo client id` and your `apaleo client secret`
    - Again remember that these values should stay always private.

1. Run the project in your IDE of preference or command line.

1. Navigate to `https://localhost:5021`

## Deploying

1. Make sure to have the heroku cli installed

1. You should login with the following command `heroku login` and `heroku container:login`.

1. Run `heroku container:push web -a ReplaceThisWithYourAppName` this will run the Dockerfile and push the resulting image to heroku

1. Run `heroku container:release web -a ReplaceThisWithYourAppName` this will release the most recent pushed image to your heroku app.

1. Now you can navigate to your app's URL `ReplaceThisWithYourAppName.herokuapp.com` or your already setup URL for your app.

## Going Live

1. You'll need to collect your logs somewhere. You can pick one of https://elements.heroku.com/addons#logging - but you don't necessarily need to get it through heroku. 

1. Make sure nlog (or any other logger) can log to the service. Either via HTTP or via custom libraries.

1. Add a drain to your app, to also recieve logs from heroku. This is done via `heroku drain:add --app [YOUR_APP_NAME] [URL_WHERE_HEROKU_SENDS_LOGS]`

1. Bonus points if you add a dedicated service for error handling for instance one of these: https://elements.heroku.com/addons#errors-exceptions (again, you don't need to get it through heroku, it's just a nice list to start with)
