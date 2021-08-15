package pubsub

import (
	"encoding/json"
	"fmt"
	"github/gorilla/websocket"
)

var (
	PUBLISH = "publish"
	SUBSCRIBE = "subscribe"
)

type PubSub struct {
	Clients []Client
	Subscriptions []Subscription
}

type Client struct {
	Id string
	Connection *websocket.Conn
}

type Message struct {
	Action string `json:"action"`
	Topic string `json:"topic"`
	Message json.RawMessage `json:"message"`
}

type Subscription struct {
	Topic string
	Client* Client
}

func (ps *PubSub)AddClient(client Client) (*PubSub) {

	ps.Clients = append(ps.Clients, client)

	fmt.Println("Adding new Client to the list: [", client.Id, "] [ total:", len(ps.Clients), "]")
	payload := []byte("Welcome! UUID [" + client.Id + "]!")
	client.Connection.WriteMessage(1, payload)

	return ps
}

func (ps *PubSub)GetSubscriptions(topic string, client* Client) ([]Subscription) {

	var subscriptionList []Subscription

	for _, subscription := range ps.Subscriptions{

		if client != nil{
			if subscription.Client.Id == client.Id && subscription.Topic == topic{
				subscriptionList = append(subscriptionList, subscription)
			}
		} else {
			if subscription.Topic == topic{
				subscriptionList = append(subscriptionList, subscription)
			}
		}
	}

	return subscriptionList
}

func (ps *PubSub)Subscribe(client* Client, topic string) (*PubSub) {

	clientSubs := ps.GetSubscriptions(topic, client)

	if len(clientSubs) > 0 {
	// duplicate subscribe
		return ps
	}

	fmt.Println("Adding new viewer subscription. uuid [", client.Id, "], topic [", topic, "]")
	newSubscription := Subscription{
		Topic: topic,
		Client: client,
	}

	ps.Subscriptions = append(ps.Subscriptions, newSubscription)

	return ps
}

func (ps *PubSub)Publish(topic string, message []byte, excludeClient *Client){

	subscriptions := ps.GetSubscriptions(topic, nil)

	for _, sub := range subscriptions{

		fmt.Printf("Publishing Message to uuid [%s] from Topic [%s]\n", sub.Client.Id, topic)
		sub.Client.Connection.WriteMessage(1, message)
	}
}

func (ps *PubSub)HandleReceiveMessage(client Client, messageType int, payload []byte)(*PubSub){

	m := Message{}

	err := json.Unmarshal(payload, &m)
	if err != nil{
		fmt.Println("Incorrect Message payload")
		return ps
	}

	fmt.Println("Client Correct Message payload: ", m.Action, m.Message, m.Topic)

	switch m.Action {
	case PUBLISH:
		fmt.Println("Action type: publish")
		ps.Publish(m.Topic, m.Message, nil)
		break
	case SUBSCRIBE:
		ps.Subscribe(&client, m.Topic)
		break
	default:
		break
	}

	return ps
}