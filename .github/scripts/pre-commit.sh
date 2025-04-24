#!/bin/bash

echo "Running EditMode tests..."
/Applications/Unity/Hub/Editor/2022.3.18f1/Unity.app/Contents/MacOS/Unity -quit -batchmode -nographics -projectPath . \
    -runTests -testPlatform editmode \
    -logFile Logs/editmode.log

echo "Running PlayMode tests..."
/Applications/Unity/Hub/Editor/2022.3.18f1/Unity.app/Contents/MacOS/Unity -quit -batchmode -nographics -projectPath . \
    -runTests -testPlatform playmode \
    -logFile Logs/playmode.log

echo "Checking for compilation errors..."
if grep "Compilation failed" Logs/*.log ; then 
    echo "❌ Compilation errors found"
    exit 1
fi

echo "Running Android build check..."
/Applications/Unity/Hub/Editor/2022.3.18f1/Unity.app/Contents/MacOS/Unity -quit -batchmode -nographics -projectPath . \
    -buildTarget Android -executeMethod BuildCLI.BuildAll \
    -logFile Logs/build.log

# Check APK size
if [ -f "Build/Android/TequilaSunrise.apk" ]; then
    size=$(stat -f%z "Build/Android/TequilaSunrise.apk")
    max_size=$((150 * 1024 * 1024)) # 150 MB in bytes
    if [ $size -gt $max_size ]; then
        echo "❌ APK size exceeds 150 MB limit"
        exit 1
    fi
fi

echo "✅ All checks passed"
exit 0 