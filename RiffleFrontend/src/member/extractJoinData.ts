import { RedirectInterruptError } from "../redirectError";

interface JoinData {
    plName : string;
    joinCode : string;
}

export function extractJoinData() : JoinData {

    const joinCode = document.getElementById("join-code-data")!.dataset.code;
    if (!joinCode || typeof joinCode !== "string" || joinCode.length != 6) {
        window.location.replace("/Join");
        throw new RedirectInterruptError();
    }

    const plName = String(document.getElementById("player-name-data")!.dataset.name);
    if (!plName) {
        window.location.replace("/Join");
        throw new RedirectInterruptError();
    }

    return { joinCode, plName };

}