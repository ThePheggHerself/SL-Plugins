# TutorialPlus
Basic plugin to add various config options regarding the tutorial role.

# Config
| Config  | Default Value | Description |
| ------------- | ------------- | ------------- |
| tutorial_trigger096  | false  | Should tutorials be able to trigger SCP-096 and become a target |
| tutorial_observe173  | false  | Should tutorials be able to freeze SCP-173 and become an observer |
| cuffable_tutorial | false | Should tutorials be able to be handcuffed |
| tutorial_godmode | true  | Automatically enable godmode when a player spawns as tutorial (Will also automatically disable if the player changes role) |
| tutorial_bypass | true | Automatically enable bypass mode when a player spawns as tutorial (Will also automatically disable if the player changes role) |
| tutorial_noclip | true | Automatically enable noclip when a player spawns as tutorial (Will also automatically disable if the player changes role) |
| debug | false | Enables debug messages in the server console |

# Commands
This plugin has no commands

# Events
This plugin uses the following events:
 - Scp096AddingTarget
 - Scp173NewObserver
 - PlayerChangeRole
 - PlayerHandcuff
