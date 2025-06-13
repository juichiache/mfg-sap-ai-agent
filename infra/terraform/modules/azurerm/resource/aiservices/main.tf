variable "ai_services_name" {
  type        = string
  default     = "sap-agent-aiservices"
  description = "The name of the Azure AI Services resource."
}
variable "ai_services_sku" {
  type        = string
  default     = "Standard"
  description = "The SKU for the Azure AI Services resource."
}
variable "ai_services_kind" {
  type        = string
  default     = "AI"
  description = "The kind of the Azure AI Services resource."
}
variable "resource_group_name" {
  type        = string
  description = "The name of the resource group where the AI Services resource will be created."
}
variable "location" {
  type        = string
  description = "The Azure region where the AI Services resource will be created."
}
variable "tags" {
  type        = map(string)
  default     = {}
  description = "A map of tags to assign to the AI Services resource."
}
variable "environment" {
  type        = string
  default     = "sap-agent-environment"
  description = "The name of the environment for the Azure AI Services resource."
}
variable "suffix" {
  type        = string
  default     = ""
  description = "Suffix to append to resource names for uniqueness, typically derived from the environment or a random value."
}
variable "ai_services_workspace_name" {
  type        = string
  default     = "sap-agent-aiservices-workspace"
  description = "The name of the AI Services workspace."
}