using AdaptiveCards;
using Assistants.API.Core;
using Assistants.Hub.API.Assistants.RAG;
using Assistants.Hub.API.Assistants.SAP;
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
        private readonly SAPAzureAIAgent _sapAzureAIAgent;

        public SapAgent(AgentApplicationOptions options, SAPAzureAIAgent sapAzureAIAgent) : base(options)
        {
            _sapAzureAIAgent = sapAzureAIAgent ?? throw new ArgumentNullException(nameof(sapAzureAIAgent));

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
                var chatThreadRequest = new ChatThreadRequest(turnContext.Activity.Text);

                var responses = new List<string>();               
                await foreach (var responseChunk in _sapAzureAIAgent.ExecuteAsync(chatThreadRequest, 
                    intermediateMessage => turnContext.StreamingResponse.QueueInformativeUpdateAsync(intermediateMessage, cancellationToken), 
                    cancellationToken))
                {
                    if (responseChunk != null)
                    {
                        //don't stream the response back since we need to fully populate the adaptive card
                        //intermediate status messages will be 
                        response.Append(responseChunk.Content);
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