This discord bot is written with the Discord .Net API Wrapper in C#. I don't know how many things I will add but for now there will be basic things for server management.

Features so far
- Keeping track of new members and giving them access to the server with a single command.
- "Muting" users by taking away their roles and assigning a special muted role. Original roles will be given back upon unmuting.
- Banning and kicking users
- adding/removing roles from users
- Archiving Utility: You can configure an archive channel, as well as channels to monitor. If a message is pinned in a monitored channel, the bot will automatically remove the pint and post a copy of that message into the archive channel.
- The bot can purge messages. Either just generally in a channel or from a specific user in a channel. This is prone to being severly rate limited though.
- Most features can be used with either a textbased command using the '!' prefix or slash commands (Slash commands hate arrays so that kinda sucks),
- The bot token will be read from an external text file.


Planned Features:
- Basic Serialization. I don't want to bother with databases, so later down the line, configurations will be serialized and saved in a file. Other stuff like muted members, new members, etc. will be serialized as well. Helps recovering data if the bot ever crashes.
- More moderation features
- Features that can be used by regular members (idk what yet though)