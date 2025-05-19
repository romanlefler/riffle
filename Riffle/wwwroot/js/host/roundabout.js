"use strict";

const conn = new signalR.HubConnectionBuilder().withUrl("/roomHub").build();
var joinCode = null;

conn.on("RoomCreated", givenCode => {
    console.log("Room created: " + givenCode);
    joinCode = givenCode;
    document.getElementById("joinCode").textContent = joinCode;
});

conn.on("RoomError", msg => {
    console.log("Room error: " + msg);
    document.getElementById("joinCode").textContent = `Error: ${msg}`;
})

conn.start().catch(console.error).then(() => {

    conn.invoke("CreateRoom", 1).catch(console.error);

});

