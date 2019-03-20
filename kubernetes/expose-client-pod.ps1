$pod = kubectl get pods -l component=web -o=jsonpath='{.items[*].metadata.name}' -n default
kubectl port-forward $pod 8089:80