# oss-traces
This is an open source blueprint for a Traces/To-Do application to be coupled with apaleo UI via apaleo One integrations.

## Prerequisites

- You need to have Docker installed
- For deployment you need to have heroku cli
    - [how to install heroku cli](https://devcenter.heroku.com/articles/heroku-cli)
- For amazon s3 you need to have aws cli
    - [how to install amazon cli](https://docs.aws.amazon.com/cli/latest/userguide/install-cliv2.html)

## Testing locally

### Run postgres and localstack container

**This step should not be skipped as it is required for the project database**

While in the root directory of the project

- `docker-compose up -d` will spin up the postgres container.
- `docker-compose down` will remove all containers and all the data.
- `docker-compose stop` will only stop the container without deleting data.
- `docker-compose start` will spin up the containers in case they have been stopped.

To create a bucket in localstack please execute this command, which will create a bucket called `oss-traces.local` in the region `eu-west-1`:

`aws --endpoint-url=http://localhost:4572 s3 mb s3://oss-traces.local --region eu-west-1`

Afterward allow public read to the bucket for **development** purposes:

`aws --endpoint-url=http://localhost:4572 s3api put-bucket-acl --bucket oss-traces.local --acl public-read`

### Build and run with your IDE of preference or command line

**For this step you must have .Net Core 3.0 installed ([Install .Net Core 3.0](https://dotnet.microsoft.com/download/dotnet-core/3.0))**

1. You must have already followed the steps to spin up the database container explained above.

1. You will need to override the ClientId and ClientSecret variables in the appsettings.json in `/src/Traces.Web/appsettings.json` file with your apaleo client information **You should NEVER post your credentials publicly on Github or any public repository**.
    - To achieve this you must define two environment variables `APALEO__CLIENTID` and `APALEO__CLIENTSECRET`.
    - The value of such variables should be your respective `apaleo client id` and your `apaleo client secret`
    - Again remember that these values should stay always private.

1. You will also need to override the BucketName and Region variables in the appsettings.json 
    - To achieve this you must define two environment variables `STORAGE__S3__BUCKETNAME` and `STORAGE__S3__REGION`.
    - The value of such variables should be your respective `bucket name` (e.g. oss-traces.local) and your `region` (e.g. eu-west-1)
    - If you don't want to use amazon s3 (or localstack for development), you can also set in appsettings.json the property in `Storage.Local.IsEnabled` to true.
        - The files will be then saved under `<project-dir>/src/Traces.Web/files` 

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

1. Add a drain to your app, to also receive logs from heroku. This is done via `heroku drain:add --app [YOUR_APP_NAME] [URL_WHERE_HEROKU_SENDS_LOGS]`

1. Bonus points if you add a dedicated service for error handling for instance one of these: https://elements.heroku.com/addons#errors-exceptions (again, you don't need to get it through heroku, it's just a nice list to start with)
