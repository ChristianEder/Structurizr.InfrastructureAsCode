Add-AzureRmAccount -ServicePrincipal -CertificateThumbprint $ServicePrincipalCertThumbprint -ApplicationId $ServicePrincipalId -TenantId $TenantId -SubscriptionId $SubscriptionId

$policy = New-AzureKeyVaultCertificatePolicy -SubjectName $SubjectName -IssuerName Self -ValidityInMonths 12
Add-AzureKeyVaultCertificate -VaultName $vaultName -Name "$CertificateName -CertificatePolicy $policy