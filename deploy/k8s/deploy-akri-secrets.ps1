# Get the secret and convert to yaml excluding the namespace line
$secretYaml = kubectl get secret dapr-trust-bundle --namespace=dapr-system -o yaml | 
    Select-String -Pattern '^\s*namespace:\s' -NotMatch

# Save to a temporary file
$tempFile = [System.IO.Path]::GetTempFileName()
$secretYaml | Set-Content -Path $tempFile

# Apply the yaml to the new namespace
kubectl apply --namespace=mec-accelerator -f $tempFile

# Cleanup the temporary file
Remove-Item -Path $tempFile