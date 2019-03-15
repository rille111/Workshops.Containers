$pod = kubectl get pods -l app=prometheus-server -o=jsonpath='{.items[*].metadata.name}'
kubectl port-forward $pod 8080:9090