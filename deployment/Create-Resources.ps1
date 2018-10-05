# Creates a resource group and the all the necessary resources in it.
#
#     # Specify the name of the environment as an argument, e.g.
#     .\Create-Resources.ps1 staging

[CmdletBinding()]
param(
  [Parameter(Mandatory=$True)][string]$resourceGroupId
)

$resourceGroupName = "erp-int-$resourceGroupId"

Select-AzureRmSubscription `
  -SubscriptionName Integrations

New-AzureRmResourceGroup `
  -Location "North Europe" `
  -Name $resourceGroupName

if ($resourceGroupId -eq "prod") {
  New-AzureRmResourceGroupDeployment `
  -ResourceGroupName $resourceGroupName `
  -TemplateFile ./azuredeploy.json `
  -TemplateParameterFile ./azuredeploy.parameters.prod.json `
  # -Debug
}
else {
  New-AzureRmResourceGroupDeployment `
  -ResourceGroupName $resourceGroupName `
  -TemplateFile ./azuredeploy.json `
  # -Debug
}