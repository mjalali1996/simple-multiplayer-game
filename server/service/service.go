package service

// IService interface must implemented by Nakama services
type IService interface {
	InitModule() error
}
