variable "loganalytics_workspace_name" {
  type        = string
  default     = "sap-agent-loganalytics"
  description = "The name of the Log Analytics Workspace."
}
variable "loganalytics_sku" {
  type        = string
  default     = "PerGB2018"
  description = "The SKU for the Log Analytics Workspace."
}
variable "loganalytics_retention_in_days" {
  type        = number
  default     = 30
  description = "The retention period in days for the Log Analytics Workspace."
}
# variable "loganalytics_solution_name" {
#   type        = string
#   default     = "sap-agent-loganalytics-solution"
#   description = "The name of the Log Analytics Solution."
# }
# variable "loganalytics_solution_product" {
#   type        = string
#   default     = "OMSGallery/LogAnalytics"
#   description = "The product for the Log Analytics Solution."
# }
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