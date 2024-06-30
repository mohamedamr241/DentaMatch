
var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

connection.start().then(function () {
    console.log('connected to hub');
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("OnConnected", function () {
    console.log("savingUser")
    OnConnected();
});

function OnConnected() {
    var username = "MohamedAmr303";
    console.log(username)
    connection.invoke("SaveUserConnection", username).catch(function (err) {
        return console.error(err.toString());
    })
}

connection.on("ReceivedNotification", function (message) {
    console.log(message)
});

connection.on("ReceivedPersonalNotification", function (message, username) {
    console.log(username + " " + message)
});
