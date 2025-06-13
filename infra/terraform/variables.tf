#############################################################################################################
# Variables for Azure SAP Agent Terraform Module
# This file defines the variables used in the Azure SAP Agent Terraform module.
#############################################################################################################

# --- Resource Group and Environment Variables ---

variable "environment" {
  description = "The environment for the resources (e.g., dev, staging, prod)"
  type        = string
  default     = "dev"
}
variable "location" {
  description = "The Azure region where resources will be created"
  type        = string
  default     = "eastus"
}
variable "resource_group_name" {
  description = "The name of the resource group where resources will be created"
  type        = string
  default     = "sap-agent-rg"
}
variable "tags" {
  description = "A map of tags to assign to the resources"
  type        = map(string)
  default     = {}
}
variable "tenant_id" {
  description = "The Azure Active Directory tenant ID"
  type        = string
  default     = "16b3c013-d300-468d-ac64-7eda0820b6d3" # Replace with your tenant ID
}
variable "subscription_id" {
  description = "The Azure subscription ID"
  type        = string
  default     = "b0c1d2e3-f4a5-6789-0abc-def123456789" # Replace with your subscription ID
}

#############################################################################################################
# --- Monitoring and Application Insights Variables ---
#############################################################################################################

variable "sap_agent_appinsights_name" {
  description = "The name of the Application Insights resource"
  type        = string
  default     = "sap-agent-appinsights"
}
variable "sap_agent_log_analytics_workspace_name" {
  description = "The name of the Log Analytics Workspace resource"
  type        = string
  default     = "sap-agent-log-analytics-workspace"
}
variable "sap_agent_log_analytics_sku" {
  description = "The SKU for the Log Analytics Workspace"
  type        = string
  default     = "PerGB2018"
}
variable "sap_agent_log_analytics_retention_in_days" {
  description = "The retention period for the Log Analytics Workspace in days"
  type        = number
  default     = 30
}

#############################################################################################################
# OpenAI Model Deployment Variables
#############################################################################################################

variable "openai_name" {
  description = "The name of the OpenAI resource"
  type        = string
  default     = "sap-agent-openai"
}
variable "openai_kind" {
  description = "The kind of the OpenAI resource"
  type        = string
  default     = "OpenAI"
}
variable "openai_sku_name" {
  description = "The SKU for the OpenAI resource"
  type        = string
  default     = "S0"
}

variable "openai_model_deployment" {
  description = "A map of deployment configurations"
  type = map(object({
    model = object({
      name    = string
      version = string
    })
    scale = object({
      type    = string
      capcity = number
    })
  }))
}

#############################################################################################################
# AI Foundry Variables
#############################################################################################################

variable "ai_foundry_name" {
  description = "The name of the Azure AI Foundry resource"
  type        = string
  default     = "sap-agent-ai-foundry"
}
variable "ai_foundry_storage_account_name" {
  description = "The name of the storage account to be used by AI Foundry"
  type        = string
  default     = "sapagentstorage"
}
variable "ai_foundry_sku" {
  description = "The SKU for the Azure AI Foundry resource"
  type        = string
  default     = "Standard"
}

#############################################################################################################
# Azure AI Services Variables
#############################################################################################################

variable "ai_services_name" {
  description = "The name of the Azure AI Services resource"
  type        = string
  default     = "sap-agent-ai-services"
}
variable "ai_services_sku" {
  description = "The SKU for the Azure AI Services resource"
  type        = string
  default     = "S1"
}
variable "ai_services_endpoint" {
  description = "The endpoint for the Azure AI Services resource"
  type        = string
  default     = "https://example.com/api/messages"
}
variable "ai_services_kind" {
  description = "The kind of the Azure AI Services resource"
  type        = string
  default     = "OpenAI"
}

#############################################################################################################
# Container Apps Variables
#############################################################################################################

variable "containerappname" {
  description = "The name of the Azure Container App"
  type        = string
  default     = "sap-agent-container-apps"
}
variable "container_name" {
  description = "The name of the container within the Azure Container App"
  type        = string
  default     = "sap-agent-container"
}
variable "container_image" {
  description = "The Docker image for the container"
  type        = string
  default     = "mcr.microsoft.com/azuredocs/containerapps-helloworld:latest"
}
variable "container_cpu" {
  description = "The CPU allocation for the container"
  type        = number
  default     = 0.5
}
variable "container_memory" {
  description = "The memory allocation for the container in GB"
  type        = number
  default     = 1.0
}
variable "revision_mode" {
  description = "The revision mode for the Azure Container App (e.g., Single, Multiple)"
  type        = string
  default     = "Single"
}
variable "containerapp_environment_name" {
  description = "The name of the Azure Container App Environment"
  type        = string
  default     = "sap-agent-containerapp-environment"
}

#############################################################################################################
# Azure Bot Service Variables
#############################################################################################################

variable "bot_service_name" {
  description = "The name of the Azure Bot Service"
  type        = string
  default     = "sap-agent-bot-service"
}
variable "bot_service_sku" {
  description = "The SKU for the Azure Bot Service"
  type        = string
  default     = "F0"
}
variable "bot_service_endpoint" {
  description = "The endpoint for the Azure Bot Service"
  type        = string
  default     = "https://example.com/api/messages"
}

#############################################################################################################
# Azure Blob Storage Variables
#############################################################################################################
variable "storage_account_name" {
  description = "The name of the Azure Blob Storage account"
  type        = string
  default     = "sapagentstorage"
}
variable "storage_account_sku" {
  description = "The SKU for the Azure Blob Storage account"
  type        = string
  default     = "Standard_LRS"
}
variable "storage_container_name" {
  description = "The name of the blob container within the Azure Blob Storage account"
  type        = string
  default     = "sap-agent-container"
}
variable "storage_account_access_tier" {
  description = "The access tier for the Azure Blob Storage account"
  type        = string
  default     = "Hot"
}
variable "storage_account_replication" {
  description = "The replication type for the Azure Blob Storage account"
  type        = string
  default     = "LRS"
}
variable "storage_blob_name" {
  description = "The name of the blob within the Azure Blob Storage container"
  type        = string
  default     = "sap-agent-blob"
}
#############################################################################################################
# Azure Event Grid Variables
#############################################################################################################

variable "eventgrid_name" {
  description = "The name of the Azure Event Grid resource"
  type        = string
  default     = "sap-agent-eventgrid"
}
variable "eventgrid_topic_type" {
  description = "The type of the Event Grid topic"
  type        = string
  default     = "Microsoft.Storage.StorageAccounts"
}
variable "eventgrid_system_topic_name" {
  description = "The name of the Event Grid system topic"
  type        = string
  default     = "sap-agent-eventgrid-system-topic"
}
variable "eventgrid_event_subscription_name" {
  description = "The name of the Event Grid event subscription"
  type        = string
  default     = "sap-agent-eventgrid-subscription"
}
variable "storage_queue_name" {
  description = "The name of the storage queue for Event Grid events"
  type        = string
  default     = "sap-agent-queue"
}

#############################################################################################################
# Azure Key Vault Variables
#############################################################################################################
variable "keyvault_name" {
  description = "The name of the Azure Key Vault resource"
  type        = string
  default     = "sap-agent-keyvault"
}
variable "keyvault_sku_name" {
  description = "The SKU for the Azure Key Vault resource"
  type        = string
  default     = "standard"
}
variable "keyvault_soft_delete_retention_days" {
  description = "The number of days to retain soft-deleted keys in the Key Vault"
  type        = number
  default     = 7
}
variable "keyvault_purge_protection_enabled" {
  description = "Whether to enable purge protection for the Key Vault"
  type        = bool
  default     = true
}

##############################################################################################################
# Azure App Service Plan Variables
##############################################################################################################
variable "appserviceplan_name" {
  description = "The name of the Azure App Service Plan"
  type        = string
  default     = "sap-agent-appserviceplan"
}
variable "appserviceplan_kind" {
  description = "The kind of the Azure App Service Plan (e.g., Linux, Windows)"
  type        = string
  default     = "Linux"
}
variable "appserviceplan_tier" {
  description = "The tier of the Azure App Service Plan"
  type        = string
  default     = "PremiumV2"
}
variable "appserviceplan_size" {
  description = "The size of the Azure App Service Plan"
  type        = string
  default     = "P2v2"
}
variable "appserviceplan_sku_name" {
  description = "The SKU name for the Azure App Service Plan"
  type        = string
  default     = "P2v2"
}
variable "appserviceplan_tags" {
  description = "A map of tags to apply to the Azure App Service Plan"
  type        = map(string)
  default     = {
    environment = "dev"
    project     = "sap-agent"
  }
} 
##############################################################################################################
# Azure Functions Variables
##############################################################################################################
variable "function_app_name" {
  description = "The name of the Azure Function App"
  type        = string
  default     = "sap-agent-function-app"
}
variable "function_app_sku_name" {
  description = "The SKU name for the Azure Function App"
  type        = string
  default     = "Y1"
}
variable "function_app_kind" {
  description = "The kind of the Azure Function App (e.g., FlexConsumption)"
  type        = string
  default     = "FlexConsumption"
}
variable "function_app_tags" {
  description = "A map of tags to apply to the Azure Function App"
  type        = map(string)
  default     = {
    environment = "dev"
    project     = "sap-agent"
  }
}
variable "function_app_os_type" {
  description = "The operating system type for the Azure Function App (e.g., linux, windows)"
  type        = string
  default     = "linux"
}
variable "function_app_runtime_version" {
  description = "The runtime version for the Azure Function App"
  type        = string
  default     = "~4"
}
variable "docker_image" {
  description = "The Docker image for the Azure Function App"
  type        = string
  default     = "mcr.microsoft.com/azuredocs/functions-helloworld:latest"
} 
