package room

import (
	"common"
	"net"
)

//Room 房间类型
type Room struct {
	roomID      int64
	playertatil int
	players     map[int64]net.Conn
}

//RoomInit 房间初始化
func (m *Room) RoomInit() {
	m.playertatil = 0
	m.players = make(map[int64]net.Conn)
	m.roomID = common.CreateSessionID()
}

//RoomAdd 添加
func (m *Room) RoomAdd(id int64, conn net.Conn) bool {
	//判断是否存在
	_, ok := m.players[id]
	if ok {
		return false
	}
	m.players[id] = conn
	m.playertatil++
	return true
}

//RoomDel 删除
func (m *Room) RoomDel(id int64) {
	_, ok := m.players[id]
	if ok {
		delete(m.players, id)
		m.playertatil--
	}
}

//RoomEmpty 清空
func (m *Room) RoomEmpty() {
	m.playertatil = 0
	m.players = make(map[int64]net.Conn)
}

//Getplayertatil 获取房间人数
func (m *Room) Getplayertatil() int {
	return m.playertatil
}

//GetRoomID 获取房间id
func (m *Room) GetRoomID() int64 {
	return m.roomID
}

//CreateRoom 创建大厅
func CreateRoom() *Room {
	room := &Room{}
	room.RoomInit()
	return room
}
