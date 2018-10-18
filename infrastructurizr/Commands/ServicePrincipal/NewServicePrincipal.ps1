$ignore = Add-AzureRmAccount -TenantId $TenantId
$principal = Get-AzureRmADServicePrincipal -SearchString "$ServicePrincipalName" | Where-Object DisplayName -eq '$ServicePrincipalName' | Where-Object Type -eq ServicePrincipal
if ($principal -eq $null)
{
	$certFileName = '$ServicePrincipalName.pfx'
	$cert = New-SelfSignedCertificate -CertStoreLocation "Cert:\CurrentUser\My" -Subject 'CN=$ServicePrincipalName' -KeySpec KeyExchange
    $keyValue = [System.Convert]::ToBase64String($cert.GetRawCertData())
    
    # register app in Azure AD and export cert to pfx file
    try
    {
        $principal = New-AzureRMADServicePrincipal -DisplayName "$ServicePrincipalName" -CertValue $keyValue -EndDate $cert.NotAfter -StartDate $cert.NotBefore
        $password = ConvertTo-SecureString -String $CertPassword -AsPlainText -Force
		$ignore = Export-PfxCertificate -Cert $cert -Password $password -FilePath $certFileName
    }
    finally
    {
        $ignore = Get-ChildItem Cert:\CurrentUser\My\$($cert.Thumbprint) | Remove-Item
    }
	$certFileFullName = (Get-ChildItem $certFileName).FullName

    $newRole = $null
    $retries = 0
    do
    {
        if ($retries -gt 0)
        {
            Start-Sleep 10
        }
        $newRole = New-AzureRmRoleAssignment -RoleDefinitionName Contributor -Scope '/subscriptions/$SubscriptionId' -ServicePrincipalName $principal.ApplicationId -ErrorAction SilentlyContinue
        $retries++
    }
    while ($newRole -eq $null -and $retries -le 5)

    if ($newRole -eq $null)
    {
        $out = ConvertTo-Json -InputObject @{
			"Log" = 'failed to assign role to service principal $ServicePrincipalName'
			"Success" = $False
		}
		Write-Host $out
    }
	else
	{
		$out = ConvertTo-Json -InputObject @{
			"Log" = 'Service principal $ServicePrincipalName was created. The .pfx certificate file for authenticating as the service principal can be found at ' + $certFileFullName + ' and the password used to secure the certificate is $CertPassword'
			"Success"= $True
			"Created" = $True
			"Thumbprint" = $cert.Thumbprint
			"CertificateLocation" = $certFileFullName
		}
		Write-Host $out
	}	
}
else
{
	$out = ConvertTo-Json -InputObject @{
		"Log" = 'Service principal $ServicePrincipalName already exists'
		"Success" = $True
		"Created" = $False
	}
	Write-Host $out
}

