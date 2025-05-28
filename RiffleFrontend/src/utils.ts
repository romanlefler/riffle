import { showDialog } from "./dialog";
import { RedirectInterruptError } from "./redirectError";

export async function sleep(seconds : number) : Promise<void> {
    return new Promise<void>(resolve => {
        setTimeout(resolve, seconds * 1000.0);
    });
}

/**
 * Resolves/rejects the given promise unless a certain amount
 * of time passes before, where it will reject with the message
 * "Timed out."
 * @param promise The promise to try
 * @param maxSeconds THe seconds to give before rejection
 */
export async function maxWait<T>(promise : Promise<T>, maxSeconds : number) : Promise<T> {
    return Promise.race([
        promise,
        new Promise((_, reject) => sleep(maxSeconds).then(() => reject("Timed out.")))
    ]) as Promise<T>;
}

export async function fatalErrorDialog(err : any) {

    if(err instanceof RedirectInterruptError) return;

    await showDialog({
        title: "Error Occurred",
        content: String(err ?? "An unknown error occurred."),
        buttons: [ "Reload" ]
    });
    window.location.reload();
}

export async function roomClosedDialog() {
    await showDialog({
        title: "Room Closed",
        content: "The host closed the room.",
        buttons: [ "Exit" ]
    });
    window.location.href = `${window.location.origin}/Join`;
    throw new RedirectInterruptError();
}
