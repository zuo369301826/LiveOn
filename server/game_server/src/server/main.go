package main

import (
	"encoding/json"
	"fmt"
	"log"
	"net"
)

const (
	//SERVERIP 服务器IP
	SERVERIP = "127.0.0.1:1234"
)

//Msg 消息类型
type Msg struct {
	Ordle  int    `json:"ordle"`
	Name   string `json:"name"`
	Passwd string `json:"passwd"`
}

func serverHeadle(conn net.Conn) {
	clientAddr := conn.RemoteAddr().String()
	fmt.Println(clientAddr, "connect.")

	defer func() {
		conn.Close()
		fmt.Println(clientAddr, "close.")
	}()

	buf := make([]byte, 1024)
	for {
		n, err := conn.Read(buf)
		if err != nil {
			return
		}
		Player := &Msg{}

		if err = json.Unmarshal(buf[:n], Player); err != nil {
			log.Println("Unmarshal fail.")
			continue
		}
		fmt.Println("Player", Player)
		conn.Write([]byte("你好呀\n"))
	}
}

func main() {
	log.SetFlags(log.LstdFlags | log.Llongfile)

	listen, err := net.Listen("tcp", SERVERIP)
	if err != nil {
		log.Panicln("listen err.", err)
	}

	fmt.Println("server success.")

	for {
		conn, err := listen.Accept()
		if err != nil {
			log.Println("Accept err.")
			continue
		}
		fmt.Println("Accept success.")
		go serverHeadle(conn)
	}
}
