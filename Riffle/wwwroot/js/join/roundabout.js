"use strict";

// const conn is signalr connection
// const joinCode is the join code
// const plName is the player name

const lobbyScreen = document.getElementById("lobbyScreen");
const guessScreen = document.getElementById("guessScreen");

const guessBox = document.getElementById("guessBox");

function hideAllScreens() {
    lobbyScreen.style.display = "none";
    guessScreen.style.display = "none";
}

conn.on("RoomError", msg => {
    console.log("Room error: " + msg);
    document.body.textContent += `Error: ${msg}`;
});

conn.on("GameStarted", () => {
    console.log(`Game ${joinCode} started as ${plName}.`);

    hideAllScreens();
    guessScreen.style.display = "block";
});

conn.start().catch(console.error).then(() => {

    document.getElementById("connectingText").textContent = "You're in!";

    conn.invoke("JoinRoom", joinCode, plName);

});
