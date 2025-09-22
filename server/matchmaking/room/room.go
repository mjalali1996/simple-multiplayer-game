package room

import (
	"context"
	"crypto/rand"
	"encoding/hex"
	"os/exec"
	"simpleServer/environment"
	"simpleServer/errors"
	"strconv"
	"sync"
	"time"

	"github.com/heroiclabs/nakama-common/runtime"
)

var (
	portMutex       = sync.Mutex{}
	takenPorts      = make([]bool, 100)
	gameIdToPortMap = make(map[string]int)
)

type IGameRoom interface {
	Create() (*Config, error)
	Dispose(roomId string) error
}

type room struct {
	ctx    context.Context
	logger runtime.Logger
	env    environment.IEnvironment
}

func NewGameRoom(ctx context.Context, logger runtime.Logger, env environment.IEnvironment) IGameRoom {
	return &room{
		ctx:    ctx,
		logger: logger,
		env:    env,
	}
}

func (rc *room) Create() (*Config, error) {
	startupData, err := rc.getStartupData(rc.env)
	if err != nil {
		return nil, err
	}

	gameRoomAppPath, err := rc.env.GetGameRoomAppPath()
	if err != nil {
		return nil, err
	}

	portString := strconv.Itoa(startupData.Port)

	cmd := exec.Command(gameRoomAppPath, "-id", startupData.GameId, "-ip", "0.0.0.0", "-port", portString, "-mode", "server")
	err = cmd.Start()

	if err != nil {
		rc.logger.Error("cmd.Start error: %v", err)
	}

	return startupData, nil
}

func (rc *room) Dispose(gameId string) error {

	startingPort, err := rc.env.GetStartingGameRoomPorts()
	if err != nil {
		return err
	}

	port := gameIdToPortMap[gameId]

	return releasePort(gameId, startingPort, port)
}

func (rc *room) getStartupData(env environment.IEnvironment) (*Config, error) {
	startingPort, err := env.GetStartingGameRoomPorts()
	if err != nil {
		return nil, err
	}

	gameId := getUniqueGameId()
	gameRoomPort, err := getAFreePortForGameRoom(gameId, startingPort)
	if err != nil {
		return nil, err
	}

	return &Config{
		GameId: gameId,
		Port:   gameRoomPort,
	}, nil
}

func getUniqueGameId() string {

	unixTime := strconv.FormatInt(time.Now().Unix(), 10)

	uniqueId := unixTime + ":" + generateSecureToken(5)

	return uniqueId
}

func generateSecureToken(length int) string {
	b := make([]byte, length)
	if _, err := rand.Read(b); err != nil {
		return ""
	}
	return hex.EncodeToString(b)
}

func getAFreePortForGameRoom(gameId string, startingPort int) (int, error) {
	portMutex.Lock()
	defer portMutex.Unlock()

	portSuccess := false
	port := 0
	for i := 0; i < len(takenPorts); i++ {
		if takenPorts[i] == false {
			takenPorts[i] = true
			portSuccess = true
			port = startingPort + i
			gameIdToPortMap[gameId] = port
			break
		}
	}

	if !portSuccess {
		return 0, errors.FailedToGetFreePort
	}
	return port, nil
}

func releasePort(gameId string, startingPort int, releasePort int) error {
	index := releasePort - startingPort
	if index < 0 || index >= len(takenPorts) {
		return errors.FailedToReleasePort
	}

	portMutex.Lock()
	defer portMutex.Unlock()
	delete(gameIdToPortMap, gameId)
	takenPorts[index] = false

	return nil
}
