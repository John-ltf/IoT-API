dotnet new sln
dotnet new webapi -n IoT-API
dotnet new classlib -n IoTLib
dotnet sln add .\IoT-API\IoT-API.csproj
dotnet sln add .\IoTLib\IoTLib.csproj
Set-Location -Path .\IoT-API
dotnet add reference ..\IoTLib
dotnet new gitignore
dotnet add package Swashbuckle.AspNetCore.Swagger

dotnet add package Microsoft.OpenApi
dotnet add package Swashbuckle.AspNetCore.Swagger
dotnet add package Swashbuckle.AspNetCore.SwaggerGen
dotnet add package Swashbuckle.AspNetCore.SwaggerUI

Set-Location -Path  ..\IoTLib
dotnet add package Microsoft.Azure.Devices
dotnet add package Microsoft.Azure.Devices.Client

#run commands
dotnet watch run


#develop/deploy an to function app
func start --csharp
# func start --dotnet-isolated-debug
$webappname='iot-web-backend'
$rg='shoc'
$sp='iot-api'
$user=''
$pass=''
az webapp create --name  --resource-group $rg --plan $sp
az webapp cors add --resource-group $rg --name $webappname --allowed-origins 'http://localhost:3000'
az webapp config appsettings set --name $webappname --resource-group $rg --settings DEPLOYMENT_BRANCH=master
az webapp config appsettings set --resource-group $rg --name $webappname --settings PROJECT="IoT-API/IoT-API.csproj"
az webapp deployment user set --user-name $user --password $pass
#echo Git deployment URL: $(az webapp deployment source config-local-git --name $webappname --resource-group $rg --query url --output tsv)
git remote add azure-prod "https://$user@$webappnamescm.azurewebsites.net/$webappname.git"
git push azure-prod master
