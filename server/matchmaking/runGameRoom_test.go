package matchmaking

import (
	"fmt"
	"os"
	"os/exec"
	"testing"
)

func TestRunGameRoom(t *testing.T) {
	gameRoomAppPath := "D:/projects/task/server/data/game/Game.exe"

	cmd := exec.Command(gameRoomAppPath, "-mode", "server", "-ip", "127.0.0.1", "-port", "6666")
	cmd.Stdout = os.Stdout

	err := cmd.Start()
	if err != nil {
		fmt.Print(err)
	}

	err = cmd.Wait()
	if err != nil {
		fmt.Print(err)
	}
}
