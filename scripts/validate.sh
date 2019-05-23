#!/usr/bin/env bash

# Requires print_helpers.sh to be sourced as well

# Output colors
NC='\033[0m' # No Color
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'

# Ensures the previous command completed successfully otherwise prints given message and exits
function validate {
  if [[ $? -ne 0 ]]; then
    # Use argument as message or default message if no argument was given
    msg=${1:-Something went wrong, scroll up for details.}
    print_red_line "FAILED: $msg"
    [ "$2" != true ] && exit 1;
  fi
}

# Ensures the previous command completed successfully otherwise prints given message
function validate_without_exit {
  validate "$1" true
}

# Ensures the script is being run on the project root
function ensure_project_root {
  ls unity-sample-app > /dev/null 2>&1 /dev/null
  validate "Ensure this script is running from the project root directory."
}

function ensure_unity_bin {
  if [ -z "$UNITY_BIN" ]; then
    print_red_line "FATAL: UNITY_BIN environment variable is not defined!"
    print_blue_line "Please set UNITY_BIN to the desired Unity executable (e.g., /Applications/Unity/Unity.app/Contents/MacOS/Unity) and try again"
    exit 1
  fi
}

# Scripts that use validate.sh should run from the root directory
ensure_project_root
