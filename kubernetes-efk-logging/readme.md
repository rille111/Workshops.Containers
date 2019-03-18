# EFK

It's a stack with the components: ElasticSearch (Db), Fluent Bit or FluentD (log collectors), Kibana (Dashboard, graphs).
Useful for output logging for the components in our cluster, for example.

We're NOT gonna follow this guide: https://akomljen.com/get-kubernetes-logs-with-efk-stack-in-5-minutes/
But we're gonna read https://kubernetes.io/docs/concepts/cluster-administration/logging/ and decide what to use.
If you're using GKE, you can (almost) automatically set this up, although with FLuentD, see here: https://kubernetes.io/docs/tasks/debug-application-cluster/logging-elasticsearch-kibana/.

If we wanna do everything manually, we will do https://www.digitalocean.com/community/tutorials/how-to-set-up-an-elasticsearch-fluentd-and-kibana-efk-logging-stack-on-kubernetes

## Requirements

An entire EFK-stack eats lots of resources. So to be safe, you need at least 6vcpus if you're running in the cloud, and 4 if Minikube. RAM - dunno. Will monitor the resource usage in grafana before and after installation.

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

I elected to compose yamls for the first guide, but using FluentBit instead of FluentD, and therefore some stuff from the second guide. Reading these guides, the yamls are ready for use here.

## Step by step 

### Install EFK

We can use an a) image or a b) Helm installation or c) composing our own yamls. The a) is the cheaper variant for testing, not really for production, and b) requires a lot more resources, where c) we got more control. So we go with c).

#### 1. Install ElasticSearch Cluster
* Tune the __elasticsearch-*.yaml__'s to your liking
* Run `kubectl create -f logging-namespace.yaml`
* Run `kubectl create -f elasticsearch-service.yaml`
* Run `kubectl create -f elasticsearch-statefulset.yaml`
* Run this command until you get Status Bound: `kubectl get pvc -n logging` that's the persisted disk.
* Run this command until you see the Pod up and running `kubectl get pods -n logging`
* Optionally run this command to wait until everything is finished: `kubectl rollout status sts/es-cluster -n logging`
* Run `kubectl port-forward es-cluster-0 9200:9200 -n logging`
    * And verify that ES works by doing a webrequest with curl or Postman like `curl http://localhost:9200/_cluster/state?pretty` get some response
#### 2. Install Kibana
* Tune the __kibana-service-and-deployment.yaml__ to your liking
* Run `kubectl create -f .\kibana-service-and-deployment.yaml`
* Run this command to wait until all finished `kubectl rollout status deployment/kibana -namespace logging`
* Run `expose-kibana-pod.ps1` and browse to http://localhost:5601, make sure it works and then exit
#### 2. Install FluentBit (avert from the guide)