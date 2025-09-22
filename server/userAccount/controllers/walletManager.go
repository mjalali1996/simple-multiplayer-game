package userAccount

import (
	"context"
	"simpleServer/userAccount/models"

	"github.com/heroiclabs/nakama-common/runtime"
)

type WalletManager interface {
	UpdateWins(userId string, wins int64, metadata map[string]interface{}) (int64, error)
}

type walletManager struct {
	ctx context.Context
	nk  runtime.NakamaModule
}

func NewWalletManager(ctx context.Context, nk runtime.NakamaModule) WalletManager {
	return &walletManager{
		ctx: ctx,
		nk:  nk,
	}
}

func (wm *walletManager) UpdateWins(userId string, win int64, metadata map[string]interface{}) (int64, error) {
	changeset := map[string]int64{
		models.WinsKey: win,
	}

	updated, _, err := wm.nk.WalletUpdate(wm.ctx, userId, changeset, metadata, true)
	if err != nil {
		return 0, err
	}
	updatedWins := updated[models.WinsKey]
	return updatedWins, nil
}
