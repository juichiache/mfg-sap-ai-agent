resource "azurerm_eventgrid_system_topic" "eventgrid_system_topic" {
  name                   = "${var.environment}-${var.eventgrid_system_topic_name}-${var.suffix}"
  location               = var.location
  resource_group_name    = var.resource_group_name
  source_arm_resource_id = var.storage_account_id
  topic_type             = var.eventgrid_topic_type

  identity {
    type = "SystemAssigned"
  }

  tags = merge(var.tags, {
    environment = var.environment
  })
}
resource "azurerm_eventgrid_event_subscription" "eventgrid_event_subscription" {
  name  = "${var.environment}-${var.eventgrid_event_subscription_name}-${var.suffix}"
  scope = azurerm_eventgrid_system_topic.eventgrid_system_topic.id

  webhook_endpoint {
    url = "https://example.com/api/events"
  }

  storage_queue_destination {
    queue_name         = "sap-agent-queue"
    storage_account_id = var.storage_account_id
  }

  tags = merge(var.tags, {
    environment = var.environment
  })
}

output "eventgrid_system_topic_id" {
  value = azurerm_eventgrid_system_topic.eventgrid_system_topic.id
}
output "eventgrid_system_topic_name" {
  value = azurerm_eventgrid_system_topic.eventgrid_system_topic.name
}
output "eventgrid_system_topic_endpoint" {
  value = azurerm_eventgrid_system_topic.eventgrid_system_topic.endpoint
}
output "eventgrid_system_topic_identity_principal_id" {
  value = azurerm_eventgrid_system_topic.eventgrid_system_topic.identity[0].principal_id
}
output "eventgrid_system_topic_topic_type" {
  value = azurerm_eventgrid_system_topic.eventgrid_system_topic.topic_type
}