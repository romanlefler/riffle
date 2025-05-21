"use strict";

// const conn is signalr connection
// const joinCode is the join code
// const plName is the player name

const lobbyScreen = document.getElementById("lobbyScreen");

const choiceScreen = document.getElementById("choiceScreen");
const choiceBox = document.getElementById("choiceBox");
const choiceSubmit = document.getElementById("choiceSubmit");

const chosenScreen = document.getElementById("chosenScreen");
const chosenText = document.getElementById("chosenText");

function hideAllScreens() {
    lobbyScreen.style.display = "none";
    choiceScreen.style.display = "none";
    chosenScreen.style.display = "none";
}

conn.on("RoomError", msg => {
    console.log("Room error: " + msg);
    document.body.textContent += `Error: ${msg}`;
});

conn.on("GameStarted", () => {
    console.log(`Game ${joinCode} started as ${plName}.`);

    hideAllScreens();
    choiceScreen.style.display = "block";
});

choiceSubmit.addEventListener("click", () => {

    choiceBox.disabled = true;
    choiceSubmit.disabled = true;
    conn.invoke("StringMsg", "ChooseWord", choiceBox.value);
});

conn.on("ChoiceAccepted", () => {

    const msgs = [ "Good word!", "Not what I would've chosen", "Interesting choice..." ];
    chosenText.textContent = msgs[Math.floor(Math.random() * 3)];

    hideAllScreens();

    choiceBox.disabled = false;
    choiceSubmit.disabled = false;
    chosenScreen.style.display = "block";
});

conn.start().catch(console.error).then(() => {

    document.getElementById("connectingText").textContent = "You're in!";

    conn.invoke("JoinRoom", joinCode, plName);

});
