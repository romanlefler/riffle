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

async function main() {

    lobbyScreen.style.display = "block";
    await room.connect();
    lobbyMsg.textContent = `Joining Room ${joinCode}...`;    
    await room.joinRoom();
    lobbyMsg.textContent = "Waiting for Host to Start";

}

main().catch(fatalErrorDialog);
room.addRoomErrorListener(fatalErrorDialog);
