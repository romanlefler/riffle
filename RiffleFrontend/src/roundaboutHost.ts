import { GameType } from "./defines";
import { RoomManHost } from "./roomManHost";

const room: RoomManHost = new RoomManHost(GameType.Roundabout);

function showUsers() {
    console.log(String(room.users));
}

async function main() {

    room.addUserJoinedListener(showUsers);
    room.addUserLeftListener(showUsers);

    await room.connect();
    await room.createRoom();

    console.log(room.joinCode);
    document.getElementById("join-code")!.textContent = room.joinCode;

}

main();
