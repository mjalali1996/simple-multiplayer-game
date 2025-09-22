package errors

import "github.com/heroiclabs/nakama-common/runtime"

var (
	Internal                = runtime.NewError("internal server error", 505)
	InvalidUserId           = runtime.NewError("invalid user id", 1)
	InvalidSessionId        = runtime.NewError("invalid session id", 1)
	InvalidEnvKey           = runtime.NewError("environment key not found", 2)
	FailedToGetFreePort     = runtime.NewError("failed to get free port", 3)
	FailedToReleasePort     = runtime.NewError("failed to release port", 3)
	PermissionDeniedForUser = runtime.NewError("permission denied for user", 3)
	NoRecordFound           = runtime.NewError("no record found", 4)
)
