#!/bin/sh

if which gradle >/dev/null 2>&1; then
	GRADLE="exec gradle"
else
	GRADLE=". ./gradlew"
fi

# Spark requires spark.driver.memory to be at least 475MB
# For default value, see: java -XX:+PrintFlagsFinal -version | grep -F MaxHeapSize
export _JAVA_OPTIONS="-Xmx1g"

${GRADLE} run --args="$*"
