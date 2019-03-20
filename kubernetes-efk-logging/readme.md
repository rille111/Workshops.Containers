# EFK
It's a stack with the components: ElasticSearch (Db), Fluent Bit or FluentD (log collectors), Kibana (Dashboard, graphs).
Useful for output logging for the components in our cluster, for example.
We're choosing FluentBit because it's smaller, more performant and specific for our needs, and enriches data with pod_name, pod_id etc.

We're NOT gonna follow this guide: https://akomljen.com/get-kubernetes-logs-with-efk-stack-in-5-minutes/
But we're gonna read https://kubernetes.io/docs/concepts/cluster-administration/logging/ and decide what to use.
If you're using GKE, you can (almost) automatically set this up, although with FLuentD, see here: https://kubernetes.io/docs/tasks/debug-application-cluster/logging-elasticsearch-kibana/.

If we wanna do everything manually, we will do https://www.digitalocean.com/community/tutorials/how-to-set-up-an-elasticsearch-fluentd-and-kibana-efk-logging-stack-on-kubernetes

## Requirements
An entire EFK-stack eats lots of resources. So to be safe, you need at least 3 nodes or 6 vcpus if you're running in the cloud, and 4 if Minikube. For RAM? Dunno. Will monitor the resource usage in grafana before and after installation.

## Choices
The docs state that we can either:
* Use a node-level logging agent that runs on every node. (prefet DaemonSet replica) this is most common.
    * We can use the cloud providers guide to do this: https://kubernetes.io/docs/tasks/debug-application-cluster/logging-elasticsearch-kibana/
    * Or we can do it ourselves.
* Include a dedicated sidecar container for logging in an application pod.
* Push logs directly to a backend from within an application

We shall focus on the node level logging agents. There are a couple of guides:
* https://www.digitalocean.com/community/tutorials/how-to-set-up-an-elasticsearch-fluentd-and-kibana-efk-logging-stack-on-kubernetes
* https://medium.com/@jbsazon/aggregated-kubernetes-container-logs-with-fluent-bit-elasticsearch-and-kibana-5a9708c5dd9a
* https://mherman.org/blog/logging-in-kubernetes-with-elasticsearch-Kibana-fluentd/
* https://prabhatsharma.in/blog/logging-in-kubernetes-using-elasticsearch-the-easy-way/

I elected to compose yamls following the topmost guide for Elastic and Kibana, but for FluentBit I use stuff from the Prabhat guide (Helm) with some modifications.

## Installation

We can use an a) image or a b) Helm installation or c) composing our own yamls. The a) is the cheaper variant for testing, not really for production, and b) requires a lot more resources, where c) we got more control. So we go with c).

#### 1. Install ElasticSearch Cluster (E of EFK)
We're gonna use the 1-pod version, so it might not suit all production needs!
* Tune the __elasticsearch-*.yaml__'s to your liking
* Run `kubectl create -f logging-namespace.yaml`
* Run `kubectl create -f elasticsearch-service.yaml`
* **Important!** If you're running Minikube, then edit elastic-statefulset.yaml and change the storageclass from default to standard! 
* Run `kubectl create -f elasticsearch-statefulset.yaml`
* Run this command until you get Status Bound: `kubectl get pvc -n logging` that's the persisted disk.
* Run this command until you see the Pod up and running `kubectl get pods -n logging`
* Optionally run this command to wait until everything is finished: `kubectl rollout status sts/es-cluster -n logging`
* Run `kubectl port-forward es-cluster-0 9200:9200 -n logging`
    * Go to `http://localhost:9200/_cluster/state?pretty` and verify

#### 2. Install Kibana (K of EFK)
* Tune the __kibana-service-and-deployment.yaml__ to your liking
* Run `kubectl create -f .\kibana-service-and-deployment.yaml`
* Run this command to wait until all finished `kubectl rollout status deployment/kibana -namespace logging`
* Run `expose-kibana-pod.ps1` and browse to http://localhost:5601, make sure it works and then exit

#### 3. Install FluentBit (F of EFK)
* run `helm install stable/fluent-bit --name=fluent-bit --namespace=logging --set backend.type=es --set backend.es.host=elasticsearch`
* run `kubectl get pods -n logging` until you see X many running fluentbit pods where X is as many nodes as you got
* Run `expose-kibana-pod.ps1` again
* Browse to http://localhost:5601
    * Create an Index with: `kubernetes_*` and choose @timestamp
    * Wait, Click Discover, and see logs!

#### Notes
Using the 1-pod version it seems like CPU only went up from 7 to 11% across 3 nodes (Azure Standard DSv2), and memory from 17 to 20 %. That's a good deal!

Next time, I'll use the aforementioned guide: https://prabhatsharma.in/blog/logging-in-kubernetes-using-elasticsearch-the-easy-way/ and use Helm with parameters to specialize the installation.