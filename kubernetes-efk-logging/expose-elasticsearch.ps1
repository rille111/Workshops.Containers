$pod = kubectl get pods -l app=elasticsearch -o=jsonpath='{.items[*].metadata.name}' -n logging
kubectl port-forward $pod 9200:9200 -n logging