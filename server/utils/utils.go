package utils

import (
	"context"
	"simpleServer/errors"

	"github.com/heroiclabs/nakama-common/runtime"
)

func GetUserId(ctx context.Context) (string, error) {
	userID, ok := ctx.Value(runtime.RUNTIME_CTX_USER_ID).(string)
	if !ok {
		return "", errors.InvalidUserId
	}

	return userID, nil
}

func GetSessionId(ctx context.Context) (string, error) {
	sessionId, ok := ctx.Value(runtime.RUNTIME_CTX_SESSION_ID).(string)
	if !ok {
		return "", errors.InvalidSessionId
	}

	return sessionId, nil
}

func GetUserIdAndSessionId(ctx context.Context) (string, string, error) {
	userID, err := GetUserId(ctx)
	if err != nil {
		return "", "", err
	}

	sessionID, err := GetSessionId(ctx)
	if err != nil {
		return "", "", err
	}

	return userID, sessionID, nil
}
