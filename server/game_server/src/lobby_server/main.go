package main

import (
	"encoding/json"
	"fmt"
	"lobby_server/lobby"
	"log"
	"net"
)

//LobbyHandle 大厅
var LobbyHandle *lobby.Lobby

const (
	//SERVERIP 服务器IP
	SERVERIP = ":7000"
)

//ClientMsg 客户端消息
type ClientMsg struct {
	Order int `json:"order"`
}

//RobbyMsg 大厅消息
type RobbyMsg struct {
	Joinlobby bool  `json:"joinlobby"`
	Playerid  int64 `json:"playerid"`
}

//RoomMsg 房间消息
type RoomMsg struct {
	Joinroom bool  `json:"joinroom"`
	Roomid   int64 `json:"roomid"`
}

func serverHeadle(conn net.Conn) {

	var roomid int64
	var ID int64
	var rooming = false

	clientAddr := conn.RemoteAddr().String()
	fmt.Println(clientAddr, "connect.")
	defer func() {
		conn.Close()
		fmt.Println(clientAddr, "close.")
	}()

	//加入大厅
	ID, ok := LobbyHandle.LobbyAdd(conn)
	defer func() {
		LobbyHandle.LobbyDel(ID) //退出大厅
		fmt.Println("[", clientAddr, "]退出大厅")
	}()

	//返回消息
	robbymsg := &RobbyMsg{}
	robbymsg.Joinlobby = ok
	robbymsg.Playerid = ID
	data, err := json.Marshal(robbymsg)
	if err != nil {
		fmt.Println("json.Marshal fail.")
		return
	}
	conn.Write(data)

	//服务器日志
	if !ok {
		fmt.Println("LobbyAdd fail.")
		return
	}
	fmt.Println("LobbyAdd suc.")
	fmt.Println("玩家id:", ID)

	for {
		buf := make([]byte, 1024)
		n, err := conn.Read(buf)
		if err != nil {
			break
		}
		clientmsg := &ClientMsg{}
		if err = json.Unmarshal(buf[:n], clientmsg); err != nil {
			fmt.Println("json.Unmarshal fail.")
			continue
		}
		switch clientmsg.Order {
		case 0: //加入房间
			{
				var PIs = false
				var RIs = false
				if !rooming {
					roomid, PIs, RIs = LobbyHandle.JoinRoom(ID, conn)
					if PIs {
						fmt.Println("[", clientAddr, "]成功加入房间，房间ID：", roomid)
						rooming = true
						if RIs {
							fmt.Println("[", clientAddr, "]房间人数已满")
						} else {
							fmt.Println("[", clientAddr, "]房间人数未满, 当前房间人数：", LobbyHandle.GetRoom(roomid).Getplayertatil())
						}
					} else {
						fmt.Println("[", clientAddr, "]加入房间失败")
					}
					defer func() {
						if rooming {
							empty := LobbyHandle.ExitRoom(roomid, ID)
							rooming = false
							if empty { //退出房间
								fmt.Println("[", clientAddr, "]退出房间, 当前房间人数：0;房间已注销")
							} else {
								fmt.Println("[", clientAddr, "]退出房间, 当前房间人数：", LobbyHandle.GetRoom(roomid).Getplayertatil())
							}
						}
					}()
				}
				//返回消息
				roommsg := &RoomMsg{}
				roommsg.Joinroom = PIs
				roommsg.Roomid = roomid
				data, err := json.Marshal(roommsg)
				if err != nil {
					fmt.Println("json.Marshal fail.")
					return
				}
				conn.Write(data)
			}
		case 1: //退出房间
			{
				if rooming {
					empty := LobbyHandle.ExitRoom(roomid, ID)
					rooming = false
					if empty { //退出房间
						fmt.Println("[", clientAddr, "]退出房间, 当前房间人数：0;房间已注销")
					} else {
						fmt.Println("[", clientAddr, "]退出房间, 当前房间人数：", LobbyHandle.GetRoom(roomid).Getplayertatil())
					}
				}
			}
		}
	}
}

func main() {
	listen, err := net.Listen("tcp", SERVERIP)
	if err != nil {
		log.Panicln("listen err.", err)
	}

	fmt.Println("lobby server success.")

	//创建大厅
	LobbyHandle = lobby.CreateLobby()

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
