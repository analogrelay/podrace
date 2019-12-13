# Perfnetes with Podrace

## Prerequisites

* Kubernetes Cluster
* `kubectl` installed locally and pointed to your cluster
* A *separate* nodepool named `d3nodes` (if it's different, you'll have to update the `nodeSelector`s in the YAML), ideally at the "benchmarking" spec for ASP.NET: Standard_D3_v2 for comparable result.s

## How to run a benchmark

1. Deploy the resources
    * From the root of the repo, run `kubectl apply -f .\deploy`
1. Wait for them to be deployed
    1. Run `kubectl get deployment -w` (to poll for changes in deployments)
    1. Wait for all three deployments to hit `1/1` under `READY`
    1. Ctrl-C to stop watching
1. Identify the node on which the target pod is running:
    1. Run `kubectl get pod -l "podrace-role=application,podrace-scenario=plaintext" --template='{{(index .items 0).spec.nodeName}}'` to get the node name
1. Copy the content of `wrk-command-line.txt`.
1. Replace `NODENAME` in that content with the node name
1. Get the load generator pod name from `kubectl get pod -l "podrace-role=load"`
1. Run `kubectl exec [LOAD GENERATOR POD NAME] [PASTE WRK COMMAND]` to run the benchmark!