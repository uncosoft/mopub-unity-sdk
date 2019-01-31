#!/usr/bin/env bash

# Output colors
NC='\033[0m' # No Color
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'

function print_color_line {
  message=$1
  color=$2
  echo -e "\n$color$message$NC\n"
}

function print_blue_line {
  message=$1
  print_color_line "$message" $BLUE
}

function print_green_line {
  message=$1
  print_color_line "$message" $GREEN
}

function print_red_line {
  message=$1
  print_color_line "$message" $RED
}

function print_build_starting {
  platform=$1
  sdk_name=$2
  print_blue_line "Building the MoPub Unity plugin for $platform using the $sdk_name..."
}

function print_build_finished {
  platform=$1
  sdk_name=$2
  print_green_line "DONE! Built the $platform wrapper with the $sdk_name"
}

function print_export_starting {
  print_blue_line "Exporting the MoPub Unity plugin unity package..."
}

function print_export_finished {
  destination=$1
  print_green_line "DONE! Exported the unity package to $destination"
}

function print_command_starting {
  cmd=$1
  if [ ! -z $2 ]; then
    args="($2)"
  fi
  print_blue_line "==> Running release command: $cmd$args"
}

function print_command_finished {
  print_blue_line "==> Done!"
}
