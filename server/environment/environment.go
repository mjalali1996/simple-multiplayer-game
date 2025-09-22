package environment

import (
	"context"
	"simpleServer/errors"
	"strconv"

	"github.com/heroiclabs/nakama-common/runtime"
)

var (
	environmentVariables     map[string]string = nil
	startingGameRoomPortsKey                   = "GetStartingGameRoomPorts"
	gameRoomAppPath                            = "GameRoomAppPath"
	gameRoomAppDirectory                       = "GameRoomAppDirectory"
)

type IEnvironment interface {
	GetStartingGameRoomPorts() (int, error)
	GetGameRoomAppPath() (string, error)
	GetGameRoomAppDirectory() (string, error)
}

type environment struct {
	ctx context.Context
}

func New(ctx context.Context) IEnvironment {
	return &environment{ctx: ctx}
}

func getEnvironmentStringValue(ctx context.Context, key string) (string, error) {
	if environmentVariables == nil {
		environmentVariables = ctx.Value(runtime.RUNTIME_CTX_ENV).(map[string]string)
	}

	key, ok := environmentVariables[key]
	if !ok {
		return "", errors.InvalidEnvKey
	}
	return key, nil
}

func getEnvironmentIntValue(ctx context.Context, key string) (int, error) {
	v, err := getEnvironmentStringValue(ctx, key)
	if err != nil {
		return 0, err
	}

	num, err := strconv.Atoi(v)
	if err != nil {
		return 0, err
	}

	return num, nil
}

func (env *environment) GetStartingGameRoomPorts() (int, error) {
	return getEnvironmentIntValue(env.ctx, startingGameRoomPortsKey)
}

func (env *environment) GetGameRoomAppPath() (string, error) {
	return getEnvironmentStringValue(env.ctx, gameRoomAppPath)
}

func (env *environment) GetGameRoomAppDirectory() (string, error) {
	return getEnvironmentStringValue(env.ctx, gameRoomAppDirectory)
}
