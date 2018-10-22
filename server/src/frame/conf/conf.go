package conf

import "time"

var (
	// gate config
	PendingWriteNum        = 2000
	MaxMsgLen       uint32 = 409600
	HTTPTimeout            = 10 * time.Second
	LenMsgLen              = 4
	LittleEndian           = true

	// skeleton config
	GoLen              = 10000
	TimerDispatcherLen = 10000
	AsynCallLen        = 10000
	ChanRPCLen         = 10000
)
