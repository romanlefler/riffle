import "../styles/member.css";
import { extractJoinData } from "./extractJoinData";
import { RoomManMember } from "./roomManMember";
import { fatalErrorDialog } from "../utils";

const { plName, joinCode } = extractJoinData();
const room = new RoomManMember(joinCode, plName);

const lobbyScreen = document.getElementById("lobby-screen") as HTMLDivElement;
const lobbyMsg = document.getElementById("lobby-msg") as HTMLParagraphElement;

const choiceScreen = document.getElementById("choice-screen") as HTMLDivElement;
const choiceMsg = document.getElementById("choice-msg") as HTMLParagraphElement;
const choiceInput = document.getElementById("choice-input") as HTMLInputElement;
const choiceSubmit = document.getElementById("choice-submit") as HTMLButtonElement;

const chosenScreen = document.getElementById("chosen-screen") as HTMLDivElement;
const chosenMsg = document.getElementById("chosen-msg") as HTMLParagraphElement;

room.hubConn.on("GameStarted", () => {
    
    choiceMsg.textContent = "Choose your secret word!";
    choiceInput.value = "";
    choiceSubmit.textContent = "Submit";
    choiceSubmit.addEventListener("click", submitChoice);
    choiceSubmit.disabled = false;

    lobbyScreen.style.display = "none";
    choiceScreen.style.display = "block";
});

function submitChoice() {
    const phrase = choiceInput.value;
    if(!phrase.trim()) return;

    // Disable button until word is accepted
    choiceSubmit.disabled = true;

    room.hubConn.invoke("StringMsg", "ChooseWord", phrase).catch(fatalErrorDialog);
}

room.hubConn.on("ChoiceAccepted", () => {
    chosenMsg.textContent = "Good choice!";

    choiceScreen.style.display = "none";
    chosenScreen.style.display = "block";

    choiceSubmit.removeEventListener("click", submitChoice);
});

room.hubConn.on("GuessingStarted", (connId : string) => {

    if(room.hubConn.connectionId === connId) {
        // Getting guessed
        // TODO: Being guessed screen
        choiceMsg.textContent = "You're Being Guessed";
        choiceInput.value = "";
        choiceSubmit.textContent = "Continue";
        choiceSubmit.disabled = true;

        chosenScreen.style.display = "none";
        choiceScreen.style.display = "block";
    } else {
        // Guess someone else's
        choiceMsg.textContent = "Guess the Word";
        choiceInput.value = "";
        choiceSubmit.textContent = "Try";
        choiceSubmit.addEventListener("click", submitGuess);
        choiceSubmit.disabled = false;

        chosenScreen.style.display = "none";
        choiceScreen.style.display = "block";
    }
});

function submitGuess() {

    const phrase = choiceInput.value;
    choiceInput.value = "";
    if(!phrase.trim()) return;

    room.hubConn.invoke("StringMsg", "GuessWord", phrase);
}

room.hubConn.on("SuccessfulGuess", () => {
    choiceScreen.style.display = "none";
    choiceSubmit.removeEventListener("click", submitGuess);
});

room.hubConn.on("GameEnded", (userIds : string[]) => {
    const place = userIds.indexOf(room.hubConn.connectionId!) + 1;
    const podium = Math.min(3, userIds.length - 1);
    chosenMsg.textContent = place <= podium ? "Congratulations!" : "Tough luck";
    chosenScreen.style.display = "block";
});

async function main() {

    lobbyScreen.style.display = "block";
    await room.connect();
    lobbyMsg.textContent = `Joining Room ${joinCode}...`;    
    await room.joinRoom();
    lobbyMsg.textContent = "Waiting for Host to Start";

}

main().catch(fatalErrorDialog);
room.addRoomErrorListener(fatalErrorDialog);
