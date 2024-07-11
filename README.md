# Scrabble Game

## Overview
Scrabble Game is a two-player Windows Form Application developed as a semester project for the Parallel Distributed Programming course. The game utilizes a comprehensive dictionary of nearly 300,000 words, allowing players to connect by entering their IP addresses and names. Real-time communication between players is achieved through socket programming, which handles player movements and score updates based on Scrabble tile weights.

## Features

### Two-Player Mode
- **Player Connection**: Players can connect to the game by entering their IP address and name.
- **Real-Time Communication**: Socket programming ensures that player movements and score updates are communicated instantly.

### Game Mechanics
- **Dictionary**: The game includes a dictionary of nearly 300,000 words to validate player moves.
- **Scoring**: Scores are calculated according to Scrabble tile weights, following standard Scrabble rules.

## Technology Stack
- **Frontend**: Windows Form Application
- **Backend**: C#
- **Networking**: Socket Programming

## Installation and Setup

1. Clone the repository:
    ```bash
    git clone https://github.com/abimonyo/Scrabble-Game.git
    ```

2. Open the project in Visual Studio:
    - Load the solution file (`.sln`) in Visual Studio.

3. Build the project:
    - Use the `Build` option in Visual Studio to compile the project.

4. Run the game:
    - Start the application by running the compiled executable or through Visual Studio.

## Usage
- **Start Game**: Players enter their IP address and name to connect.
- **Gameplay**: Players take turns placing tiles on the board, with moves validated against the dictionary.
- **Scoring**: Scores are automatically updated based on the tile weights as per Scrabble rules.

## Contact
For any inquiries or support, please contact [abdullahmustafa3607@gmail.com].
