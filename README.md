# Steam Game Checklist

## About (purpose)

I need a list of the steam games I own so I can make sure I've written
a review and added it to my [Steam Curator](http://store.steampowered.com/curator/26701155-The-Pepp-Selection/).

I wanted something simple so I used SQLite. It's a simple tool for personal use.

You'll need to supply your own Steam Api Key and Steam UserId in the appsettings.json file.

## Changes to accommodate Svelte

I started by using the "react" template, `dotnet new react` to get a base I knew
was a single page app (SPA).

First I swapped out the contents of the ClientApp folder with a simple template
from the Svelte's website.

Second I needed to change [Startup.cs](/src/SteamGameChecklist.Web/Startup.cs) so it
wasn't trying to start a react server but to simply pass
through requests to the dev server. It's this line,

```csharp
spa.UseProxyToSpaDevelopmentServer("http://localhost:8080");
```

Then I had to go back and change the
[ClientApp/package.json](/src/SteamGameChecklist.Web/ClientApp/package.json)
so it starts the dev server on the port I set in Startup.cs.

I also had to switch where the SPA build directory in the
[csproj file](/src/SteamGameChecklist.Web/SteamGameChecklist.Web.csproj) and to
Startup.cs so it will move the files on publish.

## Steam integration

I needed to run the games download every time the site started so I wanted to include it
in startup. This is the part of the app that could be improved. I made a server to make it
easy to be used in other locations if I ever decide to move it but as it is right now
it's a little weird.

Maybe adding a refresh button to the website that would run it on the user's request would
be clearer.

## How to build

To build on Windows run,

```
dotnet publish --self-contained -r win10-x64 -c Release
```

To create an exe you can run that will startup the site. You'll need to move a checklist.db into
the directory for the site to run.

## How to develop

You have to start the dev server for Svelte yourself while you're developing. There's
no way I found to have it start automatically on `dotnet run`. However, it works
well and the dev server will restart when you make changes so you can run it and
forget it.

### Steps

1. `dotnet ef database update`
1. in a new terminal run `npm run dev`
1. in the first terminal `dotnet run`
