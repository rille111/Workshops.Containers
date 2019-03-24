kubectl apply -f .\fluent-bit-configmap.yaml
$pod = kubectl get pods -l k8s-app=fluent-bit-logging -o=jsonpath='{.items[*].metadata.name}' -n logging
kubectl delete pod $pod -n logging