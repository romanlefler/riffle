"use strict";

// const conn is signalr connection
// const joinCode is the join code
// const plName is the player name

conn.on("RoomError", msg => {
    console.log("Room error: " + msg);
    document.body.textContent += `Error: ${msg}`;
});

conn.on("GameStarted", () => {
    console.log(`Game ${joinCode} started as ${plName}.`);
});

conn.start().catch(console.error).then(() => {

    conn.invoke("JoinRoom", joinCode, plName);

});
