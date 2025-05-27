import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { GameType, SignalRUser } from "./defines";

export type SignalRUserHandler = (user : SignalRUser) => void;

interface HandlerInfo {
    map : Map<(...args : any[]) => void, (...args : any[]) => void>;
    fn : (fn : (...args : any[]) => void, ...args : any[]) => void;
}

export class RoomManHost {

    #conn : HubConnection;
    #joinCode : string;
    #users : SignalRUser[];
    #gameType : GameType;

    constructor(gameType : GameType) {
        this.#conn = new HubConnectionBuilder()
            .withUrl("/RoomHub")
            .configureLogging(LogLevel.Debug)
            .build();
        this.#joinCode = "";
        this.#users = [ ];
        this.#gameType = gameType;
    }

    get hubConn() : HubConnection {
        return this.#conn;
    }

    get users() : SignalRUser[] {
        return this.#users;
    }

    get joinCode() : string {
        return this.#joinCode;
    }

    async connect() : Promise<void> {
        return this.#conn.start();
    }

    async createRoom() : Promise<void> {
        
        return new Promise<void>((resolve, reject) => {

            this.#conn.on("RoomCreated", (joinCode : string) => {
                console.log(joinCode);
                console.log(typeof joinCode);
                if(!joinCode || typeof joinCode !== "string") {
                    reject("Failed to receive join code.");
                }

                this.#joinCode = joinCode;
                console.log(joinCode);
                resolve();
            });

            this.#conn.invoke("CreateRoom", this.#gameType);
            console.log("invoked)");
        });

    }

    #addListener(name : string, handler : any) {
        const info = this.#handlers[name];
        if(!info) throw new Error("This event doesn't exist.");
        const wrappedHandler = info.fn.bind(this, handler);
        info.map.set(handler, wrappedHandler);
        this.#conn.on(name, wrappedHandler);
    }

    #removeListener(name : string, handler : (...args : any[]) => void) {
        const info = this.#handlers[name];
        const wrappedHandler = info.map.get(handler);
        if(!wrappedHandler) throw new Error("Cannot remove handler before registering it.");
        info.map.delete(handler);
        this.#conn.off(name, wrappedHandler);
    }

    addUserJoinedListener(handler : SignalRUserHandler) : void {
        this.#addListener("UserJoined", handler);
    }

    removeUserJoinedListener(handler : SignalRUserHandler) : void {
        this.#removeListener("UserJoined", handler);
    }

    addUserLeftListener(handler : SignalRUserHandler) : void {
        this.#addListener("UserLeft", handler);
    }

    removeUserLeftListener(handler : SignalRUserHandler) : void {
        this.#removeListener("UserLeft", handler);
    }

    #handlers : Record<string, HandlerInfo> = {
        "UserJoined": {
            map: new Map(),
            fn: this.#userJoinedHandler
        },
        "UserLeft": {
            map: new Map(),
            fn: this.#userLeftHandler
        }
    };

    #userJoinedHandler(fn : SignalRUserHandler, id : string, name : string) {

        if (!id || !name || typeof id !== "string" || typeof name !== "string") {
            console.error("Failed to handle user join event.");
            return;
        }

        const user: SignalRUser = { id, name };
        this.#users.push(user);
        fn(user);

    }

    #userLeftHandler(fn : SignalRUserHandler, id : string) {

        if (!id || typeof id !== "string") {
            console.error("Failed to handle user left event.");
            return;
        }

        const usr = this.#users.find(u => u.id === id);
        if (!usr) {
            console.error("User ID sent by user left event wasn't found.");
            return;
        }

        const index: number = this.#users.indexOf(usr);
        this.#users.splice(index, 1);
        fn(usr);

    }

}