:; set -eo pipefail
:; SCRIPT_DIR=$(cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd)
:; ${SCRIPT_DIR}/build.sh "$@"
:; exit $?

@ECHO OFF
<<<<<<< HEAD
powershell -ExecutionPolicy ByPass -NoProfile "%~dp0build.ps1" %*
=======
powershell -ExecutionPolicy ByPass -NoProfile -File "%~dp0build.ps1" %*
>>>>>>> f397afe5fedd9a70b859ef0dbae3289201f012df
