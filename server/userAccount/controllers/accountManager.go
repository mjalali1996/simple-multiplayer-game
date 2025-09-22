package userAccount

import (
	"context"
	"database/sql"
	"encoding/json"
	"simpleServer/userAccount/models"
	"strconv"

	"github.com/heroiclabs/nakama-common/api"
	"github.com/heroiclabs/nakama-common/runtime"
)

type AccountManager interface {
	CreateNewUser(userId string) (*models.UserModel, error)
	ChangeName(userId string, name string) error
	ChangeAvatar(userId string, avatarId int) error
	GetLanguage(userId string) (string, error)
	ChangeLanguage(userId string, lang string) error
	GetUserModel(userId string) (*models.UserModel, error)
}

type accountManager struct {
	ctx    context.Context
	nk     runtime.NakamaModule
	wm     WalletManager
	db     *sql.DB
	logger runtime.Logger
}

func NewAccountManager(ctx context.Context, nk runtime.NakamaModule, walletManager WalletManager, db *sql.DB, logger runtime.Logger) AccountManager {
	return &accountManager{
		ctx:    ctx,
		nk:     nk,
		wm:     walletManager,
		db:     db,
		logger: logger,
	}
}

func (ac *accountManager) CreateNewUser(userId string) (*models.UserModel, error) {
	account, err := ac.nk.AccountGetId(ac.ctx, userId)
	if err != nil {
		return nil, err
	}

	metadata := make(map[string]interface{})
	displayName := ""
	timezone := ""
	location := ""
	langTag := "en"
	avatarUrl := "0"

	if err := ac.nk.AccountUpdateId(ac.ctx, userId, account.User.Id, metadata, displayName, timezone, location, langTag, avatarUrl); err != nil {
		// Handle error.
		return nil, err
	}

	_, err = ac.wm.UpdateWins(userId, 0, metadata)
	if err != nil {
		return nil, err
	}

	return getUserModelByAccount(account, account.Wallet)
}

func (ac *accountManager) ChangeName(userId string, name string) error {
	prepare, err := ac.db.Prepare("UPDATE users SET display_name = $2 WHERE id = $1")
	if err != nil {
		return err
	}

	if _, err = prepare.Exec(userId, name); err != nil {
		return err
	}

	return nil
}

func (ac *accountManager) ChangeAvatar(userId string, avatarId int) error {
	prepare, err := ac.db.Prepare("UPDATE users SET avatar_url = $2 WHERE id = $1")
	if err != nil {
		return err
	}

	if _, err = prepare.Exec(userId, strconv.Itoa(avatarId)); err != nil {
		return err
	}

	return nil
}

func (ac *accountManager) GetLanguage(userId string) (string, error) {
	result, err := ac.db.Query("SELECT lang_tag FROM users WHERE id = $1", userId)
	if err != nil {
		return "", err
	}

	if result.Next() {
		lang := ""
		err := result.Scan(&lang)
		if err != nil {
			return "", err
		}
		return lang, nil
	}

	return "En", err
}

func (ac *accountManager) ChangeLanguage(userId string, lang string) error {
	prepare, err := ac.db.Prepare("UPDATE users SET lang_tag = $2 WHERE id = $1")
	if err != nil {
		return err
	}

	if _, err = prepare.Exec(userId, lang); err != nil {
		return err
	}

	return nil
}

func (ac *accountManager) GetUserModel(userId string) (*models.UserModel, error) {
	account, err := ac.nk.AccountGetId(ac.ctx, userId)
	if err != nil {
		return nil, err
	}

	return getUserModelByAccount(account, account.Wallet)
}

func getUserModelByAccount(account *api.Account, walletData string) (*models.UserModel, error) {
	avatarId, err := strconv.Atoi(account.User.AvatarUrl)
	if err != nil {
		return nil, err
	}

	walletMap := make(map[string]int64)
	err = json.Unmarshal([]byte(walletData), &walletMap)
	if err != nil {
		return nil, err
	}

	wallet := models.NewUserWallet(walletMap)

	return &models.UserModel{
		Id:       account.User.Id,
		Username: account.User.Username,
		Name:     account.User.DisplayName,
		AvatarId: avatarId,
		Lang:     account.User.LangTag,
		Wallet:   wallet,
	}, nil
}
