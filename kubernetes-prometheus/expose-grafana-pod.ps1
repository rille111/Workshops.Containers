$pod = kubectl get pods -l app=grafana -o=jsonpath='{.items[*].metadata.name}' -n monitoring
kubectl port-forward $pod 3000:3000 -n monitoring