
# Riffle Room SignalR Hub Methods

Documentation for methods sent between the server and client via SignalR.

# Common

These methods are standard for every room.

### Hub Methods

| Name         | Caller      | Param 1   | Param 2     |
|--------------|-------------|-----------|-------------|
| CreateRoom   | Host        | Game ID   |             |
| JoinRoom     | Member      | Join Code | Player Name |
| StartGame    | Host        |           |             |
| StringMsg    | Any         | Msg Name  | Msg Content |

### Client Methods

| Name         | Callee      | Param 1   | Param 2     |
|--------------|-------------|-----------|-------------|
| RoomError    | Any         | Error Msg |             |
| RoomCreated  | Host        | Join Code |             |
| RoomClosed   | Member      |           |             |
| UserJoined   | Host        | User ID   |             |
| UserLeft     | Host        | User ID   |             |
| GameStarted  | Any         |           |             |
| StringMsg    | Any         | Msg Name  | Msg Content |

# Game-Specific

`StringMsg` methods are used back and forth for game-specific information.

## Roundabout

### Hub String Msgs

| Name         | Caller      | Param 1   |
|--------------|-------------|-----------|
| ChooseWord   | Member      | User ID   |

### Client Methods

| Name             | Callee  | Param 1   |
|------------------|---------|-----------|
| UserChoseWord    | Host    | User ID   |
| ChoiceAccepted   | Member  | User ID   |
| GuessingAccepted | Any     |           |

