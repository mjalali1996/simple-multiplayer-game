package userAccount

import (
	"context"
	"database/sql"
	userAccount "simpleServer/userAccount/controllers"
	"simpleServer/utils"

	"github.com/heroiclabs/nakama-common/api"
	"github.com/heroiclabs/nakama-common/runtime"
)

type AccountService struct {
	logger         runtime.Logger
	initializer    runtime.Initializer
	accountManager userAccount.AccountManager
}

func NewService(logger runtime.Logger, initializer runtime.Initializer, manager userAccount.AccountManager) AccountService {
	return AccountService{
		logger:         logger,
		initializer:    initializer,
		accountManager: manager,
	}
}

func (api *AccountService) InitModule() error {
	if err := api.initializer.RegisterAfterAuthenticateDevice(api.initializeUser); err != nil {
		api.logger.Error("Unable to register: %v", err)
		return err
	}

	return nil
}

func (api *AccountService) initializeUser(ctx context.Context, logger runtime.Logger, db *sql.DB, nk runtime.NakamaModule, out *api.Session, in *api.AuthenticateDeviceRequest) error {
	if out.Created {
		userId, err := utils.GetUserId(ctx)
		if err != nil {
			return err
		}

		_, err = api.accountManager.CreateNewUser(userId)
		if err != nil {
			return err
		}
	}

	return nil
}
