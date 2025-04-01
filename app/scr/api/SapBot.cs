using Assistants.API.Core;
using Assistants.Hub.API.Assistants.RAG;
using Microsoft.Agents.BotBuilder;
using Microsoft.Agents.BotBuilder.App;
using Microsoft.Agents.BotBuilder.State;
using Microsoft.Agents.Core.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;

namespace Assistants.Hub.API
{
    public class SapBot : AgentApplication
    {
        private readonly SAPChatService _sapChatService;

        public SapBot(AgentApplicationOptions options, SAPChatService sapChatService) : base(options)
        {
            _sapChatService = sapChatService ?? throw new ArgumentNullException(nameof(sapChatService));

            OnConversationUpdate(ConversationUpdateEvents.MembersAdded, WelcomeMessageAsync);
            OnActivity(ActivityTypes.Message, MessageActivityAsync, rank: RouteRank.Last);
        }

        protected async Task MessageActivityAsync(ITurnContext turnContext, ITurnState turnState, CancellationToken cancellationToken)
        {
            var chatHistory = turnState.GetValue("conversation.chatHistory", () => new ChatHistory());

            // Invoke the SAPChatService to process the message
            ChatMessageContent message = new(AuthorRole.User, turnContext.Activity.Text);
            chatHistory.Add(message);

            // var forecastResponse = await _sapChatService.ExecuteAsync(chatHistory, cancellationToken);
            var responses = new List<string>();
            await foreach (var responseChunk in _sapChatService.ExecuteAsync(chatHistory, cancellationToken))
            {
                if (responseChunk != null)
                {
                    responses.Add(responseChunk.Text);
                }
            }

            if (responses.Count == 0)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text("Sorry, I couldn't query SAP at the moment."), cancellationToken);
                return;
            }

            // Create a response message based on the response content type from the WeatherForecastAgent
            //copy the responses into the response text
            IActivity response = MessageFactory.Text(string.Join("\n", responses));
            //IActivity response = forecastResponse.ContentType switch
            //{
            //    WeatherForecastAgentResponseContentType.AdaptiveCard => MessageFactory.Attachment(new Attachment()
            //    {
            //        ContentType = "application/vnd.microsoft.card.adaptive",
            //        Content = forecastResponse.Content,
            //    }),
            //    _ => MessageFactory.Text(forecastResponse.Content),
            //};

            // Send the response message back to the user. 
            await turnContext.SendActivityAsync(response, cancellationToken);
        }

        protected async Task WelcomeMessageAsync(ITurnContext turnContext, ITurnState turnState, CancellationToken cancellationToken)
        {
            foreach (ChannelAccount member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text("Hello and Welcome! I'm here to help with all your SAP needs!"), cancellationToken);
                }
            }
        }
    }
}