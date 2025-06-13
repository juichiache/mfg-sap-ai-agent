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
variable "log_analytics_workspace_id" {
  type        = string
  description = "The ID of the Log Analytics Workspace to use for logging."
}
variable "containerapp_environment_name" {
  type        = string
  default     = "sap-agent-containerapp-environment"
  description = "The name of the Container App Environment."
}
variable "container_app_name" {
  type        = string
  default     = "sap-agent-containerapp"
  description = "The name of the Container App."
}
variable "container_image" {
  type        = string
  description = "The image to use for the Container App."
}
variable "container_cpu" {
  type        = number
  default     = 0.5
  description = "The CPU allocation for the Container App."
}
variable "container_memory" {
  type        = number
  default     = 1.0
  description = "The memory allocation for the Container App in GB."
}
variable "revision_mode" {
  type        = string
  default     = "Single"
  description = "The revision mode for the Container App (e.g., Single, Multiple)."
}
variable "environment" {
  type        = string
  default     = "sap-agent-containerapp-environment"
  description = "The name of the Container App Environment."
}