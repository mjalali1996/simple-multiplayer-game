package main

import (
	"context"
	"database/sql"
	"encoding/json"
	"github.com/heroiclabs/nakama-common/runtime"
)

type HealthCheckResponse struct {
	Success bool `json:"success"`
}

func RpcHealthCheck(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, payload string) (string, error) {
	logger.Debug("HealthCheck RPC called")
	response := &HealthCheckResponse{Success: true}

	out, err := json.Marshal(response)
	if err != nil {
		logger.Error("Errors marshaling response type to Json")
		return "", err
	}
	return string(out), nil
}
