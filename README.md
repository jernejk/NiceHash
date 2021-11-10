# NiceHash

With GPU prices rising and with no apparent end to be able to get a cost effective GPU for foreseeable future, using your existing GPU seems to be one of the few way how to patch the gap.

There a few goals for this project:
- Control NiceHash miner
- Get real-time power usage
- Get basic stats about the wallet/miner
- Allow controlling miner

One of the long term goals is to run the miner when the electric grid isn't at peak capacity to reduce the stress on electric grid.

This project is not a fork of official NiceHash GitHub repos and was written for .NET developers in mind.

# Usage

Atm, you can use `NiceHash.Cmd` to control it as command line or `NiceHash.Core` if you're more interested in using it directly in C#.
It will require .NET 6 SDK to compile the project. (you'll need Visual Studio 2022+, VS Code or Jetbrains Rider)

I'm also working on Elgato Stream Deck support so that I can control it faster.

## Console application

Duplicate `appsettings.json` as `appsettings.Local.json` and replace variables with API values from NiceHash portal to avoid accidental commits.

![Console application examples](images/console-example.png)

## Elgato Stream

Install:

1. Have .NET 6 SDK
2. Open `src\NiceHash.ElgatoStreamDeck` with Windows Terminal or PowerShell
3. Run `dotnet build`
4. Run `.\RegisterPluginAndStartStreamDeck.ps1`

Currently supported buttons:
- Wallet + Rigs control
   - Show NiceHash wallet balance and rigs status
   - Tap - Update wallet balance
   - Long press - Start/Stop rig
- Start rig - on tap start rig
- Stop rig - on tap stop rig

![image](https://user-images.githubusercontent.com/5943653/141119433-aeb5e2d2-9f5a-437d-8be5-c789a19ac239.png)

**Figure:** Various buttons in Stream Deck.

![image](https://user-images.githubusercontent.com/5943653/141119836-ff038286-a994-4a28-9c47-7a0027b9599a.png)

**Figure:** Show wallet balance and rig status.

**TIP:** You can use Stream Deck Multi Action to stop rigs and start a game.

![image](https://user-images.githubusercontent.com/5943653/141119258-f17f7b0f-edb3-4538-94bc-d271e4250bba.png)

**Figure:** Different games which are Multi Actions.

![image](https://user-images.githubusercontent.com/5943653/141119341-1bf856f2-18db-4ee3-81d7-10f41b97d6dd.png)

**Figure:** Rigs are stopped before running the game.

# Resources

- [Official NiceHash demo apps](https://github.com/nicehash/rest-clients-demo)
- [NiceHash documentation](https://www.nicehash.com/docs/rest)
