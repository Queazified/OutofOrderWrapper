# OutOfOrder Screensaver Wrapper

This repository wraps your OutOfOrder screensaver `.scr` so it:

- Runs full-screen in `/s` screensaver mode
- Loops indefinitely if it exits
- Uses a custom EXE icon
- Can be built automatically using GitHub Actions

## Usage

1. Copy your screensaver file as `Screensaver.scr` to the same directory as `ScreensaverWrapper.exe`
2. Run `ScreensaverWrapper.exe`
3. The screensaver will loop forever and restart automatically if it exits

Note: The screensaver file can also be placed in a `screensaver` subfolder if preferred.
