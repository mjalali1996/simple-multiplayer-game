package main

import (
	"context"
	"database/sql"
	"github.com/heroiclabs/nakama-common/runtime"
)

//goland:noinspection GoUnusedExportedFunction
func InitModule(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, initializer runtime.Initializer) error {
	app, err := BuildApplicationWire(ctx, logger, db, nk, initializer)
	if err != nil {
		return err
	}

	for _, service := range app.services {
		if err = service.InitModule(); err != nil {
			return err
		}
	}

	if err := initializer.RegisterRpc("healthCheck", RpcHealthCheck); err != nil {
		return err
	}

	return nil
}
