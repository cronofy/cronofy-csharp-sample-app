# Cronofy C# Sample Application

## Prerequisites

### A Git Tool

We would recommend downloading and using the Git Bash tool, which you can find [here](https://git-scm.com/downloads).

### A cloned version of this repository

For help in cloning this repository please see [this](https://help.github.com/articles/cloning-a-repository/) article.

### A .NET IDE

We would recommend using [Xamarin Studio](https://www.xamarin.com) to build and
run this project on mac and [Visual Studio](https://www.visualstudio.com/) for Windows.

## Set-up

### Create a Cronofy application

To use the Cronofy C# Sample App you need to create a Cronofy application. To do this, [create a free developer account](https://app.cronofy.com/sign_up/developer), click "Create New App" in the left-hand navigation and create an application.

Once you've created your application you will need to set the `cronofy_client_id` and `cronofy_client_secret` in the application's `Web.config` file.

An example of how to do this:

```
<configuration>
  ...
  <appSettings>
    <add key="cronofy_client_id" value="{{cronofy_client_id}}" />
    <add key="cronofy_client_secret" value="{{cronofy_client_secret}}" />
  </appSettings>
</configuration>
```

### Setting up a Remote URL

In order to test [Push Notification](https://www.cronofy.com/developers/api/#push-notifications) callbacks and Enterprise Connect user authentications your application will need to be reachable by the internet.

To do this we would recommend using [ngrok](https://ngrok.com/) to create a URL that is accessible by the Cronofy API.

Once you have ngrok installed you can initialise it for your application by using the following line in your terminal:

`ngrok http -host-header=localhost localhost:8080`
(Replace `localhost:8080` with `localhost:[port number]` where appropriate)

Your terminal will then display a URL in the format `http://[unique identifier].ngrok.io`. You will need to set the `domain` variable in the application's `Web.config` in order to test these remote features for example.

```
<configuration>
  ...
  <appSettings>
    ...
    <add key="domain" value="http://examplecallback.ngrok.io" />
  </appSettings>
</configuration>
```
