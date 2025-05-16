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

namespace Assistants.Hub.API.M365
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
            catch (Exception ex)
            {
                turnContext.StreamingResponse.QueueTextChunk(ex.Message);
            }
            finally
            {
                AdaptiveCard adaptiveCard = new("1.5");
                List<string> fileIds;
                List<StringBuilder> codeResponses;
                ExtractFileIdsAndCodeResponses(responses, out fileIds, out codeResponses);

                //read CodeBlock.json file for adaptive card template
                var codeBlockTemplate = File.ReadAllText("M365/AdaptiveCards/CodeBlock.json");
                var codeBlockJson = AdaptiveCard.FromJson(codeBlockTemplate);
                var codeBlockCard = codeBlockJson.Card;

                foreach (var codeResponse in codeResponses)
                {
                    var codeBlock = codeBlockCard.Body[0];
                    codeBlock.AdditionalProperties["codeSnippet"] = codeResponse.ToString();
                    //adaptiveCard.Body.Add(new AdaptiveTextBlock(codeResponse.ToString()));
                    adaptiveCard.Body.Add(codeBlock);
                }

                foreach (var fileId in fileIds)
                {
                    adaptiveCard.Body.Add(new AdaptiveImage(url: fileId));
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

        private static void ExtractFileIdsAndCodeResponses(List<ChatChunkResponse> responses, out List<string> fileIds, out List<StringBuilder> codeResponses)
        {
            fileIds = new List<string>();
            foreach (var response in responses)
            {
                if (response.ContentType == ChatChunkContentType.Image)
                {
                    fileIds.Add(response.Content);
                }
            }

            codeResponses = new List<StringBuilder>();
            foreach (var response in responses)
            {
                if (response.ContentType == ChatChunkContentType.Code)
                {
                    if (codeResponses.Count == 0)
                    {
                        codeResponses.Add(new StringBuilder());
                    }
                    codeResponses[0].Append(response.Content);
                }
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