package models

type UserModel struct {
	Id       string
	Username string
	Name     string
	AvatarId int
	Lang     string
	Wallet   UserWalletModel
}
