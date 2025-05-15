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
            //StringBuilder response = new();
            var responses = new List<ChatChunkResponse>();
            try
            {
                await turnContext.StreamingResponse.QueueInformativeUpdateAsync("Reticulating splines...", cancellationToken);

                var chatHistory = turnState.GetValue("conversation.chatHistory", () => new ChatHistory());

                // Invoke the SAPChatService to process the message
                var chatThreadRequest = new ChatThreadRequest(turnContext.Activity.Text);

                await foreach (var responseChunk in _sapAzureAIAgent.ExecuteAsync(chatThreadRequest,
                    intermediateMessage => turnContext.StreamingResponse.QueueInformativeUpdateAsync(intermediateMessage, cancellationToken),
                    cancellationToken))
                {
                    if (responseChunk != null)
                    {
                        switch (responseChunk.ContentType)
                        {
                            case ChatChunkContentType.Text:
                                turnContext.StreamingResponse.QueueTextChunk(responseChunk.Content);
                                break;
                        }
                        responses.Add(responseChunk);
                    }
                }
            }
            finally
            {
                AdaptiveCard adaptiveCard = new("1.5");

                foreach (var response in responses)
                {
                    switch (response.ContentType)
                    {
                        case ChatChunkContentType.Image:
                            adaptiveCard.Body.Add(
                                new AdaptiveImage($"data&colon;image/jpeg;base64,{response.Content}")
                            );
                            break;
                        case ChatChunkContentType.Code:
                            //TODO: AdaptiveCodeBlock?
                            break;
                    }
                }
                
                if (adaptiveCard.Body.Count > 0)
                {
                    turnContext.StreamingResponse.FinalMessage = MessageFactory.Attachment(new Attachment()
                    {
                        ContentType = "application/vnd.microsoft.card.adaptive",
                        Content = adaptiveCard.ToJson(),
                    });
                }

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