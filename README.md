# DiscordRandomStatus
An app that randomly changes your custom status on Discord
# Usage
You can either use the executable which you can find under releases, or build this repository yourself  

Login with either your e-mail and password, or your token, if you know how to get that  

You can add as many custom statuses as you want by pressing the add button  

To add a custom emoji, click on the emoji button and paste an escaped emoji string into the text box that pops up (You can escape emojis in Discord by adding a \ behind them, such as \:emoji:, the result should look like <:hl3:258406409236905995> for a custom emoji and 😃 for default emojis)  

You can find settings and whatnot under %AppData%/araghon007/DiscordRandomCustomStatus
# Security
I tried to make this app as secure as I knew how  

The token is stored using System.Security.Cryptography, which should make the resulting file only decryptable on your account, on your computer  

The executable in releases is just a painstaking merge of the main project and Newtonsoft.Json + Typography, so they aren't in separate assemblies  
# Todo
Clean up the code like, a lot  

Add functionality to display rare statuses  

I'm still pretty new to the whole git thing, so I need to figure out how to properly configure submodules
