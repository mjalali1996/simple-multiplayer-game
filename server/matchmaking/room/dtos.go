package room

type Config struct {
	GameId string `json:"GameId"`
	Port   int    `json:"Port"`
}

type EndMatchData struct {
	GameId   string `json:"GameId"`
	WinnerId string `json:"WinnerId"`
}

type Result struct {
	IsSuccess bool
	Message   string
}

type ResultWithData struct {
	Result
	Data interface{}
}
