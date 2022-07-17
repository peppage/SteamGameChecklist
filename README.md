# Steam Game Checklist

❗ **Just an example repo. Archived because it's not going to be updated.**

## About (purpose)

I need a list of the steam games I own so I can make sure I've written
a review and added it to my [Steam Curator](http://store.steampowered.com/curator/26701155-The-Pepp-Selection/).

I wanted something simple so I used SQLite. It's a simple tool for personal use.

You'll need to supply your own Steam Api Key and Steam UserId in the appsettings.json file.

## Using Svelte

Check [this repo](https://github.com/Kiho/aspcore-spa-cli/tree/master/samples/SvelteCliSample) for an updated
way to use Svelte.

## Steam integration

I needed to run the games download every time the site started so I wanted to include it
in startup. This is the part of the app that could be improved. I made a server to make it
easy to be used in other locations if I ever decide to move it but as it is right now
it's a little weird.

Maybe adding a refresh button to the website that would run it on the user's request would
be clearer.

### Limitations

Of course Steam doesn't return games that are "Profile Features Limited" so there will be games that
are missing. I found the information [on the steam forums](https://steamcommunity.com/discussions/forum/1/1643167006290763265/).

## How to build

To build on Windows run,

```
dotnet publish -c Release
```

To create an exe you can run that will startup the site. You'll need to move a checklist.db into
the directory for the site to run.
