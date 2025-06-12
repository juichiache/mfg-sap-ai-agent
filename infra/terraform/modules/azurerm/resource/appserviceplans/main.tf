variable "suffix" {
  type        = string
  default     = ""
  description = "suffix to append to resource names for uniqueness, typically derived from the environment or a random value."
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
variable "appserviceplan" {
  type        = string
  default     = "sap-agent-appserviceplan"
  description = "The name of the App Service Plan."
}
variable "appserviceplan_kind" {
  type        = string
  default     = "Linux"
  description = "The kind of App Service Plan (e.g., Linux, Windows)."
}
variable "appserviceplan_tier" {
  type        = string
  default     = "PremiumV2"
  description = "The pricing tier for the App Service Plan."
}
variable "appserviceplan_size" {
  type        = string
  default     = "P2v2"
  description = "The size of the App Service Plan."
}
variable "tags" {
  type        = map(string)
  default     = {}
  description = "A map of tags to assign to the resources."
}

