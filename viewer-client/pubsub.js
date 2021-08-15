
var ws = new WebSocket('ws://jaygan.com:3000/ws')
ws.onopen = () => {
    console.log("Connected to Pubsub Server.")
    console.log("sending subscription message")
    ws.send('{"action":"subscribe", "topic":"youtube", "message":"Hi yall"}');
}
ws.onmessage = (msg) => {
    console.log("Message from Pubsub Server: ", msg.data)
    readyVideo(msg.data);
}
ws.onclose = () => {
    console.log("Disconnected from Pubsub Server")
}
window.ws = ws;

