package gate

import (
	"github.com/gogo/protobuf/proto"
	"github.com/name5566/leaf/gate"
	"github.com/name5566/leaf/chanrpc"
	"github.com/name5566/leaf/log"
	"reflect"
	"frame/base"
)

var (
	skeleton = base.NewSkeleton()
	ChanRPC  = skeleton.ChanRPCServer
	msgHandlers map[interface{}]*chanrpc.Server
)

func ProcessCSMsg(args []interface{}) {
	msg := args[0]
	agent := args[1].(gate.Agent)

	if agent.UserData() == nil {
		//还没有登录
		log.Error("agent has not login...")
		return
	}

	msgID := reflect.TypeOf(msg)
	chanServer, ok := msgHandlers[msgID]
	if !ok {
		//没有注册目标模块
		log.Error("msg(%v) not register", msgID)
		return
	}
	chanServer.Go(msgID, agent.UserData().(uint32), msg)
}

func RegisterMsgHandler(id proto.Message, f interface{}, chanServer *chanrpc.Server) {
	base.HandleMsg(ChanRPC, id, ProcessCSMsg)
	msgHandlers[reflect.TypeOf(id)] = chanServer
	chanServer.Register(reflect.TypeOf(id), f)
}
