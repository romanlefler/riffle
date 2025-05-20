"use strict";

const conn = new signalR.HubConnectionBuilder().withUrl("/roomHub").build();
var joinCode = null;
var users = [];

function showUsers() {
    const list = document.getElementById("playerList");
    list.textContent = users.map(u => u.name).join(", ");
}

conn.on("RoomCreated", givenCode => {
    console.log("Room created: " + givenCode);
    joinCode = givenCode;
    document.getElementById("joinCode").textContent = joinCode;
});

conn.on("RoomError", msg => {
    console.log("Room error: " + msg);
    document.getElementById("joinCode").textContent = `Error: ${msg}`;
});

conn.on("UserJoined", (id, name) => {
    users.push({ id, name });
    showUsers();
});

conn.on("UserLeft", id => {
    users = users.filter(u => u.id === id);
    showUsers();
});

conn.start().catch(console.error).then(() => {

    conn.invoke("CreateRoom", 1);

    const playBtn = document.getElementById("playButton");
    const playTimer = document.getElementById("playTimer");
    console.log(playBtn);
    playBtn.addEventListener("click", () => {
        var playClickTime = Date.now();
        var intervalId = setInterval(() => {
            const deltaS = Math.ceil(4.0 - (Date.now() - playClickTime) / 1000);
            if (deltaS <= 0.0) {
                clearInterval(intervalId);
                playTimer.textContent = "";
                conn.invoke("StartGame");
            }
            else playTimer.textContent = String(deltaS);
        }, 1000);
    });

});

