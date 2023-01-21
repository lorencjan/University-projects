#!/bin/sh

if which gradle >/dev/null 2>&1; then
	GRADLE="exec gradle"
else
	GRADLE=". ./gradlew"
fi

${GRADLE} clean $@
