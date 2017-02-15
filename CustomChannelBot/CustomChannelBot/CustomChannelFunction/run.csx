using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    string directlinesecret = "jJbYxB8zB4Q.cwA.4zU.4Q_AD-XXXXXXXXXXXXXXXXXXXXXXXXXXXXXxx";
    BotConversation newConv;
    int waterMark = 0;

    log.Info("C# HTTP trigger function processed a request.");

    // parse query parameter
    string message = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "message", true) == 0)
        .Value;

    // parse query parameter
    string id = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "id", true) == 0)
        .Value;

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    HttpStatusCode statusCode = HttpStatusCode.OK;
    string replyMessage = "";
    if (message == "" || id == "")
    {
        replyMessage = "{ \"id\": \"0\", \"replymessage\": \"Please pass the conversation id and message.\"}";
        statusCode = HttpStatusCode.BadRequest;
    }
    else
    {
        newConv = new BotConversation();

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + directlinesecret);
            var botResponse = await client.PostAsync("https://directline.botframework.com/v3/directline/conversations", null);

            newConv = JsonConvert.DeserializeObject<BotConversation>(await botResponse.Content.ReadAsStringAsync());
        }

        //Connecting to the Bot
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + newConv.token);

            TextMessage textMessage = new TextMessage();
            From fromID = new From();
            fromID.id = "chatapp";
            textMessage.from = fromID;
            textMessage.text = message;
            textMessage.type = "message";

            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(textMessage), Encoding.UTF8, "application/json");
            var botResponse = await client.PostAsync("https://directline.botframework.com/v3/directline/conversations/" + newConv.conversationId + "/activities", httpContent);

            DirectLineResponse dResponse = JsonConvert.DeserializeObject<DirectLineResponse>(await botResponse.Content.ReadAsStringAsync());
        }

        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + newConv.token);

            var botResponse = await client.GetAsync("https://directline.botframework.com/v3/directline/conversations/" + newConv.conversationId + "/activities?watermark=" + waterMark);
            if (botResponse.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ReplyActivity reply = JsonConvert.DeserializeObject<ReplyActivity>(await botResponse.Content.ReadAsStringAsync());
                waterMark = Convert.ToInt32(reply.watermark);
                id = reply.activities[reply.activities.Count - 1].conversation.id;
                message = reply.activities[reply.activities.Count - 1].text;
                replyMessage = "{ \"id\": \"" + id + "\"," + "\"replymessage\": \"" + message + "\"}";

                statusCode = HttpStatusCode.OK;
            }
            else
            {
                statusCode = botResponse.StatusCode;
            }
        }
    }

    return req.CreateResponse(statusCode, replyMessage);
}

public class BotConversation
{
    public string conversationId { get; set; }
    public string token { get; set; }
    public string streamUrl { get; set; }
}

public class From
{
    public string id { get; set; }
}

public class TextMessage
{
    public string type { get; set; }
    public From from { get; set; }
    public string text { get; set; }
}

public class DirectLineResponse
{
    public string id { get; set; }
}

public class Conversation
{
    public string id { get; set; }
}

public class Activity
{
    public string type { get; set; }
    public string channelId { get; set; }
    public Conversation conversation { get; set; }
    public string id { get; set; }
    public From from { get; set; }
    public string text { get; set; }
}

public class ReplyActivity
{
    public List<Activity> activities { get; set; }
    public string watermark { get; set; }
}