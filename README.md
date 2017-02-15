---
platforms: Bot Framework + Azure Function
development language: C#
author: Ashish Sahu
---

#What is this code?

This respository contains code that can be deployed as an Azure Fuction app. The purpose of this code is to allow you to connect your own messaging channels with the bots that you can create using Microsoft Bot Framework.

The benefit of this code is that you can start leveraging the Bot Framework capabilities with your existing messaging channels that you are are using or you maintain for your customers as a providers (SMS, email, chat apps etc).

#How does this work?

This code makes use of the Bot Framework DirectLine APIs to make this possible. The ideal flow is that when you receive a message from your users using a channel that is not supported with Bot Framework at the moment - you can pass that message to this Azure Function app which uses an HTTP trigger. The Azure Function app connects to the Bot Framework bot using the Directline APIs and waits for the bot to respond to the message and passes it back to you as a JSON response. You can now process this information and pass the bot response back to your users. You also get the conversation id back in the response which you can use to continuw the conversation if required.

#How to use this?

1. Create a C# HTTP trigger Azure Function app and copy/paste the code in the **run.csx** file in your run.csx file.
2. Since we also use Newtonsoft.json, you also need to create/copy the project.json file so that the require nuget packages can be installed.
3. Interacting with the Bot Framework Directline APIs requires a Directline secret which you can get from the https://dev.botframework.com portal by enabling the Directline channel in the bot configuration. Copy and paste that on the line no. 11 in your run.csx file.

You are ready to go now - use your favorite browser and access this URL - https://your-function-app-URL?message=message-from-your-channel&id=0

This should get you the same response back that you would see when using one of the supported channels such as Skype, FB Messenger, Teams or Slack etc.
