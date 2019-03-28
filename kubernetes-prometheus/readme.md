# Install Prometheus

## a) Using Helm & Tiller

I haven't tested the helm chart but it is probably more desirable than option b).

It's a monitoring framework for all kubernetes components.
The project and install guide is here: https://github.com/coreos/prometheus-operator/tree/master/contrib/kube-prometheus

Guide 1: applying yamls from git repo:
Guide 2: helm chart (untested): https://akomljen.com/get-kubernetes-cluster-metrics-with-prometheus-in-5-minutes/

## b) Using the prometheus git repository

* Choose a temporary folder for where to store the cloned git repo
* `git clone https://github.com/coreos/prometheus-operator.git`
* `cd prometheus-operator`
* `cd contrib`
* `cd kube-prometheus`
* `kubectl apply -f manifests/`
* `.\expose-grafana-pod.ps1`
* browse to `http://localhost:3000`  login: admin password: admin
* checkout the dashboards

## Make Grafana accessible from outside

To expose it with a real url and make it accessible via a route via the ingress, follow this guide:
https://github.com/coreos/prometheus-operator/blob/master/contrib/kube-prometheus/docs/exposing-prometheus-alertmanager-grafana-ingress.md

* For the __htpasswd__ part, use http://www.htaccesstools.com/htpasswd-generator-windows/ to generate a file.
* Use that file and set a secret in the `monitoring` namespace with this file.

Not done this yet, but will add steps here later.