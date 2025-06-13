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
variable "containerap_environment_name" {
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
