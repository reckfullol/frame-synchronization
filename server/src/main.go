package main

import (
	"github.com/name5566/leaf"
	lconf "github.com/name5566/leaf/conf"
	"log"
	"frame/conf"
	"frame/gate"
	"frame/login"
)

func main() {
	lconf.LogLevel = conf.Server.LogLevel
	lconf.LogPath = conf.Server.LogPath
	lconf.LogFlag = log.LstdFlags | log.Lmicroseconds | log.Llongfile
	lconf.ConsolePort = conf.Server.ConsolePort
	lconf.ProfilePath = conf.Server.ProfilePath

	leaf.Run(
		login.Module,      //登录逻辑
		gate.Module,       //网关
	)
}
