#!/usr/bin/env bash

# Output colors
NC='\033[0m' # No Color
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'

function print_build_starting {
  platform=${1}
  sdk_name=${2}
  echo -e "\n${BLUE}Building the MoPub Unity plugin for $platform using the $sdk_name...${NC}\n"
}

function print_build_finished {
  platform=${1}
  sdk_name=${2}
  echo -e "\n${GREEN}DONE! Built the $platform wrapper with the $sdk_name${NC}\n"
}

function print_export_starting {
  echo -e "\n${BLUE}Exporting the MoPub Unity plugin unity package...${NC}\n"
}

function print_export_finished {
  destination=${1}
  echo -e "\n${GREEN}DONE! Exported the unity package to $destination${NC}\n"
}
