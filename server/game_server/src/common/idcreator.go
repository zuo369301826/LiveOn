/*Package common ID生成器 */
package common

import (
	"flag"
	"log"
	"sync/atomic"
	"time"
)

var (
	serverID   int64 // 服务器的编号
	baseSecond int64 // 启始时间 2018-1-1的秒数
	sequence   int64 // 序号
	timeSecond int64
)

func updateSecond() {
	timeSecond = time.Now().Unix() - baseSecond
}

// CreateSessionID 生成ID
func CreateSessionID() int64 {
	order := atomic.AddInt64(&sequence, 1) & (2<<13 - 1)
	if 0 == order {
		updateSecond()
	}
	return serverID | order<<7 | timeSecond<<21
}

func init() {
	var sid int
	flag.IntVar(&sid, "id", 1, "server id")
	flag.Parse()

	sid &= 127
	if sid == 0 {
		log.Fatalln("serverID has to be somewhere between 1 and 127")
	}
	serverID = int64(sid)
	baseSecond = time.Date(2018, 1, 1, 0, 0, 0, 0, time.Local).Unix()
	updateSecond()
}
