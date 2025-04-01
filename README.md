# mfg-sap-ai-agent

## Deployment instructions

1.  Create Entra ID app registration & service principal

    https://learn.microsoft.com/en-us/azure/bot-service/bot-service-quickstart-registration?view=azure-bot-service-4.0&tabs=userassigned

1.  Deploy Azure infrastructure

    1.  Enable Channels->Microsoft Teams

    1.  Copy the `Microsoft App ID`

1.  Configure application settings

1.  Build M365 manifest

    1.  Navigate to https://dev.teams.microsoft.com/

    1.  Click on `Create new app`

    1.  Fill out form, most important part is the `Application (client) ID` (this is the Entra ID app ID)

    1.  Fill out single sign-on, with `api://tenant-id/app-id`

    1.  On `App features`, select `Bot` and provide `bot ID` (this is the Microsfot App ID from the Azure Bot)

    1.  Click Publish

    1.  Select `Download`

    1.  Update manifest with the following:

    ```
    {
        "$schema": "https://developer.microsoft.com/en-us/json-schemas/teams/vdevPreview/MicrosoftTeams.schema.json",
        "version": "1.0.0",
        "manifestVersion": "devPreview",
        "id": "8f3164b6-62cf-44da-9746-a06a523780a5",
        "name": {
            "short": "bot-mfg-sap-ai-agents",
            "full": ""
        },
        "developer": {
            "name": "MFG",
            "mpnId": "",
            "websiteUrl": "https://microsoft.com",
            "privacyUrl": "https://microsoft.com",
            "termsOfUseUrl": "https://microsoft.com"
        },
        "description": {
            "short": "bot-mfg-sap-ai-agents",
            "full": "bot-mfg-sap-ai-agents"
        },
        "icons": {
            "outline": "outline.png",
            "color": "color.png"
        },
        "accentColor": "#FFFFFF",
        "staticTabs": [
            {
            "entityId": "conversations",
            "scopes": [
                "personal"
            ]
            },
            {
            "entityId": "about",
            "scopes": [
                "personal"
            ]
            }
        ],
        "bots": [
            {
            "botId": "f79b6d54-3137-4632-9fc3-4b08a6344ae9",
            "scopes": [
                "personal",
                "groupChat"
            ],
            "isNotificationOnly": false,
            "supportsCalling": false,
            "supportsVideo": false,
            "supportsFiles": false
            }
        ],
        "validDomains": [],
        "webApplicationInfo": {
            "id": "f79b6d54-3137-4632-9fc3-4b08a6344ae9",
            "resource": "api://66beb9f0-9df6-4ded-8e48-126b39813500/f79b6d54-3137-4632-9fc3-4b08a6344ae9"
        },
        "copilotAgents": {
            "customEngineAgents": [
            {
                "type": "bot",
                "id": "f79b6d54-3137-4632-9fc3-4b08a6344ae9"
            }
            ]
        }
        }
        ```

1.  Import M365 app

    1.  Navigate to https://admin.microsoft.com/Adminportal/Home

    1.  Click on Settings->IntegratedApps

    1.  In the `Deployed apps` tab, click on `Upload custom apps`.

    1.  Upload the ZIP file