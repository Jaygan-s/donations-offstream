package main

import (
	"fmt"
	"github/google/uuid"
	"github/gorilla/websocket"
	"log"
	"net/http"
	"pubsub"
)

var upgrader = websocket.Upgrader{
	ReadBufferSize:  1024,
	WriteBufferSize: 1024,
}
var ps = &pubsub.PubSub{}

func CreateUUID() (string) {
	return uuid.Must(uuid.NewRandom()).String()
}

func ViewerSocketHandler(w http.ResponseWriter, r *http.Request) {

	upgrader.CheckOrigin = func(*http.Request) bool {
		return true
	}

	conn, err := upgrader.Upgrade(w, r, nil)
	if err != nil {
		log.Println(err)
		return
	}

	fmt.Println("New Client is connected")

	client := pubsub.Client{
		Id: CreateUUID(),
		Connection:conn,
	}

	// add this client into the list
	ps.AddClient(client)

	// handles client message
	for
	{
		messageType, p, err := conn.ReadMessage()
		if err != nil {
			log.Println("Something went wrong", err)
			return
		}

		ps.HandleReceiveMessage(client, messageType, p)
	}
}


func StreamerSocketHandler(w http.ResponseWriter, r *http.Request) {

	// most part of this function is just a copy-paste from Viewer handler. so it does not have any security checks!
	// (it's because I'm still learning Golang as a beginner.)
	// if you use this code for launching app, there is POSSIBILITY OF HIJACK by third party.
	// and it can lead to stream terror with some 'unintended' youtube clips.

	upgrader.CheckOrigin = func(*http.Request) bool {
		return true
	}

	conn, err := upgrader.Upgrade(w, r, nil)
	if err != nil {
		log.Println(err)
		return
	}

	fmt.Println("New Streamer Client is connected")

	client := pubsub.Client{
		Id: CreateUUID(),
		Connection:conn,
	}

	// add this client into the list
	ps.AddClient(client)

	// handles client message
	for
	{
		messageType, p, err := conn.ReadMessage()
		if err != nil {
			log.Println("Something went wrong", err)
			return
		}

		aMessage := []byte("Hi Streamer, I'm Server.")
		if err := conn.WriteMessage(messageType, aMessage); err != nil {
			log.Println(err)

			return
		}

		fmt.Printf("New message from Streamer: %s\n", p)
		ps.Publish("youtube", p, nil )
	}
}

func main() {

	http.HandleFunc("/", func(w http.ResponseWriter, r *http.Request){
		http.ServeFile(w, r, "static")
	})

	// Enlist handler for viewer client pattern
	http.HandleFunc("/ws", ViewerSocketHandler)

	// Enlist handler for streamer client pattern
	http.HandleFunc("/streamer", StreamerSocketHandler) // WARNING!!! for tests only. needs security!

	// Opens server for 3000 port
	http.ListenAndServe(":3000", nil)
	fmt.Print("Viewer Server is Running on: http://loaclhost:3000")
}
