# Pikcube.Common

This is a collection of generic utilities and extension I wrote for my own mods.

## Another mod says it needs Pikcube.Common to work, how do I install it?

Just download the .zip file and place the contents of it inside your mods folder. Or download it from the workshop (once workshop support drops).

## Requirements

This mod uses C# extension blocks, a language feature added in C# 14. You will need to add `<LangVersion>14<LangVersion/>` to your project file to use this library.

## Feature: Relic Spawn Filter API

An API that allows for removing relics from the common Relic Grab Bag. Doesn't prevent relics from spawning from events and ancients.

Adding a rule is as easy as calling RegisterRule and passing in a filter.

```cs
RelicSpawnManager relicSpawnManager = new();
relicSpawnManager.RegisterRule<UnceasingTop>(runstate => runstate.AscensionLevel < 4); // Only allows UnceasingTop to spawn below Ascension 4.
```

Your rule can be any function that takes an IRunState as a parameter and returns a bool. Return true if the relic is allowed to spawn and return false if the relic should not be allowed to spawn.

You can also remove rules if that behavior is destired.

```cs
relicSpawnManager.DeregisterRuleIfExist<UnceasingTop>(); //Use the same relicSpawnManager you used to create the rule in the first place
```

Registration uses the type itself instead of the canonical instance as a key, so you are free to create rules before ModelDB is initialized.

## Feature: CustomRunManager

An API that allows for adding custom runs to the Custom Run list. Entries are appended after the base game runs.

Adding a run modifier is as easy as calling CustomRunManager.Register

```cs
CustomRunManager.Register<PraiseSnecko>(CustomRunType.Good); //Adds a run modifier that starts you with Snecko Eye
```

The CustomRunType defines whether the modifier will be in the good list (colored green) or the red list (colored red).

```cs
CustomRunManager.Register<PeakGaming>(CustomRunType.Bad); // Adds a run modifier that duplicates your starting deck and starts you with Bing Bong
```

## Feature: An implementation of the Cursed Debuff from Dicey Dungeons

Like I said, I wrote these for my own mods. I'm a huge fan of Dicey Dungeons, and I use this debuff enough it was easier to just stick it in my common library.

[You can read up on it here](https://wiki.diceydungeons.com/doku.php?id=statuses:cursed), but the upshot is that each card has a 50/50 chance to be discarded without being played. Disappears when triggered or at the end of the turn.

## Utilities: Way Too Many Extensions

I love extension methods and I add them for nearly everything. Examples include

* Creating a card with `Claw.CreateInstance(player)`, the equivalent of `player.RunState.CreateCard<Claw>(player)`
* Getting a relic's canonical instance with `SneckoEye.Canonical`, the equivalent of `ModelDb.Relic<SneckoEye>().CanonicalInstance`
* Replacing an item in a list with `myList.TryReplaceValue(oldItem, newItem)`, which is a bit more complicated than a simple alias

## How To Use as a Developer

Eventually you'll be able to install this with NuGet, but for now, you can install it manually.

Download Pikcube.Common.dll, add it as a reference to your project, and include "Pikcube.Common" as a dependency in your mod's json definition. You can copy the .pdb files over if you want them for interactive debugging purposes.
