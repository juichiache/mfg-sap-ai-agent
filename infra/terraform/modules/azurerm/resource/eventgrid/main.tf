variable "eventgrid_system_topic_name" {
  type        = string
  default     = "sap-agent-eventgrid"
  description = "The name of the Event Grid resource."
}
variable "eventgrid_topic_type" {
  type        = string
  default     = "Microsoft.Storage.StorageAccounts"
  description = "The type of the Event Grid topic."
}
variable "eventgrid_event_subscription_name" {
  type        = string
  default     = "sap-agent-eventgrid-subscription"
  description = "The name of the Event Grid event subscription."
}
variable "storage_account_id" {
  type        = string
  description = "The ARM ID of the storage account to be used by Event Grid."
}
variable "location" {
  type        = string
  default     = "eastus"
  description = "The Azure region where resources will be created."
}
variable "resource_group_name" {
  type        = string
  default     = "sap-agent-rg"
  description = "The name of the resource group where resources will be created."
}
variable "tags" {
  type        = map(string)
  default     = {}
  description = "A map of tags to assign to the resources."
}
variable "suffix" {
  type        = string
  default     = ""
  description = "Suffix to append to resource names for uniqueness, typically derived from the environment or a random value."
}
variable "environment" {
  type        = string
  default     = "sap-agent-environment"
  description = "The name of the environment for the Event Grid resource."
}