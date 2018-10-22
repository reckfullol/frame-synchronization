package base

import (
	"github.com/name5566/leaf/chanrpc"
	"github.com/name5566/leaf/module"
	"github.com/gogo/protobuf/proto"
	"reflect"
	"frame/conf"
	"frame/msg"
)

func NewSkeleton() *module.Skeleton {
	skeleton := &module.Skeleton{
		GoLen:              conf.GoLen,
		TimerDispatcherLen: conf.TimerDispatcherLen,
		AsynCallLen:        conf.AsynCallLen,
		ChanRPCServer:      chanrpc.NewServer(conf.ChanRPCLen),
	}
	skeleton.Init()
	return skeleton
}

func HandleMsg(chanServer *chanrpc.Server, id proto.Message, f interface{}) {
	msg.Processor.SetRouter(id, chanServer)
	chanServer.Register(reflect.TypeOf(id), f)
}
