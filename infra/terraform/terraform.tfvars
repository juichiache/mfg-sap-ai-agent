#############################################################################################################
# Terraform Variables for SAP Agent Infrastructure
#############################################################################################################

environment         = "dev"
location            = "eastus"
resource_group_name = "mfg-sap-ai-agents"
tenant_id           = "16b3c013-d300-468d-ac64-7eda0820b6d3" # Replace with your Azure tenant ID
subscription_id     = "b0c1d2e3-f4a5-6789-0abc-def123456789" # Replace with your Azure subscription ID
tags = {
  environment = "dev"
  project     = "sap-agent"
}

#############################################################################################################
# Monitoring and Application Insights Configuration
#############################################################################################################

sap_agent_appinsights_name                = "sap-agent-appinsights"
sap_agent_log_analytics_workspace_name    = "sap-agent-log-analytics-workspace"
sap_agent_log_analytics_sku               = "PerGB2018"
sap_agent_log_analytics_retention_in_days = 30

#############################################################################################################
# Open AI Configuration
#############################################################################################################

openai_name     = "sap-agent-openai"
openai_kind     = "OpenAI"
openai_sku_name = "S0"

openai_model_deployment = [{
  name = "gpt-4"
  model = {
    format  = "OpenAI"
    name    = "gpt-4"
    version = "1106-Preview"
  }
  sku = {
    name = "Standard"
  }
  rai_policy_name = "gpt-4-rai-policy"
  },
  {
    name = "gpt-35-turbo"
    model = {
      format  = "OpenAI"
      name    = "gpt-35-turbo"
      version = "1106"
    }
    sku = {
      name = "Standard"
    }
    rai_policy_name = "gpt-35-turbo-rai-policy"
}]

#############################################################################################################
# Azure AI Foundry Configuration
#############################################################################################################

ai_foundry_name                 = "sap-agent-ai-foundry"
ai_foundry_sku                  = "Standard"
ai_foundry_storage_account_name = "sapagentstorage"

#############################################################################################################
# Azure AI Services Configuration
#############################################################################################################

ai_services_name     = "sap-agent-ai-services"
ai_services_sku      = "Standard"
ai_services_endpoint = "https://example.com/ai-services"

#############################################################################################################
# Container Apps Configuration
#############################################################################################################

containerappname               = "sap-agent-container-apps"
container_name                 = "sap-agent-container"
container_image                = "mcr.microsoft.com/azure-container-apps/sap-agent:latest"
container_cpu                  = 0.5
container_memory               = "1.0Gi"
revision_mode                  = "Single"
containerapp_environment_name = "sap-agent-containerapp-environment"


#############################################################################################################
# Azure Bot Service Configuration
#############################################################################################################

bot_service_name     = "sap-agent-bot-service"
bot_service_sku      = "S1"
bot_service_endpoint = "https://example.com/api/messages"

#############################################################################################################
# Azure Blob Storage Configuration
#############################################################################################################

storage_account_name        = "sapagentblobstorage"
storage_account_sku         = "Standard_LRS"
storage_container_name      = "sap-agent-container"
storage_account_access_tier = "Hot"
storage_account_replication = "LRS"
storage_blob_name           = "sap-agent-blob"

#############################################################################################################
# Azure Event Grid Configuration
#############################################################################################################

eventgrid_name                    = "sap-agent-eventgrid"
eventgrid_topic_type              = "Microsoft.Storage.StorageAccounts"
eventgrid_system_topic_name       = "sap-agent-eventgrid-system-topic"
eventgrid_event_subscription_name = "sap-agent-eventgrid-subscription"
storage_queue_name                = "sap-agent-queue"

#############################################################################################################
# Azure Key Vault Configuration
#############################################################################################################

keyvault_name                       = "sap-agent-key-vault"
keyvault_sku_name                   = "standard"
keyvault_soft_delete_retention_days = 7
keyvault_purge_protection_enabled   = true

#############################################################################################################
# Azure App Service Plan Configuration
#############################################################################################################
appserviceplan_name     = "sap-agent-appserviceplan"
appserviceplan_kind     = "Linux"
appserviceplan_tier     = "PremiumV2"
appserviceplan_size     = "P2v2"
appserviceplan_sku_name = "P2v2"
appserviceplan_tags = {
  environment = "dev"
  project     = "sap-agent"
}

#############################################################################################################
# Azure Functions Configuration
#############################################################################################################
function_app_name     = "sap-agent-function-app"
function_app_sku_name = "Y1"
function_app_kind     = "FlexConsumption"
function_app_tags = {
  environment = "dev"
  project     = "sap-agent"
}
function_app_os_type         = "linux"
function_app_runtime_version = "~4"
docker_image                 = "mcr.microsoft.com/azure-functions/dotnet:4"