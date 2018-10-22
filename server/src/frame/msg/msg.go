package msg

import (
	"github.com/name5566/leaf/network/protobuf"
	"frame/msg/pb"
	"frame/conf"
)

var Processor = protobuf.NewProcessor()

func init() {
	Processor.SetByteOrder(conf.LittleEndian)
	Processor.Register(&pb.CSRequestStart{})	//0
	Processor.Register(&pb.CSResponseStart{})	//1
	Processor.Register(&pb.SCStartGame{})		//2
	Processor.Register(&pb.CSFrameNotify{})		//3
	Processor.Register(&pb.SCFrameNotify{})		//4
}
