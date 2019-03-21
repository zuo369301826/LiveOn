package main

import (
	"encoding/json"
	"fmt"
	"log"
	"login_server/msql"
	"net"
)

const (
	//SERVERIP 服务器IP
	SERVERIP = ":7777"
)

//UserMsg 消息类型
type UserMsg struct {
	Ordle  int    `json:"order"`
	Name   string `json:"name"`
	Passwd string `json:"passwd"`
}

//Res 返回类型
type Res struct {
	Istrue bool `json:"istrue"`
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
		fmt.Println("n", n, "buf", string(buf[0:n]))
		usermsg := &UserMsg{}
		if err = json.Unmarshal(buf[:n], usermsg); err != nil {
			log.Println("Unmarshal fail.")
			continue
		}
		fmt.Println("msg", usermsg)

		switch usermsg.Ordle {
		case 0:
			{
				//注册
				var res Res
				var user msql.UserData
				user.Username = usermsg.Name
				user.Password = usermsg.Passwd
				if OK := msql.InsertUser(user); OK {
					res.Istrue = true
					fmt.Println("install data suc.")
				} else {
					res.Istrue = false
					fmt.Println("install data fail.")
				}
				data, _ := json.Marshal(res)
				if _, err := conn.Write(data); err != nil {
					log.Println("fail")
				}
				fmt.Println(data, string(data))
			}
		case 1:
			{
				//登录
				var res Res
				istrue := msql.IsPasswdTrueByUsername(usermsg.Name, usermsg.Passwd)
				if istrue {
					fmt.Println("passwd is ture.")
				} else {
					fmt.Println("passwd is false.")
				}
				res.Istrue = istrue
				data, err := json.Marshal(res)
				if err != nil {
					log.Println("Marshal fail.")
				}

				if _, err := conn.Write(data); err != nil {
					log.Println("fail")
				}
			}
		}
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
