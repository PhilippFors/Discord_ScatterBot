This discord bot is written with the DSharpPlus API Wrapper in C#. I don't know how many things I will add but for now there will be basic things for server management.

Note that this bot has been strongly customized to fit with my server setup. Feel free to yoink any code you want, but you'll most certainly have to adjust a lot of things and navigate my terrible code. This is also still a heavy work in progress. I gurantee that the entire code structure will change regularly.

Features so far
- Keeping track of new members and giving them access to the server with a single command.
- "Muting" users by taking away their roles and assigning a special muted role. Original roles will be given back upon unmuting.
- Banning and kicking users
- adding/removing roles from users
- Archiving Utility: You can configure an archive channel, as well as channels to monitor. If a message is pinned in a monitored channel, the bot will automatically remove the pin and post a copy of that message into the archive channel. It works with pictures, links and will also put spoilers on images or text if it was in the original message.
- The bot can purge messages. Either just generally in a channel or from a specific user in a channel. This is prone to being severly rate limited though.
- The bot token will be read from an external text file.
- Serialization: You can configure a welcome channel, botlog channel, muted role and access role. Muted users will be saved, new users and welcome messages will be saved. When the bot restarts, everything will be automatically loaded and reinitialized. The save file is automatically generated in a folder.

Planned Features:
- Serialization fallbacks: In case no config is provided, fallbacks should be used so it doesn't just error out (possibly just responding to the channel you typed a command in)
- Error handling so the bot notifies you of an exception with proper formatting
- More moderation features
- Features that can be used by regular members
