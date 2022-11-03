dotnet new sln
dotnet new webapi -n IoT-web-api
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
dotnet add package Microsoft.AspNet.WebApi.Client

Set-Location -Path  ..\IoTLib
dotnet add package Microsoft.Azure.Devices
dotnet add package Microsoft.Azure.Devices.Client

#run commands
dotnet watch run

#deploy to azure function with local git
$webappname='iot-webapi'
$rg='shoc'
$sp='iot-app'
$user=''
$pass=''
az webapp create --name $webappname --resource-group $rg --plan $sp
az webapp cors add --resource-group $rg --name $webappname --allowed-origins 'http://localhost:3000'
az webapp config appsettings set --name $webappname --resource-group $rg --settings DEPLOYMENT_BRANCH=master
#az webapp config appsettings set --resource-group $rg --name $webappname --settings PROJECT="IoT-web-api/IoT-web-api.csproj" #if use github please remove this from azure
az webapp deployment user set --user-name $user --password $pass
#echo Git deployment URL: $(az webapp deployment source config-local-git --name $webappname --resource-group $rg --query url --output tsv)
git remote add azure-prod "https://$user@${webappname}.scm.azurewebsites.net/$webappname.git"
git push azure-prod master
