# Changelog

## 1.2 (04.04.2022)

WARNING: Position of the mop will be reset!

### Added

- Added a version for MSCLoader

### Changes

- Actual Mop settings are now saved into .JSON file

### Bug Fixes

- Fixed an error happening during game save

## 1.1 (30.04.2021)

WARNING: Position of the mop will be reset!
Mod Loader Pro is now required to run this mod.

### Added

- Improved optimization
  - Note: If you are using Modern Optimiztion Plugin, it may display an error saying "[MOP] Couldn't find world object mop(Clone)" - you can safely ignore it, as well as you can remove the rule file "ActualMop.mopconfig"
- Added sound effect while cleaning the floor
- Added animation while mopping

### Changes

- Ported to Mod Loader Pro
- The "Urinate" key doesn't have to be binded to the keyboard anymore
- Mod now also works under Linux

### Bug Fixes

- Fixed mop clipping under the ground on save load
- You can finally place the mop inside of vehicle, without wrecking it

### Removed

- Removed user32.dll API references, as they are no longer needed

## 1.0.3 (27.04.2020)

WARNING: With this update mop will respawn at the default position!

### Changes

- Updated for MSC Mod Loader version 1.1.7
- Improved chnagelog readibility
- The message for incorrect urinating key will now pop up the console
- Actual Mop save is now located in MSC save folder
- Changed the default spawn position
- Slightly changed the save format (warning: old Actual Mop saves won't work in the current version!)

### Bug Fixes

- Fixed mod not loading for some users, and for others leaving an error in output log

## 1.0.2 (01.03.2020)

### Added

- "Use" icon will now appear when mop is in hand

### Bug Fixes

- Player can't 'clean himself' with mop

## 1.0.1 (16.02.2020)

### Added

- Mop position will reset on new game

## 1.0 (11.02.2020)

### Added

- Player can now open the doors while holding the mop, without dropping it

### Changes

- The key binded to urinating will not be clicked anymore, if you Alt+Tab from the game

## Beta 0.3 (10.02.2020)

### Changes

- Player has to hold the mop in hand, in order for it to work (press Use to grab the mop)

### Bug Fixes

- Mop should now always load back where you left it

## Beta 0.2 (08.02.2020)

### Added

- New mop model thanks to BrennFuchS!
- Added settings
  - Added changelog
  - Added "Reset Mop Position" button
- Urinating button can now be binded under any key (as long as it's one of the letters, or digit)

### Changes

- Stain removal rate is now a constant value

### Bug Fixes

- Improved in hand rotation and holding

## Beta 0.1 (07.02.2020)

- Initial release
