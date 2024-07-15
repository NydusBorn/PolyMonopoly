# Monopoly Project

This project is a simple implementation of the popular board game Monopoly. It includes a web client that allows players to play the game online.

## Getting Started

### Prerequisites (if not using Docker/Podman/devcontainer)

- Node.js (v22.3.0 or later)
- npm (v10.8.1 or later)
- dotnet core (v8.0.303 or later)

### Installation and running

#### Docker/Podman

1. Clone the repository:
    ```Bash
    git clone https://github.com/your-username/monopoly-project.git
    ```
2. Navigate to the project directory:
    ```Bash
    cd PolyMonopoly
    ```
3. Change network parameters (optional):

    Project is configured to run on localhost by default, and thus cannot be used for hosting a public instance. Change BACKEND_HOST in environment in compose-full (or compose-client if you are running the application on multiple hosts) to a full root of a host, examples include:

    - 127.0.0.1:8000 (instance for you only)
    - localhost:8000 (instance for you only)
    - 192.168.0.1:8000 (instance for you only)
    - 1.2.3.4:8000 (would be your public instance, if your public ip is 1.2.3.4)
    - your.website.com:8000 (your public instance)
   
    This is used only for telling the web clients where to connect to. Port 8000 is the default backend port, if you want to change it, change BACKEND_PORT in environment in compose-full (or compose-backend if you are running the application on multiple hosts).
    
    Web client port can be changed in similar manner, you need to change WEB_PORT in environment in compose-full (or compose-client if you are running the application on multiple hosts).

4. Start the docker containers:
    ```Bash
    docker compose up -d $$
    ```
   Where $$ is any of compose-full.yaml, compose-backend.yaml, compose-client.yaml
5. Open your firewall ports (required if this is a public instance):
    - Web client: 3000 by default
    - Backend: 8000 by default
#### Locally

1. Clone the repository:
    ```Bash
    git clone https://github.com/your-username/monopoly-project.git
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
   1. Start the development server:
    ```Bash
    npm run dev
    ```
   2. Open your browser and navigate to http://localhost:3000.
##### For Backend
1. Navigate to the backend directory:
    ```Bash
    cd asp-backend
    ```
2. Build the project:
    ```Bash
    dotnet build
    ```
3. Run the project:
    ```Bash
    dotnet run
    ```
## License
This project is licensed under the MIT License.