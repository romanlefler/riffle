"use strict";

// const conn is signalr connection
// const joinCode is the join code
// const plName is the player name

const lobbyScreen = document.getElementById("lobbyScreen");

const choiceScreen = document.getElementById("choiceScreen");
const choiceMsg = document.getElementById("choiceMsg");
const choiceBox = document.getElementById("choiceBox");
const choiceSubmit = document.getElementById("choiceSubmit");

const chosenScreen = document.getElementById("chosenScreen");
const chosenText = document.getElementById("chosenText");

const endScreen = document.getElementById("endScreen");
const endMsg = document.getElementById("endMsg");

function hideAllScreens() {
    lobbyScreen.style.display = "none";
    choiceScreen.style.display = "none";
    chosenScreen.style.display = "none";
    endScreen.style.display = "none";
}

conn.on("RoomError", msg => {
    console.log("Room error: " + msg);
    document.body.textContent += `Error: ${msg}`;
});

conn.on("GameStarted", () => {
    console.log(`Game ${joinCode} started as ${plName}.`);
    beginChoosing();
});

function submitChoice() {
    choiceSubmit.removeEventListener("click", submitChoice);
    choiceBox.disabled = true;
    choiceSubmit.disabled = true;
    conn.invoke("StringMsg", "ChooseWord", choiceBox.value);
}

function beginChoosing() {
    choiceSubmit.addEventListener("click", submitChoice);
    choiceMsg.textContent = "Choose your secret word!";

    hideAllScreens();
    choiceScreen.style.display = "block";
}

function submitGuess() {
    conn.invoke("StringMsg", "GuessWord", choiceBox.value);
    choiceBox.value = "";
}

function beginGuessing() {
    choiceSubmit.addEventListener("click", submitGuess);
    choiceMsg.textContent = "Guess the secret word!";

    hideAllScreens();
    choiceScreen.style.display = "block";
}

conn.on("ChoiceAccepted", () => {
    choiceBox.value = "";
    const msgs = [ "Good word!", "Not what I would've chosen", "Interesting choice..." ];
    chosenText.textContent = msgs[Math.floor(Math.random() * 3)];

    hideAllScreens();

    choiceBox.disabled = false;
    choiceSubmit.disabled = false;
    choiceSubmit.removeEventListener("click", submitChoice);
    chosenScreen.style.display = "block";
});

conn.on("GuessingStarted", beginGuessing);

conn.on("SuccessfulGuess", () => {
    hideAllScreens();
});

conn.on("GameEnded", (connIds) => {
    hideAllScreens();
    endScreen.style.display = "block";

    const myConnId = conn.connection?.connectionId;
    if (!connIds || !myConnId) return;

    const placing = connIds.indexOf(myConnId);
    if (placing > -1) endMsg.textContent = `#${placing + 1}`;
});

conn.start().catch(console.error).then(() => {

    document.getElementById("connectingText").textContent = "You're in!";

    conn.invoke("JoinRoom", joinCode, plName);

});
