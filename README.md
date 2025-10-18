# OutOfOrder Screensaver Wrapper

This repository wraps your OutOfOrder screensaver `.scr` so it:

- Runs full-screen in `/s` screensaver mode
- Loops indefinitely if it exits
- Uses a custom EXE icon
- Can be built automatically using GitHub Actions

## Usage

1. Run `ScreensaverWrapper.exe` from the `publish` folder.
2. Make sure `screensaver/MyScreensaver.scr` is in the same folder.
3. The screensaver will loop forever and restart automatically if it exits.
