"use strict";

const conn = new signalR.HubConnectionBuilder().withUrl("/roomHub").build();

const joinCode = document.getElementById("joinCodeData").dataset.code;
if (!joinCode || typeof joinCode !== "string" || joinCode.length != 6) {
    window.location.replace("/Join");
}

const plName = String(document.getElementById("playerNameData").dataset.name);
if (!plName) window.location.replace("/Join");

