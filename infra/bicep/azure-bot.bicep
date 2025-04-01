param location string

resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2024-11-30' = {
  name: 'id-${uniqueString(resourceGroup().id)}'
}

resource botService 'Microsoft.BotService/botServices@2022-09-15' = {
  name: 'bot-${uniqueString(resourceGroup().id)}'
  location: location
  sku: {
    name: 'F0'
  }
  kind: 'azurebot'
  properties:{
    msaAppId: managedIdentity.properties.clientId
    displayName: 'bot-${uniqueString(resourceGroup().id)}'
    endpoint: 
  }
}
