using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;


namespace Manny.Dialogs
{
    [LuisModel("6bc3b969-8bb5-4b66-91ba-0e78af21c41e", "ddf295c2da95419fb90db90b92b9fc95", domain: "westcentralus.api.cognitive.microsoft.com")]
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";
            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("greeting")]
        public async Task greeting(IDialogContext context, LuisResult result)
        {
            string message = $"Hello there. How can I help you?";
            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("help")]
        public async Task help(IDialogContext context, LuisResult result)
        {
            //TODO: fill of the help info in the message below
            string message = $"Here is the stuff you can do.........";
            await context.PostAsync(message);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("ViewSupportingDocuments")]
        public async Task ViewSupportingDocuments(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var act = await activity;
            var replyToId = act.Id;
            var materialIds = result.Entities.Where(b => b.Type == "materialId").Select(b => b.Entity).ToList();

            //TODO: ad the rest of the supporting document entity names that youi want to support here
            List<string> supportingDocumentEntities = new List<string>
            {
                "pallet pattern",
                "thermal specs",
                "allergens",
                "mandatory label copy"
            };

            var supportingDocsRequested = result.Entities.Where(b => supportingDocumentEntities.Contains(b.Type)).Select(b => b.Type).ToList();
            
            for (int i=0; i<supportingDocsRequested.Count(); i++)
            {
                IMessageActivity replyMsg = context.MakeMessage();

                //TODO: add case statements all the remain supporting doc types
                switch (supportingDocsRequested[i])
                {
                    case "pallet pattern":
                        replyMsg.TextFormat = "plain";
                        replyMsg.Locale = "en-Us";
                        replyMsg.Text = $"Here is the pallet pattern for material ID {materialIds[0]}!";
                        replyMsg.ReplyToId = replyToId;
                        //TODO: call your Oracle API to retrieve the appropriate pallet pattern URL or file
                        replyMsg.Attachments.Add(GetInternetAttachment("https://perodenstorage.blob.core.windows.net/conagrabotfiles/pallet-pattern.JPG", "pallet-pattern.jpg"));
                        await context.PostAsync(replyMsg);
                        break;
                    case "thermal specs":
                        replyMsg.TextFormat = "plain";
                        replyMsg.Locale = "en-Us";
                        replyMsg.Text = $"Here are the thermal specs for material ID {materialIds[0]}!";
                        replyMsg.ReplyToId = replyToId;
                        //TODO: call your Oracle API to retrieve the appropriate thermal spec URL or file
                        replyMsg.Attachments.Add(GetInternetAttachment("https://perodenstorage.blob.core.windows.net/conagrabotfiles/fire.jpg", "fire.jpg"));
                        await context.PostAsync(replyMsg);
                        break;
                    case "allergens":
                        replyMsg.TextFormat = "plain";
                        replyMsg.Locale = "en-Us";
                        replyMsg.Text = $"Here are the allergens for material ID {materialIds[0]}!";
                        replyMsg.ReplyToId = replyToId;
                        //TODO: call your Oracle API to retrieve the appropriate allergens URL or file
                        replyMsg.Attachments.Add(GetInternetAttachment("https://perodenstorage.blob.core.windows.net/conagrabotfiles/peanut.jpg", "peanut.jpg"));
                        await context.PostAsync(replyMsg);
                        break;
                    case "mandatory label copy":
                        replyMsg.TextFormat = "plain";
                        replyMsg.Locale = "en-Us";
                        replyMsg.Text = $"Here is the mandatory label copy for material ID {materialIds[0]}!";
                        replyMsg.ReplyToId = replyToId;
                        //TODO: call your Oracle API to retrieve the appropriate mandatory label copy URL or file
                        replyMsg.Attachments.Add(GetInternetAttachment("https://perodenstorage.blob.core.windows.net/conagrabotfiles/bender.jpg", "bender.jpg"));
                        await context.PostAsync(replyMsg);
                        break;
                }
            }
        }

        private static Attachment GetInternetAttachment(string url, string filename)
        {
            return new Attachment
            {
                Name = filename,
                ContentType = MimeHelper.GetMimeType(url),
                ContentUrl = url
            };
        }

        private static Attachment GetInlineAttachment(string filePath, string fileName)
        {
            var imagePath = HttpContext.Current.Server.MapPath(filePath);
            var imageData = Convert.ToBase64String(File.ReadAllBytes(imagePath));

            return new Attachment
            {
                Name = fileName,
                ContentType = MimeHelper.GetMimeType(filePath),
                ContentUrl = $"data:{MimeHelper.GetMimeType(filePath)};base64,{imageData}"
            };
        }

    }
}