import "./styles/roundaboutHost.css";
import gsap from "gsap";
import { GameType } from "./defines";
import { RoomManHost } from "./roomManHost";
import { sleep } from "./utils";

const room: RoomManHost = new RoomManHost(GameType.Roundabout);

const elemBg = document.querySelector(".background") as HTMLDivElement;
const elemPlCount = document.getElementById("player-count") as HTMLDivElement;
const elemJoinCode = document.getElementById("join-code") as HTMLDivElement;
const elemStartBtn = document.getElementById("play-button") as HTMLButtonElement;

elemJoinCode.addEventListener("click", () => {
    const invite = `${location.origin}/Join?code=${room.joinCode}`;
    navigator.clipboard.writeText(invite);
});

elemStartBtn.disabled = true;
elemStartBtn.addEventListener("click", startClicked);

function showUsers() {
    console.log(String(room.users));
    const count = room.users.length;
    elemPlCount.textContent = `${count} / 8`;
    elemStartBtn.disabled = count < 2;
}

function startClicked() {
    if(room.users.length < 2) return;

}

async function main() {

    await room.connect();
    await room.createRoom();

    elemJoinCode.textContent = room.joinCode;
    gsap.from(elemJoinCode, { x: "100vw", duration: 2.0 });
    await sleep(1.0);

    room.addUserJoinedListener(showUsers);
    room.addUserLeftListener(showUsers);

    showUsers();
    gsap.from(elemPlCount, { y: "100vh", ease: "bounce.out", duration: 1.5 });

}

main();
