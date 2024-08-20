# PolyMonopoly

This project is a simple implementation of the popular board game Monopoly. It includes a web client that allows players to play the game online.

## Getting Started

### Prerequisites (if not using Docker/Podman/devcontainer)

- Node.js (v22 or later)
- dotnet core (v8 or later)
- JDK (v22 or later, required only for e2e testing)

### Installation and running

It is highly recommended to use Docker/Podman. 
Since this simplifies deployment and is configured to run in https by default.

#### Docker/Podman

1. Clone the repository:
    ```Bash
    git clone https://github.com/NydusBorn/PolyMonopoly.git
    ```
2. Navigate to the project directory:
    ```Bash
    cd PolyMonopoly
    ```
3. Change network parameters (optional):

    The project is configured to run on localhost by default, and thus cannot be used for hosting a public instance.
   Change BACKEND_HOST in environment in compose.yaml to a full root of a host,
   examples include:

    - https://127.0.0.1:8000 (instance for you only)
    - https://localhost:8000 (instance for you only)
    - https://192.168.0.1:8000 (instance for you only)
    - https://1.2.3.4:8000 (would be your public instance, if your public ip is 1.2.3.4)
    - https://your.website.com:8000 (your public instance)
   
   This is used only for telling the web clients where to connect to.
   Port 8000 is the default backend port,
   if you want to change it, change host port in ports of caddy-proxy service in compose.yaml, look for one that targets 8000
    
   Web client port can be changed similarly.
   You need to change host port in ports of caddy-proxy service in compose.yaml, look for one that targets 3000

4. Start the docker containers:
    ```Bash
    docker compose --profile $$ up -d
    ```
   Where $$ is any of backend, frontend, full
5. Open host firewall ports (required if this is a public instance):
    - Web client: 3000 by default
    - Backend: 8000 by default
#### Locally

1. Clone the repository:
    ```Bash
    git clone https://github.com/NydusBorn/PolyMonopoly.git
    ```
2. Navigate to the project directory:
    ```Bash
    cd PolyMonopoly
    ```
##### For Web Client
1. Navigate to the web client directory:
    ```Bash
    cd web-client
    ```
2. Install dependencies:
    ```Bash 
    npm install
    ```
3. Running the Web Client:
   1. Make sure that BACKEND_HOST is set in your environment. (it is set by direnv within the repository)
   2. Start the development server:
       ```Bash
       npm run dev
       ```
   3. Open your browser and navigate to http://localhost:3000.
##### For Backend
1. Navigate to the backend directory:
    ```Bash
    cd asp-backend
    ```
2. Run the project:
    ```Bash
    dotnet run
    ```
##### For e2e tests
1. Navigate to the e2e directory:
    ```Bash
    cd e2e-tests
    ```
2. Run tests:
    ```Bash
    ./gradlew test
    ```
##### Automated scenarios
If you have just runner installed, then you can use just {recipe-name} to run any of the recipes specified in justfile.

More human friendly list of recipes can be achieved via 
```Bash
just --list
```

Before running any recipes, ensure there are no dependencies missing in any of the subprojects you are planning to run.
## License
This project is licensed under the MIT License.