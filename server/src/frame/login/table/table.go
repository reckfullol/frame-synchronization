package table

import (
	"time"
	"github.com/name5566/leaf/log"
	"sync"
	"frame/msg/pb"
	"github.com/name5566/leaf/gate"
)

const TIME_PRE_FRAME time.Duration = time.Second / (time.Duration)(30)

type Table struct {
	PlayerCount int
	keys map[uint64][]uint32
	startTime int64
	closeChan chan bool
	currentFrame uint32
	lock *sync.Mutex
	scFrameNtf *pb.SCFrameNotify
	plays map[uint64]gate.Agent
}

func (t *Table)Init() {
	t.PlayerCount = 0
	t.lock = &sync.Mutex{}
	t.keys = make(map[uint64][]uint32)
	t.plays = make(map[uint64]gate.Agent)

	t.scFrameNtf = new(pb.SCFrameNotify)
}
func (t *Table)AddPlayer(uin uint64, agent gate.Agent) {
	t.plays[uin] = agent
	t.PlayerCount += 1
}
func (t *Table)RemovePlayer(uin uint64) bool {
	delete(t.plays, uin)
	t.PlayerCount -= 1
	if t.PlayerCount <= 0 {
		t.CloseTable()
		return true
	}
	return false
}

func (t *Table)StartGame() {
	t.startTime = time.Now().UnixNano()
	t.currentFrame = 1

	go t.RunGame()
}

func (t *Table)OnFrameNotify(uin uint64, keyinfo []uint32) {
	t.lock.Lock()
	currentKeyinfo, ok := t.keys[uin]
	if ok {
		len1 := len(currentKeyinfo)
		len2 := len(keyinfo)
		finkey := make([]uint32, len1 + len2)
		copy(finkey, currentKeyinfo)
		copy(finkey[len1:], keyinfo)
		t.keys[uin] = finkey
	} else {
		currentKeyinfo = keyinfo
		t.keys[uin] = currentKeyinfo
	}
	t.lock.Unlock()
}

func (t *Table)RunGame() {
	for {
		select {
			case <- t.closeChan:
				log.Release("close table")
				return
			default:
				time.Sleep(TIME_PRE_FRAME)
				ct := time.Now().UnixNano()
				dt := ct - t.startTime
				var cf uint32 = (uint32)((time.Duration)(dt) / TIME_PRE_FRAME)
				t.scFrameNtf.CurrentFrame = t.currentFrame
				t.scFrameNtf.NextFrame = cf + 1

				t.scFrameNtf.Keys = make([]*pb.CSFrameNotify, len(t.keys))

				var index = 0
				t.lock.Lock()
				for k,v := range t.keys {
					n := new (pb.CSFrameNotify)
					n.Uin = k
					n.Keys = v
					t.scFrameNtf.Keys[index] = n
					index += 1
					delete(t.keys, k)
				}
				t.lock.Unlock()
				for _, v := range t.plays {
					v.WriteMsg(t.scFrameNtf)
				}

				t.currentFrame = cf + 1
		}
	}
}

func (t *Table)CloseTable() {
	t.closeChan = make(chan bool)
	t.closeChan <- true
}


