//go:build wireinject
// +build wireinject

package main

import (
	"context"
	"database/sql"
	"simpleServer/environment"
	"simpleServer/matchmaking"
	"simpleServer/matchmaking/room"
	"simpleServer/service"
	"simpleServer/userAccount"
	accountManager "simpleServer/userAccount/controllers"

	"github.com/google/wire"
	"github.com/heroiclabs/nakama-common/runtime"
)

type App struct {
	services []service.IService
}

func NewApp(accountService userAccount.AccountService, matchMakingService matchmaking.Service) (*App, error) {

	services := make([]service.IService, 0)
	services = append(services, &matchMakingService)
	services = append(services, &accountService)

	return &App{
		services: services,
	}, nil
}

func BuildApplicationWire(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, initializer runtime.Initializer) (*App, error) {

	wire.Build(NewApp, userAccount.NewService, accountManager.NewAccountManager, accountManager.NewWalletManager, matchmaking.NewMatchMakingService, room.NewGameRoom, environment.New)

	return &App{}, nil
}
