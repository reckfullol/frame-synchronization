package internal

import (
	"github.com/name5566/leaf/gate"
	"github.com/name5566/leaf/log"
	"github.com/name5566/leaf/module"
	"frame/base"
	"frame/msg/pb"
	"frame/login/table"
)

var (
	skeleton = base.NewSkeleton()
	ChanRPC  = skeleton.ChanRPCServer
)
var UIN uint64 = 0

var UINS map[uint64]gate.Agent = make(map[uint64]gate.Agent)
var tables map[uint64]*table.Table = make(map[uint64]*table.Table)

var oneuin uint64 = 0

type Module struct {
	*module.Skeleton
}

func (m *Module) OnInit() {
	m.Skeleton = skeleton

	skeleton.RegisterChanRPC("NewAgent", m.rpcNewAgent)
	skeleton.RegisterChanRPC("CloseAgent", m.rpcCloseAgent)

	base.HandleMsg(ChanRPC, &pb.CSRequestStart{}, m.rpcLogin)
	base.HandleMsg(ChanRPC, &pb.CSFrameNotify{}, m.frameNotify)
}

func (m *Module) OnDestroy() {

}

func (m *Module) frameNotify(args []interface{}) {
	framePB := args[0].(*pb.CSFrameNotify)
	agent := args[1].(gate.Agent)
	uin, ok := agent.UserData().(uint64)
	if ok {
		var tab *table.Table = tables[uin]
		tab.OnFrameNotify(uin, framePB.Keys)
	}
}

func (m *Module) rpcNewAgent(args []interface{}) {
	agent := args[0].(gate.Agent)
	log.Debug("NewAgent: %v", agent.RemoteAddr())
}

func (m *Module) rpcCloseAgent(args []interface{}) {
	agent := args[0].(gate.Agent)
	uin := agent.UserData().(uint64)
	var tab *table.Table = tables[uin]

	if tab.RemovePlayer(uin) {
		delete(tables, uin)
		log.Release("delete table")
	}

	delete(UINS, uin)
	log.Debug("CloseAgent: %v uin:%v", agent.RemoteAddr(), uin)
}

func (m *Module) rpcLogin(args []interface{}) {
	agent := args[1].(gate.Agent)

	res := new (pb.CSResponseStart)
	UIN += 1
	res.Uid = UIN

	agent.SetUserData(res.Uid)
	UINS[res.Uid] = agent

	log.Release("login uin:%v", UIN)

	agent.WriteMsg(res)

	if oneuin == 0 {
		oneuin = UIN
	} else {
		// 开始比赛
		start := new (pb.SCStartGame)
		start.Uins = make([]uint64, 2)
		start.Uins[0] = oneuin
		start.Uins[1] = UIN
		agent.WriteMsg(start)
		var ag gate.Agent = UINS[oneuin]
		ag.WriteMsg(start)
		log.Release("start game 1P:%v   2P:%v", oneuin, UIN)
		tab := new (table.Table)
		tab.Init()
		tab.AddPlayer(oneuin, ag)
		tab.AddPlayer(UIN, agent)

		tables[oneuin] = tab
		tables[UIN] = tab

		tab.StartGame()
		oneuin = 0
	}
}
