package matchmaking

import (
	"context"
	"database/sql"
	"encoding/json"
	"simpleServer/environment"
	"simpleServer/errors"
	"simpleServer/matchmaking/room"
	userAccount "simpleServer/userAccount/controllers"
	"simpleServer/utils"

	"github.com/heroiclabs/nakama-common/runtime"
)

type Service struct {
	initializer   runtime.Initializer
	gameRoom      room.IGameRoom
	walletManager userAccount.WalletManager
	environment   environment.IEnvironment
}

const (
	SetEndMatchEndPoint     = "SetEndMatch"
	DisposeGameRoomEndPoint = "DisposeGameRoom"
)

func NewMatchMakingService(initializer runtime.Initializer, gameRoom room.IGameRoom, walletManager userAccount.WalletManager, env environment.IEnvironment) Service {
	return Service{
		initializer:   initializer,
		gameRoom:      gameRoom,
		walletManager: walletManager,
		environment:   env,
	}
}

func (s *Service) InitModule() error {

	if e := s.initializer.RegisterMatchmakerMatched(s.matchMakerMatched); e != nil {
		return e
	}

	if e := s.initializer.RegisterRpc("CreateRoom", s.test); e != nil {
		return e
	}

	if e := s.initializer.RegisterRpc(SetEndMatchEndPoint, s.setEndMatch); e != nil {
		return e
	}

	if e := s.initializer.RegisterRpc(DisposeGameRoomEndPoint, s.disposeGameRoom); e != nil {
		return e
	}

	return nil
}

func (s *Service) test(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (string, error) {

	return s.createMatch()
}

func (s *Service) matchMakerMatched(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, entries []runtime.MatchmakerEntry) (info string, err error) {
	logger.Debug("matchmaker Matched")

	return s.createMatch()
}

func (s *Service) createMatch() (info string, err error) {
	startingMatchData, err := s.gameRoom.Create()
	if err != nil {
		return "", err
	}

	marshal, err := json.Marshal(startingMatchData)
	if err != nil {
		return "", err
	}

	return string(marshal), nil
}

func (s *Service) setEndMatch(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (string, error) {
	logger.Debug("setEndMatch called")

	ok, err := CheckGameServerPermission(ctx)
	if !ok {
		return ReturnErrorResult(err)
	}

	data := room.EndMatchData{}
	err = json.Unmarshal([]byte(payload), &data)
	if err != nil {
		return ReturnErrorResult(err)
	}

	_, err = s.walletManager.UpdateWins(data.WinnerId, 1, map[string]interface{}{})
	if err != nil {
		return ReturnErrorResult(err)
	}

	result := room.Result{
		IsSuccess: true,
		Message:   "",
	}

	return toJson(result)
}

func (s *Service) disposeGameRoom(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (info string, err error) {
	ok, err := CheckGameServerPermission(ctx)
	if !ok {
		return ReturnErrorResult(err)
	}

	err = s.gameRoom.Dispose(payload)
	if err != nil {
		return ReturnErrorResult(err)
	}

	result := room.Result{
		IsSuccess: true,
		Message:   "",
	}

	return toJson(result)
}

func CheckGameServerPermission(ctx context.Context) (bool, error) {
	ok := IsTheRequestFromAUser(ctx)
	if ok {
		return false, errors.PermissionDeniedForUser
	}
	return true, nil
}

func IsTheRequestFromAUser(ctx context.Context) bool {
	id, err := utils.GetUserId(ctx)
	if err == nil && id != "" {
		return true
	}
	return false
}

func ReturnErrorResult(err error) (string, error) {
	result := room.Result{
		IsSuccess: false,
		Message:   err.Error(),
	}

	return toJson(result)
}

func toJson(obj interface{}) (string, error) {
	bytes, err := json.Marshal(obj)
	if err != nil {
		return "", err
	}
	return string(bytes), nil
}
