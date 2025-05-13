using AdaptiveCards;
using Assistants.Hub.API.Assistants.RAG;
using Microsoft.Agents.Builder;
using Microsoft.Agents.Builder.App;
using Microsoft.Agents.Builder.State;
using Microsoft.Agents.Core.Models;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;

namespace Assistants.Hub.API
{
    public class SapAgent : AgentApplication
    {
        private readonly SAPChatService _sapChatService;

        public SapAgent(AgentApplicationOptions options, SAPChatService sapChatService) : base(options)
        {
            _sapChatService = sapChatService ?? throw new ArgumentNullException(nameof(sapChatService));

            OnConversationUpdate(ConversationUpdateEvents.MembersAdded, WelcomeMessageAsync);
            OnActivity(ActivityTypes.Message, MessageActivityAsync, rank: RouteRank.Last);
        }

        protected async Task MessageActivityAsync(ITurnContext turnContext, ITurnState turnState, CancellationToken cancellationToken)
        {
            StringBuilder response = new();
            try
            {
                await turnContext.StreamingResponse.QueueInformativeUpdateAsync("Reticulating splines...", cancellationToken);

                var chatHistory = turnState.GetValue("conversation.chatHistory", () => new ChatHistory());

                // Invoke the SAPChatService to process the message
                ChatMessageContent message = new(AuthorRole.User, turnContext.Activity.Text);
                chatHistory.Add(message);

                var responses = new List<string>();
                
                await foreach (var responseChunk in _sapChatService.ExecuteAsync(
                    chatHistory, 
                    intermediateMessage => turnContext.StreamingResponse.QueueInformativeUpdateAsync(intermediateMessage, cancellationToken), 
                    cancellationToken))
                {
                    if (responseChunk != null)
                    {
                        //don't stream the response back since we need to fully populate the adaptive card
                        //intermediate status messages will be 
                        response.Append(responseChunk.Text);
                    }
                }                
            }
            finally
            {
                AdaptiveCard adaptiveCard = new("1.6")
                {
                    Body = [new AdaptiveTextBlock(response.ToString()) { Wrap = true }]
                };

                turnContext.StreamingResponse.FinalMessage = MessageFactory.Attachment(new Attachment()
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = adaptiveCard.ToJson()
                });

                await turnContext.StreamingResponse.EndStreamAsync(cancellationToken);
            }
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