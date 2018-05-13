# Podrace

Benchmarking using Kubernetes

![Podrace Scene from Star Wars Episode 1](https://lh3.googleusercontent.com/v-d9mx7gkB-O3LAecjx92CnJoqeVnE1uD5kelGojslVmQXCL0N_WhYiA4V2MrJbh6k4tef6E7EnZUBss6NN8vBpZ6nfj7HUaRhRXnFj2RKpjztUDGCaY_sdzlbsnr9CvU071vRzMkkR8X1-Hjgo7s9FBokxPHK6g31p7Wk3hCvDAMP_79tG-7tY75cNBd5GlTxaCEHgOhsyYRuottcAVRbNVMvjhqGo6Y0mrAUFl1YnapDS-oYtbPRgFZNrlvdiPErANC0V9koGYIiFQuPDWozzNTXgo2VcSwPtq-CAFzKNDoYEYA14f6geB71kAHZkFgCpft9gEBwmjO1UkZM5zEOZ8IuSst429Ep2D8Br-L60R8j3QdRc_lA0pOdbXbZcfMGsauWHJmJfUviIntyNgJTQSDHxhTk3_leXnxwrkdoe_AsZ-lN-9QIZsS3LCZTDfBJCZL7dYBQugzmGYnQnw-RN1yLlprtirmFrMGID3iA1ypIgtba0piHEloaUgFaedOrjapHMcK_ujf4Rx1GSt5XO1yQJikdd2_ajEA7N3ty6e7fXD0YBRiknTO4hGqQrDj_suOcYD7K2P0giJi4TAEFJU-XbDDz9y=s0)

## Quick Example:

1. Configure local `kubectl` to point at a Kubernetes cluster. Docker for Windows Edge comes with Kuberetes now: https://docs.docker.com/docker-for-windows/#kubernetes
2. Run `./podrace.cmd run -p ./samples/StaticHttpBenchmark` (`podrace.cmd` just does a `dotnet run` on `src/podrace`, no building necessary)

Once that works, you can point `kubectl` at an Azure Kubernetes Service instance and run the exact same benchmark in the cloud! Or build a physical Kubernetes cluster and run it there.

## How it works

1. You define a `racefile.yaml` file which contains the following sections:
    * `configs` - A set of Kubernetes `.yaml` files that define Deployments representing Pods to run and Services exposing some of those Pods to other Pods. All Pods must have the label `podrace=true` and should have a `podrace-role` label setting the "role" of the Pod
    * `warmup` - A sequence of "Laps" to run during warmup. The only supported Lap currently is `exec` (see below)
    * `benchmark` - A sequence of "Laps" to run during the benchmark.
    * `collectors` - A sequence of "Collectors" to run on the Pods after the benchmark completes.
2. When `podrace run` is run, it loads the Racefile and:
    1. Deploys the Kubernetes Config files
    2. Runs the `warmup` Laps
    3. Runs the `benchmark` Laps
    4. Runs the `collectors`
    5. Deletes the Kubernetes Resources

## Laps

A "Lap" is an action to take during a Warmup or Benchmark Run. There is only one type of Lap currently:

### `exec` Laps

An `exec` Lap has a `type` value of `exec` (or no `type` value at all, as it is the default). There are two additional parameters:

* `role`: A string denoting the roles on which to run the command. The command will be run on all pods that have a `podrace-role` label matching this value
* `command`: An array of command and arguments to run on the Pod.

## Collectors

A "Collector" is an action that is run on Pods **after** the benchmark completes. There is only one type of Collector currently:

### `file` Collector

A `file` Collector has a `type` value of `file`. There are three additional parameters:

* `role`: A string denoting the roles from which to extract the file. A copy of the file from each Pod with the matching `podrace-role` label will be extracted.
* `source`: The path within the container of the file to extract, relative to the container's working directory.
* `destination`: An optional relative path within the output directory in which to place the file.

Note: The output directory is layed out with a directory for each Pod. So if the file collector is extracting the file `foobar.txt` from Pod `client-abc123`, it will end up in `<Output Directory>/client-abc123/foobar.txt`
   
## Next Steps

1. Drop the requirement for Kubernetes configurations (let the `racefile.yaml` contain container names, etc.)
1. Automatic node anti-affinity for Pods (don't want the server and the client on the same node, that would be bad!)
1. Integrate with Dockerfiles to allow building containers rather than requiring pre-built images
1. Add better support for aggregating data and publishing it
1. Add support for a SignalR-based signalling system to trigger runs.
1. Examples:
  * Kestrel Plaintext
  * SignalR
  * SignalR + Redis (easier with Kubernetes!!)
