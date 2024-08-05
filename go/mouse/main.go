package main

import (
	"fmt"
	"os"
	"syscall"

	"github.com/malinoOS/malino/libmalino"
)

var Version string = "undefined" // if verfmt = yymmdd (which it currently is), it will turn this into YYMMDD date format.

func main() {
	defer libmalino.ResetTerminalMode()
	fmt.Println("malino (project mouse v" + Version + ") booted successfully. Type a line of text to get it echoed back.")
	err := libmalino.MountDevFS()
	if err != nil {
		fmt.Println("Failed to mount /dev:", err)
		syscall.Reboot(syscall.LINUX_REBOOT_CMD_HALT)
		return
	}
	fmt.Println("Mounted /dev!")

	err = libmalino.MountProcFS()
	if err != nil {
		fmt.Println("Failed to mount /proc:", err)
		syscall.Reboot(syscall.LINUX_REBOOT_CMD_HALT)
		return
	}
	fmt.Println("Mounted /proc!")

	libmalino.LoadAllKernelModules()

	printMouseCoordinates()

	libmalino.ShutdownComputer()
}

func printMouseCoordinates() {
	const mouseDevice = "/dev/input/mice"

	file, err := os.Open(mouseDevice)
	if err != nil {
		fmt.Println("Failed to open mouse device:", err)
		syscall.Reboot(syscall.LINUX_REBOOT_CMD_HALT)
		return
	}
	defer file.Close()

	buffer := make([]byte, 3)
	x, y := 0, 0

	for {
		_, err := file.Read(buffer)
		if err != nil {
			fmt.Println("Error reading mouse device:", err)
			syscall.Reboot(syscall.LINUX_REBOOT_CMD_HALT)
			return
		}

		leftButton := (buffer[0] & 0x1) > 0
		rightButton := (buffer[0] & 0x2) > 0
		middleButton := (buffer[0] & 0x4) > 0

		xMovement := int16(buffer[1])
		if xMovement > 127 {
			xMovement -= 256
		}

		yMovement := int16(buffer[2])
		if yMovement > 127 {
			yMovement -= 256
		}

		x = max(0, x+int(xMovement))
		y = max(0, y-int(yMovement))

		if x > 160 {
			x = 160
		}
		if y > 50 {
			y = 50
		}

		libmalino.ClearScreen()

		fmt.Printf("\x1b[%d;%dH", y, x)
		fmt.Print("\x1b[107m \x1b[40m\x1b[H")

		fmt.Printf("X: %d, Y: %d, Left: %t, Right: %t, Middle: %t\n",
			x, y, leftButton, rightButton, middleButton)
	}
}

func max(a, b int) int {
	if a > b {
		return a
	}
	return b
}
