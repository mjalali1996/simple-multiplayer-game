package models

type UserWalletModel struct {
	Wins int64
}

var (
	WinsKey = "wins"
)

func NewUserWallet(data map[string]int64) UserWalletModel {
	return UserWalletModel{Wins: getWins(data)}
}

func getWins(data map[string]int64) int64 {
	value, ok := data[WinsKey]
	if ok == false {
		return 0
	}

	return value
}
