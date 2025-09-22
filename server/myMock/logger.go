package myMock

import (
	"fmt"

	"github.com/heroiclabs/nakama-common/runtime"
)

type MockLogger struct {
}

func (m MockLogger) Debug(format string, v ...interface{}) {
	fmt.Printf(format, v)
}

func (m MockLogger) Info(format string, v ...interface{}) {
	fmt.Printf(format, v)
}

func (m MockLogger) Warn(format string, v ...interface{}) {
	fmt.Printf(format, v)
}

func (m MockLogger) Error(format string, v ...interface{}) {
	fmt.Printf(format, v)
}

func (m MockLogger) WithField(key string, v interface{}) runtime.Logger {
	fmt.Printf(key, v)
	return m
}

func (m MockLogger) WithFields(fields map[string]interface{}) runtime.Logger {
	for f := range fields {
		fmt.Printf(f)
	}
	return m
}

func (m MockLogger) Fields() map[string]interface{} {
	return nil
}
