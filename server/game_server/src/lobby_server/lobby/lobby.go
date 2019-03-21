package lobby

import (
	"common"
	"lobby_server/room"
	"net"
)

//ROOMPLAYERMAX 房间最大人数
const ROOMPLAYERMAX = 4

//Lobby 大厅对象
type Lobby struct {
	total       int32
	loddymap    map[int64]net.Conn
	roommap     map[int64]*room.Room
	leisureRoom *room.Room
}

//LobbyInit 初始化
func (m *Lobby) LobbyInit() {
	m.total = 0
	m.loddymap = make(map[int64]net.Conn)
	m.roommap = make(map[int64]*room.Room)
	m.leisureRoom = nil
}

//LobbyAdd 添加
func (m *Lobby) LobbyAdd(conn net.Conn) (int64, bool) {
	id := common.CreateSessionID()

	//判断是否存在
	_, ok := m.loddymap[id]
	if ok {
		return id, false
	}

	m.loddymap[id] = conn
	m.total++
	return id, true
}

//LobbyDel 删除
func (m *Lobby) LobbyDel(id int64) {
	_, ok := m.loddymap[id]
	if ok {
		delete(m.loddymap, id)
		m.total--
	}
}

//LobbyMmpty 清空
func (m *Lobby) LobbyMmpty() {
	m.total = 0
	m.loddymap = make(map[int64]net.Conn)
}

//JoinRoom 加入房间
func (m *Lobby) JoinRoom(playerid int64, conn net.Conn) (int64, bool, bool) {
	if m.leisureRoom == nil {
		m.leisureRoom = room.CreateRoom() //创建房间
	}
	roomid := m.leisureRoom.GetRoomID()

	if m.leisureRoom.RoomAdd(playerid, conn) {
		if m.leisureRoom.Getplayertatil() == ROOMPLAYERMAX { //房间人数已满
			m.roommap[roomid] = m.leisureRoom
			m.leisureRoom = nil
			return roomid, true, true // 加入成功，房间人数已满
		}
		return roomid, true, false //加入成功，但是房间人数未满
	}
	return roomid, false, false //加入失败
}

//ExitRoom  退出房间
func (m *Lobby) ExitRoom(roomid int64, playid int64) bool {
	if _, ok := m.roommap[roomid]; ok { //房间已经开
		m.roommap[roomid].RoomDel(playid)
		if m.roommap[roomid].Getplayertatil() == 0 { //是否为空房间
			delete(m.roommap, roomid)
			return true
		}
	} else {
		m.leisureRoom.RoomDel(playid)
	}
	return false
}

//GetRoom  获得房间房间
func (m *Lobby) GetRoom(roomid int64) *room.Room {
	if _, ok := m.roommap[roomid]; ok { //房间已经开
		return m.roommap[roomid]
	}
	return m.leisureRoom
}

//CreateLobby 创建大厅
func CreateLobby() *Lobby {
	lobby := &Lobby{}
	lobby.LobbyInit()
	return lobby
}
