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

### Build and run project image in docker

(This step is only required if you want to run the project from a docker image)

1. You must have already followed the steps to spin up the database container explained above.

1. You will need to override the ClientId and ClientSecret variables in the appsettings.json in `/src/Traces.Web/appSettings.json` file with your apaleo client information.

1. In the root directory of the project run `docker build . -t traces -f Dockerfile` this will create a docker image that you can run. To check if it was created correctly you can run `docker image ls` and there should be one called `traces`

1. Run `docker run -d -p 5021:5021 traces` should run the container

1. Navigate to `http://localhost:5021`

### Build and run with Visual Studio or Rider

(This step is only required if you want to build and execute the project directly from your development IDE)

**For this step you must have .Net Core 3.0 installed [Install .Net Core 3.0](https://dotnet.microsoft.com/download/dotnet-core/3.0)**

1. You must have already followed the steps to spin up the database container explained above.

1. You will need to override the ClientId and ClientSecret variables in the appsettings.json in `/src/Traces.Web/appSettings.json` file with your apaleo client information.

1. Run the project in Visual Studio or Rider

1. Navigate to `https://localhost:5021`

## Deploying

1. Make sure to have the heroku cli installed

1. You should login with the following command `heroku login` and `heroku container:login`.

1. Run `heroku container:push web -a ReplaceThisWithYourAppName` this will run the Dockerfile and push the resulting image to heroku

1. Run `heroku container:release web -a ReplaceThisWithYourAppName` this will release the most recent pushed image to your heroku app.

1. Now you can navigate to your app's URL `ReplaceThisWithYourAppName.herokuapp.com` or your already setup URL for your app.
