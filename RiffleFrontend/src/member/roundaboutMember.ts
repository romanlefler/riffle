import "../styles/member.css";
import { extractJoinData } from "./extractJoinData";
import { RoomManMember } from "./roomManMember";
import { fatalErrorDialog } from "../utils";
import { showDialog } from "../dialog";

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

const sentScreen = document.getElementById("sent-screen") as HTMLDivElement;
const sentMsg = document.getElementById("sent-msg") as HTMLParagraphElement;
const sentCol = document.getElementById("sent-columns") as HTMLDivElement;
const sentSubmit = document.getElementById("sent-submit") as HTMLButtonElement;

function shrinkFontSize(elem : HTMLElement) {
    let sz = parseFloat(elem.style.fontSize);
    if(isNaN(sz)) sz = 1.0;
    elem.style.fontSize = `${sz * 0.9}em`;
}

function verifyColSize() {
    sentCol.style.fontSize = "1.0em";
    const buttons : NodeListOf<HTMLButtonElement> = sentCol.querySelectorAll(".col button");
    for(let b of buttons) {
        let i = 0;
        while(b.scrollWidth > b.clientWidth) {
            shrinkFontSize(b);
            if(++i > 10) break;
        }
    }

    const last : HTMLButtonElement | null = sentCol.querySelector(".col:last-child button:last-child");
    if(!last) return;
    let rect = last.getBoundingClientRect();

    let i = 0;
    while(rect.bottom > window.innerHeight) {
        shrinkFontSize(sentCol);
        if (++i > 10) break;
        rect = last.getBoundingClientRect();
    }
}

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

function radioClicked(this: HTMLButtonElement) {
    const parent = this.parentElement;
    for(const n of parent!.querySelectorAll("button.radio-sel")) n.classList.remove("radio-sel");
    this.classList.add("radio-sel");
}

function submitSentence(this: HTMLButtonElement) {
    const indices = [];
    for (const r of sentCol.getElementsByClassName("col")) {
        const selected = r.querySelectorAll("button.radio-sel");
        if (selected.length === 0) {
            showDialog({ title: "Pick Options", content: "Response blank." });
            return;
        } else if (selected.length > 1) throw new Error("Multiple selected!?!");
        const s = selected[0] as HTMLButtonElement;
        const idxStr = s.dataset.index;
        if (!idxStr) throw new Error("No index?!?");
        indices.push(parseInt(idxStr));
    }
    this.removeEventListener("click", submitSentence);
    room.hubConn.invoke("StringMsg", "SelSentence", JSON.stringify(indices));
    sentScreen.style.display = "none";
}

function showSentenceOptions(options: string[][]) {
    const cols : HTMLDivElement[] = [ ];
    for(const opt of options) {
        const c = document.createElement("div");
        c.classList.add("col", "radio");
        for(let i = 0; i < opt.length; i++) {
            const btn = document.createElement("button");
            btn.textContent = opt[i];
            btn.dataset.index = i.toString();
            btn.addEventListener("click", radioClicked);
            c.appendChild(btn);
        }
        cols.push(c);
    }
    sentCol.replaceChildren(...cols);
    sentSubmit.addEventListener("click", submitSentence);

    requestAnimationFrame(() => verifyColSize());
}

room.hubConn.on("ChoiceAccepted", () => {
    chosenMsg.textContent = "Good choice!";

    choiceScreen.style.display = "none";
    chosenScreen.style.display = "block";

    choiceSubmit.removeEventListener("click", submitChoice);
});

room.hubConn.on("GuessingStarted", (connId : string) => {
    choiceMsg.textContent = "Guess the Word";
    choiceInput.value = "";
    choiceSubmit.textContent = "Try";
    choiceSubmit.addEventListener("click", submitGuess);
    choiceSubmit.disabled = false;

    chosenScreen.style.display = "none";
    sentScreen.style.display = "none";
    choiceScreen.style.display = "block";
});

room.hubConn.on("SentenceOptions", (base : string, options : string[][]) => {
    const msg = base.replaceAll(/\{(\d+)\}/g, "___");
    sentMsg.textContent = msg;
    showSentenceOptions(options);

    chosenScreen.style.display = "none";
    choiceScreen.style.display = "none";
    sentScreen.style.display = "block";
});

function submitGuess() {

    const phrase = choiceInput.value;
    choiceInput.value = "";
    if(!phrase.trim()) return;

    room.hubConn.invoke("StringMsg", "GuessWord", phrase);
}

room.hubConn.on("SuccessfulGuess", () => {
    sentScreen.style.display = "none";
    choiceScreen.style.display = "none";
    sentScreen.removeEventListener("click", submitSentence);
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

window.addEventListener("resize", () => verifyColSize());

function enterSubmit(input: HTMLInputElement, submit: HTMLButtonElement) {
    input.addEventListener("keypress", e => {
        if(e.code === "Enter") submit.click();
    });
}
enterSubmit(choiceInput, choiceSubmit);

room.addRoomErrorListener(fatalErrorDialog);
main().catch(fatalErrorDialog);
