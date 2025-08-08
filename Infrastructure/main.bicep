param location string = resourceGroup().location
@minLength(3)
param webSiteName string
param appServicePlanName string
param appServicePlanResourceGroupName string

resource appServicePlan 'Microsoft.Web/serverfarms@2024-11-01' existing = {
  name: appServicePlanName
  scope: resourceGroup(appServicePlanResourceGroupName)
}

resource appService 'Microsoft.Web/sites@2024-11-01' = {
  name: webSiteName
  location: location
  kind: 'app'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    clientAffinityEnabled: false
    siteConfig: {
      alwaysOn: false
      http20Enabled: true
      minTlsVersion: '1.2'
      linuxFxVersion: 'DOTNETCORE|9.0'
      use32BitWorkerProcess: false
    }
  }
}
