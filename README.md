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

Adding a run modifier is as easy as creating a class that inherits from `CustomRunModifierModel`

```cs
public class PraiseSnecko() : CustomRunModifierModel(CustomRunType.Good, new CustomRunModifierInfo(MainFile.ModId, "Praise Snecko")
```

Your custom run modifier needs to pass in whether this modifier is Green (CustomRunType.Good) or Red (CustomRunType.Bad), along with an instance of CustomRunModifierInfo.

Your ModId can be passed manually, but if you are using a BaseLib tempalte it'll be found in MainFile.ModId, along with the name of your modifier (for sorting purposes).

You can also define a sort priority to set whether your modifier comes before the base game modifiers (`ModifierPriority.PrefixGeneric`) or after (`ModifierPriority.PostfixGeneric`).

You can also use `ModifierPriority.PrefixSegmented` and `ModifierPriority.PostfixSegmented` if you prefer your modifiers to all be grouped together (instead of being mixed in with all custom modifiers).

There is also a special Priority of `ModifierPriority.Immediate` which is for modifiers that belong at the very top of the list. This is intended for modifiers that are essencially customization options (such as a "Always Whale" modifier to start with standard Neow Options, or "The Ending" to enable Act 4 (once it comes out).

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

Download Pikcube.Common.dll, add it as a reference to your project, and include "Pikcube.Common" as a dependency in your mod's json definition. You can copy the .pdb files over to your game's mod directory if you want them for interactive debugging purposes.
