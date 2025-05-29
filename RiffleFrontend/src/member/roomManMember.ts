import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";

export class RoomManMember {

    #conn : HubConnection;
    #joinCode : string;
    #playerName : string;

    constructor(joinCode : string, playerName : string) {
        this.#conn = new HubConnectionBuilder()
            .withUrl("/RoomHub")
            .configureLogging(LogLevel.Debug)
            .build();
        this.#joinCode = joinCode;
        this.#playerName = playerName;
    }

    get hubConn() : HubConnection {
        return this.#conn;
    }

    get joinCode() : string {
        return this.#joinCode;
    }

    async connect() : Promise<void> {
        return this.#conn.start();
    }

    async joinRoom() : Promise<void> {
        return this.#conn.invoke("JoinRoom", this.#joinCode, this.#playerName);
    }

    addRoomErrorListener(callback : (err : string) => void) : void {
        this.#conn.on("RoomError", callback);
    }

}