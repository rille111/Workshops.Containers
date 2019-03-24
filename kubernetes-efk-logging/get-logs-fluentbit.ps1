$pod = kubectl get pods -l k8s-app=fluent-bit-logging -o=jsonpath='{.items[*].metadata.name}' -n logging
kubectl logs $pod -n logging