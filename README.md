# Podrace

Benchmarking using Docker

![Podrace Scene from Star Wars Episode 1](https://lh3.googleusercontent.com/v-d9mx7gkB-O3LAecjx92CnJoqeVnE1uD5kelGojslVmQXCL0N_WhYiA4V2MrJbh6k4tef6E7EnZUBss6NN8vBpZ6nfj7HUaRhRXnFj2RKpjztUDGCaY_sdzlbsnr9CvU071vRzMkkR8X1-Hjgo7s9FBokxPHK6g31p7Wk3hCvDAMP_79tG-7tY75cNBd5GlTxaCEHgOhsyYRuottcAVRbNVMvjhqGo6Y0mrAUFl1YnapDS-oYtbPRgFZNrlvdiPErANC0V9koGYIiFQuPDWozzNTXgo2VcSwPtq-CAFzKNDoYEYA14f6geB71kAHZkFgCpft9gEBwmjO1UkZM5zEOZ8IuSst429Ep2D8Br-L60R8j3QdRc_lA0pOdbXbZcfMGsauWHJmJfUviIntyNgJTQSDHxhTk3_leXnxwrkdoe_AsZ-lN-9QIZsS3LCZTDfBJCZL7dYBQugzmGYnQnw-RN1yLlprtirmFrMGID3iA1ypIgtba0piHEloaUgFaedOrjapHMcK_ujf4Rx1GSt5XO1yQJikdd2_ajEA7N3ty6e7fXD0YBRiknTO4hGqQrDj_suOcYD7K2P0giJi4TAEFJU-XbDDz9y=s0)

## Sample Racefile

```yaml
roles:
  server:
    image: 'aspnetbenchmarks/KestrelPlaintext'
  client:
    image: wrk
    cmd: [ "/bin/sh", "-c", "wrk -c 10 -d 5s http://server/ > client.wrk.log" ]
collectors:
- type: file
  role: client
  source: "client.wrk.log"
```