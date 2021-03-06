This discord bot is written with the DSharpPlus API Wrapper in C#. This is more or less gonna be a very basic bot for the most part as I am just experimenting with stuff.

I've customized this bot to work with the server I'm setting up but feel free to yoink any code you want. It's MIT. This is also still a heavy work in progress. I gurantee that the entire code structure will change regularly.

### Features so far
- Keeping track of new members and giving them access to the server with a single command. The welcome channel id has to be configured.
- "Muting" users by taking away their roles and assigning a special muted role. Original roles will be given back upon unmuting. Muted role needs to be configured.
- Banning and kicking users
- adding/removing roles from users
- Archiving Utility: You can configure an archive channel, as well as channels to monitor. If a message is pinned in a monitored channel, the bot will automatically remove the pin and post a copy of that message into the archive channel. It works with pictures, links and will also put spoilers on images or text if it was in the original message.
- The bot can purge messages. Either just generally in a channel or from a specific user in a channel. This is prone to being severly rate limited though.
- The bot token will be read from an external text file which needs to be configured seperately.
- Dedicated log channel: You can configure a dedicated log channel in which the bot will post logs about things. Logging is still inconsistent though.
- Serialization: All of the configurations you can do (welcome channel, bot log channel, archiving, etc) will be saved in an external file which is automatically generated in a sub folder. This is for convenience in case the bot crashes or needs to be shut down. Datatbases for such a small bot would be a pain in the ass. The current process id is also saved in a text file so the bot can be terminated in a console window.
- The bot can be terminated with a commmand if desired (Only with administrator access).
- Error logging: If anything goes wrong with a command, the bot will notify whoever executed the command. There is also logging functionality which logs everything to a file using Serilog, so anything can be reviewed later.

### Planned Features:
- More moderation features
- Features that can be used by regular members
- Better command structure
- Configuration options for a customizable experience
