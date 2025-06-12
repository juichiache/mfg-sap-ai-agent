
variable "functionappname" {
  type        = string
  default     = "sap-agent-functionapp"
  description = "The name of the Function App."
}
variable "appserviceplan_kind" {
  type        = string
  default     = "Linux"
  description = "The kind of App Service Plan (e.g., Linux, Windows)."
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
variable "appserviceplan_id" {
  type        = string
  description = "The ID of the App Service Plan to use for the Function App."
}
variable "storage_account_name" {
  type        = string
  description = "The name of the storage account to use for the Function App."
}
variable "environment" {
  type        = string
  default     = "dev"
  description = "The environment for which the resources are being created (e.g., dev, prod)."
}