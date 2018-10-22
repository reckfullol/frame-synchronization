package conf

import (
	"encoding/json"
	"github.com/name5566/leaf/log"
	"io/ioutil"
)

var Server struct {
	LogLevel            string
	LogPath             string
	TCPAddr             string
	CertFile            string
	KeyFile             string
	MaxConnNum          int
	MysqlServer         string
	MysqlMaxOpenConns   int
	MysqlMaxIdleConns   int
	RedisServer         string
	RedisMaxActive      int
	RedisMaxIdle        int
	RedisConnectTimeout int
	RedisReadTimeout    int
	RedisWriteTimeout   int
	ConsolePort         int
	ProfilePath         string
	LoginSalt			string
}

func init() {
	data, err := ioutil.ReadFile("src/frame/conf/server.json")
	if err != nil {
		log.Fatal("%v", err)
	}
	err = json.Unmarshal(data, &Server)
	if err != nil {
		log.Fatal("%v", err)
	}
}
