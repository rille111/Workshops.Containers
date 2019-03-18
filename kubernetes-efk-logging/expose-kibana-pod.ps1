$pod = kubectl get pods -l app=kibana -o=jsonpath='{.items[*].metadata.name}' -n logging
kubectl port-forward $pod 5601:5601 -n logging