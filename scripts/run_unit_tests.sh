#!/usr/bin/env bash
my_dir="$(dirname "$0")"
source "$my_dir/print_helpers.sh"
source "$my_dir/validate.sh"

ensure_unity_bin

PROJECT_PATH="`pwd`/unity-sample-app"
TEST_RESULTS="`pwd`/scripts/testresults.xml"
TEST_LOG="`pwd`/scripts/testlog.txt"

function tests_passed {
  print_green_line "Tests PASSED!"
}

function tests_error {
  print_red_line "Script ERROR!"
  print_blue_line "Log excerpt: "
  grep -A 5 "\-----CompilerOutput:-stderr----------" $TEST_LOG
  print_blue_line "Please see $TEST_LOG for full details."
}

function tests_failed {
  print_red_line "Tests FAILED!"
  print_blue_line "Results excerpt: "
  grep -A 5 -B 3 "Expected" $TEST_RESULTS
  print_blue_line "Please see $TEST_RESULTS for full test results."
}

echo "Running unit tests with Unity Editor at:"
print_blue_line $UNITY_BIN
$UNITY_BIN -runEditorTests -projectPath $PROJECT_PATH -editorTestsResultFile $TEST_RESULTS -logFile $TEST_LOG -force-free -batchmode > /dev/null 2>&1 /dev/null

case $? in
  0 ) tests_passed;;
  1 ) tests_error;;
  2 ) tests_failed;;
esac
